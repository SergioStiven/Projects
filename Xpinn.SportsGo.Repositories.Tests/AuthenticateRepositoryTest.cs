using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using System.IO;
using System.Linq;
using System.Text;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Repositories.Tests
{
    [TestClass]
    public class AuthenticateRepositoryTest
    {
        [TestMethod]
        public async Task AuthenticateRepository_VerificarUsuarios_ShouldVerify()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);

                Usuarios usuarioParaVerificar = new Usuarios
                {
                    Usuario = "Sergio",
                    Clave = "Sergio"
                };

                UsuariosDTO usuarioVerificado = await authenticateRepo.VerificarUsuario(usuarioParaVerificar);

                Assert.IsNotNull(usuarioVerificado);
                Assert.AreNotEqual(usuarioVerificado.Consecutivo, 0);
            }
        }

        [TestMethod]
        public async Task gagaga()
        {
            string ahh = File.ReadAllText(@"C:\Users\Administrador.EXPINN-TEC-5\Documents\Visual Studio 2017\123.txt");

            SportsGoEntities context = new SportsGoEntities();

            FormatoEmail formato = new FormatoEmail
            {
                TextoHtml = ahh,
                CodigoIdioma = 2,
                CodigoTipoFormato = 2
            };

            context.FormatoEmail.Add(formato);

            int numeroCambios = await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task AuthenticateRepository_ActualizarFechaAcceso_ShouldUpdate()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioParaVerificar = new Usuarios
                {
                    Usuario = "Sergio",
                    Clave = "Sergio"
                };

                UsuariosDTO usuarioVerificado = await authenticateRepo.VerificarUsuario(usuarioParaVerificar);

                Usuarios usuarioExistente = await authenticateRepo.ActualizarFechaUltimoAcceso(usuarioParaVerificar.Consecutivo);

                Assert.IsNotNull(usuarioVerificado);
                Assert.AreNotEqual(usuarioVerificado.Consecutivo, 0);
                Assert.AreEqual(usuarioVerificado.UltimoAcceso.Value.Date, DateTime.Today);
            }
        }

        [TestMethod]
        public async Task AuthenticateRepository_VerificarSiUsuarioYaExiste_ShouldVerify()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioParaCrear = new Usuarios
                {
                    Usuario = "Sergio",
                };

                WrapperSimpleTypesDTO wrapperExisteUsuario = await authenticateRepo.VerificarSiUsuarioYaExiste(usuarioParaCrear);

                Assert.IsNotNull(wrapperExisteUsuario);
                Assert.IsTrue(wrapperExisteUsuario.Existe);
                Assert.AreNotEqual(wrapperExisteUsuario.NumeroRegistrosAfectados, 0);
            }
        }

        [TestMethod]
        public async Task AuthenticateRepository_VerificarSiEmailYaExiste_ShouldVerify()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                Usuarios usuarioParaCrear = new Usuarios
                {
                    Email = "Sergio@Sergio.com"
                };

                WrapperSimpleTypesDTO wrapperExisteEmail = await authenticateRepo.VerificarSiEmailYaExiste(usuarioParaCrear);

                Assert.IsNotNull(wrapperExisteEmail);
                Assert.IsTrue(wrapperExisteEmail.Existe);
                Assert.AreNotEqual(wrapperExisteEmail.NumeroRegistrosAfectados, 0);
            }
        }

        [TestMethod]
        public async Task AuthenticateRepository_BuscarFormatoCorreoPorCodigoIdioma_ShouldSearch()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);

                string formatoTexto = await authenticateRepo.BuscarFormatoCorreoPorCodigoIdioma(1, TipoFormatosEnum.ConfirmacionCuenta);

                Assert.IsNotNull(formatoTexto);
            }
        }
    }
}
