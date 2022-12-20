using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsFeed.Data.Migrations
{
    public partial class DEV_Seed_News : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string[] CategoryArray = { "Polska", "Sport", "World", "Konflikty", "Polityka", "Gospodarka" };
            var iterator = 0;
            for (int i = 0; i < CategoryArray.Length; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    var path = "";
                    if (i == 0)
                    {
                        path = "polska.jpg";
                    }
                    if (i == 1)
                    {
                        path = "sport.jpg";
                    }
                    if (i == 2)
                    {
                        path = "world.jpg";
                    }
                    if (i == 3)
                    {
                        path = "konflikty.jpg";
                    }
                    if (i == 4)
                    {
                        path = "polityka.jpg";
                    }
                    if (i == 5)
                    {
                        path = "gospodarka.jpg";
                    }
                    migrationBuilder.InsertData(
                        table: "News",
                        columns: new[] { "Id", "Title", "ImgPath", "Article", "Category_Id" },
                        values: new object[] { iterator, "Suspendisse finibus consequat est, vel elementum mauris tempus eget. In cursus dui velit, non vestibulum dolor pellentesque quis. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.",
                        path,"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras vestibulum odio et pulvinar consectetur. Suspendisse semper diam non urna pretium volutpat. Pellentesque aliquet, turpis at pellentesque tincidunt, ante sem ornare mauris, ac tincidunt lacus ante vitae ante. Ut pulvinar, quam ac ultricies fermentum, lacus nulla facilisis nisl, nec iaculis magna erat quis ligula. Donec nibh ipsum, luctus ut lorem at, convallis pretium nisl. Suspendisse quis ipsum at mauris semper cursus. Phasellus dapibus eleifend nisl, a pulvinar quam congue ut. Cras turpis est, ultricies ac blandit eget, congue a orci. Praesent mattis massa quis ipsum ullamcorper viverra non et risus. Sed sit amet elit et turpis malesuada maximus sit amet sit amet quam. Donec gravida dolor aliquam mauris blandit sodales. Praesent id enim in libero faucibus lacinia. Vestibulum sed consectetur dui. Duis at dapibus leo. Sed at arcu quis risus molestie rutrum. Nullam imperdiet tortor enim, nec sodales odio volutpat sed.\r\n<br>\r\nUt sed mauris vel ipsum elementum aliquam vel at risus. In a tortor sed velit dictum lobortis id ac ex. Integer a augue sollicitudin, porttitor odio at, finibus quam. Phasellus vel mauris non orci molestie volutpat. Ut ultricies purus magna, blandit venenatis sapien porttitor quis. Nullam in mattis libero, et bibendum mauris. Praesent a efficitur nibh, vitae volutpat dui. In imperdiet orci et finibus elementum. Nulla varius scelerisque suscipit.\r\n<br>\r\nSuspendisse finibus consequat est, vel elementum mauris tempus eget. In cursus dui velit, non vestibulum dolor pellentesque quis. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus id nisi non justo tristique faucibus. Mauris egestas sodales dolor, at rutrum quam auctor sit amet. Etiam ut tempus ipsum. Vestibulum eu augue semper, sagittis odio nec, ullamcorper tortor. Nullam luctus erat mattis neque mattis, a aliquam ipsum consequat.\r\n<br>\r\nIn quis aliquet dolor. Etiam mattis egestas felis, luctus sollicitudin odio. Etiam eget tincidunt massa. Maecenas egestas fermentum finibus. Aenean et iaculis justo. Donec sodales lorem sed ultrices mollis. Nulla facilisi.\r\n<br>\r\nNunc eu pretium diam. Ut ut luctus eros, viverra porttitor metus. Quisque eu tellus eleifend, tristique nulla vel, maximus libero. Integer mattis eros in magna rutrum, et molestie massa bibendum. Nulla ante turpis, sollicitudin ut dolor sit amet, convallis finibus ex. Sed malesuada vitae ex quis laoreet.",
                        i});
                    iterator++;
                }
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 180; i++)
            {
                migrationBuilder.DeleteData(
                    table: "News",
                    keyColumn: "Id",
                    keyValue: i);
            }
        }
    }
}
