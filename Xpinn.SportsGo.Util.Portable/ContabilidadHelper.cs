using System;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Util.Portable
{
    public enum TipoCuota
    {
        Escalonada,
        Fija
    }

    public class ContabilidadHelper
    {
        public IEnumerable<FilaPlanPago> PlanDePagos { get; set; }

        // Tasa de interés ya transformada a la periodicidad 
        public IEnumerable<FilaPlanPago> CrearPlanDePagos(int numeroDeCuotas, int numeroDiasSumarPorPeriodicidad, decimal valorCuotaAproximada, decimal montoSolicitado, DateTime fechaPrimerPago, DateTime? fechaAprobacion = null, TipoCuota tipoCuota = TipoCuota.Escalonada, int tipoCalendario = 0, decimal tasaDeInteres = 0)
        {
            List<FilaPlanPago> lstPlanDePago = new List<FilaPlanPago>();
            decimal valorAcumuladoAbonadoCapital = 0;

            for (int i = 1; i <= numeroDeCuotas; i++)
            {
                FilaPlanPago filaPlanPago = new FilaPlanPago(i, numeroDiasSumarPorPeriodicidad, fechaPrimerPago, tipoCalendario);
                valorAcumuladoAbonadoCapital += filaPlanPago.InicializarFila(valorCuotaAproximada, montoSolicitado, tasaDeInteres, numeroDeCuotas, valorAcumuladoAbonadoCapital, tipoCuota, fechaAprobacion);

                lstPlanDePago.Add(filaPlanPago);
            }

            PlanDePagos = lstPlanDePago;
            return lstPlanDePago;
        }

        public decimal CalcularCuota(decimal montoSolicitado, decimal tasaInteres, int numeroDeCuotas)
        {
            tasaInteres /= 100;
            var calculo = montoSolicitado * tasaInteres;
            var calculo2 = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + tasaInteres), numeroDeCuotas * -1));
            var calculo3 = 1 - calculo2;
            var calculo4 = calculo / calculo3;
            return calculo4;
        }
    }


    public class FilaPlanPago
    {
        DateTime _fechaPrimerPago;
        int _numeroDiasSumarPorPeriodicidad;
        int _tipoCalendario;

        public FilaPlanPago(int numeroCuota, int numeroDiasSumar, DateTime fechaPrimerPago, int tipoCalendario = 0)
        {
            _numeroDiasSumarPorPeriodicidad = numeroDiasSumar;
            _fechaPrimerPago = fechaPrimerPago;
            _tipoCalendario = tipoCalendario;

            NumeroCuota = numeroCuota;
        }

        public int NumeroCuota { get; private set; }


        public DateTime Fecha
        {
            get
            {
                if (_tipoCalendario == 0)
                {
                    return _fechaPrimerPago.AddDays(_numeroDiasSumarPorPeriodicidad * (NumeroCuota - 1));
                }
                else
                {
                    return DateTimeHelper.SumarDiasSegunTipoCalendario(_fechaPrimerPago, _numeroDiasSumarPorPeriodicidad * (NumeroCuota - 1), TipoCalendario.DiasHabiles);
                }
            }
        }

        public decimal ValorAbonarCapital { get; private set; }

        public decimal ValorAbonarInteres { get; private set; }

        public decimal TotalValorCuota { get; private set; }

        public decimal SaldoCapitalPendiente { get; private set; }

        public decimal InicializarFila(decimal valorCuotaAproximada, decimal montoSolicitado, decimal tasaInteres, int numeroDeCuotas, decimal valorAcumuladoAbonadoCapital = 0, TipoCuota tipoCuota = TipoCuota.Escalonada, DateTime? fechaAprobacion = null)
        {
            tasaInteres /= 100;
            ValorAbonarInteres = Math.Round((montoSolicitado - valorAcumuladoAbonadoCapital) * tasaInteres);

            if (NumeroCuota == 1 && fechaAprobacion.HasValue && _fechaPrimerPago != DateTime.MinValue)
            {
                // Calculo los dias de ajuste 
                long diferenciaEntreFechas = DateTimeHelper.DiferenciaEntreDosFechasDias(_fechaPrimerPago, fechaAprobacion.Value);
                long diasAjuste = diferenciaEntreFechas - _numeroDiasSumarPorPeriodicidad;

                // Sumo a mi interes actuales los intereses de mis dias de ajuste 
                ValorAbonarInteres += Math.Round((ValorAbonarInteres / _numeroDiasSumarPorPeriodicidad) * diasAjuste);
            }

            if (tipoCuota == TipoCuota.Fija)
            {
                TotalValorCuota = Math.Round(valorCuotaAproximada);
                ValorAbonarCapital = Math.Round(TotalValorCuota - ValorAbonarInteres);
            }
            else if (tipoCuota == TipoCuota.Escalonada)
            {
                TotalValorCuota = Math.Round(ValorAbonarCapital + ValorAbonarInteres);
                ValorAbonarCapital = Math.Round(valorCuotaAproximada);
            }

            SaldoCapitalPendiente = Math.Round(montoSolicitado - (ValorAbonarCapital + valorAcumuladoAbonadoCapital));

            // Ajusto la ultima cuota en caso de diferencias tanto positiva como negativa
            if (NumeroCuota == numeroDeCuotas)
            {
                TotalValorCuota += Math.Round(SaldoCapitalPendiente);
                ValorAbonarCapital += Math.Round(SaldoCapitalPendiente);
                SaldoCapitalPendiente = 0;
            }

            return ValorAbonarCapital;
        }
    }
}
