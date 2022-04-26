using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_hotel_1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassList_EF",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassList_EF", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms_EF",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Room_empty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Number_of_places = table.Column<int>(type: "int", nullable: false),
                    ClassIDId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms_EF", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_EF_ClassList_EF_ClassIDId",
                        column: x => x.ClassIDId,
                        principalTable: "ClassList_EF",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_EF_ClassIDId",
                table: "Rooms_EF",
                column: "ClassIDId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms_EF");

            migrationBuilder.DropTable(
                name: "ClassList_EF");
        }
    }
}
