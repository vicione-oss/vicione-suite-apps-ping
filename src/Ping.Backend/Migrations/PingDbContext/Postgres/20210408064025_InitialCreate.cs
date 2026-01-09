using Microsoft.EntityFrameworkCore.Migrations;

namespace Ping.Backend.Migrations.PingDbContext.Postgres;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "ping");

        migrationBuilder.CreateTable(
            name: "Pings",
            schema: "ping",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                Message = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pings", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Pings",
            schema: "ping");
    }
}
