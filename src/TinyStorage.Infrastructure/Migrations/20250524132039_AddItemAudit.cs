using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itmo.TinyStorage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_audits",
                schema: "tiny_storage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    edited_by = table.Column<int>(type: "integer", nullable: false),
                    property = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_audits", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_audits_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "tiny_storage",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_item_audits_item_id",
                schema: "tiny_storage",
                table: "item_audits",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_audits",
                schema: "tiny_storage");
        }
    }
}
