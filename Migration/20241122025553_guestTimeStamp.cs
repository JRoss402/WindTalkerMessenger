using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTalkerMessenger.Data.Migrations
{
    public partial class guestTimeStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "Guests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "Guests");
        }
    }
}
