using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoctawGivingCircle.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationDonationDropOffEmailLogAndFundApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipientEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    EmailType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RelatedContributionId = table.Column<int>(type: "INTEGER", nullable: true),
                    RelatedAssistanceRequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    RelatedUserId = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_AssistanceRequests_RelatedAssistanceRequestId",
                        column: x => x.RelatedAssistanceRequestId,
                        principalTable: "AssistanceRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmailLogs_Contributions_RelatedContributionId",
                        column: x => x.RelatedContributionId,
                        principalTable: "Contributions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FundAllocationApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContributionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssistanceRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedByAdminId = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ApprovedByAdminId = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundAllocationApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundAllocationApprovals_AspNetUsers_ApprovedByAdminId",
                        column: x => x.ApprovedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FundAllocationApprovals_AspNetUsers_RequestedByAdminId",
                        column: x => x.RequestedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundAllocationApprovals_AssistanceRequests_AssistanceRequestId",
                        column: x => x.AssistanceRequestId,
                        principalTable: "AssistanceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundAllocationApprovals_Contributions_ContributionId",
                        column: x => x.ContributionId,
                        principalTable: "Contributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Hours = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonationDropOffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContributionId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RescheduleToken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationDropOffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationDropOffs_Contributions_ContributionId",
                        column: x => x.ContributionId,
                        principalTable: "Contributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonationDropOffs_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationDropOffs_ContributionId",
                table: "DonationDropOffs",
                column: "ContributionId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationDropOffs_LocationId",
                table: "DonationDropOffs",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_RelatedAssistanceRequestId",
                table: "EmailLogs",
                column: "RelatedAssistanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_RelatedContributionId",
                table: "EmailLogs",
                column: "RelatedContributionId");

            migrationBuilder.CreateIndex(
                name: "IX_FundAllocationApprovals_ApprovedByAdminId",
                table: "FundAllocationApprovals",
                column: "ApprovedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_FundAllocationApprovals_AssistanceRequestId",
                table: "FundAllocationApprovals",
                column: "AssistanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FundAllocationApprovals_ContributionId",
                table: "FundAllocationApprovals",
                column: "ContributionId");

            migrationBuilder.CreateIndex(
                name: "IX_FundAllocationApprovals_RequestedByAdminId",
                table: "FundAllocationApprovals",
                column: "RequestedByAdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationDropOffs");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "FundAllocationApprovals");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
