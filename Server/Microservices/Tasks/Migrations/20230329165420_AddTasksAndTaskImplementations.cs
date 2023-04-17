using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks.Migrations
{
    /// <inheritdoc />
    public partial class AddTasksAndTaskImplementations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "varchar(4096)", maxLength: 4096, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Beta = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserIdRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdRecords", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Implementations",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Language = table.Column<string>(type: "varchar(255)", nullable: false),
                    Version = table.Column<string>(type: "varchar(255)", nullable: false),
                    Details = table.Column<string>(type: "longtext", nullable: true),
                    InitialSolution = table.Column<string>(type: "longtext", nullable: false),
                    CompletedSolution = table.Column<string>(type: "longtext", nullable: false),
                    PreloadedCode = table.Column<string>(type: "longtext", nullable: true),
                    ExampleTests = table.Column<string>(type: "longtext", nullable: false),
                    Tests = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "TaskTagTask_",
                columns: table => new
                {
                    TagsId = table.Column<string>(type: "varchar(255)", nullable: false),
                    TasksId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTagTask_", x => new { x.TagsId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_TaskTagTask__Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTagTask__Tasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Implementations_Task_Id",
                table: "Implementations",
                column: "Task_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTagTask__TasksId",
                table: "TaskTagTask_",
                column: "TasksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Implementations");

            migrationBuilder.DropTable(
                name: "TaskTagTask_");

            migrationBuilder.DropTable(
                name: "UserIdRecords");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
