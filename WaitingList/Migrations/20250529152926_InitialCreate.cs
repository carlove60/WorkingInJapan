using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaitingList.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WaitingLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    TotalSeatsAvailable = table.Column<int>(type: "int", nullable: false),
                    TimeForService = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingLists", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    IsReady = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsServiceEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    WaitingListModelId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parties_WaitingLists_WaitingListModelId",
                        column: x => x.WaitingListModelId,
                        principalTable: "WaitingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_WaitingListModelId",
                table: "Parties",
                column: "WaitingListModelId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingLists_Name",
                table: "WaitingLists",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropTable(
                name: "WaitingLists");
        }
    }
}
