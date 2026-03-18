using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace ApiPeliculas.Controllers.V1
{
    [Route("api/v{version:apiVersion}/usuarios")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuariosV1Controller : ControllerBase
    {
        private readonly IUsuarioRepositorio _userRepo;
        private readonly IMapper _mapper;//para utilizar el mapper
        private RespuestasAPI _respuestaApi;
        public UsuariosV1Controller(IUsuarioRepositorio userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _respuestaApi = new();
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult getUsuarios()
        {
            var listaUsuarios = _userRepo.getUsuarios();
            var listaUsuariosDto = new List<UsuarioDTO>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDTO>(lista));
            }
            return Ok(listaUsuariosDto);
        }

        [Authorize]
        [HttpGet("{usuarioId}", Name = "getUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult getUsuario(string usuarioId)
        {
            var itemUsuario = _userRepo.getUsuario(usuarioId);
            if (itemUsuario == null)
            {
                return NotFound();
            }
            var itemUsuarioDto = _mapper.Map<UsuarioDTO>(itemUsuario);

            return Ok(itemUsuarioDto);
        }

        //Para registrar un usuario
        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDTO usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _userRepo.validarUsuario(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _userRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> login([FromBody] UsuarioLoginDTO usuarioLoginDto)
        {
            var respuestaLogin = await _userRepo.Login(usuarioLoginDto);
            if (respuestaLogin==null || string.IsNullOrEmpty(respuestaLogin.Token) ) //si no tiene cuenta
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }
            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);
        }
    }
}
