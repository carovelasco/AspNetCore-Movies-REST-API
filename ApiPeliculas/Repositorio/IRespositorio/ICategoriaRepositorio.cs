using ApiPeliculas.Models;

namespace ApiPeliculas.Repositorio.IRespositorio
{
    public interface ICategoriaRepositorio
    {
        //aqui solo se definen los metodos 
        ICollection<Categoria> getCategorias();

        Categoria getCategoria(int CategoriaId);

        bool existeCategoria(int id);
        bool existeCategoria(string nombre);

        bool crearCategoria(Categoria categoria);
        bool actualizarCategoria(Categoria categoria);
        bool borrarCategoria(Categoria categoria);

        bool guardar();

    }
}
