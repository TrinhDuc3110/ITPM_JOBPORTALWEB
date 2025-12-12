using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOBPORTALWEB.INFRASTRUCTURE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToJobApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasApplied",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "JobApplications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasApplied",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JobApplications");
        }
    }
}
