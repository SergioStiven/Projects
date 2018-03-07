using System;
using System.Collections.Generic;
using System.IO;

namespace Xpinn.SportsGo.Util.Portable.HelperClasses
{
    public static class StreamsHelper
    {
        public static byte[] LeerTodosLosBytesDeUnStream(Stream input)
        {
            if (input == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        /// <summary>
        /// Delimita el archivo por default con '|'
        /// </summary>
        public static IEnumerable<string[]> LeerLineaArchivoDelimitadoYDevolverSinLeerTodo(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");

            return LeerLineaArchivoDelimitadoYDevolverSinLeerTodo(stream, '|');
        }

        public static IEnumerable<string[]> LeerLineaArchivoDelimitadoYDevolverSinLeerTodo(Stream stream, char separador)
        {
            if (stream == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");
            if (separador.ToString() == null) throw new ArgumentNullException("Separador no puede ser nulo!.");

            string linea = string.Empty;

            using (StreamReader strReader = new StreamReader(stream))
            {
                while ((linea = strReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    // Retorna el string[] por cada vuelta, no espera a que el while termine
                    // Después de retornar, vuelve al while y retorna el siguiente string[]
                    // Sale del while al no haber mas lineas que leer (trReader.ReadLine()) == null)
                    // Ese es el comportamiento del yield return

                    yield return linea.Split(separador);
                }
            }
        }

        public static IEnumerable<string> LeerLineaArchivoYDevolverSinLeerTodo(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");

            using (StreamReader strReader = new StreamReader(stream))
            {
                string linea = string.Empty;

                while ((linea = strReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    // Retorna el string[] por cada vuelta, no espera a que el while termine
                    // Después de retornar, vuelve al while y retorna el siguiente string[]
                    // Sale del while al no haber mas lineas que leer (trReader.ReadLine()) == null)
                    // Ese es el comportamiento del yield return
                    yield return linea;
                }
            }
        }

        /// <summary>
        /// Delimita el archivo por default con '|'
        /// </summary>
        public static IEnumerable<string[]> LeerLineasArchivoDelimitado(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");

            return LeerLineasArchivoDelimitado(stream, '|');
        }

        public static IEnumerable<string[]> LeerLineasArchivoDelimitado(Stream stream, char separador)
        {
            if (stream == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");
            if (separador.ToString() == null) throw new ArgumentNullException("Separador no puede ser nulo!.");

            List<string[]> lstLineas = new List<string[]>();
            string linea = string.Empty;

            using (StreamReader strReader = new StreamReader(stream))
            {
                while ((linea = strReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    lstLineas.Add(linea.Split(separador));
                }
            }

            return lstLineas;
        }


        public static IEnumerable<string> LeerLineasArchivo(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("Stream a leer los bytes no puede ser nulo!.");

            List<string> lstLineas = new List<string>();

            using (StreamReader strReader = new StreamReader(stream))
            {
                string linea = string.Empty;
                while ((linea = strReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    lstLineas.Add(strReader.ReadLine());
                }
            }

            return lstLineas;
        }
    }
}
