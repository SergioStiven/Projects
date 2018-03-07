using PropertyChanged;
using System;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Resources;

namespace Xpinn.SportsGo.Movil.Models
{
    class HabilidadesModelArgs : EventArgs
    {
        public HabilidadesModel HabilidadCambiada { get; set; }

        public HabilidadesModelArgs(HabilidadesModel habilidad)
        {
            HabilidadCambiada = habilidad;
        }
    }

    [AddINotifyPropertyChangedInterface]
    class HabilidadesModel
    {
        public static event EventHandler<HabilidadesModelArgs> NumeroEstrellasCambiadas;

        public int CodigoCategoriaParaListarHabilidades { get; set; }
        public int CodigoCategoriaPerfilParaGuardarHabilidades { get; set; }
        public ICollection<HabilidadesCandidatosDTO> HabilidadesCandidatosExistentes { get; set; }

        public HabilidadesDTO Habilidad { get; set; }
        public bool EstaAgregada { get; set; }

        int _numeroEstrellas;
        public int NumeroEstrellas
        { 
            get { return _numeroEstrellas; }
            set
            {
                _numeroEstrellas = value;

                if (NumeroEstrellasCambiadas != null)
                {
                    NumeroEstrellasCambiadas(this, new HabilidadesModelArgs(this));
                }
            }
        }

        public string NombreTituloGrupo
        {
            get
            {
                string nombreTituloGrupo = SportsGoResources.Habilidades;

                if (EstaAgregada)
                {
                    nombreTituloGrupo = SportsGoResources.MisHabilidades;
                }

                return nombreTituloGrupo;
            }
        }

        public HabilidadesModel(CategoriasModel categoriaModel)
        {
            CodigoCategoriaParaListarHabilidades = categoriaModel.CodigoCategoria;
            CodigoCategoriaPerfilParaGuardarHabilidades = categoriaModel.CodigoCategoriaPerfil;
        }

        public HabilidadesModel(ICollection<HabilidadesCandidatosDTO> habilidadesCandidatosExistentes, CategoriasModel categoriaModel)
        {
            HabilidadesCandidatosExistentes = habilidadesCandidatosExistentes;
            CodigoCategoriaParaListarHabilidades = categoriaModel.CodigoCategoria;
            CodigoCategoriaPerfilParaGuardarHabilidades = categoriaModel.CodigoCategoriaPerfil;
        }

        public HabilidadesModel(HabilidadesDTO habilidad, bool estaAgregada = false)
        {
            Habilidad = habilidad;
            EstaAgregada = estaAgregada;
        }

        public static List<HabilidadesModel> CrearListaHabilidades(ICollection<HabilidadesDTO> listaHabilidades)
        {
            List<HabilidadesModel> listaHabilidadesModel = new List<HabilidadesModel>();

            if (listaHabilidades != null && listaHabilidades.Count > 0)
            {
                foreach (var habilidad in listaHabilidades)
                {
                    if (habilidad != null)
                    {
                        listaHabilidadesModel.Add(new HabilidadesModel(habilidad));
                    }
                }
            }

            return listaHabilidadesModel;
        }
    }
}
