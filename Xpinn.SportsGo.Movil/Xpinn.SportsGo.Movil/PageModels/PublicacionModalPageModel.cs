using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class PublicacionModalPageModel : BasePageModel
    {
        public PublicacionModalModel Publicacion { get; set; }

        public override void Init(object initData)
        {
            base.Init(initData);

            Publicacion = initData as PublicacionModalModel;
        }

        public Command CerrarModal
        {
            get
            {
                return new Command(async () => {
                    
                    // PopPageModal, lanza una excepcion inentendible asi que toco cerrarlo a lo down sin Freshmvvm

                    // await CoreMethods.PopPageModel(true);
                    await App.Current.MainPage.Navigation.PopModalAsync();
                });
            }
        }
    }
}