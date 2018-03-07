using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class CategoriaPageModel : BasePageModel
    {
        CategoriasServices _categoriaService;
        HabilidadesServices _habilidadesService;

        PersonasDTO _persona;
        CategoriasModel _categoriaSeleccionada;
        CategoriasModel _categoriaParaVer;

        public ObservableCollection<CategoriasModel> Categorias { get; set; }

        // Index del segment control para saber que tipo de habilidad estoy enseñando
        public int SelectIndexSegmentBar { get; set; }

        // booleana para saber si listo solo las primeras 5 habilidades o todas
        public bool MostrandoTodo { get; set; }

        ObservableRangeCollection<HabilidadesCandidatosDTO> _habilidadesCandidato;
        public ObservableRangeCollection<HabilidadesCandidatosDTO> HabilidadesCandidato
        {
            get
            {
                ObservableRangeCollection<HabilidadesCandidatosDTO> listaParaRetornar = null;
                if (_habilidadesCandidato != null)
                {
                    // Si la lista no es nula, listo las habilidades para la categoria seleccionada y el tipo de habilidad seleccionada
                    IEnumerable<HabilidadesCandidatosDTO> habilidadesCandidato = _habilidadesCandidato.Where(x => x.Habilidades.CodigoTipoHabilidad == (SelectIndexSegmentBar + 1) && x.Habilidades.CodigoCategoria == _categoriaSeleccionada.CodigoCategoria);
                    if (!MostrandoTodo)
                    {
                        // Si no esta clickeado el mostrar todo, muestro solo los 5 primeros
                        listaParaRetornar = new ObservableRangeCollection<HabilidadesCandidatosDTO>(habilidadesCandidato.Take(5));
                    }
                    else
                    {
                        // Si esta clickeado el mostrar todo, muestro todo
                        listaParaRetornar = new ObservableRangeCollection<HabilidadesCandidatosDTO>(habilidadesCandidato);
                    }
                }

                return listaParaRetornar;
            }
            private set
            {
                _habilidadesCandidato = value;
            }
        }

        public bool EsMiPersonaYNoPrimerRegistro
        {
            get
            {
                // Si el codigo de persona del usuario que inicio la sesion es igual al que estoy viendo, es que soy yo
                // Tener en cuenta que en el primer registro esto es "FALSE" siempre
                return App.Persona != null && _persona.Consecutivo == App.Persona.Consecutivo;
            }
        }

        public bool EsPrimerRegistro
        {
            get
            {
                // Si el codigo de persona es invalido y estoy registrando la categoria es que es un primer registro
                return _persona.Consecutivo <= 0 && App.Persona == null;
            }
        }

        public bool EsPrimerRegistroEdicionCategoria
        {
            get
            {
                // Si es primer registro y el codigo de categoria es valido, es que estoy editando una categoria en el primer registro
                return EsPrimerRegistro && _categoriaParaVer.CodigoCategoria > 0;
            }
        }

        public bool EsRegistroCategoria
        {
            get
            {
                // Si el codigo de categoria es 0 o menor es que es invalido y significa que estoy registrando una categoria
                return _categoriaParaVer.CodigoCategoriaPerfil <= 0;
            }
        }

        public string TituloPage
        {
            get
            {
                string tituloPage = SportsGoResources.VerDeporte;

                if (EsRegistroCategoria)
                {
                    tituloPage = SportsGoResources.CrearDeporte;
                }
                else if (_persona.Consecutivo == App.Usuario.PersonaDelUsuario.Consecutivo || EsPrimerRegistroEdicionCategoria)
                {
                    tituloPage = SportsGoResources.EditarDeporte;
                }

                return tituloPage;
            }
        }

        public bool EsCandidato
        {
            get
            {
                return _persona.TipoPerfil == TipoPerfil.Candidato;
            }
        }

        public bool SePuedeBorrarLaCategoria
        {
            get
            {
                return (!EsRegistroCategoria && EsMiPersonaYNoPrimerRegistro) || (EsPrimerRegistro && _categoriaParaVer.CodigoCategoria > 0);
            }
        }

        public bool EsMiPersonaORegistro
        {
            get
            {
                // Solo usado para el .xaml
                return EsRegistroCategoria || EsMiPersonaYNoPrimerRegistro;
            }
        }

        public bool PuedeSeleccionarUbicacionEnElCampo
        {
            get
            {
                // Solo usado para el .xaml
                return EsCandidato && (_categoriaSeleccionada.EsBaseBall || _categoriaSeleccionada.EsBasketBall || _categoriaSeleccionada.EsFutbol || _categoriaSeleccionada.EsVolleyBall);
            }
        }

        public CategoriaPageModel()
        {
            _categoriaService = new CategoriasServices();
            _habilidadesService = new HabilidadesServices();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            ControlPerfil control = initData as ControlPerfil;

            // La persona que estoy viendo, puede ser yo, otra persona o un primer registro
            _persona = control.PersonaParaVer;

            // La categoria que seleccione para ver, si es una nueva, el codigo de categoria es <= 0
            // Si es registro de categoria esto es nulo y no hay problema
            _categoriaParaVer = control.CategoriaSeleccionada;

            // Marco esa categoria como seleccionada
            // Si es registro de categoria esto es nulo y no hay problema
            _categoriaSeleccionada = control.CategoriaSeleccionada;

            // Si voy a registrar una nueva categoria, soy yo o es un nuevo registro
            if (EsMiPersonaYNoPrimerRegistro || EsPrimerRegistro)
            {
                CategoriasDTO categoria = new CategoriasDTO
                {
                    IdiomaBase = App.IdiomaPersona
                };

                try
                {
                    if (IsNotConnected) return;
                    List<CategoriasModel> listaCategorias = CategoriasModel.CrearListaCategorias(await _categoriaService.ListarCategoriasPorIdioma(categoria));

                    // Filtro las categorias que ya estan agregadas y que no es la que seleccione, asi evito repetir categorias
                    listaCategorias = listaCategorias.Where(x => x.CodigoCategoria == _categoriaParaVer.CodigoCategoria || !control.CategoriasQueYaEstanAgregadas.Contains(x.CodigoCategoria)).ToList();

                    if (!EsRegistroCategoria || EsPrimerRegistroEdicionCategoria)
                    {
                        // Si no estoy registrando una categoria, selecciono la categoria que estoy editando
                        CategoriasModel categoriaModel = listaCategorias.Where(x => x.CodigoCategoria == _categoriaParaVer.CodigoCategoria).FirstOrDefault();

                        categoriaModel.EstaSeleccionado = true;
                        categoriaModel.CodigoCategoriaPerfil = _categoriaParaVer.CodigoCategoriaPerfil; 

                        listaCategorias.Remove(categoriaModel);
                        listaCategorias.Insert(0, categoriaModel);

                        categoriaModel.PosicionJugador = control.CategoriaSeleccionada.PosicionJugador;

                        _categoriaParaVer = categoriaModel;
                        _categoriaSeleccionada = categoriaModel;
                    }

                    Categorias = new ObservableCollection<CategoriasModel>(listaCategorias);
                }
                catch (Exception)
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorListarDeportes, "OK");
                    return;
                }
            }
            else
            {
                // Si no soy yo o no es un primer registro, cargo la categoria que estoy viendo nada mas
                Categorias = new ObservableCollection<CategoriasModel> { _categoriaParaVer };
            }

            // Si soy candidato, no estoy registrando una categoria y no es un primer registro, 
            // Listo las habilidades del candidato, sin importar si soy yo u otra persona viendo mi perfil
            if (EsCandidato && !EsRegistroCategoria && !EsPrimerRegistro)
            {
                HabilidadesCandidatosDTO habilidadesCandidatoParaBuscar = new HabilidadesCandidatosDTO
                {
                    CodigoCategoriaCandidato = _categoriaParaVer.CodigoCategoriaPerfil,
                    IdiomaBase = App.IdiomaPersona
                };

                try
                {
                    if (IsNotConnected) return;
                    HabilidadesCandidato = new ObservableRangeCollection<HabilidadesCandidatosDTO>(await _habilidadesService.ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(habilidadesCandidatoParaBuscar) ?? new List<HabilidadesCandidatosDTO>());
                }
                catch (Exception)
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorListarHabilidades, "OK");
                }
            }
            else if (EsCandidato && EsPrimerRegistroEdicionCategoria)
            {
                CategoriasCandidatosDTO categoriaParaVer = _persona.CandidatoDeLaPersona.CategoriasCandidatos.Where(x => x.CodigoCategoria == _categoriaParaVer.CodigoCategoria).FirstOrDefault();
                HabilidadesCandidato = new ObservableRangeCollection<HabilidadesCandidatosDTO>(categoriaParaVer.HabilidadesCandidatos);
            }
        }

        public override void ReverseInit(object returnData)
        {
            base.ReverseInit(returnData);

            // La lista de habilidades que guarde de la seleccion de nuevas habilidades
            List<HabilidadesCandidatosDTO> listaHabilidades = returnData as List<HabilidadesCandidatosDTO>;

            if (listaHabilidades != null)
            {
                HabilidadesCandidato = new ObservableRangeCollection<HabilidadesCandidatosDTO>(listaHabilidades);
            }
            else
            {
                // Categoria devuelta por la pantalla de seleccion de posicion del jugador
                PosicionJugadorModel posicionModel = returnData as PosicionJugadorModel;

                if (posicionModel != null)
                {
                    _categoriaSeleccionada = posicionModel.CategoriaParaUbicar;
                }
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        public ICommand ToogleMostrar
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    // Para modificar la booleana que indica si se muestra todo o solo las primeras 5
                    MostrandoTodo = !MostrandoTodo;
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrPosicion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    // Valido que tengo una categoria seleccionada y se la paso a la pagina para seleccionar la posicion
                    if (EsMiPersonaORegistro && (_categoriaSeleccionada == null || _categoriaSeleccionada.EstaSeleccionado == false))
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DebesSeleccionarDeporte, "OK");
                    }
                    else
                    {
                        PosicionJugadorModel posicionModel = new PosicionJugadorModel
                        {
                            CategoriaParaUbicar = _categoriaSeleccionada,
                            EsRegistroCategoria = EsRegistroCategoria,
                            EsMiPersonaORegistro = EsMiPersonaORegistro
                        };

                        await CoreMethods.PushPageModel<PosicionJugadorPageModel>(posicionModel);
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand ToogleSeleccion
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    CategoriasModel seleccion = parameter as CategoriasModel;

                    // Si estoy registrando una categoria, soy yo o es un primer registro, sombreo la categoria que estoy seleccionando
                    if (EsRegistroCategoria || EsMiPersonaYNoPrimerRegistro || EsPrimerRegistro || EsPrimerRegistroEdicionCategoria)
                    {
                        if (_categoriaSeleccionada != null)
                            _categoriaSeleccionada.EstaSeleccionado = false;

                        _categoriaSeleccionada = seleccion;

                        if (_categoriaSeleccionada != null)
                            _categoriaSeleccionada.EstaSeleccionado = true;

                        RaisePropertyChanged(nameof(PuedeSeleccionarUbicacionEnElCampo));
                        RaisePropertyChanged(nameof(HabilidadesCandidato));
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand AgregarHabilidades
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    // Valido que tengo una categoria seleccionada y se la paso a la pagina para seleccionar nuevas habilidades
                    if (_categoriaSeleccionada == null || _categoriaSeleccionada.EstaSeleccionado == false)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DebesSeleccionarDeporte, "OK");
                        tcs.SetResult(true);
                        return;
                    }

                    // Paso las habilidades actuales seleccionadas para la categoria que seleccione a la pagina de seleccion de habilidades
                    // La pagina de seleccion de nuevas habilidades lista todas y marca las habilidades actuales que ya estan registradas
                    HabilidadesModel habilidadModel = null;

                    if (_habilidadesCandidato != null)
                    {
                        habilidadModel = new HabilidadesModel(_habilidadesCandidato.Where(x => x.Habilidades.CodigoCategoria == _categoriaSeleccionada.CodigoCategoria).ToList(), _categoriaSeleccionada);
                    }
                    else
                    {
                        habilidadModel = new HabilidadesModel(_categoriaSeleccionada);
                    }

                    await CoreMethods.PushPageModel<HabilidadesPageModel>(habilidadModel);
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand GuardarCategoria
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsRegistroCategoria || EsMiPersonaYNoPrimerRegistro || EsPrimerRegistro || EsPrimerRegistroEdicionCategoria)
                    {
                        // Valido que tengo una categoria seleccionada para guardar
                        // Si soy candidato, valido que tengo habilidades registradas
                        if (_categoriaSeleccionada == null || _categoriaSeleccionada.EstaSeleccionado == false)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DebesSeleccionarDeporte, "OK");
                            tcs.SetResult(true);
                            return;
                        }
                        else if (_persona.TipoPerfil == TipoPerfil.Candidato && (_habilidadesCandidato == null || _habilidadesCandidato.Where(x => x.Habilidades.CodigoCategoria == _categoriaSeleccionada.CodigoCategoria).Count() <= 0))
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.DebesRegistrarHabilidad, "OK");
                            tcs.SetResult(true);
                            return;
                        }

                        // Si soy yo, mando a guardar la categoria con la que estamos trabajando
                        // O modifico la categoria que se selecciono con la que estamos trabajando
                        // En los metodos esta la logica para saber si modifica o crea
                        if (EsMiPersonaYNoPrimerRegistro)
                        {
                            try
                            {
                                WrapperSimpleTypesDTO wrapper = null;
                                switch (_persona.TipoPerfil)
                                {
                                    case TipoPerfil.Candidato:
                                        wrapper = await AsignarCategoriaCandidato();
                                        break;
                                    case TipoPerfil.Grupo:
                                        wrapper = await AsignarCategoriaGrupo();
                                        break;
                                    case TipoPerfil.Representante:
                                        wrapper = await AsignarCategoriaRepresentante();
                                        break;
                                }

                                // Si creo una nueva categoria, el wrapper me devuelve un nuevo consecutivo,
                                // Si la estoy editando entonces el wrapper no tendra consecutivo y usare el consecutivo de la categoria que estoy editando
                                if (wrapper != null && wrapper.Exitoso)
                                {
                                    int consecutivoCreado = _categoriaParaVer.CodigoCategoriaPerfil;

                                    if (wrapper.ConsecutivoCreado > 0)
                                    {
                                        consecutivoCreado = (int)wrapper.ConsecutivoCreado;
                                    }

                                    // Sustituyo la categoria que estoy viendo (si hay alguna) con la categoria que seleccione para guardar 
                                    RemoverCategoriaParaVerDeEntidad();
                                    AdicionarCategoriaSeleccionadaEnEntidad(consecutivoCreado);
                                    await CoreMethods.PopPageModel(_persona);
                                }
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorGuardarDeporte, "OK");
                            }
                        }
                        else
                        {
                            // Sustituyo la categoria que estoy viendo (si hay alguna) con la categoria que seleccione para guardar 
                            // Si es primer registro, solo la agrego a la entidad, no la mando a guardar y me devuelvo a la pantalla de registro
                            // Alli se guarda todo el bloque
                            RemoverCategoriaParaVerDeEntidad();
                            AdicionarCategoriaSeleccionadaEnEntidad();
                            await CoreMethods.PopPageModel(_persona);
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand BorrarHabilidad
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    HabilidadesCandidatosDTO habilidadParaBorrar = parameter as HabilidadesCandidatosDTO;

                    if (EsRegistroCategoria || EsMiPersonaYNoPrimerRegistro || EsPrimerRegistro || EsPrimerRegistroEdicionCategoria)
                    {
                        // Si no estoy registrando una categoria y soy yo, mando a borrar la habilidad
                        if (!EsRegistroCategoria && EsMiPersonaYNoPrimerRegistro)
                        {
                            try
                            {
                                if (IsNotConnected)
                                {
                                    tcs.SetResult(true);
                                    return;
                                }
                                WrapperSimpleTypesDTO wrapper = await _habilidadesService.EliminarHabilidadCandidato(habilidadParaBorrar);

                                if (wrapper != null && wrapper.Exitoso)
                                {
                                    _habilidadesCandidato.Remove(habilidadParaBorrar);
                                    RaisePropertyChanged("HabilidadesCandidato");
                                }
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorEliminarHabilidad, "OK");
                            }
                        }
                        else
                        {
                            // Si soy yo o primer registro y estoy registrando una categoria simplemente borro la habilidad de la lista
                            _habilidadesCandidato.Remove(habilidadParaBorrar);
                            RaisePropertyChanged("HabilidadesCandidato");
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand BorrarCategoria
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersonaYNoPrimerRegistro || EsPrimerRegistroEdicionCategoria)
                    {
                        if (!EsRegistroCategoria && EsMiPersonaYNoPrimerRegistro)
                        {
                            try
                            {
                                WrapperSimpleTypesDTO wrapper = null;
                                switch (_persona.TipoPerfil)
                                {
                                    case TipoPerfil.Candidato:
                                        wrapper = await BorrarCategoriaCandidato();
                                        break;
                                    case TipoPerfil.Grupo:
                                        wrapper = await BorrarCategoriaGrupo();
                                        break;
                                    case TipoPerfil.Representante:
                                        wrapper = await BorrarCategoriaRepresentante();
                                        break;
                                }

                                if (wrapper != null && wrapper.Exitoso)
                                {
                                    RemoverCategoriaParaVerDeEntidad();
                                    await CoreMethods.PopPageModel(_persona);
                                }
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorEliminarDeporte, "OK");
                            }
                        }
                        else if(EsPrimerRegistroEdicionCategoria)
                        {
                            RemoverCategoriaParaVerDeEntidad();
                            await CoreMethods.PopPageModel(_persona);
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<WrapperSimpleTypesDTO> AsignarCategoriaCandidato()
        {
            // Guardo la categoria que seleccione para guardar
            CategoriasCandidatosDTO categoriaCandidatoParaBorrar = new CategoriasCandidatosDTO
            {
                CodigoCandidato = _persona.CandidatoDeLaPersona.Consecutivo,
                CodigoCategoria = _categoriaSeleccionada.CodigoCategoria,
                PosicionCampo = _categoriaSeleccionada.PosicionJugador,
                HabilidadesCandidatos = _habilidadesCandidato.Where(x => x.Habilidades.CodigoCategoria == _categoriaSeleccionada.CodigoCategoria).ToList()
            };

            WrapperSimpleTypesDTO wrapper = null;
            if (!EsRegistroCategoria)
            {
                // Jalo el codigo de categoria que estoy viendo, no la que estoy seleccionando
                categoriaCandidatoParaBorrar.Consecutivo = _categoriaParaVer.CodigoCategoriaPerfil;

                if (IsNotConnected) return null;
                wrapper = await _categoriaService.ModificarCategoriaCandidato(categoriaCandidatoParaBorrar);
            }
            else
            {
                if (IsNotConnected) return null;
                wrapper = await _categoriaService.CrearCategoriaCandidatos(categoriaCandidatoParaBorrar);
            }

            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> AsignarCategoriaGrupo()
        {
            // Guardo la categoria que seleccione para guardar
            CategoriasGruposDTO categoriaGrupoParaBorrar = new CategoriasGruposDTO
            {
                CodigoGrupo = _persona.GrupoDeLaPersona.Consecutivo,
                CodigoCategoria = _categoriaSeleccionada.CodigoCategoria,
            };

            WrapperSimpleTypesDTO wrapper = null;
            if (!EsRegistroCategoria)
            {
                // Jalo el codigo de categoria que estoy viendo, no la que estoy seleccionando
                categoriaGrupoParaBorrar.Consecutivo = _categoriaParaVer.CodigoCategoriaPerfil;

                if (IsNotConnected) return null;
                wrapper = await _categoriaService.ModificarCategoriaGrupo(categoriaGrupoParaBorrar);
            }
            else
            {
                if (IsNotConnected) return null;
                wrapper = await _categoriaService.CrearCategoriaGrupos(categoriaGrupoParaBorrar);
            }

            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> AsignarCategoriaRepresentante()
        {
            // Guardo la categoria que seleccione para guardar
            CategoriasRepresentantesDTO categoriaRepresentante = new CategoriasRepresentantesDTO
            {
                CodigoRepresentante = _persona.RepresentanteDeLaPersona.Consecutivo,
                CodigoCategoria = _categoriaSeleccionada.CodigoCategoria,
            };

            WrapperSimpleTypesDTO wrapper = null;
            if (!EsRegistroCategoria)
            {
                // Jalo el codigo de categoria que estoy viendo, no la que estoy seleccionando
                categoriaRepresentante.Consecutivo = _categoriaParaVer.CodigoCategoriaPerfil;

                if (IsNotConnected) return null;
                wrapper = await _categoriaService.ModificarCategoriaRepresentante(categoriaRepresentante);
            }
            else
            {
                if (IsNotConnected) return null;
                wrapper = await _categoriaService.CrearCategoriaRepresentante(categoriaRepresentante);
            }

            return wrapper;
        }

        void AdicionarCategoriaSeleccionadaEnEntidad(int consecutivoCreado = 0)
        {
            switch (_persona.TipoPerfil)
            {
                case TipoPerfil.Candidato:

                    CategoriasCandidatosDTO categoriaCandidato = new CategoriasCandidatosDTO
                    {
                        Consecutivo = consecutivoCreado,
                        CodigoCategoria = _categoriaSeleccionada.CodigoCategoria,
                        CodigoCandidato = _persona.CandidatoDeLaPersona.Consecutivo,
                        PosicionCampo = _categoriaSeleccionada.PosicionJugador,
                        HabilidadesCandidatos = _habilidadesCandidato.Where(x => x.Habilidades.CodigoCategoria == _categoriaSeleccionada.CodigoCategoria).ToList(),
                        Categorias = new CategoriasDTO
                        {
                            CodigoArchivo = _categoriaSeleccionada.CodigoArchivo
                        }
                    };

                    _persona.CandidatoDeLaPersona.CategoriasCandidatos.Add(categoriaCandidato);

                    break;
                case TipoPerfil.Grupo:

                    CategoriasGruposDTO categoriaGrupo = new CategoriasGruposDTO
                    {
                        Consecutivo = consecutivoCreado,
                        CodigoCategoria = _categoriaSeleccionada.CodigoCategoria,
                        CodigoGrupo = _persona.GrupoDeLaPersona.Consecutivo,
                        Categorias = new CategoriasDTO
                        {
                            CodigoArchivo = _categoriaSeleccionada.CodigoArchivo
                        }
                    };

                    _persona.GrupoDeLaPersona.CategoriasGrupos.Add(categoriaGrupo);

                    break;
                case TipoPerfil.Representante:

                    CategoriasRepresentantesDTO categoriaRepresentante = new CategoriasRepresentantesDTO
                    {
                        Consecutivo = consecutivoCreado,
                        CodigoCategoria = _categoriaSeleccionada.CodigoCategoria,
                        CodigoRepresentante = _persona.RepresentanteDeLaPersona.Consecutivo,
                        Categorias = new CategoriasDTO
                        {
                            CodigoArchivo = _categoriaSeleccionada.CodigoArchivo
                        }
                    };

                    _persona.RepresentanteDeLaPersona.CategoriasRepresentantes.Add(categoriaRepresentante);

                    break;
            }
        }

        async Task<WrapperSimpleTypesDTO> BorrarCategoriaCandidato()
        {

            CategoriasCandidatosDTO categoriaCandidatoParaBorrar = new CategoriasCandidatosDTO
            {
                Consecutivo = _categoriaParaVer.CodigoCategoriaPerfil
            };

            if (IsNotConnected) return null;
            WrapperSimpleTypesDTO wrapper = await _categoriaService.EliminarCategoriaCandidato(categoriaCandidatoParaBorrar);
            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> BorrarCategoriaGrupo()
        {
            CategoriasGruposDTO categoriaGrupoParaBorrar = new CategoriasGruposDTO
            {
                Consecutivo = _categoriaParaVer.CodigoCategoriaPerfil
            };

            if (IsNotConnected) return null;
            WrapperSimpleTypesDTO wrapper = await _categoriaService.EliminarCategoriaGrupo(categoriaGrupoParaBorrar);
            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> BorrarCategoriaRepresentante()
        {
            CategoriasRepresentantesDTO categoriaRepresentante = new CategoriasRepresentantesDTO
            {
                Consecutivo = _categoriaParaVer.CodigoCategoriaPerfil
            };

            if (IsNotConnected) return null;
            WrapperSimpleTypesDTO wrapper = await _categoriaService.EliminarCategoriaRepresentante(categoriaRepresentante);
            return wrapper;
        }

        void RemoverCategoriaParaVerDeEntidad()
        {
            switch (_persona.TipoPerfil)
            {
                case TipoPerfil.Candidato:
                    CategoriasCandidatosDTO categoriaCandidato = _persona.CandidatoDeLaPersona.CategoriasCandidatos.Where(x => x.CodigoCategoria == _categoriaParaVer.CodigoCategoria).FirstOrDefault();
                    _persona.CandidatoDeLaPersona.CategoriasCandidatos.Remove(categoriaCandidato);

                    break;
                case TipoPerfil.Grupo:
                    CategoriasGruposDTO categoriaGrupo = _persona.GrupoDeLaPersona.CategoriasGrupos.Where(x => x.CodigoCategoria == _categoriaParaVer.CodigoCategoria).FirstOrDefault();
                    _persona.GrupoDeLaPersona.CategoriasGrupos.Remove(categoriaGrupo);

                    break;
                case TipoPerfil.Representante:
                    CategoriasRepresentantesDTO categoriaRepresentante = _persona.RepresentanteDeLaPersona.CategoriasRepresentantes.Where(x => x.CodigoCategoria == _categoriaParaVer.CodigoCategoria).FirstOrDefault();
                    _persona.RepresentanteDeLaPersona.CategoriasRepresentantes.Remove(categoriaRepresentante);

                    break;
            }
        }
    }
}
