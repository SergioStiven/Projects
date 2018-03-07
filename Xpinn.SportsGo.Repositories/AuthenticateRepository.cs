using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Repositories
{
    public class AuthenticateRepository
    {
        SportsGoEntities _context;

        public AuthenticateRepository(SportsGoEntities context)
        {
            _context = context;
        }

        public async Task<UsuariosDTO> VerificarUsuario(Usuarios usuarioParaVerificar)
        {
            UsuariosDTO usuarioVerificado = await (from usuario in _context.Usuarios
                                                   where usuario.Usuario.Trim().ToUpper() == usuarioParaVerificar.Usuario.Trim().ToUpper()
                                                   && usuario.Clave.Trim() == usuarioParaVerificar.Clave.Trim()
                                                   select new UsuariosDTO
                                                   {
                                                       Consecutivo = usuario.Consecutivo,
                                                       Usuario = usuario.Usuario,
                                                       Clave = usuario.Clave,
                                                       Creacion = usuario.Creacion,
                                                       CodigoTipoPerfil = usuario.CodigoTipoPerfil,
                                                       Email = usuario.Email,
                                                       CodigoPlanUsuario = usuario.CodigoPlanUsuario,
                                                       DeviceId = usuario.DeviceId,
                                                       CuentaActiva = usuario.CuentaActiva,
                                                       Personas = usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo)
                                                        .Select(x => new PersonasDTO
                                                        {
                                                            Consecutivo = x.Consecutivo,
                                                            Nombres = x.Nombres,
                                                            Apellidos = x.Apellidos,
                                                            CodigoIdioma = x.CodigoIdioma,
                                                            CodigoUsuario = x.CodigoUsuario,
                                                            CodigoArchivoImagenPerfil = x.CodigoArchivoImagenPerfil,
                                                            CodigoArchivoImagenBanner = x.CodigoArchivoImagenBanner,
                                                            CodigoPais = x.CodigoPais,
                                                            CodigoTipoPerfil = x.CodigoTipoPerfil,
                                                            Candidatos = x.Candidatos.Where(y => y.CodigoPersona == x.Consecutivo)
                                                                                           .Select(y => new CandidatosDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo,
                                                                                           }).ToList(),
                                                            Grupos = x.Grupos.Where(y => y.CodigoPersona == x.Consecutivo)
                                                                                           .Select(y => new GruposDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo,
                                                                                               NombreContacto = y.NombreContacto
                                                                                           }).ToList(),
                                                            Anunciantes = x.Anunciantes.Where(y => y.CodigoPersona == x.Consecutivo)
                                                                                           .Select(y => new AnunciantesDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo
                                                                                           }).ToList(),
                                                            Representantes = x.Representantes.Where(y => y.CodigoPersona == x.Consecutivo)
                                                                                           .Select(y => new RepresentantesDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo
                                                                                           }).ToList(),
                                                        }).ToList(),
                                                       PlanesUsuarios = new PlanesUsuariosDTO
                                                       {
                                                           Consecutivo = usuario.PlanesUsuarios.Consecutivo,
                                                           CodigoPlan = usuario.PlanesUsuarios.CodigoPlan,
                                                           Adquisicion = usuario.PlanesUsuarios.Adquisicion,
                                                           Vencimiento = usuario.PlanesUsuarios.Vencimiento,
                                                           NumeroCategoriasUsadas = usuario.PlanesUsuarios.NumeroCategoriasUsadas,
                                                           Planes = new PlanesDTO
                                                           {
                                                               Consecutivo = usuario.PlanesUsuarios.Planes.Consecutivo,
                                                               Creacion = usuario.PlanesUsuarios.Planes.Creacion,
                                                               Precio = usuario.PlanesUsuarios.Planes.Precio,
                                                               CodigoPeriodicidad = usuario.PlanesUsuarios.Planes.CodigoPeriodicidad,
                                                               PlanDefault = usuario.PlanesUsuarios.Planes.PlanDefault,
                                                               VideosPerfil = usuario.PlanesUsuarios.Planes.VideosPerfil,
                                                               ServiciosChat = usuario.PlanesUsuarios.Planes.ServiciosChat,
                                                               ConsultaCandidatos = usuario.PlanesUsuarios.Planes.ConsultaCandidatos,
                                                               DetalleCandidatos = usuario.PlanesUsuarios.Planes.DetalleCandidatos,
                                                               ConsultaGrupos = usuario.PlanesUsuarios.Planes.ConsultaGrupos,
                                                               DetalleGrupos = usuario.PlanesUsuarios.Planes.DetalleGrupos,
                                                               ConsultaEventos = usuario.PlanesUsuarios.Planes.ConsultaEventos,
                                                               CreacionAnuncios = usuario.PlanesUsuarios.Planes.CreacionAnuncios,
                                                               EstadisticasAnuncios = usuario.PlanesUsuarios.Planes.EstadisticasAnuncios,
                                                               Modificacion = usuario.PlanesUsuarios.Planes.Modificacion,
                                                               CodigoTipoPerfil = usuario.PlanesUsuarios.Planes.CodigoTipoPerfil,
                                                               TiempoPermitidoVideo = usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo,
                                                               NumeroCategoriasPermisibles = usuario.PlanesUsuarios.Planes.NumeroCategoriasPermisibles,
                                                               CodigoArchivo = usuario.PlanesUsuarios.Planes.CodigoArchivo,
                                                               NumeroAparicionesAnuncio = usuario.PlanesUsuarios.Planes.NumeroAparicionesAnuncio,
                                                               NumeroDiasVigenciaAnuncio = usuario.PlanesUsuarios.Planes.NumeroDiasVigenciaAnuncio,
                                                               DescripcionIdiomaBuscado  = usuario.PlanesUsuarios.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo).Select(x => x.CodigoIdioma).FirstOrDefault()).Select(y => y.Descripcion).FirstOrDefault(),
                                                           }
                                                       }
                                                   }).AsNoTracking()
                                                  .FirstOrDefaultAsync();

            return usuarioVerificado;
        }

        public async Task<UsuariosDTO> VerificarUsuarioConDeviceID(Usuarios usuarioParaVerificar)
        {
            UsuariosDTO usuarioVerificado = await (from usuario in _context.Usuarios
                                                   where usuario.DeviceId.Trim() == usuarioParaVerificar.DeviceId.Trim()
                                                   select new UsuariosDTO
                                                   {
                                                       Consecutivo = usuario.Consecutivo,
                                                       Usuario = usuario.Usuario,
                                                       Clave = usuario.Clave,
                                                       Creacion = usuario.Creacion,
                                                       CodigoTipoPerfil = usuario.CodigoTipoPerfil,
                                                       Email = usuario.Email,
                                                       CodigoPlanUsuario = usuario.CodigoPlanUsuario,
                                                       DeviceId = usuario.DeviceId,
                                                       CuentaActiva = usuario.CuentaActiva,
                                                       Personas = usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo)
                                                        .Select(x => new PersonasDTO
                                                        {
                                                            Consecutivo = x.Consecutivo,
                                                            CodigoIdioma = x.CodigoIdioma,
                                                            CodigoPais = x.CodigoPais,
                                                        }).ToList(),
                                                       PlanesUsuarios = new PlanesUsuariosDTO
                                                       {
                                                           Consecutivo = usuario.PlanesUsuarios.Consecutivo,
                                                           CodigoPlan = usuario.PlanesUsuarios.CodigoPlan,
                                                           Adquisicion = usuario.PlanesUsuarios.Adquisicion,
                                                           Vencimiento = usuario.PlanesUsuarios.Vencimiento,
                                                           NumeroCategoriasUsadas = usuario.PlanesUsuarios.NumeroCategoriasUsadas,
                                                           Planes = new PlanesDTO
                                                           {
                                                               Consecutivo = usuario.PlanesUsuarios.Planes.Consecutivo,
                                                               Creacion = usuario.PlanesUsuarios.Planes.Creacion,
                                                               Precio = usuario.PlanesUsuarios.Planes.Precio,
                                                               CodigoPeriodicidad = usuario.PlanesUsuarios.Planes.CodigoPeriodicidad,
                                                               PlanDefault = usuario.PlanesUsuarios.Planes.PlanDefault,
                                                               VideosPerfil = usuario.PlanesUsuarios.Planes.VideosPerfil,
                                                               ServiciosChat = usuario.PlanesUsuarios.Planes.ServiciosChat,
                                                               ConsultaCandidatos = usuario.PlanesUsuarios.Planes.ConsultaCandidatos,
                                                               DetalleCandidatos = usuario.PlanesUsuarios.Planes.DetalleCandidatos,
                                                               ConsultaGrupos = usuario.PlanesUsuarios.Planes.ConsultaGrupos,
                                                               DetalleGrupos = usuario.PlanesUsuarios.Planes.DetalleGrupos,
                                                               ConsultaEventos = usuario.PlanesUsuarios.Planes.ConsultaEventos,
                                                               CreacionAnuncios = usuario.PlanesUsuarios.Planes.CreacionAnuncios,
                                                               EstadisticasAnuncios = usuario.PlanesUsuarios.Planes.EstadisticasAnuncios,
                                                               Modificacion = usuario.PlanesUsuarios.Planes.Modificacion,
                                                               CodigoTipoPerfil = usuario.PlanesUsuarios.Planes.CodigoTipoPerfil,
                                                               TiempoPermitidoVideo = usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo,
                                                               NumeroCategoriasPermisibles = usuario.PlanesUsuarios.Planes.NumeroCategoriasPermisibles,
                                                               CodigoArchivo = usuario.PlanesUsuarios.Planes.CodigoArchivo,
                                                               NumeroAparicionesAnuncio = usuario.PlanesUsuarios.Planes.NumeroAparicionesAnuncio,
                                                               NumeroDiasVigenciaAnuncio = usuario.PlanesUsuarios.Planes.NumeroDiasVigenciaAnuncio,
                                                               DescripcionIdiomaBuscado = usuario.PlanesUsuarios.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo).Select(x => x.CodigoIdioma).FirstOrDefault()).Select(y => y.Descripcion).FirstOrDefault(),
                                                           }
                                                       }
                                                   }).AsNoTracking()
                                                    .FirstOrDefaultAsync();

            return usuarioVerificado;
        }

        public async Task<UsuariosDTO> VerificarUsuarioConEmailUsuarioYDeviceId(Usuarios usuarioParaVerificar)
        {
            UsuariosDTO usuarioVerificado = await (from usuario in _context.Usuarios
                                                   where usuario.DeviceId.Trim() == usuarioParaVerificar.DeviceId.Trim()
                                                   && usuario.Usuario.Trim() == usuarioParaVerificar.Usuario
                                                   && usuario.Email.Trim() == usuario.Email.Trim()
                                                   select new UsuariosDTO
                                                   {
                                                       Consecutivo = usuario.Consecutivo,
                                                       Usuario = usuario.Usuario,
                                                       Clave = usuario.Clave,
                                                       Creacion = usuario.Creacion,
                                                       CodigoTipoPerfil = usuario.CodigoTipoPerfil,
                                                       Email = usuario.Email,
                                                       CodigoPlanUsuario = usuario.CodigoPlanUsuario,
                                                       DeviceId = usuario.DeviceId,
                                                       CuentaActiva = usuario.CuentaActiva,
                                                       Personas = usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo)
                                                        .Select(x => new PersonasDTO
                                                        {
                                                            Consecutivo = x.Consecutivo,
                                                            CodigoIdioma = x.CodigoIdioma,
                                                            CodigoPais = x.CodigoPais,
                                                        }).ToList(),
                                                       PlanesUsuarios = new PlanesUsuariosDTO
                                                       {
                                                           Consecutivo = usuario.PlanesUsuarios.Consecutivo,
                                                           CodigoPlan = usuario.PlanesUsuarios.CodigoPlan,
                                                           Adquisicion = usuario.PlanesUsuarios.Adquisicion,
                                                           Vencimiento = usuario.PlanesUsuarios.Vencimiento,
                                                           NumeroCategoriasUsadas = usuario.PlanesUsuarios.NumeroCategoriasUsadas,
                                                           Planes = new PlanesDTO
                                                           {
                                                               Consecutivo = usuario.PlanesUsuarios.Planes.Consecutivo,
                                                               Creacion = usuario.PlanesUsuarios.Planes.Creacion,
                                                               Precio = usuario.PlanesUsuarios.Planes.Precio,
                                                               CodigoPeriodicidad = usuario.PlanesUsuarios.Planes.CodigoPeriodicidad,
                                                               PlanDefault = usuario.PlanesUsuarios.Planes.PlanDefault,
                                                               VideosPerfil = usuario.PlanesUsuarios.Planes.VideosPerfil,
                                                               ServiciosChat = usuario.PlanesUsuarios.Planes.ServiciosChat,
                                                               ConsultaCandidatos = usuario.PlanesUsuarios.Planes.ConsultaCandidatos,
                                                               DetalleCandidatos = usuario.PlanesUsuarios.Planes.DetalleCandidatos,
                                                               ConsultaGrupos = usuario.PlanesUsuarios.Planes.ConsultaGrupos,
                                                               DetalleGrupos = usuario.PlanesUsuarios.Planes.DetalleGrupos,
                                                               ConsultaEventos = usuario.PlanesUsuarios.Planes.ConsultaEventos,
                                                               CreacionAnuncios = usuario.PlanesUsuarios.Planes.CreacionAnuncios,
                                                               EstadisticasAnuncios = usuario.PlanesUsuarios.Planes.EstadisticasAnuncios,
                                                               Modificacion = usuario.PlanesUsuarios.Planes.Modificacion,
                                                               TiempoPermitidoVideo = usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo,
                                                               CodigoTipoPerfil = usuario.PlanesUsuarios.Planes.CodigoTipoPerfil,
                                                               NumeroCategoriasPermisibles = usuario.PlanesUsuarios.Planes.NumeroCategoriasPermisibles,
                                                               CodigoArchivo = usuario.PlanesUsuarios.Planes.CodigoArchivo,
                                                               NumeroAparicionesAnuncio = usuario.PlanesUsuarios.Planes.NumeroAparicionesAnuncio,
                                                               NumeroDiasVigenciaAnuncio = usuario.PlanesUsuarios.Planes.NumeroDiasVigenciaAnuncio,
                                                               DescripcionIdiomaBuscado = usuario.PlanesUsuarios.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo).Select(x => x.CodigoIdioma).FirstOrDefault()).Select(y => y.Descripcion).FirstOrDefault(),
                                                           }
                                                       }
                                                   }).AsNoTracking()
                                                  .FirstOrDefaultAsync();

            return usuarioVerificado;
        }

        public async Task<Usuarios> ActualizarFechaUltimoAcceso(int consecutivoUsuarioParaActualizar)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == consecutivoUsuarioParaActualizar).FirstOrDefaultAsync();

            usuarioExistente.UltimoAcceso = DateTime.Now;

            return usuarioExistente;
        }

        public async Task<Usuarios> ModificarUsuario(Usuarios usuarioParaModificar)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == usuarioParaModificar.Consecutivo).FirstOrDefaultAsync();

            usuarioExistente.Usuario = usuarioParaModificar.Usuario.Trim();
            usuarioExistente.Clave = usuarioParaModificar.Clave.Trim();
            usuarioExistente.Email = usuarioParaModificar.Email.Trim();

            return usuarioExistente;
        }

        public async Task<Usuarios> ActivarUsuario(Usuarios usuarioParaActivar)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == usuarioParaActivar.Consecutivo).FirstOrDefaultAsync();

            usuarioExistente.CuentaActiva = 1;

            return usuarioExistente;
        }

        public async Task<Usuarios> BloquearUsuario(Usuarios usuarioParaBloquear)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == usuarioParaBloquear.Consecutivo).FirstOrDefaultAsync();

            usuarioExistente.CuentaActiva = 0;

            return usuarioExistente;
        }

        public async Task<Usuarios> ModificarDeviceId(Usuarios usuarioParaModificar)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == usuarioParaModificar.Consecutivo).FirstOrDefaultAsync();

            usuarioExistente.DeviceId = !string.IsNullOrWhiteSpace(usuarioParaModificar.DeviceId) ? usuarioParaModificar.DeviceId.Trim() : null;

            return usuarioExistente;
        }

        public async Task EliminarDeviceIdSimilares(Usuarios usuarioParaModificar)
        {
            List<Usuarios> listaUsuariosExistentes = await _context.Usuarios.Where(x => x.DeviceId.Trim() == usuarioParaModificar.DeviceId.Trim()).ToListAsync();

            listaUsuariosExistentes.ForEach(x => x.DeviceId = null);
        }

        public async Task<Usuarios> ModificarClave(int codigoUsuario, string nuevaClave)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == codigoUsuario).FirstOrDefaultAsync();

            usuarioExistente.Clave = nuevaClave.Trim();

            return usuarioExistente;
        }

        public async Task<Usuarios> ModificarEmailUsuario(Usuarios usuarioParaModificar)
        {
            Usuarios usuarioExistente = await _context.Usuarios.Where(x => x.Consecutivo == usuarioParaModificar.Consecutivo).FirstOrDefaultAsync();

            usuarioExistente.Email = usuarioParaModificar.Email.Trim();

            return usuarioExistente;
        }

        public async Task<string> BuscarFormatoCorreoPorCodigoIdioma(int codigoIdioma, TipoFormatosEnum tipoFormato)
        {
            int codigoTipoFormato = (int)tipoFormato;

            string textoFormato = await (from formato in _context.FormatoEmail
                                         where formato.CodigoIdioma == codigoIdioma
                                         && formato.CodigoTipoFormato == codigoTipoFormato
                                         select formato.TextoHtml).FirstOrDefaultAsync();

            return textoFormato;
        }

        public async Task<UsuariosDTO> VerificarSiUsuarioExisteYPertenecesAlEmail(Usuarios usuarioParaVerificar)
        {
            UsuariosDTO usuarioExistente = await (from usuario in _context.Usuarios
                                                  where usuario.Usuario.Trim() == usuarioParaVerificar.Usuario.Trim() 
                                                  && usuario.Email.Trim().ToUpper() == usuarioParaVerificar.Email.Trim().ToUpper()
                                                  select new UsuariosDTO
                                                  {
                                                      Consecutivo = usuario.Consecutivo,
                                                      Usuario = usuario.Usuario,
                                                      Creacion = usuario.Creacion,
                                                      CodigoTipoPerfil = usuario.CodigoTipoPerfil,
                                                      Email = usuario.Email,
                                                      CodigoPlanUsuario = usuario.CodigoPlanUsuario,
                                                      Personas = usuario.Personas.Where(x => x.CodigoUsuario == usuario.Consecutivo)
                                                                            .Select(x => new PersonasDTO
                                                                            {
                                                                                Consecutivo = x.Consecutivo,
                                                                                Nombres = x.Nombres,
                                                                                Apellidos = x.Apellidos,
                                                                                CodigoIdioma = x.CodigoIdioma,
                                                                            }).ToList(),
                                                  })
                                                  .AsNoTracking()
                                                  .FirstOrDefaultAsync();

            return usuarioExistente;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiUsuarioYaExiste(Usuarios usuarioParaVerificar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            wrapper.Existe = await (from usuarios in _context.Usuarios
                                    where usuarios.Usuario.Trim().ToUpper() == usuarioParaVerificar.Usuario.Trim().ToUpper()
                                    select usuarios).AnyAsync();

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiEmailYaExiste(Usuarios emailParaVerificar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            wrapper.Existe = await (from usuarios in _context.Usuarios
                                    where usuarios.Email.Trim().ToUpper() == emailParaVerificar.Email.Trim().ToUpper()
                                    select usuarios).AnyAsync();

            return wrapper;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiCuentaEstaActiva(Usuarios usuarioParaVerificar)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            wrapper.Existe = await (from usuarios in _context.Usuarios
                                    where usuarios.Usuario.Trim().ToUpper() == usuarioParaVerificar.Usuario.Trim().ToUpper()
                                    && usuarios.CuentaActiva == 1
                                    select usuarios).AnyAsync();

            return wrapper;
        }

        public async Task<List<TiposPerfiles>> ListarTiposPerfiles()
        {
            List<TiposPerfiles> listaTiposPerfiles = await (from tipoPerfil in _context.TiposPerfiles
                                                            select tipoPerfil).AsNoTracking()
                                                                              .ToListAsync();
            return listaTiposPerfiles;
        }

    }
}