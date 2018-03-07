using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class HabilidadesPageModel : BasePageModel
    {
        HabilidadesServices _habilidadesServices;
        HabilidadesModel _habilidadesModel;
        List<HabilidadesModel> _habilidadesParaAgregar;
        int _numeroEstrellasMaximas;
        bool _estrellasMaximasAlcanzadas { get; set; }

        public string HabilidadBusqueda { get; set; }
        public int SelectIndexSegmentBar { get; set; }

        public Color NumeroEstrellasGastadasColor
        {
            get
            {
                Color colorDefault = (Color)App.Current.Resources["PrimaryAppColor"];

                if (_estrellasMaximasAlcanzadas)
                {
                    colorDefault = Color.Red;
                }

                return colorDefault;
            }
        }

        ObservableCollection<HabilidadesModel> _habilidades;
        public ObservableCollection<HabilidadesModel> Habilidades
        {
            get
            {
                if (_habilidades != null)
                {
                    // Filtro las habilidades por el tipo de habilidad seleccionado en el segment bar control
                    IEnumerable<HabilidadesModel> habilidades = _habilidades.Where(x => x.Habilidad.CodigoTipoHabilidad == (SelectIndexSegmentBar + 1));

                    // Si tengo una descripcion en el searchbar control, filtro por esa descripcion
                    if (!string.IsNullOrWhiteSpace(HabilidadBusqueda))
                    {
                        habilidades = habilidades.Where(x =>
                        {
                            // Puede suceder y sucedio que la descripcion del idioma viene vacia, por un error al guardar puede ser
                            if (!string.IsNullOrWhiteSpace(x.Habilidad.DescripcionIdiomaBuscado))
                            {
                                return x.Habilidad.DescripcionIdiomaBuscado.ToUpperInvariant().Contains(HabilidadBusqueda.ToUpperInvariant());
                            }

                            return false;
                        });
                    }

                    return new ObservableCollection<HabilidadesModel>(habilidades.OrderByDescending(x => x.EstaAgregada));
                }

                return null;
            }
            private set
            {
                _habilidades = value;
            }
        }

        // Numero de habilidades totales seleccionadas entre los tipos de habilidades
        public string NumerosHabilidadesSeleccionadas
        {
            get
            {
                int numeroHabilidadesSeleccionadas = 0;

                if (_habilidades != null)
                {
                    numeroHabilidadesSeleccionadas = _habilidadesParaAgregar.Count;
                }

                return numeroHabilidadesSeleccionadas.ToString();
            }
        }

        // Booleana para saber si al darle a guardar, mando a guardar al ws para la categoria del perfil que estamos trabajando
        public bool SePuedeAgregarHabilidades
        {
            get
            {
                return _habilidadesModel.CodigoCategoriaPerfilParaGuardarHabilidades > 0;
            }
        }

        // Numero de habilidades totales seleccionadas entre los tipos de habilidades
        public string NumeroEstrellasGastadas
        {
            get
            {
                int numeroEstrellasGastadas = 0;

                if (_habilidades != null)
                {
                    numeroEstrellasGastadas = _habilidadesParaAgregar.Sum(x => x.NumeroEstrellas);
                }

                return numeroEstrellasGastadas.ToString();
            }
        }

        public string NumeroEstrellasMaximasFormated
        {
            get
            {
                return "/ " + _numeroEstrellasMaximas;
            }
        }

        public int HeightRequest
        {
            get
            {
                int heightRequest = 45;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    heightRequest = 70;
                }

                return heightRequest;
            }
        }

        public HabilidadesPageModel()
        {
            _habilidadesServices = new HabilidadesServices();
            _habilidadesParaAgregar = new List<HabilidadesModel>();
            _numeroEstrellasMaximas = 50;
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            _habilidadesModel = initData as HabilidadesModel;

            // Categoria a la cual vamos a listar todas sus habilidades
            HabilidadesDTO habilidades = new HabilidadesDTO
            {
                CodigoCategoria = _habilidadesModel.CodigoCategoriaParaListarHabilidades,
                IdiomaBase = App.IdiomaPersona
            };

            try
            {
                // Listamos todas las categorias
                if (IsNotConnected) return;
                List<HabilidadesModel> listaHabilidades = HabilidadesModel.CrearListaHabilidades(await _habilidadesServices.ListarHabilidadesPorCodigoCategoriaAndIdioma(habilidades));

                // Si la persona ya tiene habilidades agregadas desde antes las marcamos como ya agregadas
                if (_habilidadesModel.HabilidadesCandidatosExistentes != null && _habilidadesModel.HabilidadesCandidatosExistentes.Count > 0)
                {
                    foreach (HabilidadesCandidatosDTO habilidadExistente in _habilidadesModel.HabilidadesCandidatosExistentes)
                    {
                        HabilidadesModel habilidad = listaHabilidades.FirstOrDefault(x => x.Habilidad.Consecutivo == habilidadExistente.CodigoHabilidad);

                        if (habilidad != null)
                        {
                            habilidad.EstaAgregada = true;
                            habilidad.NumeroEstrellas = habilidadExistente.NumeroEstrellas;
                            _habilidadesParaAgregar.Add(habilidad);
                        }
                    }
                }

                Habilidades = new ObservableCollection<HabilidadesModel>(listaHabilidades);

                int numeroEstrellasGastadas = _habilidadesParaAgregar.Sum(x => x.NumeroEstrellas);
                _estrellasMaximasAlcanzadas = numeroEstrellasGastadas == _numeroEstrellasMaximas;

                RaisePropertyChanged(nameof(NumeroEstrellasGastadasColor));
            }
            catch (Exception)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorListarHabilidades, "OK");
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            HabilidadesModel.NumeroEstrellasCambiadas += HabilidadesModel_NumeroEstrellasCambiadas;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
            HabilidadesModel.NumeroEstrellasCambiadas -= HabilidadesModel_NumeroEstrellasCambiadas;
        }

        void HabilidadesModel_NumeroEstrellasCambiadas(object sender, HabilidadesModelArgs e)
        {
            bool yaEstaAgregada = e.HabilidadCambiada.EstaAgregada;

            if (!yaEstaAgregada && e.HabilidadCambiada.NumeroEstrellas > 0)
            {
                ToogleAgregarEntidad.Execute(e.HabilidadCambiada);
            }
            else if (yaEstaAgregada && e.HabilidadCambiada.NumeroEstrellas == 0)
            {
                ToogleAgregarEntidad.Execute(e.HabilidadCambiada);
            }
            else
            {
                int numeroEstrellasGastadas = _habilidadesParaAgregar.Sum(x => x.NumeroEstrellas);
                _estrellasMaximasAlcanzadas = numeroEstrellasGastadas == _numeroEstrellasMaximas;

                RaisePropertyChanged(nameof(NumeroEstrellasGastadasColor));

                // Verifico su nuevo estado si esta agregado o no
                if (yaEstaAgregada)
                {
                    if (numeroEstrellasGastadas > _numeroEstrellasMaximas)
                    {
                        e.HabilidadCambiada.NumeroEstrellas -= numeroEstrellasGastadas - _numeroEstrellasMaximas;

                        if (e.HabilidadCambiada.NumeroEstrellas <= 0)
                        {
                            e.HabilidadCambiada.EstaAgregada = false;
                            RaisePropertyChanged(nameof(Habilidades));
                        }
                    }
                }
            }

            RaisePropertyChanged(nameof(NumeroEstrellasGastadas));
        }

        // Borramos y agregamos habilidades de nuestra lista de control, y toogleamos la booleana que indica si esta agregada
        // Para asi el icono pueda cambiar, aumentamos el contador de igual manera
        public ICommand ToogleAgregarEntidad
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    HabilidadesModel habilidad = parameter as HabilidadesModel;

                    bool nuevoEstado = !habilidad.EstaAgregada && habilidad.NumeroEstrellas > 0;

                    int numeroEstrellasGastadas = _habilidadesParaAgregar.Sum(x => x.NumeroEstrellas);

                    // Verifico su nuevo estado si esta agregado o no
                    if (nuevoEstado)
                    {
                        numeroEstrellasGastadas += habilidad.NumeroEstrellas;

                        if (numeroEstrellasGastadas > _numeroEstrellasMaximas)
                        {
                            habilidad.NumeroEstrellas -= numeroEstrellasGastadas - _numeroEstrellasMaximas;

                            if (habilidad.NumeroEstrellas <= 0)
                            {
                                nuevoEstado = false;
                                RaisePropertyChanged(nameof(Habilidades));
                            }
                        }
                    }
                    else
                    {
                        numeroEstrellasGastadas -= habilidad.NumeroEstrellas;
                    }

                    _estrellasMaximasAlcanzadas = numeroEstrellasGastadas == _numeroEstrellasMaximas;
                    RaisePropertyChanged(nameof(NumeroEstrellasGastadasColor));

                    habilidad.EstaAgregada = nuevoEstado;

                    if (habilidad.EstaAgregada)
                    {
                        _habilidadesParaAgregar.Add(habilidad);
                    }
                    else
                    {
                        _habilidadesParaAgregar.Remove(habilidad);
                    }

                    RaisePropertyChanged(nameof(NumerosHabilidadesSeleccionadas));
                    RaisePropertyChanged(nameof(NumeroEstrellasGastadas));

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand GuardarHabilidades
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (_habilidadesParaAgregar == null || _habilidadesParaAgregar.Count <= 0)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DebesRegistrarHabilidad, "OK");
                        tcs.SetResult(true);
                        return;
                    }

                    List<HabilidadesCandidatosDTO> listaHabilidades = new List<HabilidadesCandidatosDTO>();

                    // Transformamos las habilidades seleccionadas como habilidades candidato, la lista que esta antes de esta pantalla
                    // Trabaja con habilidades candidatos y asi mismo para poder guardar esas habilidades para el candidato
                    foreach (var habilidadModel in _habilidadesParaAgregar)
                    {
                        HabilidadesCandidatosDTO habilidadCan = new HabilidadesCandidatosDTO(habilidadModel.Habilidad, habilidadModel.NumeroEstrellas, _habilidadesModel.CodigoCategoriaPerfilParaGuardarHabilidades);
                        listaHabilidades.Add(habilidadCan);
                    }

                    // Si ya tengo un perfil valido mando a guardarle las habilidades
                    if (SePuedeAgregarHabilidades)
                    {
                        try
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapper = await _habilidadesServices.CrearHabilidadesCandidato(listaHabilidades);
                        }
                        catch (Exception)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorGuardarHabilidades, "OK");
                            tcs.SetResult(true);
                            return;
                        }
                    }

                    await CoreMethods.PopPageModel(listaHabilidades);
                    tcs.SetResult(true);
                });
            }
        }
    }
}
