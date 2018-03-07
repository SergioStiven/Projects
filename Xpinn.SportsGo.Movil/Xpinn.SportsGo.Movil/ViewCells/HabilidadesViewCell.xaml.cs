using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Movil.Models;

namespace Xpinn.SportsGo.Movil.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HabilidadesViewCell : ViewCell
	{
		public HabilidadesViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty ToogleAgregarEntidadProperty =
	        BindableProperty.Create(
	            nameof(ToogleAgregarEntidad),
	            typeof(ICommand),
	            typeof(HabilidadesViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand ToogleAgregarEntidad
        {
	        get { return (ICommand)GetValue(ToogleAgregarEntidadProperty); }
	        set { SetValue(ToogleAgregarEntidadProperty, value); }
	    }

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        HabilidadesModel item = BindingContext as HabilidadesModel;

	        if (item == null) return;
	    }

        private void ToogleAgregarEntidad_OnTapped(object sender, EventArgs e)
	    {
	        if (ToogleAgregarEntidad != null)
	        {
	            ToogleAgregarEntidad.Execute(BindingContext);
            }
	    }
	}
}