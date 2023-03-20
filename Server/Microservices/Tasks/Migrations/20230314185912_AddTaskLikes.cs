using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserIdRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Task_Id = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserIdRecords_Tasks_Task_Id",
                        column: x => x.Task_Id,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdRecords_Task_Id",
                table: "UserIdRecords",
                column: "Task_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserIdRecords");
        }
    }
}
