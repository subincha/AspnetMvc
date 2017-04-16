using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Data.Migrations
{
    public partial class RemoveRequiredFieldsInPersonalDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "PersonalInfo",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tole",
                table: "PersonalInfo",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "PersonalInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "PersonalInfo",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Tole",
                table: "PersonalInfo",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "PersonalInfo",
                nullable: false);
        }
    }
}
