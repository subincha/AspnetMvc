using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Data.Migrations
{
    public partial class ChangesInExtenstionNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "JobDetails",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "JobDetails",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "ExtensionNumber",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "JobDetails",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "JobDetails",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExtensionNumber",
                table: "Employees",
                nullable: false);
        }
    }
}
