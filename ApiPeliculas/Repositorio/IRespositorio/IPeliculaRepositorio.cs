using ApiPeliculas.Models;

namespace ApiPeliculas.Repositorio.IRespositorio
{
    public interface IPeliculaRepositorio
    {
        //aqui solo se definen los metodos 

        //V1
        //ICollection<Pelicula> getPeliculas();

        //V2
        ICollection<Pelicula> getPeliculas(int pageNumber,int pageSize);
        int getTotalPeliculas();
        ICollection<Pelicula> getPeliculasEnCategoria(int categoriaId);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);
        Pelicula getPelicula(int peliculaId);

        bool existePelicula(int id);
        bool existePelicula(string nombre);
        bool guardar();
        bool crearPelicula(Pelicula pelicula);
        bool actualizarPelicula(Pelicula pelicula);
        bool borrarPelicula(Pelicula pelicula);

    }
}
