using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xpinn.SportsGo.Util.Portable.HelperClasses
{
    public static partial class ExtensionMethodsHelper
    {
        // Extension Method para usar un foreach improvisado en cualquier colleccion :D
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException("collection nulo!.");
            if (action == null) throw new ArgumentNullException("action nulo!.");

            foreach (T item in collection)
                action(item);
        }       

        // Ver si un string esta exactamente igual en un IEnumerable<string>
        public static bool VerifyIfStringIsInCollection(this string source, IEnumerable<string> collection, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            if (source == null) throw new ArgumentNullException("source nulo!.");
            if (collection == null) throw new ArgumentNullException("collection nulo!.");

            bool esIgual = false;
            foreach (var item in collection)
            {
                esIgual = source.Equals(item, comp);

                if (esIgual) break;
            }

            return esIgual;
        }

        // Verifica si un string esta en otro string teniendo la opcion de eliminar la comparacion del el case
        public static string ConvertToCommaSeparatedString<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source nulo!.");

            return string.Join(",", source.Select(n => n.ToString()).ToArray());
        }

        // Verifica si un string esta en otro string teniendo la opcion de eliminar la comparacion del el case
        public static bool Contains(this string source, string stringToCheck, StringComparison comp)
        {
            if (source == null) throw new ArgumentNullException("source nulo!.");
            if (stringToCheck == null) throw new ArgumentNullException("stringToCheck nulo!.");

            return source.IndexOf(stringToCheck, comp) >= 0;
        }

        // Verifica si un string esta dentro de un array de string teniendo la opcion de eliminar la comparacion del el case
        // Valida si mi source existe en el array (OJO no significa que sea exactamente igual el string, solo significa que esta adentro)
        public static bool Contains(this string source, string[] stringArrayToCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            if (source == null) throw new ArgumentNullException("source nulo!.");
            if (stringArrayToCheck == null) throw new ArgumentNullException("stringToCheck nulo!.");

            foreach (var item in stringArrayToCheck)
            {
                if (source.Contains(item)) return true;
            }

            return false;
        }


        // Verifica si un array de string contiene un string teniendo la opcion de eliminar la comparacion del case
        // Valida si en mi sourceArray existe el string (OJO no significa que sea exactamente igual el string, solo significa que esta adentro)
        public static bool Contains(this string[] source, string stringToCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            if (source == null) throw new ArgumentNullException("source nulo!.");
            if (stringToCheck == null) throw new ArgumentNullException("stringToCheck nulo!.");

            foreach (var item in source)
            {
                if (item.Contains(stringToCheck, comp)) return true;
            }

            return false;
        }

        // Verifica si un PropertyInfo es una Collecion, pero no afecta al string que es un IEnumrable
        public static bool IsEnumerableNonString(this PropertyInfo propertyInfo)
        {
            return propertyInfo != null && propertyInfo.PropertyType.IsEnumerableNonString();
        }

        // Verifica si un object es una Collecion, pero no afecta al string que es un IEnumrable
        public static bool IsEnumerableNonString(this object instance)
        {
            return instance != null && instance.GetType().IsEnumerableNonString();
        }
    }
}
