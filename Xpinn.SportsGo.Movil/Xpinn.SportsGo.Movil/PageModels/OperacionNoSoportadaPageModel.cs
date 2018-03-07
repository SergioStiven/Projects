using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class OperacionNoSoportadaPageModel : BasePageModel
    {
        public OperacionControlModel OperacionControl { get; set; }

        public string TextoNoSoportaOperacion
        {
            get
            {
                string texto = string.Empty;

                switch (OperacionControl.TipoOperacion)
                {
                    case TipoOperacion.MultiplesCategorias:
                        texto = SportsGoResources.PlanMaximoCategoria;
                        break;
                    default:
                        texto = SportsGoResources.PlanNoSoportaOperacion;
                        break;
                }

                return texto;
            }
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            OperacionControl = initData as OperacionControlModel;
        }

        public ICommand VerPlanes
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<ListaPlanesPageModel>();

                    tcs.SetResult(true);
                });
            }
        }
    }
}
