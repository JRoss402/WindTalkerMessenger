"use strict";

$(document).ready(() => {




	//https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-8.0
	//https://www.roundthecode.com/dotnet-tutorials/add-signalr-hub-aspnet-core-connect-using-javascript

	var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
	connection.on("ReceiveMessage", function (user, message) {
		var encodedUser = $("<div />").text(user).html();
		var encodedMsg = $("<div />").text(message).html();
		console.log("Message received from: ", encodedUser, "Message: ", encodedMsg);
		$("#leftinnerdiv").append("<li><strong>" + encodedUser + '</strong>:<span class="messageLoad">' + encodedMsg + "</span> < /li>");
	});

	//route the queued message here and add the (Queued Message) styling.
	connection.on("MessageQueued", function (user,message) {
		var encodedUser = $("<div />").text(user).html();
		console.log(encodedUser, " is offline.Your message has been queued until they return.");

		$("#leftinnerdiv").append("<li><strong>" + encodedUser + "</strong> has gone dark. They are currently on a mission-critical assignment and will return after completion.</li>");

	});

	connection.on("GuestGone", function (user) {
		var encodedUser = $("<div />").text(user).html();

		console.log(encodedUser, " was a guest and have disconnected.");

		$("#leftinnerdiv").append("<li><strong>" + encodedUser + "</strong> has gone rogue and removed their fingerprint from this site.</li>");
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

	$("#sendButton").click(function () {
		var username = "";
		if (!username) {
			username = $("#username").val();
		}
		var message = $("#message").val();
		if (username && message) {
			username = $("#username").val();
			var encodedMsg = $("<div />").text(message).html();
			$("#leftinnerdiv").append('<li><strong>You</strong>: <span class="messageLoad"> ' + encodedMsg + "</span></li>");
			connection.invoke("SendMessage", username, message)
				.catch(function (err) {
					return console.error(err.toString());
				});
			$("#message").val("").focus();
		} else if (!username) {
			alert("Please enter your username!");
		}
	});

	//==============Connection Start===================//

	connection.start().then(function () {
		console.log("Connected!");
		const request = '../API/GetUserChatList';
		fetch(request, {
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

	//https://stackoverflow.com/questions/9643311/pass-a-string-parameter-in-an-onclick-function
	//https://stackoverflow.com/questions/1276870/how-to-pass-an-event-object-to-a-function-in-javascript
	//https://stackoverflow.com/questions/39144210/pass-a-variable-to-foreach-function


	function getUserChats(item, index, arr) {
		let name = item;
		$("#chatList").append('<button id="nameButton" class="btn btn-primary" onclick="GetMessages(event)" name-arg="' + name + '">' + name + "</button>");

	}



}
)