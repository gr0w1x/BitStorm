using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndRefreshTokenRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokenRecords",
                columns: table => new
                {
                    Token = table.Column<string>(type: "varchar(255)", nullable: false),
                    Expired = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_RefreshTokenRecords", x => x.Token))
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Roles = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false),
                    Confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Avatar = table.Column<string>(type: "longtext", nullable: true),
                    Trophies = table.Column<int>(type: "int", nullable: false),
                    Registered = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    LastSeen = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Users", x => x.Id))
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokenRecords");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
