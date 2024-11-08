using System;
using System.Collections.Generic;
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
                name: "Clusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LoadBalancingPolicy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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
                    Methods = table.Column<List<string>>(type: "text[]", nullable: false),
                    Hosts = table.Column<List<string>>(type: "text[]", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteMatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Health = table.Column<string>(type: "text", nullable: true),
                    Host = table.Column<string>(type: "text", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destination_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RouteHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Values = table.Column<List<string>>(type: "text[]", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    RouteMatchId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteHeader_RouteMatch_RouteMatchId",
                        column: x => x.RouteMatchId,
                        principalTable: "RouteMatch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RouteQueryParameter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Values = table.Column<List<string>>(type: "text[]", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    RouteMatchId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteQueryParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteQueryParameter_RouteMatch_RouteMatchId",
                        column: x => x.RouteMatchId,
                        principalTable: "RouteMatch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    AuthorizationPolicy = table.Column<string>(type: "text", nullable: true),
                    RateLimiterPolicy = table.Column<string>(type: "text", nullable: true),
                    CorsPolicy = table.Column<string>(type: "text", nullable: true),
                    TimeoutPolicy = table.Column<string>(type: "text", nullable: true),
                    Timeout = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MaxRequestBodySize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "WeightedCluster",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClusterId = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightedCluster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightedCluster_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destination_ClusterId",
                table: "Destination",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteHeader_RouteMatchId",
                table: "RouteHeader",
                column: "RouteMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteQueryParameter_RouteMatchId",
                table: "RouteQueryParameter",
                column: "RouteMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_MatchId",
                table: "Routes",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightedCluster_RouteId",
                table: "WeightedCluster",
                column: "RouteId");
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
                name: "WeightedCluster");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "RouteMatch");
        }
    }
}
