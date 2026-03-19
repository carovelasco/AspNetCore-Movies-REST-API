using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

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
        public IActionResult getPeliculas([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalPeliculas = _peliRepo.getTotalPeliculas();
                var peliculas = _peliRepo.getPeliculas(pageNumber, pageSize);

                if (peliculas == null || !peliculas.Any())
                {
                    return NotFound("No se encontraron peliculas.");
                }
                var peliculasDto = peliculas.Select(p => _mapper.Map<PeliculaDto>(p)).ToList();

                var response = new
                {
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalPeliculas / (double)pageSize),
                    totalPeliculas = totalPeliculas,
                    Items = peliculasDto
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la apliación");

            }
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

        public IActionResult crearPelicula([FromForm] CrearPeliculaDTO crearPeliculaDto)
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

            //if (!_peliRepo.crearPelicula(Pelicula))
            //{
            //    ModelState.AddModelError("", $"Algo salio mal guardando el resgitro{Pelicula.Nombre}");
            //    return StatusCode(404, ModelState);
            //}

            //subida de archivo
            if (crearPeliculaDto.Imagen != null)
            {
                string nombreArchivo = Pelicula.Id + Guid.NewGuid().ToString() + Path.GetExtension(crearPeliculaDto.Imagen.FileName);

                // Definir la carpeta y la ruta física completa
                string carpetaFotos = @"wwwroot\imgPeliculas";
                string rutaFisicaCompleta = Path.Combine(Directory.GetCurrentDirectory(), carpetaFotos, nombreArchivo);

                // Asegurar que el directorio existe
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), carpetaFotos)))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), carpetaFotos));
                }

                // Guardar el archivo
                using (var fileStream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                {
                    crearPeliculaDto.Imagen.CopyTo(fileStream);
                }

                // Construir la URL 
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                Pelicula.RutaImagen = $"{baseUrl}/imgPeliculas/{nombreArchivo}";
                Pelicula.RutaLocalImagen = Path.Combine(carpetaFotos, nombreArchivo);
            }
            else
            {
                Pelicula.RutaImagen = "https://placehold.co/600x400";
            }

            _peliRepo.crearPelicula(Pelicula);
            return CreatedAtRoute("GetPelicula", new { PeliculaId = Pelicula.Id }, Pelicula);
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("{peliculaId:int}", Name = "actualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult actualizarPatchPelicula(int peliculaId, [FromForm] ActualizarPeliculaDTO actualizarPeliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (actualizarPeliculaDto == null || peliculaId != actualizarPeliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = _peliRepo.getPelicula(peliculaId);
            if (peliculaExistente == null)
            {
                return NotFound($"No se encontro la pelicula con Id {peliculaExistente}");
            }

            var pelicula = _mapper.Map<Pelicula>(actualizarPeliculaDto);


            //if (!_peliRepo.actualizarPelicula(pelicula)) //boolean
            //{
            //    ModelState.AddModelError("", $"Algo salio mal actualizando el resgitro{pelicula.Nombre}");
            //    return StatusCode(500, ModelState);
            //}

            if (actualizarPeliculaDto.Imagen != null)
            {
                string nombreArchivo = actualizarPeliculaDto.Id + Guid.NewGuid().ToString() + Path.GetExtension(actualizarPeliculaDto.Imagen.FileName);

                // Definir la carpeta y la ruta física completa
                string carpetaFotos = @"wwwroot\imgPeliculas";
                string rutaFisicaCompleta = Path.Combine(Directory.GetCurrentDirectory(), carpetaFotos, nombreArchivo);

                // Asegurar que el directorio existe
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), carpetaFotos)))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), carpetaFotos));
                }

                // Guardar el archivo
                using (var fileStream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                {
                    actualizarPeliculaDto.Imagen.CopyTo(fileStream);
                }

                // Construir la URL 
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                pelicula.RutaImagen = $"{baseUrl}/imgPeliculas/{nombreArchivo}";
                pelicula.RutaLocalImagen = Path.Combine(carpetaFotos, nombreArchivo);
            }
            else
            {
                pelicula.RutaImagen = "https://placehold.co/600x400";
            }

            _peliRepo.actualizarPelicula(pelicula);
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
            try
            {
                var listaPeliculas = _peliRepo.getPeliculasEnCategoria(categoriaId);
                if (listaPeliculas == null || !listaPeliculas.Any())
                {
                    return NotFound($"No se encontraron peliculas en la categoria con ID {categoriaId}");
                }
                var itemPelicula = listaPeliculas.Select(pelicula => _mapper.Map<PeliculaDto>(pelicula)).ToList();

                return Ok(itemPelicula);

            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de apliación");
            }           
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
                if (!resul.Any())
                {
                    return NotFound($"No se encontraron peliculas que coincidan con los criterios de busqueda.");
                }
                //lo que te paso entre () conviertelo al tipo de dato peliculadto
                var peliculasDto = _mapper.Map<IEnumerable<PeliculaDto>>(resul);
                return Ok(peliculasDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando  datos de la aplicación");
            }
        
        }
    }
}
