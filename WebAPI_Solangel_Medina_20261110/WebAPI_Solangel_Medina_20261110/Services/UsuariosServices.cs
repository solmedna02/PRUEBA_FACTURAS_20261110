using Dapper;
using Microsoft.AspNetCore.Identity;
using WebAPI_Solangel_Medina_20261110.Data;
using WebAPI_Solangel_Medina_20261110.Models;

namespace WebAPI_Solangel_Medina_20261110.Services
{
    public class UsuariosServices
    {
        private readonly DataContext _context;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public UsuariosServices(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Registrar(
            string nombre, string correo, string contrasena)
        {
            using var db = _context.CreateConnection();

            var existe = await db.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1)
                FROM Usuario
                WHERE Correo = @Correo",
                new { Correo = correo });

            if (existe > 0)
                return false;

            var usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Rol = "Ejecutivo"
            };

            usuario.Contrasena =
                _hasher.HashPassword(usuario, contrasena);

            await db.ExecuteAsync(@"
                INSERT INTO Usuario
                (Nombre, Correo, Contrasena, Rol)
                VALUES
                (@Nombre, @Correo, @Contrasena, @Rol)",
                usuario);

            return true;
        }

        public async Task<Usuario?> Login(
            string correo, string contrasena)
        {
            using var db = _context.CreateConnection();

            var usuario = await db.QueryFirstOrDefaultAsync<Usuario>(@"
                SELECT *
                FROM Usuario
                WHERE Correo = @Correo",
                new { Correo = correo });

            if (usuario == null)
                return null;

            var resultado = _hasher.VerifyHashedPassword(
                usuario,
                usuario.Contrasena,
                contrasena);

            return resultado == PasswordVerificationResult.Failed
                ? null
                : usuario;
        }
    }
}
