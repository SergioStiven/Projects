using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class ChatsRepository
    {
        SportsGoEntities _context;

        public ChatsRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Contactos


        public void CrearContacto(Contactos contactoParaCrear)
        {
            _context.Contactos.Add(contactoParaCrear);
        }

        public async Task<Contactos> BuscarContacto(Contactos contactoParaBuscar)
        {
            Contactos contactoBuscado = await (from contacto in _context.Contactos
                                               where contacto.Consecutivo == contactoParaBuscar.Consecutivo
                                               select contacto).Include(x => x.PersonasContacto)
                                                               .AsNoTracking()
                                                               .FirstOrDefaultAsync();

            return contactoBuscado;
        }

        public async Task<Tuple<Contactos,int?>> BuscarConsecutivoContactoContrario(Contactos contactoParaBuscar)
        {
            Contactos contactoBuscado = await _context.Contactos.Where(x => x.Consecutivo == contactoParaBuscar.Consecutivo)
                                                                .AsNoTracking()
                                                                .FirstOrDefaultAsync();

            int? consecutivoContrarioBuscado = await _context.Contactos.Where(x => x.CodigoPersonaContacto == contactoBuscado.CodigoPersonaOwner && x.CodigoPersonaOwner == contactoBuscado.CodigoPersonaContacto)
                                                                      .Select(x => x.Consecutivo)
                                                                      .FirstOrDefaultAsync();

            return Tuple.Create(contactoBuscado, consecutivoContrarioBuscado);
        }

        public async Task<Contactos> VerificarSiLaPersonaEstaAgregadaContactos(Contactos contactoParaBuscar)
        {
            Contactos contactoBuscado = await (from contacto in _context.Contactos
                                               where contacto.CodigoPersonaContacto == contactoParaBuscar.CodigoPersonaContacto && contacto.CodigoPersonaOwner == contactoParaBuscar.CodigoPersonaOwner
                                               select contacto).AsNoTracking()
                                                               .FirstOrDefaultAsync();

            return contactoBuscado;
        }

        public async Task<List<ContactosDTO>> ListarContactos(Contactos contactoParaListar)
        {
            IQueryable<Contactos> queryContactos = _context.Contactos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(contactoParaListar.IdentificadorParaBuscar))
            {
                contactoParaListar.IdentificadorParaBuscar = contactoParaListar.IdentificadorParaBuscar.Trim();
                string[] arrayIdentificador = contactoParaListar.IdentificadorParaBuscar.Split(new char[] { ' ' }, 2);

                string nombre = arrayIdentificador.ElementAtOrDefault(0);
                string apellido = arrayIdentificador.ElementAtOrDefault(1);

                if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(apellido))
                {
                    queryContactos = queryContactos.Where(x => x.PersonasContacto.Nombres.Trim().ToUpper().Contains(nombre.Trim().ToUpper()) || x.PersonasContacto.Apellidos.Trim().ToUpper().Contains(apellido.Trim().ToUpper()));
                }
                else
                {
                    queryContactos = queryContactos.Where(x => x.PersonasContacto.Nombres.Trim().ToUpper().Contains(nombre.Trim().ToUpper()));
                }
            }

            List<ContactosDTO> listaContactos = await _context.Contactos
                                                           .Where(x => x.CodigoPersonaOwner == contactoParaListar.CodigoPersonaOwner)
                                                           .Select(x => new ContactosDTO
                                                           {
                                                               Consecutivo = x.Consecutivo,
                                                               CodigoPersonaOwner = x.CodigoPersonaOwner,
                                                               CodigoPersonaContacto = x.CodigoPersonaContacto,
                                                               PersonasContacto = new PersonasDTO
                                                               {
                                                                   Consecutivo = x.PersonasContacto.Consecutivo,
                                                                   Nombres = x.PersonasContacto.Nombres,
                                                                   Apellidos = x.PersonasContacto.Apellidos,
                                                                   CodigoTipoPerfil = x.PersonasContacto.CodigoTipoPerfil,
                                                                   CodigoArchivoImagenPerfil = x.PersonasContacto.CodigoArchivoImagenPerfil,
                                                                   CodigoPais = x.PersonasContacto.CodigoPais,
                                                                   Usuarios = new UsuariosDTO
                                                                   {
                                                                       Consecutivo = x.PersonasContacto.Usuarios.Consecutivo,
                                                                       Email = x.PersonasContacto.Usuarios.Email
                                                                   },
                                                                   Paises = new PaisesDTO
                                                                   {
                                                                       Consecutivo = x.PersonasContacto.Paises.Consecutivo,
                                                                       CodigoArchivo = x.PersonasContacto.Paises.CodigoArchivo,
                                                                       DescripcionIdiomaBuscado = x.PersonasContacto.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == contactoParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                                                                   }
                                                               }
                                                           })
                                                           .OrderBy(x => x.PersonasContacto.Nombres)
                                                           .Skip(() => contactoParaListar.SkipIndexBase)
                                                           .Take(() => contactoParaListar.TakeIndexBase)
                                                           .AsNoTracking()
                                                           .ToListAsync();

            return listaContactos;
        }

        public void EliminarContacto(Contactos contactoParaEliminar)
        {
            _context.Contactos.Attach(contactoParaEliminar);
            _context.Contactos.Remove(contactoParaEliminar);
        }


        #endregion


        #region Metodos Chats


        public void CrearChat(Chats chatParaCrear)
        {
            _context.Chats.Add(chatParaCrear);
        }

        public async Task<ChatsDTO> BuscarChat(Chats chatParaBuscar)
        {
            ChatsDTO chatBuscado = await _context.Chats.Where(chat => chat.Consecutivo == chatParaBuscar.Consecutivo)
                                                        .Select(x => new ChatsDTO
                                                        {
                                                            Consecutivo = x.Consecutivo,
                                                            Creacion = x.Creacion,
                                                            CodigoEstado = x.CodigoEstado,
                                                            CodigoPersonaOwner = x.CodigoPersonaOwner,
                                                            CodigoPersonaNoOwner = x.CodigoPersonaNoOwner,
                                                            CodigoChatRecibe = _context.Chats.Where(y => y.CodigoPersonaNoOwner == x.CodigoPersonaOwner && y.CodigoPersonaOwner == x.CodigoPersonaNoOwner).Select(z => z.Consecutivo).FirstOrDefault(),
                                                            PersonasNoOwner = new PersonasDTO
                                                            {
                                                                Consecutivo = x.PersonasNoOwner.Consecutivo,
                                                                Nombres = x.PersonasNoOwner.Nombres,
                                                                Apellidos = x.PersonasNoOwner.Apellidos,
                                                                CodigoArchivoImagenPerfil = x.PersonasNoOwner.CodigoArchivoImagenPerfil,
                                                            }
                                                        })
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync();

            return chatBuscado;
        }

        public async Task<int> BuscarConsecutivoChat(Chats chatParaVerificar)
        {
            int consecutivoChat = await _context.Chats.Where(x => x.CodigoPersonaOwner == chatParaVerificar.CodigoPersonaOwner && x.CodigoPersonaNoOwner == chatParaVerificar.CodigoPersonaNoOwner).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return consecutivoChat;
        }

        public async Task<ChatsDTO> BuscarChatEntreDosPersonas(Chats chatParaBuscar)
        {
            ChatsDTO chatBuscado = await _context.Chats.Where(chat => chat.CodigoPersonaOwner == chatParaBuscar.CodigoPersonaOwner && chat.CodigoPersonaNoOwner == chatParaBuscar.CodigoPersonaNoOwner)
                                                        .Select(x => new ChatsDTO
                                                        {
                                                            Consecutivo = x.Consecutivo,
                                                            Creacion = x.Creacion,
                                                            CodigoEstado = x.CodigoEstado,
                                                            CodigoPersonaOwner = x.CodigoPersonaOwner,
                                                            CodigoPersonaNoOwner = x.CodigoPersonaNoOwner,
                                                            CodigoChatRecibe = _context.Chats.Where(y => y.CodigoPersonaNoOwner == x.CodigoPersonaOwner && y.CodigoPersonaOwner == x.CodigoPersonaNoOwner).Select(z => z.Consecutivo).FirstOrDefault(),
                                                            PersonasNoOwner = new PersonasDTO
                                                            {
                                                                Consecutivo = x.PersonasNoOwner.Consecutivo,
                                                                Nombres = x.PersonasNoOwner.Nombres,
                                                                Apellidos = x.PersonasNoOwner.Apellidos,
                                                                CodigoArchivoImagenPerfil = x.PersonasNoOwner.CodigoArchivoImagenPerfil,
                                                            }
                                                        })
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync();

            return chatBuscado;
        }

        public async Task<List<ChatsDTO>> ListarChats(Chats chatParaListar)
        {
            int codigoEstadoActivo = (int)EstadosChat.Activo;

            IQueryable<Chats> queryChats = _context.Chats.Where(x => x.CodigoPersonaOwner == chatParaListar.CodigoPersonaOwner && x.CodigoEstado == codigoEstadoActivo).AsQueryable();

            if (chatParaListar.FechaFiltroBase != DateTime.MinValue)
            {
                queryChats = queryChats.Where(x => x.ChatsMensajesEnvia.Where(y => y.FechaMensaje > chatParaListar.FechaFiltroBase).Any() || x.ChatsMensajesRecibe.Where(y => y.FechaMensaje > chatParaListar.FechaFiltroBase).Any());
            }

            queryChats = queryChats.Where(x => x.PersonasOwner.ContactosOwner.Any(y => y.CodigoPersonaContacto == x.CodigoPersonaNoOwner));

            int numeroRegistros = await queryChats.CountAsync();

            List<ChatsDTO> listaChats = await queryChats
                .Select(x => new ChatsDTO
                {
                    Consecutivo = x.Consecutivo,
                    Creacion = x.Creacion,
                    CodigoEstado = x.CodigoEstado,
                    CodigoPersonaOwner = x.CodigoPersonaOwner,
                    CodigoPersonaNoOwner = x.CodigoPersonaNoOwner,
                    CodigoChatRecibe = _context.Chats.Where(y => y.CodigoPersonaNoOwner == x.CodigoPersonaOwner && y.CodigoPersonaOwner == x.CodigoPersonaNoOwner).Select(z => z.Consecutivo).FirstOrDefault(),
                    UltimoMensaje = _context.ChatsMensajes.Where(y => y.CodigoChatEnvia == x.Consecutivo || y.CodigoChatRecibe == x.Consecutivo)
                                            .OrderByDescending(y => y.FechaMensaje)
                                            .Select(y => new ChatsMensajesDTO
                                            {
                                                Consecutivo = y.Consecutivo,
                                                CodigoChatEnvia = y.CodigoChatEnvia,
                                                CodigoChatRecibe = y.CodigoChatRecibe,
                                                Mensaje = y.ChatsEnvia.PersonasOwner.Nombres + ": " + y.Mensaje,
                                                FechaMensaje = y.FechaMensaje,
                                                UltimoMensajeEsMio = y.ChatsEnvia.PersonasOwner.Consecutivo == chatParaListar.CodigoPersonaOwner
                                            }).FirstOrDefault(),
                    NumeroRegistrosExistentes = numeroRegistros,
                    PersonasNoOwner = new PersonasDTO
                    {
                        Consecutivo = x.PersonasNoOwner.Consecutivo,
                        Nombres = x.PersonasNoOwner.Nombres,
                        Apellidos = x.PersonasNoOwner.Apellidos,
                        CodigoArchivoImagenPerfil = x.PersonasNoOwner.CodigoArchivoImagenPerfil,
                    }
                })
                .Where(x => x.UltimoMensaje != null)
                .OrderByDescending(x => x.Creacion)
                .Skip(() => chatParaListar.SkipIndexBase)
                .Take(() => chatParaListar.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaChats.OrderByDescending(x => x.UltimoMensaje.FechaMensaje).ToList();
        }

        public async Task<bool> VerificarSiYaExisteChat(Chats chatParaVerificar)
        {
            bool existeChat = await _context.Chats.Where(x => x.CodigoPersonaOwner == chatParaVerificar.CodigoPersonaOwner && x.CodigoPersonaNoOwner == chatParaVerificar.CodigoPersonaNoOwner).AnyAsync();

            return existeChat;
        }

        public async Task<Chats> ReactivarChat(Chats chatParaReactivar)
        {
            Chats chatExistente = await _context.Chats.Where(x => x.CodigoPersonaOwner == chatParaReactivar.CodigoPersonaOwner && x.CodigoPersonaNoOwner == chatParaReactivar.CodigoPersonaNoOwner).FirstOrDefaultAsync();

            chatExistente.EstadoChat = EstadosChat.Activo;

            return chatExistente;
        }

        public async Task<Chats> ReactivarChatPorConsecutivo(Chats chatParaReactivar)
        {
            Chats chatExistente = await _context.Chats.Where(x => x.Consecutivo == chatParaReactivar.Consecutivo).FirstOrDefaultAsync();

            chatExistente.EstadoChat = EstadosChat.Activo;

            return chatExistente;
        }

        public async Task<bool> VerificarSiSePuedenBorrarMensajesNoOwner(Chats chatParaVerificar)
        {
            int codigoEstadoParaBorrar = (int)EstadosChat.PendienteParaBorrarMensajes;
            bool sePuedeBorrarMensajesNoOwner = await _context.Chats.Where(x => x.CodigoPersonaOwner == chatParaVerificar.CodigoPersonaNoOwner && x.CodigoPersonaNoOwner == chatParaVerificar.CodigoPersonaOwner && x.CodigoEstado == codigoEstadoParaBorrar).AnyAsync();

            return sePuedeBorrarMensajesNoOwner;
        }

        public async Task<Chats> MarcarChatComoPendienteParaBorrarMensajes(Chats chatParaMarcar)
        {
            Chats chatExistente = await _context.Chats.Where(x => x.CodigoPersonaOwner == chatParaMarcar.CodigoPersonaOwner && x.CodigoPersonaNoOwner == chatParaMarcar.CodigoPersonaNoOwner).FirstOrDefaultAsync();

            chatExistente.EstadoChat = EstadosChat.PendienteParaBorrarMensajes;
            chatExistente.Creacion = DateTime.Now;

            return chatExistente;
        }

        public void EliminarChat(Chats chatParaEliminar)
        {
            _context.Chats.Attach(chatParaEliminar);
            _context.Chats.Remove(chatParaEliminar);
        }


        #endregion


        #region Metodos ChatsMensaje


        public void CrearChatMensaje(ChatsMensajes chatMensajeParaCrear)
        {
            _context.ChatsMensajes.Add(chatMensajeParaCrear);
        }

        public async Task<ChatsMensajes> BuscarChatMensaje(ChatsMensajes chatMensajeParaBuscar)
        {
            ChatsMensajes chatMensajeBuscado = await (from chatMensaje in _context.ChatsMensajes
                                                      where chatMensaje.Consecutivo == chatMensajeParaBuscar.Consecutivo
                                                      select chatMensaje).AsNoTracking()
                                                                         .FirstOrDefaultAsync();

            return chatMensajeBuscado;
        }

        public async Task<List<ChatsMensajes>> ListarChatsMensajes(ChatsMensajes chatMensajeParaListar)
        {
            DateTime margenDeMensajes = await _context.ChatsMensajes.Where(x => x.CodigoChatEnvia == chatMensajeParaListar.CodigoChatEnvia && x.CodigoChatRecibe == chatMensajeParaListar.CodigoChatRecibe).Select(x => x.ChatsEnvia.Creacion).FirstOrDefaultAsync();

            IQueryable<ChatsMensajes> queryMensajes = _context.ChatsMensajes
                                                              .Where(x => (x.CodigoChatEnvia == chatMensajeParaListar.CodigoChatEnvia && x.CodigoChatRecibe == chatMensajeParaListar.CodigoChatRecibe) || (x.CodigoChatEnvia == chatMensajeParaListar.CodigoChatRecibe && x.CodigoChatRecibe == chatMensajeParaListar.CodigoChatEnvia))
                                                              .Where(x => x.FechaMensaje > margenDeMensajes)
                                                              .AsQueryable();

            List<ChatsMensajes> listaContactos = null;
            if (chatMensajeParaListar.FechaFiltroBase != DateTime.MinValue)
            {
                queryMensajes = queryMensajes.Where(x => x.FechaMensaje > chatMensajeParaListar.FechaFiltroBase);

                listaContactos = await queryMensajes
                                        .OrderByDescending(x => x.FechaMensaje)
                                        .AsNoTracking()
                                        .ToListAsync();
            }
            else
            {
                listaContactos = await queryMensajes
                                        .OrderByDescending(x => x.FechaMensaje)
                                        .Skip(() => chatMensajeParaListar.SkipIndexBase)
                                        .Take(() => chatMensajeParaListar.TakeIndexBase)
                                        .AsNoTracking()
                                        .ToListAsync();
            }

            return listaContactos.OrderBy(x => x.FechaMensaje).ToList();
        }

        public void EliminarChatMensaje(ChatsMensajes chatMensajeParaEliminar)
        {
            _context.ChatsMensajes.Remove(chatMensajeParaEliminar);
        }

        public void EliminarTodosChatMensaje(ChatsMensajes chatMensajeParaEliminar)
        {
            _context.ChatsMensajes.RemoveRange(_context.ChatsMensajes.Where(x => (x.ChatsEnvia == chatMensajeParaEliminar.ChatsEnvia && x.ChatsRecibe == chatMensajeParaEliminar.ChatsRecibe) || (x.ChatsEnvia == chatMensajeParaEliminar.ChatsRecibe && x.ChatsRecibe == chatMensajeParaEliminar.ChatsEnvia)));
        }


        #endregion


    }
}
