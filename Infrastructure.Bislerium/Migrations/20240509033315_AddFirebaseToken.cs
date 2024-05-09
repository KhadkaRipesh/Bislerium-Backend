using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Bislerium.Migrations
{
    /// <inheritdoc />
    public partial class AddFirebaseToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "ID",
                keyValue: new Guid("e42f073a-3d73-4ffe-8ca9-7c9881b0f860"));

            migrationBuilder.CreateTable(
                name: "FirebaseToken",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirebaseToken", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FirebaseToken_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "ID", "Email", "Image", "IsActive", "IsDeleted", "Password", "Role", "UserName" },
                values: new object[] { new Guid("a072d4af-6d46-42b0-9bfb-e9dd9ab9bac0"), "admin@gmail.com", "", true, false, "$2a$11$JntQgaO133Fx6yLefSEAQ.xx6Y11QErRnB8F9/hbFtbGoWXhvjANi", 0, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_FirebaseToken_UserID",
                table: "FirebaseToken",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirebaseToken");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "ID",
                keyValue: new Guid("a072d4af-6d46-42b0-9bfb-e9dd9ab9bac0"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "ID", "Email", "Image", "IsActive", "IsDeleted", "Password", "Role", "UserName" },
                values: new object[] { new Guid("e42f073a-3d73-4ffe-8ca9-7c9881b0f860"), "admin@gmail.com", "", true, false, "$2a$11$1o.nvR8GlzWAe6VxCFhzsekqXD66RedG6nnIVyvYDiI7drE7eT3V6", 0, "admin" });
        }
    }
}
