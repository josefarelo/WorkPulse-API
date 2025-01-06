using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkPulse.Migrations
{
    /// <inheritdoc />
    public partial class FixCommitModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "PullRequests");

            migrationBuilder.RenameColumn(
                name: "Sha",
                table: "Commits",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PullRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PullRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "PullRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HtmlUrl",
                table: "PullRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "PullRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HtmlUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_AuthorId",
                table: "PullRequests",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PullRequests_User_AuthorId",
                table: "PullRequests",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PullRequests_User_AuthorId",
                table: "PullRequests");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_AuthorId",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "HtmlUrl",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "State",
                table: "PullRequests");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Commits",
                newName: "Sha");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PullRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PullRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "PullRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "PullRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
