using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.Abstract;
using FreshMvvm;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class NoticiasPageModel : BasePageModel
    {
        NoticiasServices _noticiasServices;
        AnunciantesServices _anunciantesServices;
        IDateTimeHelper _dateTimeHelper;

        public ObservableRangeCollection<TimeLineNoticias> TimeLine { get; set; }

        public bool NoHayNadaMasParaCargar { get; set; }
        public bool IsRefreshing { get; set; }
        public DateTime LastRefresh { get; set; }

        public NoticiasPageModel()
        {
            _noticiasServices = new NoticiasServices();
            _anunciantesServices = new AnunciantesServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                LastRefresh = DateTime.Now;
                await CargarItemsTimeLine(0, 3);
            }
            catch (Exception)
            {

            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            NoHayNadaMasParaCargar = false;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            NoHayNadaMasParaCargar = true;
        }

        public ICommand InteracturarPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    TimeLineNoticias timeLineSeleccionada = parameter as TimeLineNoticias;

                    if (timeLineSeleccionada != null)
                    {
                        if (timeLineSeleccionada.EsVideo && timeLineSeleccionada.CodigoArchivoPublicacion.HasValue)
                        {
                            PublicacionModalModel publicacion = new PublicacionModalModel
                            {
                                CodigoArchivo = timeLineSeleccionada.CodigoArchivoPublicacion.Value,
                                UrlArchivo = timeLineSeleccionada.UrlArchivoPublicacion,
                                TipoArchivoPublicacion = TipoArchivo.Video,
                                UrlRedireccionar = timeLineSeleccionada.UrlRedireccionar
                            };

                            await CoreMethods.PushPageModel<PublicacionModalPageModel>(publicacion, true);
                        }
                        else if (timeLineSeleccionada.EsImagen && (timeLineSeleccionada.EsPublicidad || timeLineSeleccionada.EsNoticia) && !string.IsNullOrWhiteSpace(timeLineSeleccionada.UrlRedireccionar))
                        {
                            try
                            {
                                Device.OpenUri(new Uri(timeLineSeleccionada.UrlRedireccionar));

                                AnunciosDTO anuncio = new AnunciosDTO
                                {
                                    Consecutivo = timeLineSeleccionada.ConsecutivoPublicacion
                                };

                                await _anunciantesServices.AumentarContadorClickDeUnAnuncio(anuncio);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else if (timeLineSeleccionada.EsImagen && timeLineSeleccionada.EsEvento)
                        {
                            PublicacionModel publicacionParaVer = new PublicacionModel
                            {
                                CodigoPublicacion = timeLineSeleccionada.ConsecutivoPublicacion,
                                TipoPerfil = TipoPerfil.Grupo,
                            };

                            await CoreMethods.PushPageModel<PublicacionPageModel>(publicacionParaVer);
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand InteracturarIconoPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    TimeLineNoticias timeLineSeleccionada = parameter as TimeLineNoticias;

                    if (timeLineSeleccionada != null)
                    {
                        if ((timeLineSeleccionada.EsPublicidad || timeLineSeleccionada.EsNoticia) && !string.IsNullOrWhiteSpace(timeLineSeleccionada.UrlRedireccionar))
                        {
                            try
                            {
                                Device.OpenUri(new Uri(timeLineSeleccionada.UrlRedireccionar));
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else if (timeLineSeleccionada.EsEvento)
                        {
                            PublicacionModel publicacionParaVer = new PublicacionModel
                            {
                                CodigoPublicacion = timeLineSeleccionada.ConsecutivoPublicacion,
                                TipoPerfil = TipoPerfil.Grupo,
                            };

                            await CoreMethods.PushPageModel<PublicacionPageModel>(publicacionParaVer);
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrPersona
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    TimeLineNoticias timeLineSeleccionado = parameter as TimeLineNoticias;

                    if (timeLineSeleccionado != null && timeLineSeleccionado.EsEvento)
                    {
                        PersonasDTO persona = new PersonasDTO
                        {
                            Consecutivo = timeLineSeleccionado.ConsecutivoPersona
                        };
                        await CoreMethods.PushPageModel<PerfilPageModel>(persona);
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        IsRefreshing = true;
                        await CargarItemsTimeLine(0, 20, true);
                    }
                    catch (Exception)
                    {

                    }

                    IsRefreshing = false;
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMoreTimeLine
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (TimeLine != null)
                        {
                            await CargarItemsTimeLine(TimeLine.Count / 3, 3);
                        }
                        else
                        {
                            await CargarItemsTimeLine(0, 3);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarItemsTimeLine(int skipIndex, int takeIndex, bool isRefresh = false)
        {
            if (!NoHayNadaMasParaCargar || isRefresh)
            {
                BuscadorDTO buscador = new BuscadorDTO
                {
                    IdiomaBase = App.IdiomaPersona,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    EsConsultaEnLaApp = true,
                    //PaisesParaBuscar = new List<int> { App.Persona.CodigoPais },
                    //CategoriasParaBuscar = CategoriasDelPerfil(App.Persona),
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (isRefresh && LastRefresh != DateTime.MinValue)
                {
                    buscador.FechaInicio = LastRefresh;
                    LastRefresh = DateTime.Now;
                }

                if (IsNotConnected) return;
                List<TimeLineNoticias> listaTimeLine = await _noticiasServices.ListarTimeLine(buscador);

                if (listaTimeLine != null)
                {
                    if (listaTimeLine.Count > 0)
                    {
                        if (TimeLine == null)
                        {
                            TimeLine = new ObservableRangeCollection<TimeLineNoticias>(listaTimeLine);
                        }
                        else
                        {
                            // Filtro para evitar tener publicaciones repetidas
                            listaTimeLine = listaTimeLine.Where(x => !TimeLine.Any(y => y.TipoPublicacion == x.TipoPublicacion && y.ConsecutivoPublicacion == x.ConsecutivoPublicacion)).ToList();

                            if (isRefresh)
                            {
                                // Reverso la lista para mantener el orden
                                listaTimeLine.Reverse();

                                foreach (var timeLine in listaTimeLine)
                                {
                                    TimeLine.Insert(0, timeLine);
                                }
                            }
                            else
                            {
                                if (listaTimeLine.Count <= 0)
                                {
                                    NoHayNadaMasParaCargar = true;
                                }
                                else
                                {
                                    TimeLine.AddRange(listaTimeLine);
                                }
                            }
                        }
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaTimeLine.Count <= 0 && !isRefresh;
                    }
                }
            }
        }

    }
}
