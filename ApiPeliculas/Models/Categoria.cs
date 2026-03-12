using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Fecha de Creación")] //Para que en una app we se puedan poner caracteres especiales o espacios , no como en BD
        public DateTime FechaCreacion { get; set; }
    }
}
