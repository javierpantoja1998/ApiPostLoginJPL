using ApiPostLoginJPL.Helper;
using ApiPostLoginJPL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuggetPostLoginJPL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiPostLoginJPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryUsuarios repo;
        private HelperOAuthToken helper;

        public AuthController(RepositoryUsuarios repo, HelperOAuthToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        //METODO PARA VALIDAR A NUESTRO USUARIO
        // Y DEVOLVER EL TOKEN DE ACCESO
        //ESTE METODO SIEMPRE TIENE QUE SER POST
        //RECIBE EL MODELO DEL LOGIN
        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario user =
                await this.repo.ExisteUsuarioAsync
                (model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES DENTRO DEL TOKEN
                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken(),
                    SecurityAlgorithms.HmacSha256);
                string jsonEmpleado =
                    JsonConvert.SerializeObject(user);
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonEmpleado)
                };

                //EL TOKEN SE GENERA CON UNA CLASE Y DEBEMOS INDICAR
                //LOS DATOS QUE CONFORMAN DICHO TOKEN
                JwtSecurityToken token =
                new JwtSecurityToken(
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    notBefore: DateTime.UtcNow
                    );
                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
