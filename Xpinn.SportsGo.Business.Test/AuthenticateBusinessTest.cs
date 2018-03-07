using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.DomainEntities;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using System.IO;
using Xpinn.SportsGo.Repositories;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class AuthenticateBusinessTest
    {
        [TestMethod]
        public async Task AuthenticateBusiness_VerificarUsuario_ShouldVerify()
        {
            AuthenticateBusiness authBuss = new AuthenticateBusiness();

            Usuarios usuarios = new Usuarios
            {
                Usuario = "Anderson",
                Clave = "Anderson"
            };

            UsuariosDTO usuarioVerificado = await authBuss.VerificarUsuario(usuarios);

            Assert.IsNotNull(usuarioVerificado);
        }

        //[TestMethod]
        //public async Task AuthenticateBusiness_RecuperarClave_ShouldRecovery()
        //{
        //    AuthenticateBusiness authBuss = new AuthenticateBusiness();

        //    Usuarios usuarios = new Usuarios
        //    {
        //        Usuario = "Bryan",
        //        Email = @"bsanchez@expinn.com.co"
        //    };

        //     Recordar configurar la cuenta Gmail en este caso para que permita el logeo de manera insegura y poder mandar correos
        //     https://myaccount.google.com/lesssecureapps?pli=1
        //     WrapperSimpleTypesDTO wrapperRecuperarClave = await authBuss.RecuperarClave(usuarios, );

        //     Assert.IsNotNull(wrapperRecuperarClave);
        //}
    }
}
