using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRespositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio

    { 
        private readonly ApplicationDbContext _context;

        public PeliculaRepositorio(ApplicationDbContext context)
        {
            _context = context; //para poder a cualquiera de las entidades que tenemos 
        }
        public bool actualizarPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            //Arreglar problema del put
            var peliculaExistente = _context.Pelicula.Find(pelicula.Id);
            if (peliculaExistente != null)
            {
                _context.Entry(peliculaExistente).CurrentValues.SetValues(pelicula);
            }
            else
            {
                _context.Pelicula.Update(pelicula);
            }
            return guardar();
        }

        public bool borrarPelicula(Pelicula pelicula)
        {
            _context.Pelicula.Remove(pelicula);
            return guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _context.Pelicula;
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) ||e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public bool crearPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _context.Pelicula.Add(pelicula);
            return guardar();
        }

        public bool existePelicula(int id)
        {
            return _context.Pelicula.Any(c => c.Id == id);
        }

        public bool existePelicula(string nombre)
        {
            bool valor =  _context.Pelicula.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }
        public ICollection<Pelicula> getPeliculas()
        {
            return _context.Pelicula
                .Include(c => c.categoria)  // agregar esto
                .OrderBy(c => c.Nombre)
                .ToList();
        }

        public Pelicula getPelicula(int peliculaId)
        {
            return _context.Pelicula
                .Include(c => c.categoria) 
                .FirstOrDefault(c => c.Id == peliculaId);
        }

        public bool guardar()
        {
            //cuando uno de los regsistros es mayor a cero es true, si no es false
            return _context.SaveChanges() >= 0 ? true : false;
        }

        public ICollection<Pelicula> getPeliculasEnCategoria(int categoriaId)
        {
            return _context.Pelicula
                .Include(ca => ca.categoria)
                .Where(ca => ca.categoriaId == categoriaId)
                .ToList();
        }
    }
}
