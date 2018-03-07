using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class AdministracionRepository
    {
        SportsGoEntities _context;

        public AdministracionRepository(SportsGoEntities context)
        {
            _context = context;
        }

        #region Metodos Administrar Usuarios


        public async Task<Usuarios> ModificarUsuario(Usuarios usuarioParaModificar)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == usuarioParaModificar.Consecutivo).Include(x => x.PlanesUsuarios).Include(x => x.Personas).FirstOrDefaultAsync();

            usuarioExistente.Usuario = usuarioParaModificar.Usuario;
            usuarioExistente.Clave = !string.IsNullOrWhiteSpace(usuarioParaModificar.Clave) ? usuarioParaModificar.Clave : usuarioExistente.Clave;
            usuarioExistente.Email = usuarioParaModificar.Email;

            return usuarioExistente;
        }

        public async Task<int?> BuscarCodigoPersonaDeUnUsuario(int codigoUsuario)
        {
            int? codigoPersona = await _context.Personas.Where(x => x.CodigoUsuario == codigoUsuario).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return codigoPersona;
        }

        public async Task<int?> BuscarCodigoCandidatoDeUnaPersona(int codigoPersona)
        {
            int? codigoCandidato = await _context.Candidatos.Where(x => x.CodigoPersona == codigoPersona).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return codigoCandidato;
        }

        public async Task<int?> BuscarCodigoGrupoDeUnaPersona(int codigoPersona)
        {
            int? codigoGrupo = await _context.Grupos.Where(x => x.CodigoPersona == codigoPersona).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return codigoGrupo;
        }

        public async Task<int?> BuscarCodigoAnuncianteDeUnaPersona(int codigoPersona)
        {
            int? codigoAnunciante = await _context.Anunciantes.Where(x => x.CodigoPersona == codigoPersona).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return codigoAnunciante;
        }

        public async Task<int?> BuscarCodigoRepresentanteDeUnaPersona(int codigoPersona)
        {
            int? codigoRepresentante = await _context.Representantes.Where(x => x.CodigoPersona == codigoPersona).Select(x => x.Consecutivo).FirstOrDefaultAsync();

            return codigoRepresentante;
        }

        public async Task<int?> BuscarTipoPerfilDeUnaPersona(int codigoPersona)
        {
            int? codigoTipoPerfil = await _context.Personas.Where(x => x.Consecutivo == codigoPersona).Select(x => x.CodigoTipoPerfil).FirstOrDefaultAsync();

            return codigoTipoPerfil;
        }

        public async Task<int?> BuscarCodigoResponsableDeUnCandidato(int codigoCandidato)
        {
            int? codigoResponsable = await _context.Candidatos.Where(x => x.Consecutivo == codigoCandidato).Select(x => x.CodigoResponsable).FirstOrDefaultAsync();

            return codigoResponsable;
        }

        public async Task<int?> BuscarCodigoPlanUsuarioDeUnUsuario(int codigoUsuario)
        {
            int? codigoPlanUsuario = await _context.Usuarios.Where(x => x.Consecutivo == codigoUsuario).Select(x => x.CodigoPlanUsuario).FirstOrDefaultAsync();

            return codigoPlanUsuario;
        }

        public async Task<List<int>> ListarCodigoChatDeUnaPersona(int codigoPersona)
        {
            List<int> listaCodigoChats = await _context.Chats.Where(x => x.CodigoPersonaOwner == codigoPersona).Select(x => x.Consecutivo).ToListAsync();

            return listaCodigoChats;
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnCandidato(int codigoCandidato)
        {
            List<int> listaCodigoCategorias = await _context.CategoriasCandidatos.Where(x => x.CodigoCandidato == codigoCandidato).Select(x => x.Consecutivo).ToListAsync();

            return listaCodigoCategorias;
        }

        public async Task<List<int>> ListarCodigoEventosDeUnGrupo(int codigoGrupo)
        {
            List<int> listaCodigoEventos = await _context.GruposEventos.Where(x => x.CodigoGrupo == codigoGrupo).Select(x => x.Consecutivo).ToListAsync();

            return listaCodigoEventos;
        }

        public async Task<List<int>> ListarCodigoAnunciosDeUnAnunciante(int codigoAnunciante)
        {
            List<int> listaCodigoAnuncios = await _context.Anuncios.Where(x => x.CodigoAnunciante == codigoAnunciante).Select(x => x.Consecutivo).ToListAsync();

            return listaCodigoAnuncios;
        }

        public async Task<List<int>> ListarCodigoArchivosDeTodosLosAnunciosDeUnAnunciante(int codigoAnunciante)
        {
            List<int> listaCodigoArchivoAnuncios = await _context.Anuncios.Where(x => x.CodigoAnunciante == codigoAnunciante && x.CodigoArchivo != null).Select(x => x.CodigoArchivo.Value).ToListAsync();

            return listaCodigoArchivoAnuncios;
        }

        public async Task<List<int>> ListarCodigoArchivosDeTodosLosHistorialPagoDeUnaPersona(int codigoPersona)
        {
            List<int> listaCodigoArchivoHistorial = await _context.HistorialPagosPersonas.Where(x => x.CodigoPersona == codigoPersona && x.CodigoArchivo != null).Select(x => x.CodigoArchivo.Value).ToListAsync();

            return listaCodigoArchivoHistorial;
        }

        public async Task<List<int>> ListarCodigoArchivosDeTodosLosCandidatoVideosDeUnaCandidato(int codigoCandidato)
        {
            List<int> listaCodigoArchivoCandidatoVideo = await _context.CandidatosVideos.Where(x => x.CodigoCandidato == codigoCandidato).Select(x => x.CodigoArchivo).ToListAsync();

            return listaCodigoArchivoCandidatoVideo;
        }

        public async Task<List<int>> ListarCodigoArchivosDeTodosLosGrupoEventosDeUnGrupo(int codigoGrupo)
        {
            List<int> listaCodigoArchivoGrupoEvento = await _context.GruposEventos.Where(x => x.CodigoGrupo == codigoGrupo && x.CodigoArchivo != null).Select(x => x.CodigoArchivo.Value).ToListAsync();

            return listaCodigoArchivoGrupoEvento;
        }

        public async Task<int?> BuscarCodigoArchivoDeImagenPerfilDeUnaPersona(int codigoPersona)
        {
            int? codigoArchivoImagenPerfil = await _context.Personas.Where(x => x.Consecutivo == codigoPersona && x.CodigoArchivoImagenPerfil != null).Select(x => x.CodigoArchivoImagenPerfil).FirstOrDefaultAsync();

            return codigoArchivoImagenPerfil;
        }

        public async Task<int?> BuscarCodigoArchivoDeImagenBannerDeUnaPersona(int codigoPersona)
        {
            int? codigoArchivoImagenBanner = await _context.Personas.Where(x => x.Consecutivo == codigoPersona && x.CodigoArchivoImagenBanner != null).Select(x => x.CodigoArchivoImagenBanner).FirstOrDefaultAsync();

            return codigoArchivoImagenBanner;
        }

        public void EliminarNotificacionesDeUnaPersona(int codigoPersona)
        {
            _context.Notificaciones.RemoveRange(_context.Notificaciones.Where(x => x.CodigoPersonaDestinoAccion == codigoPersona || x.CodigoPersonaOrigenAccion == codigoPersona));
        }

        public void EliminarHistorialPagoDeUnaPersona(int codigoPersona)
        {
            _context.HistorialPagosPersonas.RemoveRange(_context.HistorialPagosPersonas.Where(x => x.CodigoPersona == codigoPersona));
        }

        public void EliminarChatMensajesDeUnChat(int codigoChat)
        {
            _context.ChatsMensajes.RemoveRange(_context.ChatsMensajes.Where(x => x.CodigoChatEnvia == codigoChat || x.CodigoChatRecibe == codigoChat));
        }

        public void EliminarChatsDeUnaPersona(int codigoPersona)
        {
            _context.Chats.RemoveRange(_context.Chats.Where(x => x.CodigoPersonaOwner == codigoPersona || x.CodigoPersonaNoOwner == codigoPersona));
        }

        public void EliminarContactosDeUnaPersona(int codigoPersona)
        {
            _context.Contactos.RemoveRange(_context.Contactos.Where(x => x.CodigoPersonaOwner == codigoPersona || x.CodigoPersonaContacto == codigoPersona));
        }

        public void EliminarGrupoEventosAsistentesDeUnaPersona(int codigoPersona)
        {
            _context.GruposEventosAsistentes.RemoveRange(_context.GruposEventosAsistentes.Where(x => x.CodigoPersona == codigoPersona));
        }

        public void EliminarHabilidadesDeUnaCategoriaCandidato(int codigoCategoria)
        {
            _context.HabilidadesCandidatos.RemoveRange(_context.HabilidadesCandidatos.Where(x => x.CodigoCategoriaCandidato == codigoCategoria));
        }

        public void EliminarCategoriasCandidatoDeUnCandidato(int codigoCandidato)
        {
            _context.CategoriasCandidatos.RemoveRange(_context.CategoriasCandidatos.Where(x => x.CodigoCandidato == codigoCandidato));
        }

        public void EliminarCandidatosVideoDeUnCandidato(int codigoCandidato)
        {
            _context.CandidatosVideos.RemoveRange(_context.CandidatosVideos.Where(x => x.CodigoCandidato == codigoCandidato));
        }

        public void EliminarCandidato(int codigoCandidato)
        {
            _context.Candidatos.Remove(_context.Candidatos.Where(x => x.Consecutivo == codigoCandidato).FirstOrDefault());
        }

        public void EliminarCandidatosResponsables(int codigoResponsable)
        {
            _context.CandidatosResponsables.Remove(_context.CandidatosResponsables.Where(x => x.Consecutivo == codigoResponsable).FirstOrDefault());
        }

        public void EliminarCategoriasGruposDeUnGrupo(int codigoGrupo)
        {
            _context.CategoriasGrupos.RemoveRange(_context.CategoriasGrupos.Where(x => x.CodigoGrupo == codigoGrupo));
        }

        public void EliminarGruposAsistentesDeUnEvento(int codigoEvento)
        {
            _context.GruposEventosAsistentes.RemoveRange(_context.GruposEventosAsistentes.Where(x => x.CodigoEvento == codigoEvento));
        }

        public void EliminarCategoriasEventosDeUnEvento(int codigoEvento)
        {
            _context.CategoriasEventos.RemoveRange(_context.CategoriasEventos.Where(x => x.CodigoEvento == codigoEvento));
        }

        public void EliminarEventosDeUnGrupo(int codigoGrupo)
        {
            _context.GruposEventos.RemoveRange(_context.GruposEventos.Where(x => x.CodigoGrupo == codigoGrupo));
        }

        public void EliminarGrupo(int codigoGrupo)
        {
            _context.Grupos.Remove(_context.Grupos.Where(x => x.Consecutivo == codigoGrupo).FirstOrDefault());
        }

        public void EliminarAnunciosContenidosDeUnAnuncio(int codigoAnuncio)
        {
            _context.AnunciosContenidos.RemoveRange(_context.AnunciosContenidos.Where(x => x.CodigoAnuncio == codigoAnuncio));
        }

        public void EliminarAnunciosPaisesDeUnAnuncio(int codigoAnuncio)
        {
            _context.AnunciosPaises.RemoveRange(_context.AnunciosPaises.Where(x => x.CodigoAnuncio == codigoAnuncio));
        }

        public void EliminarCategoriasAnunciosDeUnAnuncio(int codigoAnuncio)
        {
            _context.CategoriasAnuncios.RemoveRange(_context.CategoriasAnuncios.Where(x => x.CodigoAnuncio == codigoAnuncio));
        }

        public void EliminarAnunciosDeUnAnunciante(int codigoAnunciante)
        {
            _context.Anuncios.RemoveRange(_context.Anuncios.Where(x => x.CodigoAnunciante == codigoAnunciante));
        }

        public void EliminarAnunciante(int codigoAnunciante)
        {
            _context.Anunciantes.Remove(_context.Anunciantes.Where(x => x.Consecutivo == codigoAnunciante).FirstOrDefault());
        }

        public void EliminarCategoriaRepresentantesDeUnRepresentante(int codigoRepresentante)
        {
            _context.CategoriasRepresentantes.RemoveRange(_context.CategoriasRepresentantes.Where(x => x.CodigoRepresentante == codigoRepresentante));
        }

        public void EliminarRepresentante(int codigoRepresentante)
        {
            _context.Representantes.Remove(_context.Representantes.Where(x => x.Consecutivo == codigoRepresentante).FirstOrDefault());
        }

        public void EliminarPersona(int codigoPersona)
        {
            _context.Personas.Remove(_context.Personas.Where(x => x.Consecutivo == codigoPersona).FirstOrDefault());
        }

        public void EliminarPlanUsuario(int codigoPlanUsuario)
        {
            _context.PlanesUsuarios.Remove(_context.PlanesUsuarios.Where(x => x.Consecutivo == codigoPlanUsuario).FirstOrDefault());
        }

        public void EliminarUsuario(int codigoUsuario)
        {
            _context.Usuarios.Remove(_context.Usuarios.Where(x => x.Consecutivo == codigoUsuario).FirstOrDefault());
        }

        public void EliminarArchivo(int codigoArchivo)
        {
            Archivos archivoParaEliminar = new Archivos
            {
                Consecutivo = codigoArchivo
            };

            _context.Archivos.Attach(archivoParaEliminar);
            _context.Archivos.Remove(archivoParaEliminar);
        }


        #endregion


        #region Metodos TerminosCondiciones


        public void CrearTerminosCondiciones(TerminosCondiciones terminosCondicionesParaCrear)
        {
            _context.TerminosCondiciones.Add(terminosCondicionesParaCrear);
        }

        public async Task<TerminosCondiciones> BuscarTerminosCondiciones(TerminosCondiciones terminosCondicionesParaBuscar)
        {
            TerminosCondiciones terminosCondiciones = await _context.TerminosCondiciones.Where(x => x.CodigoIdioma == terminosCondicionesParaBuscar.CodigoIdioma).AsNoTracking().FirstOrDefaultAsync();

            return terminosCondiciones;
        }

        public async Task<bool> BuscarSiExisteTerminosYCondiciones(TerminosCondiciones terminosCondicionesParaBuscar)
        {
            bool existe = await _context.TerminosCondiciones.Where(x => x.CodigoIdioma == terminosCondicionesParaBuscar.CodigoIdioma).AnyAsync();

            return existe;
        }

        public async Task<List<TerminosCondiciones>> ListarTerminosCondiciones()
        {
            List<TerminosCondiciones> listaTerminosCondicionesBuscados = await _context.TerminosCondiciones.Include(x => x.Idiomas).AsNoTracking().ToListAsync();

            return listaTerminosCondicionesBuscados;
        }

        public async Task<TerminosCondiciones> ModificarTerminosCondiciones(TerminosCondiciones terminosCondicionesParaModificar)
        {
            TerminosCondiciones terminosCondicionesExistentes = await _context.TerminosCondiciones.Where(x => x.CodigoIdioma == terminosCondicionesParaModificar.CodigoIdioma).FirstOrDefaultAsync();

            terminosCondicionesExistentes.Texto = terminosCondicionesParaModificar.Texto;

            return terminosCondicionesExistentes;
        }


        #endregion


        #region Metodos ImagenesPerfilAdministradores


        public void CrearImagenPerfilAdministrador(ImagenesPerfilAdministradores imagenParaCrear)
        {
            _context.ImagenesPerfilAdministradores.Add(imagenParaCrear);
        }

        public async Task<ImagenesPerfilAdministradores> AsignarImagenPerfil(int codigoUsuario, int codigoArchivo)
        {
            ImagenesPerfilAdministradores imagenPerfilExistente = await _context.ImagenesPerfilAdministradores.Where(x => x.CodigoUsuario == codigoUsuario).FirstOrDefaultAsync();

            imagenPerfilExistente.CodigoArchivo = codigoArchivo;

            return imagenPerfilExistente;
        }

        public async Task<ImagenesPerfilAdministradores> BuscarImagenPerfilAdministrador(int codigoUsuario)
        {
            ImagenesPerfilAdministradores imagenPerfilExistente = await _context.ImagenesPerfilAdministradores.Where(x => x.CodigoUsuario == codigoUsuario).FirstOrDefaultAsync();

            return imagenPerfilExistente;
        }

        public async Task<ImagenesPerfilAdministradores> BuscarPrimeraImagenPerfilAdministrador()
        {
            ImagenesPerfilAdministradores imagenPerfilExistente = await _context.ImagenesPerfilAdministradores.FirstOrDefaultAsync();

            return imagenPerfilExistente;
        }

        #endregion


        #region Metodos Paises


        public void CrearPais(Paises paisParaCrear)
        {
            _context.Paises.Add(paisParaCrear);
        }

        public async Task<Paises> BuscarPais(Paises paisParaBuscar)
        {
            Paises paisBuscado = await (from pais in _context.Paises
                                        where pais.Consecutivo == paisParaBuscar.Consecutivo
                                        select pais).Include(x => x.PaisesContenidos)
                                                    .Include(x => x.Monedas)
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();

            return paisBuscado;
        }

        public async Task<List<PaisesDTO>> ListarPaisesPorIdioma(Paises paisParaListar)
        {
            List<PaisesDTO> listaPaises = await _context.Paises
                .Select(x => new PaisesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoArchivo = x.CodigoArchivo,
                    CodigoIdioma = x.CodigoIdioma,
                    DescripcionIdiomaBuscado = x.PaisesContenidos.Where(y => y.CodigoIdioma == paisParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    PaisesContenidos = x.PaisesContenidos.Where(y => y.CodigoIdioma == paisParaListar.CodigoIdiomaUsuarioBase)
                                            .Select(z => new PaisesContenidosDTO
                                            {
                                                Consecutivo = z.Consecutivo,
                                                CodigoIdioma = z.CodigoIdioma,
                                                CodigoPais = z.CodigoPais,
                                                Descripcion = z.Descripcion
                                            })
                                            .ToList(),
                    Monedas = new MonedasDTO
                    {
                        Consecutivo = x.Monedas.Consecutivo,
                        Descripcion = x.Monedas.Descripcion,
                        CambioMoneda = x.Monedas.CambioMoneda,
                        AbreviaturaMoneda = x.Monedas.AbreviaturaMoneda
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaPaises;
        }

        public void EliminarPais(Paises paisParaEliminar)
        {
            _context.Paises.Attach(paisParaEliminar);
            _context.Paises.Remove(paisParaEliminar);
        }


        #endregion


        #region Metodos PaisesContenido


        public void CrearPaisesContenidos(ICollection<PaisesContenidos> paisesContenidoParaCrear)
        {
            paisesContenidoParaCrear.ForEach(x => x.Descripcion.Trim());
            _context.PaisesContenidos.AddRange(paisesContenidoParaCrear);
        }

        public async Task<PaisesContenidos> BuscarPaisContenido(PaisesContenidos paisContenidoParaBuscar)
        {
            PaisesContenidos paisContenidoBuscada = await (from paisContenido in _context.PaisesContenidos
                                                           where paisContenido.Consecutivo == paisContenidoParaBuscar.Consecutivo
                                                           select paisContenido).Include(x => x.Idiomas)
                                                                                .Include(x => x.Paises)
                                                                                .AsNoTracking()
                                                                                .FirstOrDefaultAsync();

            return paisContenidoBuscada;
        }

        public async Task<Paises> ModificarPais(Paises paisParaModificar)
        {
            Paises paisExistente = await _context.Paises.Where(x => x.Consecutivo == paisParaModificar.Consecutivo).FirstOrDefaultAsync();

            paisExistente.CodigoIdioma = paisParaModificar.CodigoIdioma;
            paisExistente.CodigoMoneda = paisParaModificar.CodigoMoneda;

            return paisExistente;
        }

        public async Task<List<PaisesContenidos>> ListarContenidoDeUnPais(PaisesContenidos paisContenidoParaListar)
        {
            List<PaisesContenidos> listaPaisContenidos = await (from categoriaContenido in _context.PaisesContenidos
                                                                where categoriaContenido.CodigoPais == paisContenidoParaListar.CodigoPais
                                                                select categoriaContenido).Include(x => x.Idiomas)
                                                                                          .AsNoTracking()
                                                                                          .ToListAsync();

            return listaPaisContenidos;
        }

        public async Task<PaisesContenidos> ModificarPaisContenido(PaisesContenidos paisContenidoParaModificar)
        {
            PaisesContenidos paisContenidoExistente = await _context.PaisesContenidos.Where(x => x.Consecutivo == paisContenidoParaModificar.Consecutivo).FirstOrDefaultAsync();

            paisContenidoExistente.Descripcion = paisContenidoParaModificar.Descripcion.Trim();

            return paisContenidoExistente;
        }

        public void EliminarPaisContenido(PaisesContenidos paisContenidoParaEliminar)
        {
            _context.PaisesContenidos.Attach(paisContenidoParaEliminar);
            _context.PaisesContenidos.Remove(paisContenidoParaEliminar);
        }

        public void EliminarMultiplesPaisesContenidos(PaisesContenidos paisesContenidosParaBorrar)
        {
            _context.PaisesContenidos.RemoveRange(_context.PaisesContenidos.Where(x => x.CodigoPais == paisesContenidosParaBorrar.CodigoPais));
        }


        #endregion


    }
}
