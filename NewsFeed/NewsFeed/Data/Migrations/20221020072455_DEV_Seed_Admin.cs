using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsFeed.Data.Migrations
{
    public partial class DEV_Seed_Admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email",
                    "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp",
                    "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd",
                    "LockoutEnabled", "AccessFailedCount"},
                values: new object[] { "ce80db10-7845-44c3-9409-d3d610da3b0e", "dominikjakobsze00@gmail.com", "DOMINIKJAKOBSZE00@GMAIL.COM",
                    "dominikjakobsze00@gmail.com", "DOMINIKJAKOBSZE00@GMAIL.COM", true, "AQAAAAEAACcQAAAAELk5lhE6VuxPSIvSzfgCPckJUop4Q85K8/2Z3o6Hnq2UTV/Y5Gg7N9CasDxSmTbhGQ==",
                    "VCYV7UH6Z5BU4NNHBUADLM2G5FPXLAVJ", "9b68c052-b21c-45bf-95a0-dd20c9584f0c", null, false, false, null, true, 0});

            migrationBuilder.InsertData(
                table: "AdminRole",
                columns: new[] { "Id", "User_Id", "IsAdmin" },
                values: new object[] { 0, "ce80db10-7845-44c3-9409-d3d610da3b0e", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce80db10-7845-44c3-9409-d3d610da3b0e");

            migrationBuilder.DeleteData(
                table: "AdminRole",
                keyColumn: "Id",
                keyValue: 0);
        }
    }
}
