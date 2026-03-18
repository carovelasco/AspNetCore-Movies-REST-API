using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using AutoMapper;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _context;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ApplicationDbContext context, IConfiguration config,
            UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _context = context;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public AppUsuario getUsuario(string usuarioId)
        {
            return _context.AppUsuario.FirstOrDefault(c => c.Id == usuarioId);
        }


        public ICollection<AppUsuario> getUsuarios()
        {
            return _context.AppUsuario.OrderBy(c => c.UserName).ToList();
        }

        public async Task<UsuarioDatosDTO> Registro(UsuarioRegistroDTO usuarioRegistroDto)
        {
            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Nombre = usuarioRegistroDto.Nombre,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper()
            };

            var resultado = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);

            if (resultado.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }
                await _userManager.AddToRoleAsync(usuario, "Admin");

                var usuarioRetornado = _context.AppUsuario.FirstOrDefault(
                    u => u.UserName == usuarioRegistroDto.NombreUsuario
                );

                return new UsuarioDatosDTO()
                {
                    Id = usuarioRetornado.Id,
                    Username = usuarioRetornado.UserName,
                    Nombre = usuarioRetornado.Nombre,
                    Role = "Admin"
                };
            }

            return new UsuarioDatosDTO();
        }

        public async Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuariologinDto)
        {
            var usuario = _context.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == usuariologinDto.NombreUsuario.ToLower()
            );

            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDTO()
                {
                    Token = "",
                    Usuario = null
                };
            }

            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuariologinDto.Password);

            if (!isValid)
            {
                return new UsuarioLoginRespuestaDTO()
                {
                    Token = "",
                    Usuario = null,
                };
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = manejadorToken.CreateToken(tokenDescriptor);

            // construir DTO manualmente para incluir el rol
            UsuarioDatosDTO usuarioDto = new UsuarioDatosDTO()
            {
                Id = usuario.Id,
                Username = usuario.UserName,
                Nombre = usuario.Nombre,
                Role = roles.FirstOrDefault() ?? ""
            };

            return new UsuarioLoginRespuestaDTO()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = usuarioDto,
                Role = roles.FirstOrDefault() ?? ""
            };
        }

        public bool validarUsuario(string nombreUsuario)
        {
            // true = disponible
            var usuarioBd = _context.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == nombreUsuario.ToLower()
            );
            if (usuarioBd == null)
            {
                return true;
            }
            return false;
        }
    }
}