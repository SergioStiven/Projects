using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ContactosChatPageModel : BasePageModel
    {
        ChatsServices _chatsServices;

        ObservableRangeCollection<ContactosDTO> _contactos;
        public ObservableRangeCollection<ContactosDTO> Contactos
        {
            get
            {
                if (_contactos != null)
                {
                    IEnumerable<ContactosDTO> listaFiltrada = _contactos;

                    // Si tengo una descripcion en el searchbar control, filtro por esa descripcion
                    if (!string.IsNullOrWhiteSpace(TextoBuscador))
                    {
                        listaFiltrada = listaFiltrada.Where(x =>
                        {
                            // Puede suceder y sucedio que la descripcion viene vacia, por un error al guardar puede ser
                            if (!string.IsNullOrWhiteSpace(x.PersonasContacto.NombreYApellido))
                            {
                                return x.PersonasContacto.NombreYApellido.ToUpperInvariant().Contains(TextoBuscador.ToUpperInvariant());
                            }

                            return false;
                        });
                    }

                    return new ObservableRangeCollection<ContactosDTO>(listaFiltrada);
                }
                else
                {
                    return new ObservableRangeCollection<ContactosDTO>();
                }
            }
        }

        public bool NoHayNadaMasParaCargar { get; set; }
        public int SkipIndexWithText { get; set; }

        string _textoBuscador;
        public string TextoBuscador
        {
            get
            {
                return _textoBuscador;
            }
            set
            {
                _textoBuscador = value;
                SkipIndexWithText = 0;
                NoHayNadaMasParaCargar = false;
            }
        }

        public ContactosChatPageModel()
        {
            _chatsServices = new ChatsServices();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                await CargarContactos(0, 10);
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

        public ICommand ContactoSeleccionado
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    ContactosDTO contacto = parameter as ContactosDTO;

                    if (contacto != null)
                    {

                        try
                        {
                            ChatsDTO chatParaCrear = new ChatsDTO
                            {
                                CodigoPersonaOwner = App.Persona.Consecutivo,
                                CodigoPersonaNoOwner = contacto.PersonasContacto.Consecutivo
                            };

                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapper = await _chatsServices.CrearChat(chatParaCrear);

                            if (wrapper != null && wrapper.Exitoso)
                            {
                                chatParaCrear.Consecutivo = (int)wrapper.ConsecutivoCreado;
                                chatParaCrear.CodigoChatRecibe = wrapper.ConsecutivoChatRecibe;
                                chatParaCrear.PersonasNoOwner = contacto.PersonasContacto;

                                await CoreMethods.PushPageModel<ConversacionChatPageModel>(chatParaCrear);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMoreContacts
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (_contactos != null)
                        {
                            if (!string.IsNullOrWhiteSpace(TextoBuscador))
                            {
                                await CargarContactos(SkipIndexWithText, 10);
                                SkipIndexWithText += 5;
                            }
                            else
                            {
                                await CargarContactos(_contactos.Count, 10);
                            }
                        }
                        else
                        {
                            await CargarContactos(0, 10);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarContactos(int skipIndex, int takeIndex)
        {
            if (!NoHayNadaMasParaCargar)
            {
                ContactosDTO buscador = new ContactosDTO
                {
                    CodigoPersonaOwner = App.Persona.Consecutivo,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    IdiomaBase = App.IdiomaPersona,
                    IdentificadorParaBuscar = TextoBuscador
                };

                if (IsNotConnected) return;
                List<ContactosDTO> listaContactos = await _chatsServices.ListarContactos(buscador);

                if (listaContactos != null)
                {
                    if (listaContactos.Count > 0)
                    {
                        if (_contactos == null)
                        {
                            _contactos = new ObservableRangeCollection<ContactosDTO>(listaContactos);
                        }
                        else
                        {
                            listaContactos = listaContactos.Where(x => !_contactos.Where(y => y.Consecutivo == x.Consecutivo).Any()).ToList();
                            _contactos.AddRange(listaContactos);
                        }

                        RaisePropertyChanged("Contactos");
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaContactos.Count <= 0;
                    }
                }
            }
        }
    }
}
