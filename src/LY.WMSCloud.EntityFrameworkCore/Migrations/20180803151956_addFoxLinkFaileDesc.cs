using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LY.WMSCloud.Migrations
{
    public partial class addFoxLinkFaileDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoxLinkFaileDesc",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ExtensionData = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoxLinkFaileDesc", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoxLinkFaileDesc_Text",
                table: "FoxLinkFaileDesc",
                column: "Text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoxLinkFaileDesc");
        }
    }
}
