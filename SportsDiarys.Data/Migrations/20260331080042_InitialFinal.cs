using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsDiarys.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrainingDiaries_UserProfileId",
                table: "TrainingDiaries");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDiaries_UserProfileId_Date",
                table: "TrainingDiaries",
                columns: new[] { "UserProfileId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrainingDiaries_UserProfileId_Date",
                table: "TrainingDiaries");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDiaries_UserProfileId",
                table: "TrainingDiaries",
                column: "UserProfileId");
        }
    }
}
