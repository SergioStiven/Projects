using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class PagosBusinessTest
    {
        [TestMethod]
        public async Task PagosBusiness_ListarHistorialPagosPersonas_ShouldList()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            BuscadorDTO buscador = new BuscadorDTO
            {
                SkipIndexBase = 0,
                TakeIndexBase = 20
            };

            var lista = await pagosBusiness.ListarHistorialPagosPersonas(buscador);

            Assert.IsNotNull(lista);
        }

        [TestMethod]
        public async Task PagosBusiness_ListarHistorialPagosDeUnaPersona_ShouldList()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            BuscadorDTO buscador = new BuscadorDTO
            {
                ConsecutivoPersona = 3,
                IdiomaBase = Idioma.Español,
                SkipIndexBase = 0,
                TakeIndexBase = 20
            };

            var lista = await pagosBusiness.ListarHistorialPagosDeUnaPersona(buscador);

            Assert.IsNotNull(lista);
        }

        [TestMethod]
        public async Task PagosBusiness_ListarMonedas_ShouldList()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            var lista = await pagosBusiness.ListarMonedas();

            Assert.IsNotNull(lista);
        }

        [TestMethod]
        public async Task PagosBusiness_ConfigurarPayU_ShouldConfigure()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            HistorialPagosPersonas historial = new HistorialPagosPersonas
            {
                Consecutivo = 23
            };

            PayUModel payUModel = await pagosBusiness.ConfigurarPayU(historial);

            Assert.IsNotNull(payUModel);
        }

        [TestMethod]
        public async Task PagosBusiness_VerificarSiPagoEstaAprobado_ShouldVerify()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            HistorialPagosPersonas historial = new HistorialPagosPersonas
            {
                Consecutivo = 42
            };

            TimeLineNotificaciones timeLineNotificacion = await pagosBusiness.VerificarSiPagoEstaAprobadoYTraerNotificacion(historial);

            Assert.IsNotNull(timeLineNotificacion);
        }

        [TestMethod]
        public async Task PagosBusiness_EliminarPagoPendientePorPagar_ShouldDelete()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            HistorialPagosPersonas historial = new HistorialPagosPersonas
            {
                Consecutivo = 26,
                CodigoPersona = 3
            };

            WrapperSimpleTypesDTO wrapper = await pagosBusiness.EliminarPagoPendientePorPagar(historial);

            Assert.IsNotNull(wrapper);
        }

        [TestMethod]
        public async Task PagosBusiness_ModificarEstadoPagoPersona_ShouldModify()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            //HistorialPagosPersonas pagoParaBuscar = new HistorialPagosPersonas
            //{
            //    Consecutivo = 23,
            //    EstadoDelPago = EstadoDeLosPagos.Aprobado,
            //    ReferenciaPago = "1000",
            //    ObservacionesCliente = "AHHH"
            //};

            SecureMessagesHelper secure = new SecureMessagesHelper();

            HistorialPagosPersonas pagoParaBuscar = await secure.DecryptMessageToEntity<HistorialPagosPersonas>(@"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");

            var respuesta = await pagosBusiness.ModificarEstadoPagoPersona(pagoParaBuscar);

            Assert.IsNotNull(respuesta);
        }

        [TestMethod]
        public async Task PagosBusiness_CrearHistorialPagoPersona_ShouldCreate()
        {
            PagosBusiness pagosBusiness = new PagosBusiness();

            SecureMessagesHelper secure = new SecureMessagesHelper();

            HistorialPagosPersonas historial = await secure.DecryptMessageToEntity<HistorialPagosPersonas>(@"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");

            WrapperSimpleTypesDTO wrapper = await pagosBusiness.CrearHistorialPagoPersona(historial);
            Assert.IsNotNull(wrapper);
        }
    }
}
