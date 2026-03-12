using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    //este si va a ser el que se va a exponer , tipo el cliente no necesita saber el id 
    public class CategoriaDto
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El número maximo de caracteres es de 100")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Fecha de Creación")] //Para que en una app we se puedan poner caracteres especiales o espacios , no como en BD
        public DateTime FechaCreacion { get; set; }
    }
}
