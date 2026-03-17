using ApiPeliculas.Models;

namespace ApiPeliculas.Repositorio.IRespositorio
{
    public interface IPeliculaRepositorio
    {
        //aqui solo se definen los metodos 
        ICollection<Pelicula> getPeliculas();
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
