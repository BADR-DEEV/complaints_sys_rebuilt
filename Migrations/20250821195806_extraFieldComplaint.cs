using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace complaints_back.Migrations
{
    /// <inheritdoc />
    public partial class extraFieldComplaint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComplaintMessage",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplaintMessage",
                table: "Complaints");
        }
    }
}
