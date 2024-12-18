using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Scaffolding.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "jerneif");

            migrationBuilder.CreateTable(
                name: "game",
                schema: "jerneif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weeknumber = table.Column<int>(type: "integer", nullable: false),
                    yearnumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player",
                schema: "jerneif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activated = table.Column<bool>(type: "boolean", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "winnersequence",
                schema: "jerneif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    gameid = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sequence = table.Column<List<int>>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_winnersequence", x => x.id);
                    table.ForeignKey(
                        name: "FK_winnersequence_game_gameid",
                        column: x => x.gameid,
                        principalSchema: "jerneif",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "board",
                schema: "jerneif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    gameid = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sortednumbers = table.Column<List<int>>(type: "integer[]", nullable: false),
                    afviklet = table.Column<bool>(type: "boolean", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_board", x => x.id);
                    table.ForeignKey(
                        name: "FK_board_game_gameid",
                        column: x => x.gameid,
                        principalSchema: "jerneif",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_board_player_userid",
                        column: x => x.userid,
                        principalSchema: "jerneif",
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_board_gameid",
                schema: "jerneif",
                table: "board",
                column: "gameid");

            migrationBuilder.CreateIndex(
                name: "IX_board_userid_gameid",
                schema: "jerneif",
                table: "board",
                columns: new[] { "userid", "gameid" });

            migrationBuilder.CreateIndex(
                name: "winnersequence_gameid_key",
                schema: "jerneif",
                table: "winnersequence",
                column: "gameid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "board",
                schema: "jerneif");

            migrationBuilder.DropTable(
                name: "winnersequence",
                schema: "jerneif");

            migrationBuilder.DropTable(
                name: "player",
                schema: "jerneif");

            migrationBuilder.DropTable(
                name: "game",
                schema: "jerneif");
        }
    }
}
