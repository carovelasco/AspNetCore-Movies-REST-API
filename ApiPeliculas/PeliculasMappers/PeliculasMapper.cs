using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using AutoMapper;

namespace ApiPeliculas.PeliculasMapper
{
    public class PeliculasMapper : Profile 
    {
        public PeliculasMapper() 
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap(); // ya se pueden comunicar las entidades
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, CrearPeliculaDTO>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDatosDTO>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDTO>().ReverseMap();
            CreateMap<Pelicula, ActualizarPeliculaDTO>().ReverseMap();

        }

    }
}
