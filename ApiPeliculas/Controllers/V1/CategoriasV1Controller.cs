using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V1
{
    //[Authorize(Roles = "Admin")] //esta a nivel de clase para que sea en todos los metodos
    //[Route("api/[controller]")]opcion estatica
    [ResponseCache(Duration = 20)]// aqui aplica globalda cacheado poe 20 seg
    [Route("api/v{version:apiVersion}/categorias")] //Opcion dinamica
    [ApiController]
    [ApiVersion("1.0")]

    public class CategoriasV1Controller : ControllerBase
    {
        private readonly ICategoriaRepositorio _catRepo;
        private readonly IMapper _mapper;//para utilizar el mapper

        public CategoriasV1Controller(ICategoriaRepositorio catRepo, IMapper mapper)
        {
            _catRepo = catRepo;
            _mapper = mapper;
        }

        [HttpGet("GetString")]
        //[MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2", "valor3" };
        }

        [AllowAnonymous] //para si a nivel de clase etsa protegido, est emetodo temrina estando public
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[EnableCors("PoliticaCORS")] si pones esto, aplica la politica solo a este metodo

        public IActionResult getCategorias()
        {
            //Esto es para acceder directamente al modelo
            var listaCategorias = _catRepo.getCategorias();
            //pero como no queremos eso:
            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var lista in listaCategorias)
            {
                //para pasar de listacategorias a listacategoriasDTO
                //esta diciendo que categoria dto es el modelo
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }
            return Ok(listaCategoriasDto);
        }

        [HttpGet("{categoriaId:int}", Name = "getCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult getCategoria(int categoriaId)
        {
            var itemCategoria = _catRepo.getCategoria(categoriaId);
            if (itemCategoria == null) {
                return NotFound();
            }

            //convertir obj categoria a obj categoriadto  o sea la version filtrada
            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);

            return Ok(itemCategoriaDto);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult crearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (crearCategoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_catRepo.existeCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if (!_catRepo.crearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el resgitro{categoria.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        [Authorize(Roles = "Admin")]
        //PATCH Para actualizar solo un atributo de la categoria 
        [HttpPatch("{categoriaId:int}", Name = "actualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult actualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_catRepo.actualizarCategoria(categoria)) //boolean
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el resgitro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        //con put se deben enviar todos los campos/atributos
        [HttpPut("{categoriaId:int}", Name = "actualizarPutCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult actualizarPutCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }
            var categoriaExistente = _catRepo.getCategoria(categoriaId);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con ID {categoriaId}");
            }
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_catRepo.actualizarCategoria(categoria)) //boolean
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el resgitro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoriaId:int}", Name = "borrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult borrarCategoria(int categoriaId)
        {
            if (!_catRepo.existeCategoria(categoriaId))
            {
                return NotFound();
            }
            var categoria =_catRepo.getCategoria(categoriaId);

            if (!_catRepo.borrarCategoria(categoria)) //boolean
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el resgitro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }



    }
}
