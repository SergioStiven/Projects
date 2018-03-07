using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Util.HelperClasses
{
    public static partial class ExtensionMethodsHelper
    {
        // Extension Method para crear un DataTable de una coleccion de objetos, 
        // CUIDADO EL ORDEN DE LAS COLUMNAS SERA EL ORDEN EL CUAL FUERON DEFINIDAS LAS PROPIEDADES
        // Si quieres que solo añada unas columnas en especifico, pasa un array de string con el nombre de ellas (No importa el case)
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string[] columnaQueQuiero = null, string dataTableName = "DataTable")
        {
            if (collection == null) throw new ArgumentNullException("collection nulo!.");

            DataTable dataTable = new DataTable(dataTableName);
            Type typeObject = typeof(T);
            List<PropertyInfo> propertyInfoList = typeObject.GetProperties().Where(x => !x.IsEnumerableNonString()).ToList();

            if (columnaQueQuiero != null && columnaQueQuiero.Count() > 0)
            {
                propertyInfoList = propertyInfoList.Where(x => x.Name.VerifyIfStringIsInCollection(columnaQueQuiero)).ToList();
            }

            //Inspect the properties and create the columns in the DataTable
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                Type columnType = propertyInfo.PropertyType;
                if ((columnType.IsGenericType))
                {
                    columnType = columnType.GetGenericArguments()[0];
                }

                dataTable.Columns.Add(propertyInfo.Name, columnType);
            }

            //Populate the data table
            foreach (T item in collection)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow.BeginEdit();

                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    if (propertyInfo.GetValue(item, null) != null)
                    {
                        dataRow[propertyInfo.Name] = propertyInfo.GetValue(item, null);
                    }
                }

                dataRow.EndEdit();
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        // Crear un DataSet de una coleccion de objetos, 
        // CUIDADO EL ORDEN DE LAS COLUMNAS SERA EL ORDEN EL CUAL FUERON DEFINIDAS LAS PROPIEDADES
        // Si quieres que solo añada unas columnas en especifico, o un orden en especifico, pasa un array de string con el nombre de ellas (No importa el case)
        public static DataSet ToDataSet<T>(this IEnumerable<T> collection, string[] columnaQueQuiero = null, string dataSetName = "DataSet", string dataTableName = "DataTable")
        {
            if (collection == null) throw new ArgumentNullException("collection nulo!.");

            DataSet dataSet = new DataSet(dataSetName);

            DataTable dataTable = ToDataTable(collection, columnaQueQuiero, dataTableName);

            //Add table to dataset
            dataSet.Tables.Add(dataTable);

            return dataSet;
        }

        // Verifica si un Type es una Collecion, pero no afecta al string que es un IEnumrable
        public static bool IsEnumerableNonString(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        // Convertir un string en un Enum
        // Ejemplo =>     TipoReporteCartera tipoReporte = ddlConsultar.SelectedValue.ToEnum<TipoReporteCartera>();
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default(TEnum))
            where TEnum : struct, IConvertible
        {
            if (value == null) return defaultValue;

            TEnum result;
            return Enum.TryParse(value, true, out result) ? result : defaultValue;
        }

        // Extension Method para convertir un numero en un Enum
        // Ejemplo =>  int aww = 1   TipoReporteCartera tipo = aww.ToEnum<TipoReporteCartera, int>();
        public static TEnum ToEnum<TEnum>(this long value, TEnum defaultValue = default(TEnum))
            where TEnum : struct, IConvertible
        {
            TEnum tipo = value.ToString().ToEnum<TEnum>();
            return tipo;
        }

        public static TEnum ToEnum<TEnum>(this int value, TEnum defaultValue = default(TEnum))
        where TEnum : struct, IConvertible
        {
            TEnum tipo = value.ToString().ToEnum<TEnum>();
            return tipo;
        }

        public static TEnum ToEnum<TEnum>(this int? value, TEnum defaultValue = default(TEnum))
        where TEnum : struct, IConvertible
        {
            if (!value.HasValue) return defaultValue;

            TEnum tipo = value.ToString().ToEnum<TEnum>();
            return tipo;
        }

        public static TEnum ToEnum<TEnum>(this long? value, TEnum defaultValue = default(TEnum))
        where TEnum : struct, IConvertible
        {
            if (!value.HasValue) return defaultValue;

            TEnum tipo = value.ToString().ToEnum<TEnum>();
            return tipo;
        }
    }
}
