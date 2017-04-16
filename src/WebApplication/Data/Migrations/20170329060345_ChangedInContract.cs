using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Data.Migrations
{
    public partial class ChangedInContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsProbation",
                table: "Contracts",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ContractType",
                table: "Contracts",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractType",
                table: "Contracts");

            migrationBuilder.AlterColumn<int>(
                name: "IsProbation",
                table: "Contracts",
                nullable: false);
        }
    }
}
