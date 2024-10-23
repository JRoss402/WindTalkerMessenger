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
                name: "ChatName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageFamilyUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    MessageSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "Guest"),
                    MessageReceiverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "Guest"),
                    SenderChatName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverChatName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    GuestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.GuestId);
                });

            migrationBuilder.CreateTable(
                name: "Queues",
                columns: table => new
                {
                    MessageQueueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageFamilyUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    MessageSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "Guest"),
                    MessageReceiverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "Guest"),
                    SenderChatName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverChatName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.MessageQueueId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Queues");

            migrationBuilder.DropColumn(
                name: "ChatName",
                table: "AspNetUsers");
        }
    }
}
