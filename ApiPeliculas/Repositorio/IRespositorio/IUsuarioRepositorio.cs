using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;

namespace ApiPeliculas.Repositorio.IRespositorio
{
    public interface IUsuarioRepositorio
    {
        //aqui solo se definen los metodos 
        ICollection<Usuario> getUsuarios();

        Usuario getUsuario(int UsuarioId);
        bool validarUsuario(string Usuario);

        //obtener la respuesta cuando se autentique
        Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuariologinDto);
        Task<Usuario> Registro(UsuarioRegistroDTO usuarioRegistroDto);


    }
}
