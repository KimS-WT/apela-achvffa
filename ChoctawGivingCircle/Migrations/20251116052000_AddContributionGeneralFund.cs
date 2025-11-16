using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoctawGivingCircle.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionGeneralFund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_AssistanceRequests_AssistanceRequestId",
                table: "Contributions");

            migrationBuilder.AlterColumn<int>(
                name: "AssistanceRequestId",
                table: "Contributions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ContributionType",
                table: "Contributions",
                type: "TEXT",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralFund",
                table: "Contributions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Contributions",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_DonorUserId",
                table: "Contributions",
                column: "DonorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_AspNetUsers_DonorUserId",
                table: "Contributions",
                column: "DonorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_AssistanceRequests_AssistanceRequestId",
                table: "Contributions",
                column: "AssistanceRequestId",
                principalTable: "AssistanceRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_AspNetUsers_DonorUserId",
                table: "Contributions");

            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_AssistanceRequests_AssistanceRequestId",
                table: "Contributions");

            migrationBuilder.DropIndex(
                name: "IX_Contributions_DonorUserId",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "IsGeneralFund",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Contributions");

            migrationBuilder.AlterColumn<int>(
                name: "AssistanceRequestId",
                table: "Contributions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContributionType",
                table: "Contributions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 120);

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_AssistanceRequests_AssistanceRequestId",
                table: "Contributions",
                column: "AssistanceRequestId",
                principalTable: "AssistanceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
