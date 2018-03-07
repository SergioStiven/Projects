using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class PosicionJugadorPageModel : BasePageModel
    {
        CandidatosServices _candidatoService;

        public PosicionJugadorModel PosicionModel { get; set; }
        public int NumeroPosicion { get; set; }

        public PosicionJugadorPageModel()
        {
            _candidatoService = new CandidatosServices();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            PosicionModel = initData as PosicionJugadorModel;

            NumeroPosicion = PosicionModel.CategoriaParaUbicar.PosicionJugador ?? 0;
        }

        public ICommand PosicionSeleccionada
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    if (PosicionModel.EsMiPersonaORegistro)
                    {
                        int? posicionSeleccionada = parameter as int?;

                        if (posicionSeleccionada.HasValue)
                        {
                            NumeroPosicion = posicionSeleccionada.Value;
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand GuardarPosicion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (PosicionModel.EsMiPersonaORegistro)
                    {
                        PosicionModel.CategoriaParaUbicar.PosicionJugador = NumeroPosicion;
                        await CoreMethods.PopPageModel(PosicionModel);
                    }

                    tcs.SetResult(true);
                });
            }
        }
    }
}
