using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Xpinn.SportsGo.Util.Portable.HelperClasses
{
    public static class ReflectionHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException("Debes pasar una expresión no nula!.");

            return (propertyExpression.Body as MemberExpression).Member.Name;
        }

        public static bool IsPropertyExist<T>(T settings, params string[] names)
        {
            if (settings == null) throw new ArgumentNullException("Tipo a buscar la propiedad no puede ser nulo!.");
            if (names == null || names.Count() == 0) throw new ArgumentNullException("Debes pasar un nombre de propiedad a buscar!.");

            foreach (var name in names)
            {
                if (settings.GetType().GetTypeInfo().GetDeclaredProperty(name) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static object GetPropertyValue<T>(T src, string propName)
        {
            if (src == null) throw new ArgumentNullException("Tipo a buscar la propiedad no puede ser nulo!.");
            if (propName == null) throw new ArgumentNullException("Debes pasar un nombre de propiedad a buscar!.");

            PropertyInfo property = src.GetType().GetTypeInfo().GetDeclaredProperty(propName);
            if (property == null) return null;

            return property.GetValue(src, null);
        }
    }
}
