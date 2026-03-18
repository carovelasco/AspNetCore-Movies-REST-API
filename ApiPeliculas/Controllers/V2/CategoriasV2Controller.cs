using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRespositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2
{
    //[Authorize(Roles = "Admin")] //esta a nivel de clase para que sea en todos los metodos
    //[Route("api/[controller]")]opcion estatica
    [ResponseCache(Duration = 20)]
    [Route("api/v{version:apiVersion}/categorias")]
    [ApiController]
    [ApiVersion("2.0")]

    public class CategoriasV2Controller : ControllerBase
    {
        private readonly ICategoriaRepositorio _catRepo;
        private readonly IMapper _mapper;

        public CategoriasV2Controller(ICategoriaRepositorio catRepo, IMapper mapper)
        {
            _catRepo = catRepo;
            _mapper = mapper;
        }

        [HttpGet("GetString")]
        [MapToApiVersion("2.0")]  // <-- agregar esto
        public IEnumerable<string> Get()
        {
            return new string[] { "pelicula", "serie", "novela" };
        }
    }
}