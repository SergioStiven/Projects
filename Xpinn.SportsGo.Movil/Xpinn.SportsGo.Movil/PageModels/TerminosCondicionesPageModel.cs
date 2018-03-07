using System;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class TerminosCondicionesPageModel : BasePageModel
    {
        AdministracionServices _administracionService;

        public string TerminosCondiciones { get; set; }

        public TerminosCondicionesPageModel()
        {
            _administracionService = new AdministracionServices();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                TerminosCondicionesDTO terminosParaBuscar = new TerminosCondicionesDTO
                {
                    IdiomaDeLosTerminos = App.IdiomaPersona
                };

                if (IsNotConnected) return;
                TerminosCondicionesDTO terminosCondiciones = await _administracionService.BuscarTerminosCondiciones(terminosParaBuscar);

                if (terminosCondiciones != null)
                {
                    TerminosCondiciones = terminosCondiciones.Texto;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
