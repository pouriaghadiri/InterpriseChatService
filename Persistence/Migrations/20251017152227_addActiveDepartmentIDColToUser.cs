using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addActiveDepartmentIDColToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActiveDepartmentId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_ActiveDepartmentId",
            //    table: "Users",
            //    column: "ActiveDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_ActiveDepartmentId",
                table: "Users",
                column: "ActiveDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_ActiveDepartmentId",
                table: "Users");

            //migrationBuilder.DropIndex(
            //    name: "IX_Users_ActiveDepartmentId",
            //    table: "Users");

            migrationBuilder.DropColumn(
                name: "ActiveDepartmentId",
                table: "Users");
        }
    }
}
