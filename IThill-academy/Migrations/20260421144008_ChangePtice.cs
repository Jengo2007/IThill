using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThill_academy.Migrations
{
    /// <inheritdoc />
    public partial class ChangePtice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Courses"" 
      ALTER COLUMN ""Price"" TYPE numeric 
      USING ""Price""::numeric;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
