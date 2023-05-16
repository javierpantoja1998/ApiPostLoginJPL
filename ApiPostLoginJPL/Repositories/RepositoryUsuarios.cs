using ApiPostLoginJPL.Data;
using ApiPostLoginJPL.Helper;
using Microsoft.EntityFrameworkCore;
using NuggetPostLoginJPL.Models;

namespace ApiPostLoginJPL.Repositories
{
    public class RepositoryUsuarios
    {
        private UsuariosContext context;
        private HelperCriptography helper;

        public RepositoryUsuarios(UsuariosContext context, HelperCriptography helper)
        {
            this.context = context;
            this.helper = helper;
        }

        //IMPLEMENTACION DE SEGURIDAD EN LA API

        public async Task<Usuario> FindEmailAsync(string email)
        {
            return await this.context.Usuarios
            .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Usuario> FindUsuarioAsync(int idUsuario)
        {
            return await this.context.Usuarios
            .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
        }

        private int GetMaxIdUsuario()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Usuarios.Max(z => z.IdUsuario) + 1;
            }
        }


        //FUNCION PARA INSERTAR UN USUARIO
        public async Task RegistrarUsuarioAsync
         (string nombre, string dni, string direccion, string telefono, string email, string password)
        {
            Usuario user = new Usuario();
            int maximo = this.GetMaxIdUsuario();
            user.IdUsuario = maximo;
            user.Nombre = nombre;
            user.Dni = dni;
            user.Direccion = direccion;
            user.Telefono = telefono;
            user.Email = email;

            user.Salt =
            HelperCriptography.GenerateSalt();
            user.Password =
            HelperCriptography.EncriptPassword(password, user.Salt);

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
        }


        //METODO PARA VER SI EXISTE EL USUARIO
        public async Task<Usuario> ExisteUsuarioAsync
            (string email, string password)
        {
            Usuario user = await this.FindEmailAsync(email);
            return await
                this.context.Usuarios.FirstOrDefaultAsync
                (z => z.Email == email
                && z.Password == HelperCriptography.EncriptPassword(password, user.Salt));
        }
    }
}

