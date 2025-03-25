using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TunaBouzouki.Migrations
{
    /// <inheritdoc />
    public partial class AddSongGenreJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GenreSong",
                columns: new[] { "GenresId", "SongsId" },
                values: new object[,]
                {
                    { 1, 5 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 1 }
                });

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Album",
                value: "English Fold Classics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenreSong",
                keyColumns: new[] { "GenresId", "SongsId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "GenreSong",
                keyColumns: new[] { "GenresId", "SongsId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "GenreSong",
                keyColumns: new[] { "GenresId", "SongsId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "GenreSong",
                keyColumns: new[] { "GenresId", "SongsId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "GenreSong",
                keyColumns: new[] { "GenresId", "SongsId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Album",
                value: "English Folk Classics");
        }
    }
}
