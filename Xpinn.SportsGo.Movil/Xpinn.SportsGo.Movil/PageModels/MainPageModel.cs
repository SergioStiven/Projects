using System;
using Xpinn.SportsGo.Movil.Models;
using Xamarin.Forms;
using Xpinn.SportsGo.Movil.Infraestructure;
using System.Collections.Generic;
using System.Windows.Input;
using Xpinn.SportsGo.Movil.Abstract;
using FreshMvvm;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class MainPageModel : BasePageModel
    {
        public List<InicioCarousel> ImagenesCarousel { get; set; }
        public int Position { get; set; }
        IAdvancedTimer _timer;

        public ICommand CrearCuenta
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    StopTimer();
                    await CoreMethods.PushPageModel<TipoCuentaPerfilPageModel>();
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IniciarSesion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    StopTimer();
                    await CoreMethods.PushPageModel<InicioSesionPageModel>();
                    tcs.SetResult(true);
                });
            }
        }

        public MainPageModel()
        {
            _timer = DependencyService.Get<IAdvancedTimer>();

            ImagenesCarousel = new List<InicioCarousel>
            {
                new InicioCarousel
                {
                    ImageUrl = "CarouselOne.png",
                },
                 new InicioCarousel
                {
                    ImageUrl = "CarouselTwo.png",
                 },
                new InicioCarousel
                {
                    ImageUrl = "CarouselThree.png",
                },
                new InicioCarousel
                {
                    ImageUrl = "CarouselFour.png",
                },
            };
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            InitiateTimer();
        }

        void InitiateTimer()
        {
            _timer.InitTimer(5000, InitiateMotionCarousel(), true);

            _timer.StartTimer();
        }

        void StopTimer()
        {
            _timer.StopTimer();
        }

        EventHandler InitiateMotionCarousel()
        {
            return (o, evt) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Position == ImagenesCarousel.Count - 1)
                        Position = 0;
                    else
                        Position += 1;
                });
            };
        }
    }
}