using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Movil.Models;

namespace Xpinn.SportsGo.Movil.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConversacionViewCell : ViewCell
	{
		public ConversacionViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty UrlMiImagenPerfilProperty =
	        BindableProperty.Create(
	            nameof(UrlMiImagenPerfil),
	            typeof(string),
	            typeof(ConversacionViewCell),
	            default(string),
	            BindingMode.TwoWay
	        );

	    public string UrlMiImagenPerfil
        {
	        get { return (string)GetValue(UrlMiImagenPerfilProperty); }
	        set { SetValue(UrlMiImagenPerfilProperty, value); }
	    }

	    public static readonly BindableProperty UrlImagenPerfilDestinoProperty =
	        BindableProperty.Create(
	            nameof(UrlImagenPerfilDestino),
	            typeof(string),
	            typeof(ConversacionViewCell),
	            default(string),
	            BindingMode.TwoWay
	        );

	    public string UrlImagenPerfilDestino
        {
	        get { return (string)GetValue(UrlImagenPerfilDestinoProperty); }
	        set { SetValue(UrlImagenPerfilDestinoProperty, value); }
	    }

	    public static readonly BindableProperty MiNombreProperty =
	        BindableProperty.Create(
	            nameof(MiNombre),
	            typeof(string),
	            typeof(ConversacionViewCell),
	            default(string),
	            BindingMode.TwoWay
	        );

	    public string MiNombre
        {
	        get { return (string)GetValue(MiNombreProperty); }
	        set { SetValue(MiNombreProperty, value); }
	    }

	    public static readonly BindableProperty NombreDestinoProperty =
	        BindableProperty.Create(
	            nameof(NombreDestino),
	            typeof(string),
	            typeof(ConversacionViewCell),
	            default(string),
	            BindingMode.TwoWay
	        );

	    public string NombreDestino
        {
	        get { return (string)GetValue(NombreDestinoProperty); }
	        set { SetValue(NombreDestinoProperty, value); }
	    }

        protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        ChatMensajesModel item = BindingContext as ChatMensajesModel;

	        if (item == null) return;

            string url = string.Empty;
	        if (item.EsElOtroChatConElQueHablo)
	        {
	            url = UrlImagenPerfilDestino;
	            Nombre.Text = NombreDestino;
	        }
	        else
	        {
                url = UrlMiImagenPerfil;
	            Nombre.Text = MiNombre;
	        }

	        UrlImagenPerfil.Source = null;
            if (!string.IsNullOrWhiteSpace(url))
	        {
	            UrlImagenPerfil.Source = url;
	        }
	        else
	        {
	            UrlImagenPerfil.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
	        }
        }

    }
}