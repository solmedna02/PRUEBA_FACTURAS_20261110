using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_Solangel_Medina_20261110.Dtos;
using WebAPI_Solangel_Medina_20261110.Services;

namespace WebAPI_Solangel_Medina_20261110.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController: ControllerBase
    {
        private readonly UsuariosServices _service;
        private readonly IConfiguration _config;

        public UsuarioController(
            UsuariosServices service,
            IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [HttpPost("registro")]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(
            RegisterDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Correo) ||
                string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest("Todos los campos son obligatorios");

            var creado = await _service.Registrar(
                request.Nombre,
                request.Correo,
                request.Contrasena);

            if (!creado)
                return Conflict("El correo ya está registrado");

            return Ok("Usuario registrado correctamente");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            LoginDto request)
        {
            var usuario = await _service.Login(
                request.Correo, request.Contrasena);

            if (usuario == null)
                return Unauthorized("Credenciales incorrectas");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,
                    usuario.ID.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                nombre = usuario.Nombre,
                rol = usuario.Rol
            });
        }
    }
}
