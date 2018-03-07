using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Models;

namespace Xpinn.SportsGo.Movil.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BuscadorPerfilesViewCell : ViewCell
	{
		public BuscadorPerfilesViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty RegistroSeleccionadoProperty =
	        BindableProperty.Create(
	            nameof(RegistroSeleccionado),
	            typeof(ICommand),
	            typeof(BuscadorPerfilesViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand RegistroSeleccionado
        {
	        get { return (ICommand)GetValue(RegistroSeleccionadoProperty); }
	        set { SetValue(RegistroSeleccionadoProperty, value); }
	    }

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        BuscadorModel item = BindingContext as BuscadorModel;

	        if (item == null) return;

	        UrlArchivoPrincipal.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.UrlArchivoPrincipal))
	        {
	            UrlArchivoPrincipal.Source = item.UrlArchivoPrincipal;
	        }
	        else
	        {
	            UrlArchivoPrincipal.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
	        }

	        UrlArchivoPais.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.UrlArchivoPais))
	        {
	            UrlArchivoPais.Source = item.UrlArchivoPais;
	        }
	        else
	        {
	            UrlArchivoPais.Source = App.Current.Resources["RutaDefaultImagen"] as string;
	        }
        }

	    private void RegistroSeleccionado_OnTapped(object sender, EventArgs e)
	    {
	        if (RegistroSeleccionado != null)
	        {
	            RegistroSeleccionado.Execute(BindingContext);
	        }
	    }
	}
}