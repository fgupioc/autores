using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiWebAutores.Migrations
{
    public partial class usuariocomentario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioID",
                table: "Comentarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioID",
                table: "Comentarios",
                column: "UsuarioID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioID",
                table: "Comentarios",
                column: "UsuarioID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioID",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UsuarioID",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Comentarios");
        }
    }
}
