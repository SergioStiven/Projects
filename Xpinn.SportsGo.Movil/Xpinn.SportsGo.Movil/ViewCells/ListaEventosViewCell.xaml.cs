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
	public partial class ListaEventosViewCell : ViewCell
	{
		public ListaEventosViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty RegistroSeleccionadoProperty =
	        BindableProperty.Create(
	            nameof(RegistroSeleccionado),
	            typeof(ICommand),
	            typeof(ListaEventosViewCell),
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
	        GruposEventosAsistentesDTO item = BindingContext as GruposEventosAsistentesDTO;

	        if (item == null) return;

	        UrlImagenPerfil.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.GruposEventos.Grupos.Personas.UrlImagenPerfil))
	        {
	            UrlImagenPerfil.Source = item.GruposEventos.Grupos.Personas.UrlImagenPerfil;
	        }
	        else
	        {
	            UrlImagenPerfil.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
	        }

	        UrlImagenPais.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.GruposEventos.Paises.UrlArchivo))
	        {
	            UrlImagenPais.Source = item.GruposEventos.Paises.UrlArchivo;
	        }
	        else
	        {
	            UrlImagenPais.Source = App.Current.Resources["RutaDefaultImagen"] as string;
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