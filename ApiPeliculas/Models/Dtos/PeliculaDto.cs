using ApiPeliculas.Models;
using System.ComponentModel.DataAnnotations;

public class PeliculaDto
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public string? RutaImagen { get; set; }
    public string? RutaLocalImagen { get; set; }
    public Pelicula.TipoClasificacion Clasificacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int categoriaId { get; set; }
}