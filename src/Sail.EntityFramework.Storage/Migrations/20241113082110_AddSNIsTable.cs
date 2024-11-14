using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sail.EntityFramework.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddSNIsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sni_Certificates_CertificateId",
                table: "Sni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sni",
                table: "Sni");

            migrationBuilder.RenameTable(
                name: "Sni",
                newName: "SNI");

            migrationBuilder.RenameIndex(
                name: "IX_Sni_CertificateId",
                table: "SNI",
                newName: "IX_SNI_CertificateId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "SNI",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 521, DateTimeKind.Unspecified).AddTicks(240), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SNI",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 520, DateTimeKind.Unspecified).AddTicks(9710), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Routes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 518, DateTimeKind.Unspecified).AddTicks(5530), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Routes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 518, DateTimeKind.Unspecified).AddTicks(5010), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Clusters",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(2570), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Clusters",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(2190), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Certificates",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(4960), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Certificates",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(4610), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SNI",
                table: "SNI",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ClusterId",
                table: "Routes",
                column: "ClusterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Clusters_ClusterId",
                table: "Routes",
                column: "ClusterId",
                principalTable: "Clusters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SNI_Certificates_CertificateId",
                table: "SNI",
                column: "CertificateId",
                principalTable: "Certificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Clusters_ClusterId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_SNI_Certificates_CertificateId",
                table: "SNI");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SNI",
                table: "SNI");

            migrationBuilder.DropIndex(
                name: "IX_Routes_ClusterId",
                table: "Routes");

            migrationBuilder.RenameTable(
                name: "SNI",
                newName: "Sni");

            migrationBuilder.RenameIndex(
                name: "IX_SNI_CertificateId",
                table: "Sni",
                newName: "IX_Sni_CertificateId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Sni",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 521, DateTimeKind.Unspecified).AddTicks(240), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Sni",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 520, DateTimeKind.Unspecified).AddTicks(9710), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Routes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 518, DateTimeKind.Unspecified).AddTicks(5530), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Routes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 518, DateTimeKind.Unspecified).AddTicks(5010), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Clusters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(2570), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Clusters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(2190), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Certificates",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(4960), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Certificates",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(4610), new TimeSpan(0, 8, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sni",
                table: "Sni",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sni_Certificates_CertificateId",
                table: "Sni",
                column: "CertificateId",
                principalTable: "Certificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
