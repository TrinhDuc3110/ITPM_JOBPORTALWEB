using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOBPORTALWEB.INFRASTRUCTURE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_AspNetUsers_JobSeekerId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Jobs_JobId",
                table: "JobApplications");

            migrationBuilder.CreateTable(
                name: "SavedCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecruiterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedCandidates_AspNetUsers_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedCandidates_AspNetUsers_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    JobSeekerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedJobs_AspNetUsers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCandidates_CandidateId",
                table: "SavedCandidates",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCandidates_RecruiterId_CandidateId",
                table: "SavedCandidates",
                columns: new[] { "RecruiterId", "CandidateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_JobId_JobSeekerId",
                table: "SavedJobs",
                columns: new[] { "JobId", "JobSeekerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_JobSeekerId",
                table: "SavedJobs",
                column: "JobSeekerId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_AspNetUsers_JobSeekerId",
                table: "JobApplications",
                column: "JobSeekerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Jobs_JobId",
                table: "JobApplications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_AspNetUsers_JobSeekerId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Jobs_JobId",
                table: "JobApplications");

            migrationBuilder.DropTable(
                name: "SavedCandidates");

            migrationBuilder.DropTable(
                name: "SavedJobs");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_AspNetUsers_JobSeekerId",
                table: "JobApplications",
                column: "JobSeekerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Jobs_JobId",
                table: "JobApplications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
