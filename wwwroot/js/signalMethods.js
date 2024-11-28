
	/*https://stackoverflow.com/questions/46197844/is-it-possible-to-use-a-ternary-operator-in-css*/
	$("#enableSwitch").on("click", function () {
		var isDisabled = $("#killswitch").prop("disabled");
		$("#killswitch").prop("disabled", !isDisabled)
			.css({
				"color": isDisabled ? '#FF0000' : "#6a6a6a",
				"background-color": isDisabled ? '#7ACC00' : "#2a2a2a"
			});
	});

	async function KillSwitch() {
		let chatName = sessionStorage.getItem('UserChatName');
		const request = `/KillSwitch`
		const response = fetch(request, {
			method: 'POST',
			body: JSON.stringify({ ChatName: chatName }),
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		}).then(response => {
			sessionStorage.removeItem('UserChatName');
		}).catch(error => caughtErrors(error))

	}

	async function GetMessages(event, name) {
		$("#leftinnerdiv").empty();
		if (name == null) {
			var target = $(event.target);
			var chatName = target.attr("name-arg");
			$("#username").val(chatName);
		} else {
			var chatName = name;
		}
		sessionStorage.setItem("CurrentChatName", chatName.toString());
		const requestName = "../API/GetChatName";
		fetch(requestName, {
			method: 'POST',
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		})
			.then(response => response.json())
			.then(json => storeChatName(json))
			.catch(error => caughtErrors(error))


		const storeChatName = (json) => {
			sessionStorage.setItem('UserChatName', json.toString());

		}
		const requestMessages = `../API/GetReceivedMessages/${chatName}`;

		await fetch(requestMessages, {
			method: 'POST',
			body: JSON.stringify({ ChatName: chatName }),
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		})
			.then(response => response.json())
			.then(response => response.forEach(postSentChats))
			.catch(error => caughtErrors(error))


		const queuesRequest = `../API/GetSenderQueues/${chatName}`;
		await fetch(queuesRequest, {
			method: 'POST',
			body: JSON.stringify({ ChatName: chatName }),
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		})
			/*https://stackoverflow.com/questions/18614301/keep-overflow-div-scrolled-to-bottom-unless-user-scrolls-up*/
			.then(response => response.json())
			.then(response => response.forEach(postQueuedChats))
			.then(response => {
				setTimeout(() => { scrollBottom(); }, 0);
			})
			.catch(error => caughtErrors(error))


		function scrollBottom() {
			const chatContainer = document.getElementById("leftinnerdiv");
			chatContainer.scrollTop = chatContainer.scrollHeight;
		}
	}

	function GetDate(SentDate) {
		const dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
		const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec"];
		const date = new Date(SentDate);
		let monthNum = date.getMonth();
		const monthDate = date.getDate();
		let dayNum = date.getDay();
		const weekDay = dayNames[dayNum];
		const monthDay = monthNames[monthNum];
		const dateString = weekDay + " " + monthDay + " " + monthDate + ": ";

		return dateString;
	}
	function GetTime(SentDate) {
		const date = new Date(SentDate);
		let hour = date.getHours();
		hour = (hour % 12) || 12
		const minute = date.getMinutes();
		const time = + hour + ":" + minute;

		return time;
	}

	function postQueuedChats(item, index, arr) {
		const dateString = GetDate(item.messageDate);
		const time = GetTime(item.messageDate);
		let chatPostName = $("<div />").text(item.senderChatName).html();
		let sendName = item.senderChatName;
		let message = $("<div />").text(item.userMessage).html();
		let timeStamp = $("<div />").text(dateString).html();
		let storedChatName = sessionStorage.getItem('UserChatName');
		if (sendName !== storedChatName) {
			$("#leftinnerdiv").append("<li><strong>" + chatPostName + " (Sent) </strong>: " + message + "</li>");
		}
		else {
			$("#leftinnerdiv").append('<li><strong><span class="senderColor">You (Sent)</span></strong>:<span class="messageLoad">' + message + "</span></li>");
			$("#leftinnerdiv").append('<li class="timestampReceiver" ><strong>' + timeStamp + '<span class="timeFontColor"> ' + time + '</span></strong></li>');
		}
	};

	function postSentChats(item, index, arr) {
		const dateString = GetDate(item.messageDate);
		const time = GetTime(item.messageDate);
		let chatPostName = $("<div />").text(item.senderChatName).html();
		let sendName = item.senderChatName;
		let message = $("<div />").text(item.userMessage).html();
		let timeStamp = $("<div />").text(dateString).html();
		let storedChatName = sessionStorage.getItem('UserChatName');
		if (sendName !== storedChatName) {
			$("#leftinnerdiv").append('<li><strong>' + chatPostName + '</strong>: <span class="messageLoad">' + message + "</span></li>");
			$("#leftinnerdiv").append('<li class="timestampReceiver" ><strong>' + timeStamp + '<span class="timeFontColor"> ' + time + '</span></strong></li>');
		}
		else {
			$("#leftinnerdiv").append('<li><strong><span class="senderColor">You (Read)</span></strong>: <span class="messageLoad">' + message + "</span></li>");
			$("#leftinnerdiv").append('<li class="timestampSender" ><strong>' + timeStamp + '<span class="timeFontColor"> ' + time + '</span></strong></li>');

		}
	};
	//=================SIGNALR METHODS==================//

	//https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-8.0
	//https://www.roundthecode.com/dotnet-tutorials/add-signalr-hub-aspnet-core-connect-using-javascript

	var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
	connection.on("ReceiveMessage", async function (user, message) {
		const date = new Date();
		const timeStamp = GetDate(date);
		const time = GetTime(date);
		var encodedUser = $("<div />").text(user).html();
		var encodedMsg = $("<div />").text(message).html();
		var currentChatUser = sessionStorage.getItem('CurrentChatName')
		var namesList = await fetchChatNameList();
		let isRegistered = await IsReceiverRegistered(currentChatUser);
		var isLoggedIn = await GetLoginState();
		var chatListButtons = document.getElementById("chatList");
		var hasNoButton = false;
		//https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelectorAll
		console.log(chatListButtons);
		if (isLoggedIn) {
			hasNoButton = chatListButtons.querySelectorAll(`button[name-arg="${user}"]`).length == 0;
		}
		console.log(namesList);
		const loginState = await GetLoginState();
		console.log("Message received from: ", encodedUser, "Message: ", encodedMsg);
		if (user == currentChatUser || loginState == false) {
			$("#leftinnerdiv").append("<li><strong>" + encodedUser + '</strong>:<span class="messageLoad">' + encodedMsg + "</span></li>");
			$("#leftinnerdiv").append('<li class="timestampReceiver" ><strong>' + timeStamp + '<span class="timeFontColor"> ' + time + '</span></strong></li>');
		}
		if (hasNoButton) {
			$("#chatList").append('<button id="nameButton" class="btn btn-primary" onclick="GetMessages(event)" name-arg="' + user + '">' + user + "</button>");
		}
	});

	connection.on("NoOne", function (user, message) {
		var encodedUser = $("<div />").text(user).html();
		var encodedMsg = $("<div />").text(message).html();
		$("#leftinnerdiv").append("<li><strong>" + encodedUser + "</strong> is not a registered user nor are they a guest.</li>");

	});
	connection.on("MessageQueued", async function (receiver, message) {
		var encodedUser = $("<div />").text(receiver).html();
		console.log(encodedUser, " is offline.Your message has been queued until they return.");
		let name = sessionStorage.getItem('UserChatName');
		const loginState = await GetLoginState(name)
		if (loginState == true) {
			$("#leftinnerdiv").append("<li><strong>" + encodedUser + "</strong> is not online. They will review your message when they return.</li>");
		} else {
			$("#leftinnerdiv").append("<li><strong>" + encodedUser + "</strong> is not online. Due to your guest credentials, your message cannot be delivered. Register or Login to have your message queued for their return.</li>");
		}
	});

	connection.on("heartbeat", async function (isAlive) {
		console.log("pulse received");
		connection.invoke("HeartBeatResponse")
			.catch(function (err) {
				return console.error(err.toString());
			});
	});

	connection.on("PrintQueuedMessages", function (messages) {
		messages.forEach(printMsgs)
		function printMsgs(item, index, arr) {
			let chatName = $("<div />").text(item.senderChatName).html();
			let message = $("<div />").text(item.userMessage).html();
			$("#leftinnerdiv").append("<li><strong>" + chatName + "</strong>: " + message + "</li>");
		}
	});
	connection.on("ServerDisconnect", function (user) {
		var encodedUser = $("<div />").text(user).html();
		console.log("User: ", encodedUser, " disconnected.The server timed out");

		$("#leftinnerdiv").append("<li><strong>Server Response Timed Out. You have been disconnected.</strong></li>");
	});
	//===================SEND BUTTON====================//
	$("#sendButton").click(async function () {
		const date = new Date();
		let timeStamp = GetDate(date);
		let time = GetTime(date);
		let names = [];
		let nameList = [];
		var username = "";
		if (!username) {
			username = $("#username").val();
		}
		var message = $("#message").val();
		if (username && message) {
			username = $("#username").val();

			const loginState = await GetLoginState();
			if (loginState == true) {
				namesList = await fetchChatNameList();
				var chatListButtons = document.getElementById("chatList");
				var hasNoButton = false;
				hasNoButton = chatListButtons.querySelectorAll(`button[name-arg="${username}"]`).length == 0;

				let isRegistered = await IsReceiverRegistered(username);
				await GetMessages(event, username)
				if (!namesList.includes(username)) { 
					$("#chatList").append('<button id="nameButton" class="btn btn-primary" onclick="GetMessages(event)" name-arg="' + username + '">' + username + "</button>");
				}
			}


			var encodedMsg = $("<div />").text(message).html();
			$("#leftinnerdiv").append('<li><strong><span class="senderColor">You</span></strong>: <span class="messageLoad"> ' + encodedMsg + "</span></li>");
			$("#leftinnerdiv").append('<li class="timestampSender" ><strong>' + timeStamp + '<span class="timeFontColor"> ' + time + '</span></strong></li>');

			connection.invoke("SendMessage", username, message)
				.catch(function (err) {
					return console.error(err.toString());
				});
			$("#message").val("").focus();
		} else if (!username) {
			alert("Please enter your username!");
		}
	})
	/*https://stackoverflow.com/questions/45018338/javascript-fetch-api-how-to-save-output-to-variable-as-an-object-not-the-prom*/

	async function IsReceiverRegistered(username) {
		const request = `/IsReceiverRegistered/${username}`
		const response = await fetch(request, {
			method: 'POST',
			body: JSON.stringify({ Username: username }),
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		}).then(response => response.json())
		  .catch(error => caughtErrors(error))

		return response;
	}	

	async function GetLoginState() {
		let storedChatName = sessionStorage.getItem('UserChatName');
		const request = `/GetLoginState/${storedChatName}`
		const response = await fetch(request, {
			method: 'POST',
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		}).then(response => response.json())
		  .catch(error => caughtErrors(error))

		return response;
	}

	async function fetchChatNameList() {
		const request = '../API/GetUserChatList';
		const response = await fetch(request, {
			method: 'POST',
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		})	.then(response => response.json())
			.catch(error => caughtErrors(error))

		return response;
	}

	/*https://stackoverflow.com/questions/9643311/pass-a-string-parameter-in-an-onclick-function
	https://stackoverflow.com/questions/1276870/how-to-pass-an-event-object-to-a-function-in-javascript
	https://stackoverflow.com/questions/39144210/pass-a-variable-to-foreach-function*/

	//==============Connection Start===================//

	connection.start().then(async function () {
		console.log("Connected!");
		const request = '../API/GetUserChatList';
		await fetch(request, {
			method: 'POST',
			headers: {
				'Accept': 'application/json; charset=utf-8',
				'Content-Type': 'application/json;charset=UTF-8'
			}
		})
			.then(response => response.json())
			.then(json => json.forEach(getUserChats))
			.catch(error => caughtErrors(error))

	});

	/*https://stackoverflow.com/questions/9643311/pass-a-string-parameter-in-an-onclick-function
	https://stackoverflow.com/questions/1276870/how-to-pass-an-event-object-to-a-function-in-javascript
	https://stackoverflow.com/questions/39144210/pass-a-variable-to-foreach-function*/

	function caughtErrors(error) {
		console.log(error.toString());
	}


	function getUserChats(item, index, arr) {
		let name = item;
		$("#chatList").append('<button id="nameButton" class="btn btn-primary" onclick="GetMessages(event)" name-arg="' + name + '">' + name + "</button>");

	}