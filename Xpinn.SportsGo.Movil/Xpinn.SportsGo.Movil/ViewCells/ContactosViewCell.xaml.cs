using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ContactosViewCell : ViewCell
	{
		public ContactosViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty ContactoSeleccionadoProperty =
	        BindableProperty.Create(
	            nameof(ContactoSeleccionado),
	            typeof(ICommand),
	            typeof(ContactosViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand ContactoSeleccionado
        {
	        get { return (ICommand)GetValue(ContactoSeleccionadoProperty); }
	        set { SetValue(ContactoSeleccionadoProperty, value); }
	    }

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        ContactosDTO item = BindingContext as ContactosDTO;

	        if (item == null) return;

	        UrlImagenPerfil.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.PersonasContacto.UrlImagenPerfil))
	        {
	            UrlImagenPerfil.Source = item.PersonasContacto.UrlImagenPerfil;
	        }
	        else
	        {
	            UrlImagenPerfil.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
	        }

	        UrlImagenPais.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.PersonasContacto.Paises.UrlArchivo))
	        {
	            UrlImagenPais.Source = item.PersonasContacto.Paises.UrlArchivo;
	        }
	        else
	        {
	            UrlImagenPais.Source = App.Current.Resources["RutaDefaultImagen"] as string;
	        }
        }

        private void ContactoSeleccionado_OnTapped(object sender, EventArgs e)
	    {
	        if (ContactoSeleccionado != null)
	        {
	            ContactoSeleccionado.Execute(BindingContext);
	        }
        }
	}
}