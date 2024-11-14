using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sail.EntityFramework.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cert = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Key = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(4610), new TimeSpan(0, 8, 0, 0, 0))),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(4960), new TimeSpan(0, 8, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LoadBalancingPolicy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(2190), new TimeSpan(0, 8, 0, 0, 0))),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 519, DateTimeKind.Unspecified).AddTicks(2570), new TimeSpan(0, 8, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteMatch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Methods = table.Column<string>(type: "text", nullable: true),
                    Hosts = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteMatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sni",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HostName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CertificateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 520, DateTimeKind.Unspecified).AddTicks(9710), new TimeSpan(0, 8, 0, 0, 0))),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 521, DateTimeKind.Unspecified).AddTicks(240), new TimeSpan(0, 8, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sni_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Health = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Host = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destination_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Values = table.Column<string>(type: "text", nullable: true),
                    Mode = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "boolean", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteHeader_RouteMatch_MatchId",
                        column: x => x.MatchId,
                        principalTable: "RouteMatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteQueryParameter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Values = table.Column<string>(type: "text", nullable: true),
                    Mode = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "boolean", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteQueryParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteQueryParameter_RouteMatch_MatchId",
                        column: x => x.MatchId,
                        principalTable: "RouteMatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    AuthorizationPolicy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RateLimiterPolicy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CorsPolicy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TimeoutPolicy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Timeout = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MaxRequestBodySize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 518, DateTimeKind.Unspecified).AddTicks(5010), new TimeSpan(0, 8, 0, 0, 0))),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(2024, 11, 12, 16, 16, 20, 518, DateTimeKind.Unspecified).AddTicks(5530), new TimeSpan(0, 8, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_RouteMatch_MatchId",
                        column: x => x.MatchId,
                        principalTable: "RouteMatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destination_ClusterId",
                table: "Destination",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteHeader_MatchId",
                table: "RouteHeader",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteQueryParameter_MatchId",
                table: "RouteQueryParameter",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_MatchId",
                table: "Routes",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Sni_CertificateId",
                table: "Sni",
                column: "CertificateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destination");

            migrationBuilder.DropTable(
                name: "RouteHeader");

            migrationBuilder.DropTable(
                name: "RouteQueryParameter");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Sni");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropTable(
                name: "RouteMatch");

            migrationBuilder.DropTable(
                name: "Certificates");
        }
    }
}
