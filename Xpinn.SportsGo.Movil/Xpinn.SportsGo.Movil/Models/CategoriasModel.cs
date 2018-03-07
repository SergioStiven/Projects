using PropertyChanged;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Movil.Models
{
    [AddINotifyPropertyChangedInterface]
    class CategoriasModel
    {
        public string DescripcionCategoria { get; set; }
        public string UrlArchivo { get; set; }
        public int CodigoArchivo { get; set; }
        public int CodigoCategoriaPerfil { get; set; }
        public int CodigoCategoria { get; set; }
        public bool EstaSeleccionado { get; set; }
        public int? PosicionJugador { get; set; }

        public bool EsFutbol
        {
            get
            {
                return CodigoCategoria == 3;
            }
        }

        public bool EsBasketBall
        {
            get
            {
                return CodigoCategoria == 5;
            }
        }

        public bool EsBaseBall
        {
            get
            {
                return CodigoCategoria == 6;
            }
        }

        public bool EsVolleyBall
        {
            get
            {
                return CodigoCategoria == 7;
            }
        }

        public bool EsNuevaCategoria
        {
            get
            {
                return CodigoCategoria <= 0 && CodigoCategoriaPerfil <= 0;
            }
        }

        public CategoriasModel(int codigoCategoria = 0, int codigoCategoriaPorPerfil = 0, int codigoArchivo = 0 , string descripcionCategoria = "", string urlArchivo = "", int? posicionJugador = null)
        {
            CodigoCategoria = codigoCategoria;
            CodigoCategoriaPerfil = codigoCategoriaPorPerfil;
            CodigoArchivo = codigoArchivo;
            DescripcionCategoria = descripcionCategoria;
            UrlArchivo = urlArchivo;
            PosicionJugador = posicionJugador;
        }

        public CategoriasModel(CategoriasDTO categoria) 
            : this(categoria.Consecutivo, 0, categoria.CodigoArchivo, categoria.DescripcionIdiomaBuscado, categoria.UrlArchivo)
        {
            
        }

        public CategoriasModel(CategoriasCandidatosDTO categoriaCandidato) 
            : this(categoriaCandidato.CodigoCategoria, categoriaCandidato.Consecutivo, categoriaCandidato.Categorias.CodigoArchivo, categoriaCandidato.Categorias.DescripcionIdiomaBuscado, categoriaCandidato.Categorias.UrlArchivo, categoriaCandidato.PosicionCampo)
        {
            
        }

        public CategoriasModel(CategoriasGruposDTO categoriaGrupo)
            : this(categoriaGrupo.CodigoCategoria, categoriaGrupo.Consecutivo, categoriaGrupo.Categorias.CodigoArchivo, categoriaGrupo.Categorias.DescripcionIdiomaBuscado, categoriaGrupo.Categorias.UrlArchivo)
        {
            
        }

        public CategoriasModel(CategoriasRepresentantesDTO categoriaRepresentante)
            : this(categoriaRepresentante.CodigoCategoria, categoriaRepresentante.Consecutivo, categoriaRepresentante.Categorias.CodigoArchivo, categoriaRepresentante.Categorias.DescripcionIdiomaBuscado, categoriaRepresentante.Categorias.UrlArchivo)
        {

        }

        public CategoriasModel()
        {
            UrlArchivo = "CuadradoConMas.png";
        }

        public static List<CategoriasModel> CrearListaCategoriasDeUnaPersona(PersonasDTO persona)
        {
            List<CategoriasModel> listaCategorias = new List<CategoriasModel>();

            if (persona != null)
            {
                switch (persona.TipoPerfil)
                {
                    case TipoPerfil.Candidato:

                        if (persona.CandidatoDeLaPersona != null && persona.CandidatoDeLaPersona.CategoriasCandidatos != null && persona.CandidatoDeLaPersona.CategoriasCandidatos.Count > 0)
                        {
                            persona.CandidatoDeLaPersona.CategoriasCandidatos.ForEach(x =>
                            {
                                if (x.Categorias != null)
                                {
                                    listaCategorias.Add(new CategoriasModel(x));
                                }
                            });
                        }

                        break;
                    case TipoPerfil.Grupo:

                        if (persona.GrupoDeLaPersona != null && persona.GrupoDeLaPersona.CategoriasGrupos != null && persona.GrupoDeLaPersona.CategoriasGrupos.Count > 0)
                        {
                            persona.GrupoDeLaPersona.CategoriasGrupos.ForEach(x =>
                            {
                                if (x.Categorias != null)
                                {
                                    listaCategorias.Add(new CategoriasModel(x));
                                }
                            });
                        }

                        break;
                    case TipoPerfil.Representante:

                        if (persona.RepresentanteDeLaPersona != null && persona.RepresentanteDeLaPersona.CategoriasRepresentantes != null && persona.RepresentanteDeLaPersona.CategoriasRepresentantes.Count > 0)
                        {
                            persona.RepresentanteDeLaPersona.CategoriasRepresentantes.ForEach(x =>
                            {
                                if (x.Categorias != null)
                                {
                                    listaCategorias.Add(new CategoriasModel(x));
                                }
                            });
                        }

                        break;
                }
            }

            return listaCategorias;
        }

        public static List<CategoriasModel> CrearListaCategorias(ICollection<CategoriasDTO> listaCategorias)
        {
            List<CategoriasModel> listaCategoriasModel = new List<CategoriasModel>();

            if (listaCategorias != null && listaCategorias.Count > 0)
            {
                listaCategorias.ForEach(x =>
                {
                    if (x != null)
                    {
                        listaCategoriasModel.Add(new CategoriasModel(x));
                    }
                });
            }

            return listaCategoriasModel;
        }
    }
}