using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Models
{
    public class AppUsuario : IdentityUser
    {
        public string Nombre { get; set; }

    }
}
