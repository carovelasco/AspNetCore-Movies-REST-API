using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;

namespace ApiPeliculas.Repositorio.IRespositorio
{
    public interface IUsuarioRepositorio
    {
        //aqui solo se definen los metodos 
        ICollection<AppUsuario> getUsuarios();

        AppUsuario getUsuario(string UsuarioId);
        bool validarUsuario(string Usuario);

        //obtener la respuesta cuando se autentique
        Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuariologinDto);
        Task<UsuarioDatosDTO> Registro(UsuarioRegistroDTO usuarioRegistroDto);


    }
}
