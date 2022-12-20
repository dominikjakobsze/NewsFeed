using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsFeed.Data.Migrations
{
    public partial class DEV_Seed_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string[] CategoryArray = { "Polska", "Sport", "World", "Konflikty", "Polityka", "Gospodarka" };
            for (int i = 0; i < CategoryArray.Length; i++)
            {
                migrationBuilder.InsertData(
                    table: "Category",
                    columns: new[] { "Id", "Name" },
                    values: new object[] { i, CategoryArray[i] });
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string[] CategoryArray = { "Polska", "Sport", "World", "Konflikty", "Polityka", "Gospodarka" };
            for (int i = 0; i < CategoryArray.Length; i++)
            {
                migrationBuilder.DeleteData(
                    table: "Category",
                    keyColumn: "Id",
                    keyValue: i);
            }
        }
    }
}
