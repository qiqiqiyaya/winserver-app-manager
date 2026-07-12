using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppManager.Migrations
{
    /// <inheritdoc />
    public partial class AddIisInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_IisSites_SiteName",
                table: "App_IisSites");

            migrationBuilder.AddColumn<Guid>(
                name: "IisInstanceId",
                table: "App_IisSites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "App_IisInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConfigPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_IisInstances", x => x.Id);
                });

            // 种子数据：默认 IIS 实例
            var defaultInstanceId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "App_IisInstances",
                columns: new[] { "Id", "Name", "ConfigPath", "Status", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted" },
                values: new object[] { defaultInstanceId, "Default", "", "Connected", "{}", Guid.NewGuid().ToString(), DateTime.UtcNow, false });

            // 将现有站点关联到默认实例
            migrationBuilder.Sql($"UPDATE App_IisSites SET IisInstanceId = '{defaultInstanceId}'");

            migrationBuilder.CreateIndex(
                name: "IX_App_IisSites_IisInstanceId_SiteName",
                table: "App_IisSites",
                columns: new[] { "IisInstanceId", "SiteName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_IisInstances_ConfigPath",
                table: "App_IisInstances",
                column: "ConfigPath",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_App_IisSites_App_IisInstances_IisInstanceId",
                table: "App_IisSites",
                column: "IisInstanceId",
                principalTable: "App_IisInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_IisSites_App_IisInstances_IisInstanceId",
                table: "App_IisSites");

            migrationBuilder.DropTable(
                name: "App_IisInstances");

            migrationBuilder.DropIndex(
                name: "IX_App_IisSites_IisInstanceId_SiteName",
                table: "App_IisSites");

            migrationBuilder.DropColumn(
                name: "IisInstanceId",
                table: "App_IisSites");

            migrationBuilder.CreateIndex(
                name: "IX_App_IisSites_SiteName",
                table: "App_IisSites",
                column: "SiteName",
                unique: true);
        }
    }
}
