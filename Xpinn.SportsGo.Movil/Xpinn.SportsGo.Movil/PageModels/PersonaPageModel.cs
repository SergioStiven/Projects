using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class PersonaPageModel : BasePageModel
    {
        TipoPerfil _tipoPerfil;
        string _viejoCorreo;
        PersonasServices _personaServices;
        AdministracionServices _administracionServices;
        AuthenticateServices _authenticateServices;

        public PersonasDTO Persona { get; set; }
        public ObservableCollection<PaisesDTO> Paises { get; set; }
        public DateTime FechaMaxima { get { return DateTime.Now; } }

        public bool EsMiPersonaOPrimerRegistro
        {
            get
            {
                // Solo usado para el .xaml
                return EsPrimerRegistro || EsMiPersona;
            }
        }

        public bool EsMiPersona
        {
            get
            {
                return App.Persona != null && Persona.Consecutivo == App.Persona.Consecutivo;
            }
        }

        public bool EsPrimerRegistro
        {
            get
            {
                return Persona.Consecutivo <= 0 || App.Persona == null;
            }
        }

        public bool RequiereTutor
        {
            get
            {
                bool requiereTutor = false;

                if (Persona.CandidatoDeLaPersona != null)
                {
                    requiereTutor = DateTimeHelper.DiferenciaEntreDosFechasAños(DateTime.Now, Persona.CandidatoDeLaPersona.FechaNacimiento) < 18;
                }

                if (requiereTutor && Persona.CandidatoDeLaPersona.CandidatosResponsables == null)
                {
                    Persona.CandidatoDeLaPersona.CandidatosResponsables = new CandidatosResponsablesDTO();
                }

                return requiereTutor;
            }
        }

        public bool EsCandidatoOEsRepresentante
        {
            get
            {
                return EsCandidato || EsRepresentante;
            }
        }

        public DateTime FechaNacimiento
        {
            get
            {
                DateTime fechaNacimiento = DateTime.MinValue;

                if (Persona.CandidatoDeLaPersona != null)
                {
                    if (Persona.CandidatoDeLaPersona.FechaNacimiento == DateTime.MinValue)
                    {
                        Persona.CandidatoDeLaPersona.FechaNacimiento = DateTime.Now;
                        fechaNacimiento = DateTime.Now;
                    }
                    else
                    {
                        fechaNacimiento = Persona.CandidatoDeLaPersona.FechaNacimiento;
                    }
                }

                return fechaNacimiento;
            }
            set
            {
                if (Persona.CandidatoDeLaPersona != null)
                {
                    Persona.CandidatoDeLaPersona.FechaNacimiento = value;
                    RaisePropertyChanged(nameof(RequiereTutor));
                }
            }
        }

        public string FechaNacimientoString
        {
            get
            {
                return FechaNacimiento.ToString("d");
            }
        }

        public int AlturaImagen
        {
            get
            {
                int alturaImagen = 40;

                if (Persona.CandidatoDeLaPersona != null)
                {
                    alturaImagen = Convert.ToInt32(Math.Round(Persona.CandidatoDeLaPersona.Estatura / 3.0, 0));
                }

                if (alturaImagen < 40)
                {
                    alturaImagen = 40;
                }

                return alturaImagen;
            }
        }

        public int AnchoImagen
        {
            get
            {
                int anchoImagen = 35;

                if (Persona.CandidatoDeLaPersona != null)
                {
                    anchoImagen = Convert.ToInt32(Math.Round(Persona.CandidatoDeLaPersona.Peso / 2.0, 0));
                }

                if (anchoImagen < 35)
                {
                    anchoImagen = 35;
                }

                return anchoImagen;
            }
        }

        public bool EsHombre
        {
            get
            {
                bool esHombre = true;

                if (Persona.CandidatoDeLaPersona != null)
                {
                    if (Persona.CandidatoDeLaPersona.TipoGenero == TipoGeneros.Mujer)
                    {
                        esHombre = false;
                    }

                    if (Persona.CandidatoDeLaPersona.TipoGenero == TipoGeneros.SinGenero)
                    {
                        Persona.CandidatoDeLaPersona.TipoGenero = TipoGeneros.Hombre;
                        esHombre = true;
                    }
                }

                return esHombre;
            }
            set
            {
                if (Persona.CandidatoDeLaPersona != null)
                {
                    // Si soy hombre
                    if (value)
                    {
                        Persona.CandidatoDeLaPersona.TipoGenero = TipoGeneros.Hombre;
                    }
                    else
                    {
                        Persona.CandidatoDeLaPersona.TipoGenero = TipoGeneros.Mujer;
                    }
                }
            }
        }

        IdiomaModel _idiomaSeleccionado;
        public IdiomaModel IdiomaSeleccionado
        {
            get
            {
                if (_idiomaSeleccionado != null)
                {
                    return _idiomaSeleccionado;
                }
                else
                {
                    return App.ListaIdioma.Where(x => x.Idioma == Persona.IdiomaDeLaPersona).FirstOrDefault();
                }
            }
            set
            {
                _idiomaSeleccionado = value;

                if (value != null)
                {
                    Persona.IdiomaDeLaPersona = value.Idioma;
                }
            }
        }

        PaisesDTO _paisSeleccionado;
        public PaisesDTO PaisSeleccionado
        {
            get
            {
                PaisesDTO paisSeleccionado = null;

                if (_paisSeleccionado != null)
                {
                    paisSeleccionado = _paisSeleccionado;
                }
                else if (Persona.Paises != null && Paises != null)
                {
                    paisSeleccionado = Paises.Where(x => x.Consecutivo == Persona.CodigoPais).FirstOrDefault();
                }

                return paisSeleccionado;
            }
            set
            {
                if (value != null)
                {
                    _paisSeleccionado = value;

                    Persona.Paises = _paisSeleccionado;
                    Persona.CodigoPais = value.Consecutivo;
                }
            }
        }

        public string ImagenPais
        {
            get
            {
                string imagenPais = string.Empty;

                if (_paisSeleccionado != null)
                {
                    imagenPais = _paisSeleccionado.UrlArchivo;
                }

                return imagenPais;
            }
        }

        public string TituloPage
        {
            get
            {
                string tituloPage = SportsGoResources.VerPerfil;

                if (EsPrimerRegistro)
                {
                    tituloPage = SportsGoResources.CrearPerfil;
                }
                else if (EsMiPersona)
                {
                    tituloPage = SportsGoResources.EditarPerfil;
                }

                return tituloPage;
            }
        }

        int _alturaCandidato;
        public int AlturaCandidato
        {
            get
            {
                if (Persona.CandidatoDeLaPersona != null)
                {
                    if (Persona.CandidatoDeLaPersona.Estatura >= 130)
                    {
                        _alturaCandidato = Persona.CandidatoDeLaPersona.Estatura;
                    }
                    else if (_alturaCandidato < 130)
                    {
                        Persona.CandidatoDeLaPersona.Estatura = 130;
                        _alturaCandidato = 130;
                    }
                }

                return _alturaCandidato;
            }
            set
            {
                if (EsMiPersonaOPrimerRegistro)
                {
                    _alturaCandidato = value;

                    if (Persona.CandidatoDeLaPersona != null)
                    {
                        Persona.CandidatoDeLaPersona.Estatura = value;
                        RaisePropertyChanged(nameof(AlturaImagen));
                    }
                }
            }
        }

        int _pesoCandidato;
        public int PesoCandidato
        {
            get
            {
                if (Persona.CandidatoDeLaPersona != null)
                {
                    if (Persona.CandidatoDeLaPersona.Peso >= 20)
                    {
                        _pesoCandidato = Persona.CandidatoDeLaPersona.Peso;
                    }
                    else if (_pesoCandidato < 20)
                    {
                        Persona.CandidatoDeLaPersona.Peso = 20;
                        _pesoCandidato = 20;
                    }
                }

                return _pesoCandidato;
            }
            set
            {
                if (EsMiPersonaOPrimerRegistro)
                {
                    _pesoCandidato = value;

                    if (Persona.CandidatoDeLaPersona != null)
                    {
                        Persona.CandidatoDeLaPersona.Peso = value;
                        RaisePropertyChanged(nameof(AnchoImagen));
                    }
                }
            }
        }

        public bool EsCandidato
        {
            get
            {
                return _tipoPerfil == TipoPerfil.Candidato;
            }
        }

        public bool EsGrupo
        {
            get
            {
                return _tipoPerfil == TipoPerfil.Grupo;
            }
        }

        public bool EsRepresentante
        {
            get
            {
                return _tipoPerfil == TipoPerfil.Representante;
            }
        }

        public PersonaPageModel()
        {
            _personaServices = new PersonasServices();
            _administracionServices = new AdministracionServices();
            _authenticateServices = new AuthenticateServices();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            ControlPerfil control = initData as ControlPerfil;

            Persona = control.PersonaParaVer;
            _tipoPerfil = control.TipoPerfilControl;
            _viejoCorreo = Persona.Usuarios.Email;

            try
            {
                if (EsMiPersonaOPrimerRegistro)
                {
                    PaisesDTO paises = new PaisesDTO
                    {
                        IdiomaBase = App.IdiomaPersona
                    };

                    Paises = new ObservableCollection<PaisesDTO>(await _administracionServices.ListarPaisesPorIdioma(paises) ?? new List<PaisesDTO>());
                }
                else
                {
                    Paises = new ObservableCollection<PaisesDTO> { Persona.Paises };
                }
            }
            catch (Exception)
            {

            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        public ICommand SeleccionarGenero
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    bool? esHombre = parameter as bool?;

                    if (EsMiPersonaOPrimerRegistro && esHombre.HasValue)
                    {
                        EsHombre = esHombre.Value;
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand GuardarPersona
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersonaOPrimerRegistro && await ValidarDatosPersona())
                    {
                        bool esValido = false;

                        // Valido si la informacion esta completa, si esta completa paso, si no muestro alerta y no hago nada
                        switch (_tipoPerfil)
                        {
                            case TipoPerfil.Candidato:
                                esValido = await ValidarDatosCandidatos();
                                break;
                            case TipoPerfil.Grupo:
                                esValido = true;
                                break;
                            case TipoPerfil.Representante:
                                esValido = await ValidarDatosRepresentantes();
                                break;
                        }

                        if (esValido)
                        {
                            try
                            {
                                // Si tengo una persona valida la mando a modificar, si esto solo registrandome entonces solo me devuelvo a la anterior pagina
                                if (EsMiPersona)
                                {
                                    if (IsNotConnected)
                                    {
                                        tcs.SetResult(true);
                                        return;
                                    }
                                    WrapperSimpleTypesDTO wrapper = await _personaServices.ModificarPersona(Persona);

                                    UsuariosDTO usuario = new UsuariosDTO
                                    {
                                        Consecutivo = Persona.Usuarios.Consecutivo,
                                        Email = Persona.Usuarios.Email
                                    };

                                    if (IsNotConnected)
                                    {
                                        tcs.SetResult(true);
                                        return;
                                    }
                                    WrapperSimpleTypesDTO wrapperUsuario = await _authenticateServices.ModificarEmailUsuario(usuario);

                                    switch (_tipoPerfil)
                                    {
                                        case TipoPerfil.Candidato:
                                            CandidatosServices candidatoServices = new CandidatosServices();

                                            // Si tengo mas de 18 años no guardo tutor
                                            if (!RequiereTutor)
                                            {
                                                Persona.CandidatoDeLaPersona.CandidatosResponsables = null;
                                            }

                                            if (IsNotConnected) return;
                                            WrapperSimpleTypesDTO wrapperCandidato = await candidatoServices.ModificarInformacionCandidato(Persona.CandidatoDeLaPersona);

                                            if (RequiereTutor)
                                            {
                                                Persona.CandidatoDeLaPersona.CandidatosResponsables.CodigoCandidato = Persona.CandidatoDeLaPersona.Consecutivo;

                                                if (IsNotConnected) return;
                                                WrapperSimpleTypesDTO wrapperCandidatoResponsable = await candidatoServices.AsignarCandidatoResponsable(Persona.CandidatoDeLaPersona.CandidatosResponsables);
                                            }

                                            break;
                                        case TipoPerfil.Grupo:
                                            GruposServices grupoServices = new GruposServices();

                                            if (IsNotConnected) return;
                                            WrapperSimpleTypesDTO wrapperGrupo = await grupoServices.ModificarInformacionGrupo(Persona.GrupoDeLaPersona);

                                            break;
                                        case TipoPerfil.Representante:
                                            RepresentantesServices representanteServices = new RepresentantesServices();

                                            if (IsNotConnected) return;
                                            WrapperSimpleTypesDTO wrapperRepresentante = await representanteServices.ModificarInformacionRepresentante(Persona.RepresentanteDeLaPersona);
                                            break;
                                    }
                                }

                                App.ConfigureCultureIdiomsApp(Persona.IdiomaDeLaPersona);
                                await CoreMethods.PopPageModel(Persona);
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorGuardarInformacionPersonal, "OK");
                            }
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<bool> ValidarDatosPersona()
        {
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(Persona.Nombres) || string.IsNullOrWhiteSpace(Persona.Telefono) || string.IsNullOrWhiteSpace(Persona.CiudadResidencia)
                || Persona.CodigoIdioma <= 0 || Persona.CodigoPais <= 0 || string.IsNullOrWhiteSpace(Persona.Usuarios.Email))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPersona, "OK");
                esValido = false;
            }
            else if (_tipoPerfil != TipoPerfil.Grupo && string.IsNullOrWhiteSpace(Persona.Apellidos))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPersona, "OK");
                esValido = false;
            }
            else if (!Regex.IsMatch(Persona.Usuarios.Email.Trim(), AppConstants.RegexEmail))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.EmailFormatoInvalido, "OK");
                esValido = false;
            }
            else if (EsPrimerRegistro || (_viejoCorreo.Trim() != Persona.Usuarios.Email.Trim()))
            {
                UsuariosDTO usuarioEmail = new UsuariosDTO
                {
                    Email = Persona.Usuarios.Email
                };

                if (IsNotConnected) return false;
                WrapperSimpleTypesDTO wrapperEmail = await _authenticateServices.VerificarSiEmailYaExiste(usuarioEmail);

                if (wrapperEmail == null)
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorValidarEmail, "OK");
                    esValido = false;
                }
                else if (wrapperEmail.Existe)
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.EmailYaExiste, "OK");
                    esValido = false;
                }
            }

            return esValido;
        }

        async Task<bool> ValidarDatosCandidatos()
        {
            bool esValido = true;

            if (Persona.CandidatoDeLaPersona.Estatura <= 0 || Persona.CandidatoDeLaPersona.Peso <= 0 || Persona.CandidatoDeLaPersona.FechaNacimiento == DateTime.MinValue
                || Persona.CandidatoDeLaPersona.TipoGenero == TipoGeneros.SinGenero)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPerfil, "OK");
                esValido = false;
            }
            else if (DateTimeHelper.DiferenciaEntreDosFechasAños(DateTime.Now, Persona.CandidatoDeLaPersona.FechaNacimiento) < 18)
            {
                if (string.IsNullOrWhiteSpace(Persona.CandidatoDeLaPersona.CandidatosResponsables.Nombres)
                    || string.IsNullOrWhiteSpace(Persona.CandidatoDeLaPersona.CandidatosResponsables.TelefonoMovil) || string.IsNullOrWhiteSpace(Persona.CandidatoDeLaPersona.CandidatosResponsables.Email))
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltanDatosResponsable, "OK");
                    esValido = false;
                }
                else if (!Regex.IsMatch(Persona.CandidatoDeLaPersona.CandidatosResponsables.Email.Trim(), AppConstants.RegexEmail))
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.EmailFormatoInvalidoResponsable, "OK");
                    esValido = false;
                }
            }

            return esValido;
        }

        async Task<bool> ValidarDatosRepresentantes()
        {
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(Persona.RepresentanteDeLaPersona.NumeroIdentificacion))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPerfil, "OK");
                esValido = false;
            }

            return esValido;
        }
    }
}