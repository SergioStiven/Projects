using Xamarin.Forms;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Movil.Resources;
using System.Windows.Input;
using System;
using FreshMvvm;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class TipoCuentaPerfilPageModel : BasePageModel
    {
        MetricasServices _metricasServices;

        public TipoCuentaPerfil Candidato { get; set; }
        public TipoCuentaPerfil Grupo { get; set; }
        public TipoCuentaPerfil Representante { get; set; }
        public string NumeroCandidatos { get; set; }
        public string NumeroGrupos { get; set; }
        public string NumeroRepresentantes { get; set; }

        public TipoCuentaPerfilPageModel()
        {
            _metricasServices = new MetricasServices();

            Candidato = new TipoCuentaPerfil
            {
                ImageUrl = "Candidato.png",
                TipoPerfil = TipoPerfil.Candidato
            };
            Grupo = new TipoCuentaPerfil
            {
                ImageUrl = "Grupo.png",
                TipoPerfil = TipoPerfil.Grupo
            };
            Representante = new TipoCuentaPerfil
            {
                ImageUrl = "Representante.png",
                TipoPerfil = TipoPerfil.Representante
            };

            ConsultarMetricasUsuarios();
        }

        async void ConsultarMetricasUsuarios()
        {
            MetricasDTO metricas = new MetricasDTO();

            try
            {
                if (IsNotConnected) return;
                MetricasDTO metricasBuscadas = await _metricasServices.MetricasUsuarios(metricas);

                NumeroCandidatos = metricasBuscadas.NumeroCandidatos + " " + SportsGoResources.Usuarios;
                NumeroGrupos = metricasBuscadas.NumeroGrupos + " " + SportsGoResources.Usuarios;
                NumeroRepresentantes = metricasBuscadas.NumeroRepresentantes + " " + SportsGoResources.Usuarios;
            }
            catch (Exception)
            {

            }
        }

        public ICommand CrearCuenta
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    TipoCuentaPerfil tipoPerfil = parameter as TipoCuentaPerfil;
                    await CoreMethods.PushPageModel<InicioSesionPageModel>(tipoPerfil);
                    tcs.SetResult(true);
                });
            }
        }
    }
}
