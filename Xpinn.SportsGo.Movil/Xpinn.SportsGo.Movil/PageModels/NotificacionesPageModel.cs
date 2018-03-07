using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Args;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class NotificacionesPageModel : BasePageModel
    {
        NoticiasServices _noticiasService;
        PlanesServices _planService;
        ILockeable _lockeable;
        IAudioManager _audioManager;
        IDateTimeHelper _dateTimeHelper;

        public ObservableRangeCollection<TimeLineNotificaciones> TimeLine { get; set; }

        public bool IsRefreshing { get; set; }
        public bool NoHayNadaMasParaCargar { get; set; }
        public DateTime LastRefresh { get; set; }
        public bool NecesitaRefrescar { get; set; }

        public NotificacionesPageModel()
        {
            _noticiasService = new NoticiasServices();
            _planService = new PlanesServices();
            _lockeable = FreshIOC.Container.Resolve<ILockeable>();
            _audioManager = FreshIOC.Container.Resolve<IAudioManager>();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                await CargarItemsNotificaciones(0, 3);
                LastRefresh = DateTime.Now;
            }
            catch (Exception)
            {

            }

            ChatsServices.NotificacionRecibida += async (obj, evt) =>
            {
                if (evt.NotificacionRecibida != null)
                {
                    using (await _lockeable.LockAsync())
                    {
                        LastRefresh = DateTime.Now;
                        bool notificacionYaExiste = false;

                        // Verificamos que la notificacion ya existe, tomando en cuenta que el consecutivo debe ser valido para descartar las notificaciones de otros sources que no sea la tabla de Notificaciones (Como las noticias del admin)
                        if (TimeLine != null && TimeLine.Count > 0)
                        {
                            notificacionYaExiste = TimeLine.Where(x => x.ConsecutivoNotificacion == evt.NotificacionRecibida.ConsecutivoNotificacion && x.ConsecutivoNotificacion > 0).Any();
                        }

                        // Si la notificacion no existe, procedo a agregarla
                        if (!notificacionYaExiste)
                        {
                            evt.NotificacionRecibida = BuildearNotificacionesTraducidas(evt.NotificacionRecibida);
                            evt.NotificacionRecibida.EsNuevoMensaje = true;

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (TimeLine != null)
                                {
                                    TimeLine = new ObservableRangeCollection<TimeLineNotificaciones>(new List<TimeLineNotificaciones> { evt.NotificacionRecibida });
                                }
                                else
                                {
                                    TimeLine.Insert(0, evt.NotificacionRecibida);
                                }
                            });

                            // Aumentamos el contador del Badge
                            App.InteractuarValorBadgeNotificacion(1);

                            //Set or Get the state of the Effect sounds.
                            _audioManager.EffectsOn = true;

                            //Set the volume level of the Effects from 0 to 1.
                            _audioManager.EffectsVolume = 0.4f;

                            try
                            {
                                //await _audioManager.PlaySound("notificationSound.mp3");
                                await _audioManager.PlayNotificationDefaultSound();
                            }
                            catch (Exception)
                            {

                            }

                            if (evt.NotificacionRecibida.TipoDeLaNotificacion == TipoNotificacionEnum.PlanAprobado)
                            {
                                PlanesUsuariosDTO planUsuario = new PlanesUsuariosDTO
                                {
                                    Consecutivo = App.Usuario.CodigoPlanUsuario,
                                    IdiomaBase = App.IdiomaPersona
                                };

                                PlanesUsuariosDTO planDelUsuario = await _planService.BuscarPlanUsuario(planUsuario);

                                if (planDelUsuario != null)
                                {
                                    App.Usuario.PlanesUsuarios = planDelUsuario;
                                }
                            }
                            if (evt.NotificacionRecibida.TipoDeLaNotificacion == TipoNotificacionEnum.PersonaEliminada)
                            {
                                if (evt.NotificacionRecibida.CodigoPersonaOrigen > 0)
                                {
                                    PerfilPageModel.DispararEventoPersonaBorrada(this, evt.NotificacionRecibida.CodigoPersonaOrigen);
                                }
                            }
                        }
                    }
                }
            };

            ConnectionChanged += async (sender, args) =>
            {
                if (args.IsConnect && NecesitaRefrescar)
                {
                    try
                    {
                        await CargarItemsNotificaciones(0, 100, true, true);
                        NecesitaRefrescar = false;
                    }
                    catch (Exception)
                    {

                    }
                }

                if (!args.IsConnect)
                {
                    NecesitaRefrescar = true;
                }
            };
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
        }

        public ICommand InteractuarNotificacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    TimeLineNotificaciones timeLineSeleccionada = parameter as TimeLineNotificaciones;

                    if (timeLineSeleccionada.RedireccionaPersona)
                    {
                        PersonasDTO personaParaVer = new PersonasDTO
                        {
                            Consecutivo = timeLineSeleccionada.CodigoPersonaOrigen,
                            IdiomaDeLaPersona = App.IdiomaPersona
                        };

                        await CoreMethods.PushPageModel<PerfilPageModel>(personaParaVer);
                    }
                    else if (timeLineSeleccionada.RedireccionaUrlPublicidadSiHay || timeLineSeleccionada.RedireccionaUrlFeedSiHay)
                    {
                        try
                        {
                            Device.OpenUri(new Uri(timeLineSeleccionada.UrlPublicidad));
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (timeLineSeleccionada.EsNuevoMensaje)
                    {
                        timeLineSeleccionada.EsNuevoMensaje = false;
                        Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(TimeLine)));

                        App.InteractuarValorBadgeNotificacion(-1);
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
                        await CargarItemsNotificaciones(0, 100, true);
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
                            await CargarItemsNotificaciones(TimeLine.Count, 3);
                        }
                        else
                        {
                            await CargarItemsNotificaciones(0, 3);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarItemsNotificaciones(int skipIndex, int takeIndex, bool isRefresh = false, bool aumentarBadge = false)
        {
            if (App.Persona != null && App.Usuario != null && (!NoHayNadaMasParaCargar || isRefresh))
            {
                BuscadorDTO buscador = new BuscadorDTO
                {
                    IdiomaBase = App.IdiomaPersona,
                    ConsecutivoPersona = App.Persona.Consecutivo,
                    TipoDePerfil = App.Persona.TipoPerfil,
                    CodigoPlanUsuario = App.Usuario.CodigoPlanUsuario,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (isRefresh && LastRefresh != DateTime.MinValue)
                {
                    buscador.FechaFiltroBase = LastRefresh;
                    LastRefresh = DateTime.Now;
                }

                if (IsNotConnected) return;
                List<TimeLineNotificaciones> listaTimeLine = await _noticiasService.ListaTimeLineNotificaciones(buscador);

                if (listaTimeLine != null)
                {
                    if (listaTimeLine.Count > 0)
                    {
                        listaTimeLine = BuildearNotificacionesTraducidas(listaTimeLine);

                        using (await _lockeable.LockAsync())
                        {
                            if (TimeLine == null)
                            {
                                TimeLine = new ObservableRangeCollection<TimeLineNotificaciones>(listaTimeLine);
                            }
                            else
                            {
                                if (isRefresh)
                                {
                                    // Reverso la lista para mantener el orden
                                    listaTimeLine.Reverse();

                                    foreach (var timeLine in listaTimeLine)
                                    {
                                        if (timeLine.CreacionNotificacion > buscador.FechaFiltroBase)
                                        {
                                            timeLine.EsNuevoMensaje = true;
                                        }

                                        TimeLine.Insert(0, timeLine);
                                    }
                                }
                                else
                                {
                                    TimeLine.AddRange(listaTimeLine);
                                }
                            }

                            if (aumentarBadge)
                            {
                                App.InteractuarValorBadgeNotificacion(listaTimeLine.Count);
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

        public List<TimeLineNotificaciones> BuildearNotificacionesTraducidas(List<TimeLineNotificaciones> listaNotificaciones)
        {
            foreach (var notificacion in listaNotificaciones)
            {
                BuildearNotificacionesTraducidas(notificacion);
            }

            return listaNotificaciones;
        }

        public TimeLineNotificaciones BuildearNotificacionesTraducidas(TimeLineNotificaciones notificacion)
        {
            if (!string.IsNullOrWhiteSpace(notificacion.Titulo) && notificacion.Titulo.Length > 85)
            {
                notificacion.Titulo = notificacion.Titulo.Substring(0, 85) + "...";
            }

            if (!string.IsNullOrWhiteSpace(notificacion.Descripcion) && notificacion.Descripcion.Length > 135)
            {
                notificacion.Descripcion = notificacion.Descripcion.Substring(0, 135) + "...";
            }

            switch (notificacion.TipoDeLaNotificacion)
            {
                case TipoNotificacionEnum.NuevoPlan:
                    notificacion.Titulo = SportsGoResources.HayUnNuevoPlanTitulo;

                    string descripcionTemporalNuevoPlan = SportsGoResources.HayUnNuevoPlanDescripcion;
                    descripcionTemporalNuevoPlan = descripcionTemporalNuevoPlan.Replace(AppConstants.PlaceNombrePlan, notificacion.DescripcionPlan);

                    notificacion.Descripcion = descripcionTemporalNuevoPlan;

                    break;
                case TipoNotificacionEnum.PersonaAgregada:
                    notificacion.Titulo = SportsGoResources.AgregadoPersonaTitulo;

                    string descripcionTemporalAgregadoPersona = SportsGoResources.AgregadoPersonaDescripcion;
                    descripcionTemporalAgregadoPersona = descripcionTemporalAgregadoPersona.Replace(AppConstants.PlaceNombrePersona, notificacion.NombreApellidoPersona);

                    notificacion.Descripcion = descripcionTemporalAgregadoPersona;

                    break;
                case TipoNotificacionEnum.PersonaEliminada:
                    notificacion.Titulo = SportsGoResources.EliminadoPersonaTitulo;

                    string descripcionTemporalEliminadoPersona = SportsGoResources.EliminadoPersonaDescripcion;
                    descripcionTemporalEliminadoPersona = descripcionTemporalEliminadoPersona.Replace(AppConstants.PlaceNombrePersona, notificacion.NombreApellidoPersona);

                    notificacion.Descripcion = descripcionTemporalEliminadoPersona;

                    break;
                case TipoNotificacionEnum.EstaPorVencersePlan:
                    notificacion.Titulo = SportsGoResources.PlanEstaPorVencerseTitulo;

                    string descripcionTemporalEstaPorVencerPlan = SportsGoResources.PlanEstaPorVencerseDescripcion;

                    descripcionTemporalEstaPorVencerPlan = descripcionTemporalEstaPorVencerPlan.Replace(AppConstants.PlaceNombrePlan, notificacion.DescripcionPlan);
                    descripcionTemporalEstaPorVencerPlan = descripcionTemporalEstaPorVencerPlan.Replace(AppConstants.PlaceFechaVencimientoPlan, notificacion.FechaVencimientoPlan.ToString("d"));

                    notificacion.Descripcion = descripcionTemporalEstaPorVencerPlan;

                    break;
                case TipoNotificacionEnum.SeVencioPlan:
                    notificacion.Titulo = SportsGoResources.PlanSeHaVencidoTitulo;

                    string descripcionTemporalSeVencioPlan = SportsGoResources.PlanSeHaVencidoDescripcion;

                    descripcionTemporalSeVencioPlan = descripcionTemporalSeVencioPlan.Replace(AppConstants.PlaceNombrePlan, notificacion.DescripcionPlan);
                    descripcionTemporalSeVencioPlan = descripcionTemporalSeVencioPlan.Replace(AppConstants.PlaceFechaVencimientoPlan, notificacion.FechaVencimientoPlan.ToString("d"));

                    notificacion.Descripcion = descripcionTemporalSeVencioPlan;

                    break;
                case TipoNotificacionEnum.PlanAprobado:
                    notificacion.Titulo = SportsGoResources.PlanFueAprobadoTitulo;

                    string descripcionTemporalPlanAprobado = SportsGoResources.PlanFueAprobadoDescripcion;

                    descripcionTemporalSeVencioPlan = descripcionTemporalPlanAprobado.Replace(AppConstants.PlaceNombrePlan, notificacion.DescripcionPlan);

                    notificacion.Descripcion = descripcionTemporalSeVencioPlan;

                    break;
                case TipoNotificacionEnum.PlanRechazado:
                    notificacion.Titulo = SportsGoResources.PlanFueRechazadoTitulo;

                    string descripcionTemporalPlanRechazado = SportsGoResources.PlanFueRechazadoDescripcion;

                    descripcionTemporalSeVencioPlan = descripcionTemporalPlanRechazado.Replace(AppConstants.PlaceNombrePlan, notificacion.DescripcionPlan);

                    notificacion.Descripcion = descripcionTemporalSeVencioPlan;

                    break;
                case TipoNotificacionEnum.InscripcionEventoUsuario:
                    notificacion.Titulo = SportsGoResources.InscripcionEventoTitulo;

                    string descripcionTemporalInscripcionEventoUsuario = SportsGoResources.InscripcionEventoDescripcion;
                    descripcionTemporalInscripcionEventoUsuario = descripcionTemporalInscripcionEventoUsuario.Replace(AppConstants.PlaceNombrePersona, notificacion.NombreApellidoPersona);
                    descripcionTemporalInscripcionEventoUsuario = descripcionTemporalInscripcionEventoUsuario.Replace(AppConstants.PlaceTituloEvento, notificacion.TituloEvento);

                    notificacion.Descripcion = descripcionTemporalInscripcionEventoUsuario;

                    break;
                case TipoNotificacionEnum.DesuscripcionEventoUsuario:
                    notificacion.Titulo = SportsGoResources.DesuscripcionEventoTitulo;

                    string descripcionTemporalDesuscripcionEventoUsuario = SportsGoResources.DesuscripcionEventoDescripcion;
                    descripcionTemporalDesuscripcionEventoUsuario = descripcionTemporalDesuscripcionEventoUsuario.Replace(AppConstants.PlaceNombrePersona, notificacion.NombreApellidoPersona);
                    descripcionTemporalDesuscripcionEventoUsuario = descripcionTemporalDesuscripcionEventoUsuario.Replace(AppConstants.PlaceTituloEvento, notificacion.TituloEvento);

                    notificacion.Descripcion = descripcionTemporalDesuscripcionEventoUsuario;

                    break;
                case TipoNotificacionEnum.RssFeed:
                    notificacion.UrlArchivo = "RssIconFeed.png";

                    break;
            }

            return notificacion;
        }
    }
}
