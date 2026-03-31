using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Migrations {

    public partial class CreateDesignationTable : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DesignationName = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO Designations ([DesignationName], [State] ) Values('Admin','1')");
            migrationBuilder.Sql("INSERT INTO Designations ([DesignationName], [State] ) Values('Developer','1')");
            migrationBuilder.Sql("INSERT INTO Designations ([DesignationName], [State] ) Values('Client','1')");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "Designations");
        }
    }
}