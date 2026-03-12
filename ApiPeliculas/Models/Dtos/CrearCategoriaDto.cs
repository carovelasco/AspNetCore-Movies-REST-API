using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    //este si va a ser el que se va a exponer , tipo el cliente no necesita saber el id 
    public class CrearCategoriaDto
    { // no necesitas el id(autoincremental) , ni la fecha de creacion
        [Required (ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El número maximo de caracteres es de 100")]
        public string Nombre { get; set; }
    }
}
