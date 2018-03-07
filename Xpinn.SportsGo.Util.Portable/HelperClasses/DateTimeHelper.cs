using System;

namespace Xpinn.SportsGo.Util.Portable.HelperClasses
{
    public enum TipoCalendario
    {
        SinTipoCalendario,
        DiasHabiles,
        SinDiasHabiles
    }

    public static class DateTimeHelper
    {

        public static DateTime PrimerDiaDelMes(DateTime fechaActual)
        {
            if (fechaActual == null) throw new ArgumentNullException("Fecha no puede ser nula!.");

            return new DateTime(fechaActual.Year, fechaActual.Month, 1);
        }

        public static DateTime UltimoDiaDelMes(DateTime fechaActual)
        {
            if (fechaActual == null) throw new ArgumentNullException("Fecha no puede ser nula!.");

            DateTime primerDia = PrimerDiaDelMes(fechaActual);
            return primerDia.AddMonths(1).AddDays(-1);
        }

        // Diferencia en meses
        public static long DiferenciaEntreDosFechas(DateTime fechaMayor, DateTime fechaMenor)
        {
            if (fechaMayor == null) throw new ArgumentNullException("Fecha mayor no puede ser nula!.");
            if (fechaMenor == null) throw new ArgumentNullException("Fecha menor no puede ser nula!.");

            return Convert.ToInt64(Math.Round(fechaMayor.Subtract(fechaMenor).Days / (365.25 / 12)));
        }

        // Diferencia en años
        public static long DiferenciaEntreDosFechasAños(DateTime fechaMayor, DateTime fechaMenor)
        {
            if (fechaMayor == null) throw new ArgumentNullException("Fecha mayor no puede ser nula!.");
            if (fechaMenor == null) throw new ArgumentNullException("Fecha menor no puede ser nula!.");

            return fechaMayor.Year - fechaMenor.Year;
        }

        // Diferencia en dias
        public static long DiferenciaEntreDosFechasDias(DateTime fechaMayor, DateTime fechaMenor)
        {
            if (fechaMayor == null) throw new ArgumentNullException("Fecha mayor no puede ser nula!.");
            if (fechaMenor == null) throw new ArgumentNullException("Fecha menor no puede ser nula!.");

            return Convert.ToInt64(Math.Round((fechaMayor - fechaMenor).TotalDays));
        }

        public static DateTime SumarDiasSegunTipoCalendario(DateTime fecha, int dias, TipoCalendario tipo_cal = TipoCalendario.SinDiasHabiles)
        {
            if (fecha == null) throw new ArgumentNullException("Fecha para sumar no puede ser nula");

            int dato = 0;

            if (tipo_cal == TipoCalendario.DiasHabiles)
            {
                int año_fec = fecha.Year;
                int mes_fec = fecha.Month;
                int dia_fec = fecha.Day;
                int dias_febrero = 28;
                if (año_fec % 4 == 0)
                    dias_febrero = 29;
                if (dia_fec > 30 || (mes_fec == 2 && dia_fec >= dias_febrero))
                    dia_fec = 30;
                dato = (dias / 360);
                if (dato > 1)
                {
                    año_fec = año_fec + dato;
                    dias = dias - (360 * dato);
                }
                dato = (dias / 30);
                if (dato >= 1)
                {
                    mes_fec = mes_fec + dato;
                    if (mes_fec > 12)
                    {
                        if (mes_fec > 12)
                        {
                            while (mes_fec > 12)
                            {
                                año_fec = año_fec + 1;
                                mes_fec = mes_fec - 12;
                            }
                        }
                    }
                    dias = dias - (30 * dato);
                }
                if (dias > 0)
                {
                    if (30 - dia_fec < dias)
                    {
                        mes_fec = mes_fec + 1;
                        if (mes_fec > 12)
                        {
                            año_fec = año_fec + 1;
                            mes_fec = 1;
                        }
                        dia_fec = dias - (30 - dia_fec);
                    }
                    else
                    {
                        dia_fec = dia_fec + dias;
                    }
                }
                if (mes_fec == 2 && (dia_fec > dias_febrero && dia_fec <= 30))
                    dia_fec = dias_febrero;
                if (mes_fec == 2 && (dia_fec > 30 && dia_fec <= 59))
                    dia_fec = dia_fec - (30 - dias_febrero);
                if (mes_fec == 4 || mes_fec == 6 || mes_fec == 9 || mes_fec == 11)
                    if (dia_fec == 31)
                        dia_fec = 30;
                // Validar mes de febrero
                if (año_fec % 4 == 0)
                    dias_febrero = 29;
                else
                    dias_febrero = 28;
                if (mes_fec == 2 && (dia_fec > dias_febrero && dia_fec <= 30))
                    dia_fec = dias_febrero;
                // Crear la fecha
                fecha = new DateTime(año_fec, mes_fec, dia_fec, 0, 0, 0);
            }
            else
            {
                fecha = fecha.AddDays(dias);
            }

            return fecha;
        }
    }
}
