using ApiPeliculas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options): base(options) //para que se puedan recibir todos los servicios que vienen de dbcontext
        {   
        }

        //aqui es empezar a pasar todos los modelos/entidades
        public DbSet<Categoria> Categoria { get; set; } //si esta linea no exste no saldria tabal en migracion

    }
}
