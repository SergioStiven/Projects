using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class DetallePlanPageModel : BasePageModel
    {
        public PlanesDTO Plan { get; set; }

        public bool EsParaComprarPlan
        {
            get
            {
                return App.Usuario.PlanesUsuarios.CodigoPlan != Plan.Consecutivo;
            }
        }

        public string AbreviaturaMonedaDeLaPersona
        {
            get
            {
                return "(" + App.Persona.Paises.Monedas.AbreviaturaMoneda + "): ";
            }
        }

        public bool PlanEsParaCandidatos
        {
            get
            {
                return Plan.TipoPerfil == TipoPerfil.Candidato;
            }
        }

        public bool PlanNoEsParaRepresentantes
        {
            get
            {
                return Plan.TipoPerfil != TipoPerfil.Representante;
            }
        }

        public string TiempoMaximoDeVideo
        {
            get
            {
                if (App.Persona.IdiomaDeLaPersona == Idioma.Ingles)
                {
                    return Plan.TiempoPermitidoVideo + " sec";
                }
                else
                {
                    return Plan.TiempoPermitidoVideo + " seg";
                }
            }
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Plan = initData as PlanesDTO;
        }

        public ICommand InteractuarPlan
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsParaComprarPlan)
                    {
                        await CoreMethods.PushPageModel<PagosPageModel>(Plan);
                    }
                    else
                    {
                        await CoreMethods.PushPageModel<ListaPlanesPageModel>();
                    }

                    tcs.SetResult(true);
                });
            }
        }
    }
}
