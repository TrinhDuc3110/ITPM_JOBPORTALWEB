using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOBPORTALWEB.INFRASTRUCTURE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDownloadedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDownloaded",
                table: "JobApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDownloaded",
                table: "JobApplications");
        }
    }
}
