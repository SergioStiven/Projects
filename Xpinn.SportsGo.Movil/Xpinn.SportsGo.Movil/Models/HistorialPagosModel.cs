using PropertyChanged;
using System.Collections.Generic;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    [AddINotifyPropertyChangedInterface]
    class HistorialPagosModel
    {
        public HistorialPagosPersonasDTO HistorialPago { get; set; }

        public bool EsActualizarPlan { get; set; }

        public Color ColorDeEstado
        {
            get
            {
                Color color = Color.Default;

                if (HistorialPago != null)
                {
                    switch (HistorialPago.EstadoDelPago)
                    {
                        case EstadoDeLosPagos.EsperaPago:
                            color = Color.Orange;
                            break;
                        case EstadoDeLosPagos.PendientePorAprobar:
                            color = Color.Orange;
                            break;
                        case EstadoDeLosPagos.Rechazado:
                            color = Color.Red;
                            break;
                        case EstadoDeLosPagos.Aprobado:
                            color = Color.Green;
                            break;
                    }
                }

                return color;
            }
        }

        public string NombreEstado
        {
            get
            {
                string nombreEstado = string.Empty;

                switch (HistorialPago.EstadoDelPago)
                {
                    case EstadoDeLosPagos.EsperaPago:
                        nombreEstado = SportsGoResources.EsperaPago;
                        break;
                    case EstadoDeLosPagos.PendientePorAprobar:
                        nombreEstado = SportsGoResources.PendientePorAprobar;
                        break;
                    case EstadoDeLosPagos.Rechazado:
                        nombreEstado = SportsGoResources.Rechazado;
                        break;
                    case EstadoDeLosPagos.Aprobado:
                        nombreEstado = SportsGoResources.Aprobado;
                        break;
                }

                return nombreEstado;
            }
        }

        public bool DebeMostrarObservacionesUsuario
        {
            get
            {
                return !string.IsNullOrWhiteSpace(HistorialPago.ObservacionesCliente) && (HistorialPago.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago || HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago);
            }
        }

        public bool DebeMostrarObservacionesAdmin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(HistorialPago.ObservacionesAdministrador) && (HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado || HistorialPago.EstadoDelPago == EstadoDeLosPagos.Aprobado);
            }
        }

        public bool DebeMostrarSeguimientoDelPago
        {
            get
            {
                return HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago || HistorialPago.EstadoDelPago == EstadoDeLosPagos.Aprobado || HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado || HistorialPago.EstadoDelPago == EstadoDeLosPagos.PendientePorAprobar;
            }
        }

        public string AbreviaturaMoneda
        {
            get
            {
                return " (" + HistorialPago.Monedas.AbreviaturaMoneda + "): ";
            }
        }

        public string PeriodicidadPlanNombre
        {
            get
            {
                string periodicidadNombre = string.Empty;

                switch (HistorialPago.Planes.CodigoPeriodicidad)
                {
                    case 1:
                        periodicidadNombre = SportsGoResources.Mensual;
                        break;
                    case 2:
                        periodicidadNombre = SportsGoResources.Bimestral;
                        break;
                    case 3:
                        periodicidadNombre = SportsGoResources.Semestral;
                        break;
                    case 4:
                        periodicidadNombre = SportsGoResources.Anual;
                        break;
                }

                return periodicidadNombre;
            }
        }

        public string NombreParaElBoton
        {
            get
            {
                string nombreBoton = string.Empty;

                if (HistorialPago.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago)
                {
                    nombreBoton = SportsGoResources.ComprarPlan;
                }
                else if (HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago || HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado)
                {
                    nombreBoton = SportsGoResources.ReportarPago;
                }

                return nombreBoton;
            }
        }

        public string NombreParaElBotonAnexoArchivo
        {
            get
            {
                string nombreBoton = string.Empty;

                if (HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado)
                {
                    nombreBoton = SportsGoResources.CambiarArchivo;
                }
                else
                {
                    nombreBoton = SportsGoResources.AnexarRecibo;
                }

                return nombreBoton;
            }
        }

        public bool PuedeCambiarEstado
        {
            get
            {
                return HistorialPago.EstadoDelPago != EstadoDeLosPagos.Aprobado && HistorialPago.EstadoDelPago != EstadoDeLosPagos.PendientePorAprobar;
            }
        }

        public bool PuedeInteractuarImagen
        {
            get
            {
                return HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago || HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado;
            }
        }

        public bool DebeMostrarImagen
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UrlArchivo) && HistorialPago.EstadoDelPago != EstadoDeLosPagos.SinEstadoDelPago;
            }
        }

        public bool SePuedeBorrarPago
        {
            get
            {
                return HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago;
            }
        }

        public bool EsRegistroPago
        {
            get
            {
                return HistorialPago.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago && HistorialPago.Consecutivo > 0;
            }
        }

        public bool SeMuestraBotonPayU
        {
            get
            {
                // Si pago en Pesos Colombianos (COP) y estoy en un estado aceptable para pagar muestro la imagen 
                return HistorialPago.Monedas.MonedaEnum == MonedasEnum.PesosColombianos && (HistorialPago.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago || HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago || HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado);
            }
        }

        string _urlArchivo;
        public string UrlArchivo
        {
            get
            {
                string urlArchivo = string.Empty;

                if (!string.IsNullOrWhiteSpace(_urlArchivo))
                {
                    urlArchivo = _urlArchivo;
                }
                else
                {
                    urlArchivo = HistorialPago.UrlImagen;
                }

                return urlArchivo;
            }
            set
            {
                _urlArchivo = value;
            }
        }

        public HistorialPagosModel(HistorialPagosPersonasDTO historial)
        {
            HistorialPago = historial;
        }

        public static List<HistorialPagosModel> CrearListaHistorialPagosModel(ICollection<HistorialPagosPersonasDTO> listaPagos)
        {
            List<HistorialPagosModel> listaModel = new List<HistorialPagosModel>();

            if (listaPagos != null && listaPagos.Count > 0)
            {
                foreach (var pago in listaPagos)
                {
                    listaModel.Add(new HistorialPagosModel(pago));
                }
            }

            return listaModel;
        }
    }
}
