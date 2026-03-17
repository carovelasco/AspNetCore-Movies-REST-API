using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRespositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio

    { 
        private readonly ApplicationDbContext _context;

        public CategoriaRepositorio(ApplicationDbContext context)
        {
            _context = context; //para poder a cualquiera de las entidades que tenemos 
        }
        public bool actualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            //Arreglar problema del put
            var categoriaExistente = _context.Categoria.Find(categoria.Id);
            if (categoriaExistente != null)
            {
                _context.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }
            else
            {
                _context.Categoria.Update(categoria);
            }
            return guardar();
        }

        public bool borrarCategoria(Categoria categoria)
        {
            _context.Categoria.Remove(categoria);
            return guardar();
        }

        public bool crearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _context.Categoria.Add(categoria);
            return guardar();
        }

        public bool existeCategoria(int id)
        {
            return _context.Categoria.Any(c => c.Id == id);
        }

        public bool existeCategoria(string nombre)
        {
            bool valor =  _context.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public Categoria getCategoria(int CategoriaId)
        {
            return _context.Categoria.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> getCategorias()
        {
            return _context.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool guardar()
        {
            //cuando uno de los regsistros es mayor a cero es true, si no es false
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}
