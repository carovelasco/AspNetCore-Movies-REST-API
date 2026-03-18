using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V1
{
    [Route("api/v{version:apiVersion}/peliculas")]
    [ApiController]
    [ApiVersion("1.0")]

    public class PeliculasV1Controller : ControllerBase
    {
        private readonly IPeliculaRepositorio _peliRepo;
        private readonly IMapper _mapper;//para utilizar el mapper

        public PeliculasV1Controller(IPeliculaRepositorio peliRepo, IMapper mapper)
        {
            _peliRepo = peliRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getPeliculas()
        {
            var listaPeliculas = _peliRepo.getPeliculas();
            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculasDto);
        }

        //Solo una pelicula
        [AllowAnonymous]
        [HttpGet("{peliculaId:int}", Name = "getpelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult getpelicula(int peliculaId)
        {
            var itempelicula = _peliRepo.getPelicula(peliculaId);
            if (itempelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itempelicula);

            return Ok(itemPeliculaDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))] //WHYYYYYYYYY
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult crearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (crearPeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_peliRepo.existePelicula(crearPeliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La Pelicula ya existe");
                return StatusCode(404, ModelState);
            }


            var Pelicula = _mapper.Map<Pelicula>(crearPeliculaDto);

            if (!_peliRepo.crearPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el resgitro{Pelicula.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { PeliculaId = Pelicula.Id }, Pelicula);
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("{peliculaId:int}", Name = "actualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult actualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = _peliRepo.getPelicula(peliculaId);
            if (peliculaExistente == null)
            {
                return NotFound($"No se encontro la pelicula con Id {peliculaExistente}");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);


            if (!_peliRepo.actualizarPelicula(pelicula)) //boolean
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el resgitro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{peliculaId:int}", Name = "borrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult borrarPelicula(int peliculaId)
        {
            if (!_peliRepo.existePelicula(peliculaId))
            {
                return NotFound();
            }
            var pelicula = _peliRepo.getPelicula(peliculaId);

            if (!_peliRepo.borrarPelicula(pelicula)) //boolean
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el resgitro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult getPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _peliRepo.getPeliculasEnCategoria(categoriaId);
            if (listaPeliculas == null)
            {
                return NotFound();
            }
            var itemPelicula = new List<PeliculaDto>();
            foreach (var pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(pelicula));
            }
            
            return Ok(itemPelicula);
        }

        [AllowAnonymous]
        [HttpGet("buscar")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult buscar(string nombre)
        {
            try
            {
                var resul = _peliRepo.BuscarPelicula(nombre);
                if (resul.Any())
                {
                    return Ok(resul);
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando  datos de la aplicación");
            }
        
        }
    }
}
