using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks.Migrations
{
    /// <inheritdoc />
    public partial class AddImplementations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Implementations",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Language = table.Column<string>(type: "varchar(255)", nullable: false),
                    Version = table.Column<string>(type: "varchar(255)", nullable: false),
                    InitialSolution = table.Column<string>(type: "longtext", nullable: false),
                    CompleteSolution = table.Column<string>(type: "longtext", nullable: false),
                    PreloadedCode = table.Column<string>(type: "longtext", nullable: false),
                    ExampleTests = table.Column<string>(type: "longtext", nullable: false),
                    Tests = table.Column<string>(type: "longtext", nullable: false),
                    Task_Id = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Implementations", x => new { x.TaskId, x.Language, x.Version });
                    table.ForeignKey(
                        name: "FK_Implementations_Tasks_Task_Id",
                        column: x => x.Task_Id,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Implementations_Task_Id",
                table: "Implementations",
                column: "Task_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Implementations");
        }
    }
}
