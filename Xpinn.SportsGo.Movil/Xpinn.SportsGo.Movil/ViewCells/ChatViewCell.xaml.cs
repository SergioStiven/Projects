using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatViewCell : ViewCell
	{
		public ChatViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty IrConversacionProperty =
	        BindableProperty.Create(
	            nameof(IrConversacion),
	            typeof(ICommand),
	            typeof(ChatViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand IrConversacion
        {
	        get { return (ICommand)GetValue(IrConversacionProperty); }
	        set { SetValue(IrConversacionProperty, value); }
	    }

	    public static readonly BindableProperty BorrarChatProperty =
	        BindableProperty.Create(
	            nameof(BorrarChat),
	            typeof(ICommand),
	            typeof(ChatViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand BorrarChat
        {
	        get { return (ICommand)GetValue(BorrarChatProperty); }
	        set { SetValue(BorrarChatProperty, value); }
	    }

        protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        ChatsDTO item = BindingContext as ChatsDTO;

	        if (item == null) return;

            UrlImagenPerfil.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.PersonasNoOwner.UrlImagenPerfil))
	        {
	            UrlImagenPerfil.Source = item.PersonasNoOwner.UrlImagenPerfil;
	        }
	        else
	        {
	            UrlImagenPerfil.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
	        }
	    }

        private void IrConversacion_OnTapped(object sender, EventArgs e)
	    {
	        if (IrConversacion != null)
	        {
	            IrConversacion.Execute(BindingContext);
            }
	    }

	    private void BorrarChat_OnClicked(object sender, EventArgs e)
	    {
	        if (BorrarChat != null)
	        {
	            BorrarChat.Execute(BindingContext);
	        }
	    }
	}
}