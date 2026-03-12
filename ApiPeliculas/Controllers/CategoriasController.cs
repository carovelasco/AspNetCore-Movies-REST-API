using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    //[Route("api/[controller]")]opcion estatica
    [Route("api/categorias")] //Opcion dinamica
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _catRepo;
        private readonly IMapper _mapper;//para utilizar el mapper

        public CategoriasController(ICategoriaRepositorio catRepo,  IMapper mapper)
        {
            _catRepo = catRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

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
            if (itemCategoria == null){
                return NotFound();
            }

            //convertir obj categoria a obj categoriadto  o sea la version filtrada
            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);

            return Ok(itemCategoriaDto);
        }
    }
}
