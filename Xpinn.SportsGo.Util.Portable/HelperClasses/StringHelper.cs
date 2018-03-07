using System;

namespace Xpinn.SportsGo.Util.Portable.HelperClasses
{
    public static class StringHelper
    {
        public static string QuitarAndYColocarWhereEnFiltroQuery(string filtro)
        {
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string newFilter = filtro;

                // Si la Query esta llena la ordeno de manera que no explote por tener un "and" al iniciar
                if (filtro.StartsWith(" and "))
                {
                    newFilter = filtro.Remove(0, 4).Insert(0, " WHERE ");
                }

                return newFilter;
            }

            return filtro;
        }


        public static string FormatearNumerosComoCurrency(string stringTextBox)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stringTextBox))
                {
                    return " ";
                }
                else if (stringTextBox.Trim().StartsWith("0"))
                {
                    return "$ " + stringTextBox.Trim();
                }
                else if (stringTextBox.Contains("$"))
                {
                    return stringTextBox.Trim();
                }
                else
                {
                    return string.Format("$ {0:#,##0.00}", Convert.ToDecimal(stringTextBox.Trim().Replace(".", "")));
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Pasaste una letra al metodo (FormatearNumerosComoCurrency) que solo formatea numeros");
            }
        }

        public static string FormatearNumerosComoCurrency(int numero)
        {
            return FormatearNumerosComoCurrency(numero.ToString());
        }


        public static string FormatearNumerosComoCurrency(long numero)
        {
            return FormatearNumerosComoCurrency(numero.ToString());
        }


        public static string FormatearNumerosComoCurrency(decimal numero)
        {
            return FormatearNumerosComoCurrency(numero.ToString());
        }


        public static string FormatearNumerosComoCurrency(double numero)
        {
            return FormatearNumerosComoCurrency(numero.ToString());
        }


        public static string FormatearNumerosComoCurrency(float numero)
        {
            return FormatearNumerosComoCurrency(numero.ToString());
        }


        public static string DesformatearNumerosDecimales(string numeroConFormato)
        {
            return numeroConFormato.Replace(".", "").Replace("$", "").Trim();
        }


        public static string DesformatearNumerosEnteros(string numeroConFormato)
        {
            string numeroDesformateado = DesformatearNumerosDecimales(numeroConFormato);
            int index = numeroDesformateado.IndexOf(",");

            if (index < 0)
            {
                index = numeroDesformateado.Length;
            }

            numeroDesformateado = numeroDesformateado.Substring(0, index);

            return numeroDesformateado;
        }
    }
}
