using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsFeed.Data.Migrations
{
    public partial class DEV_Seed_Comments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Random rnd = new Random();
            for (int j = 0; j < 500; j++)
            {
                migrationBuilder.InsertData(
                    table: "Comment",
                    columns: new[] { "Id", "Content", "News_Id", "User_Id" },
                    values: new object[] { j, j+" <- Numer = Tutaj jest przykładowy komentarz. Suspendisse finibus consequat est, vel elementum mauris tempus eget. In cursus dui velit, non vestibulum dolor.",
                        rnd.Next(0,180), "ce80db10-7845-44c3-9409-d3d610da3b0e"+rnd.Next(0, 10)});
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 500; i++)
            {
                migrationBuilder.DeleteData(
                    table: "Comment",
                    keyColumn: "Id",
                    keyValue: i);
            }
        }
    }
}
