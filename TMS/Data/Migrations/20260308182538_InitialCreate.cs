using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DOB",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePic",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ThesisId1",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThesisName1",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Faculty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Thesis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Materials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ExpectedHours = table.Column<int>(type: "int", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thesis", x => new { x.Id, x.Name });
                    table.ForeignKey(
                        name: "FK_Thesis_Faculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThesisName = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Language_Thesis_ThesisId_ThesisName",
                        columns: x => new { x.ThesisId, x.ThesisName },
                        principalTable: "Thesis",
                        principalColumns: new[] { "Id", "Name" });
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThesisName = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Thesis_ThesisId_ThesisName",
                        columns: x => new { x.ThesisId, x.ThesisName },
                        principalTable: "Thesis",
                        principalColumns: new[] { "Id", "Name" });
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThesisName = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Student_Faculty_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculty",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Thesis_ThesisId_ThesisName",
                        columns: x => new { x.ThesisId, x.ThesisName },
                        principalTable: "Thesis",
                        principalColumns: new[] { "Id", "Name" });
                });

            migrationBuilder.CreateTable(
                name: "Grade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Midterm = table.Column<int>(type: "int", nullable: false),
                    Exam = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ThesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThesisName = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grade_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Grade_Thesis_ThesisId_ThesisName",
                        columns: x => new { x.ThesisId, x.ThesisName },
                        principalTable: "Thesis",
                        principalColumns: new[] { "Id", "Name" });
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ThesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThesisName = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Request_Thesis_ThesisId_ThesisName",
                        columns: x => new { x.ThesisId, x.ThesisName },
                        principalTable: "Thesis",
                        principalColumns: new[] { "Id", "Name" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ThesisId1_ThesisName1",
                table: "AspNetUsers",
                columns: new[] { "ThesisId1", "ThesisName1" });

            migrationBuilder.CreateIndex(
                name: "IX_Grade_StudentId",
                table: "Grade",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grade_ThesisId_ThesisName",
                table: "Grade",
                columns: new[] { "ThesisId", "ThesisName" });

            migrationBuilder.CreateIndex(
                name: "IX_Language_ThesisId_ThesisName",
                table: "Language",
                columns: new[] { "ThesisId", "ThesisName" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ThesisId_ThesisName",
                table: "Notification",
                columns: new[] { "ThesisId", "ThesisName" });

            migrationBuilder.CreateIndex(
                name: "IX_Request_StudentId",
                table: "Request",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ThesisId_ThesisName",
                table: "Request",
                columns: new[] { "ThesisId", "ThesisName" });

            migrationBuilder.CreateIndex(
                name: "IX_Student_FacultyId",
                table: "Student",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ThesisId_ThesisName",
                table: "Student",
                columns: new[] { "ThesisId", "ThesisName" });

            migrationBuilder.CreateIndex(
                name: "IX_Thesis_FacultyId",
                table: "Thesis",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Thesis_ThesisId1_ThesisName1",
                table: "AspNetUsers",
                columns: new[] { "ThesisId1", "ThesisName1" },
                principalTable: "Thesis",
                principalColumns: new[] { "Id", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Thesis_ThesisId1_ThesisName1",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Grade");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Thesis");

            migrationBuilder.DropTable(
                name: "Faculty");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ThesisId1_ThesisName1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePic",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ThesisId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ThesisName1",
                table: "AspNetUsers");
        }
    }
}
