using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTalkerMessenger.Data.Migrations
{
    public partial class removedMessageUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageFamilyUID",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "MessageFamilyUID",
                table: "Chats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageFamilyUID",
                table: "Queues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageFamilyUID",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
