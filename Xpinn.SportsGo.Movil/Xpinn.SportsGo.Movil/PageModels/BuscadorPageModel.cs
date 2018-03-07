using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class BuscadorPageModel : BasePageModel
    {
        CategoriasServices _categoriaService;

        public ObservableCollection<CategoriasModel> Categorias { get; set; }

        public BuscadorPageModel()
        {
            _categoriaService = new CategoriasServices();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            CategoriasDTO categoria = new CategoriasDTO
            {
                IdiomaBase = App.IdiomaPersona
            };

            try
            {
                if (IsNotConnected) return;
                Categorias = new ObservableCollection<CategoriasModel>(CategoriasModel.CrearListaCategorias(await _categoriaService.ListarCategoriasPorIdioma(categoria)));
            }
            catch (Exception)
            {

                
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        public ICommand IrBuscarPerfiles
        {
            get
            {
                return new FreshAwaitCommand(async (categoria, tcs) => 
                {
                    await CoreMethods.PushPageModel<BuscadorPerfilesPageModel>(categoria);
                    tcs.SetResult(true);
                });
            }
        }
    }
}
