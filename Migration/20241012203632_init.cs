using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTalkerMessenger.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserIpAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUserName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isLoaded = table.Column<bool>(type: "bit", nullable: false),
                    MsgSenderEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MsgReceiverEmail = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatMessageId);
                    table.ForeignKey(
                        name: "FK_Chats_AspNetUsers_MsgReceiverEmail",
                        column: x => x.MsgReceiverEmail,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chats_AspNetUsers_MsgSenderEmail",
                        column: x => x.MsgSenderEmail,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Queues",
                columns: table => new
                {
                    MessageQueueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isLoaded = table.Column<bool>(type: "bit", nullable: false),
                    MsgSenderEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MsgReceiverEmail = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.MessageQueueId);
                    table.ForeignKey(
                        name: "FK_Queues_AspNetUsers_MsgReceiverEmail",
                        column: x => x.MsgReceiverEmail,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Queues_AspNetUsers_MsgSenderEmail",
                        column: x => x.MsgSenderEmail,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_MsgReceiverEmail",
                table: "Chats",
                column: "MsgReceiverEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_MsgSenderEmail",
                table: "Chats",
                column: "MsgSenderEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Queues_MsgReceiverEmail",
                table: "Queues",
                column: "MsgReceiverEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Queues_MsgSenderEmail",
                table: "Queues",
                column: "MsgSenderEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Queues");

            migrationBuilder.DropColumn(
                name: "UserIpAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserUserName",
                table: "AspNetUsers");
        }
    }
}
