using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;
        private string claveSecreta;

        public UsuarioRepositorio(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }

        public Usuario getUsuario(int usuarioId)
        {
            return _context.Usuario.FirstOrDefault(c => c.Id == usuarioId);
        }

        public ICollection<Usuario> getUsuarios()
        {
            return _context.Usuario.OrderBy(c => c.NombreUsuario).ToList();
        }

        public async Task<Usuario> Registro(UsuarioRegistroDTO usuarioRegistroDto)
        {
            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Nombre = usuarioRegistroDto.Nombre,
                Role = usuarioRegistroDto.Role
            };

            usuario.Password = _passwordHasher.HashPassword(usuario, usuarioRegistroDto.Password);

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuariologinDto)
        {
            var usuario = _context.Usuario.FirstOrDefault(
                u => u.NombreUsuario.ToLower() == usuariologinDto.NombreUsuario.ToLower()
            );

            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDTO()
                {
                    Token = "",
                    Usuario = null
                };
            }
            //hashing incluye salt y algorit seguros , por eso esto, en lugar de solo comaprar strings
            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Password,
                usuariologinDto.Password
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                return new UsuarioLoginRespuestaDTO()
                {
                    Token = "",
                    Usuario = null
                };
            }

            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = manejadorToken.CreateToken(tokenDescriptor);

            return new UsuarioLoginRespuestaDTO()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = usuario
            };
        }

        public bool validarUsuario(string nombreUsuario)
        {
            // true = disponible
            return !_context.Usuario.Any(u => u.NombreUsuario == nombreUsuario);
        }
    }
}