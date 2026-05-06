using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAssignment.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentNumber",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNumber",
                table: "Students",
                column: "StudentNumber",
                unique: true,
                filter: "\"StudentNumber\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_StudentNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentNumber",
                table: "Students");
        }
    }
}
