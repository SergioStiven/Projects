using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.IO;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Business.Test
{
    [TestClass]
    public class ArchivosBusinessTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            using (SportsGoEntities context = new SportsGoEntities())
            {
                using (var video = File.Open(@"C:\Users\Bloodnero\Documents\Visual Studio 2017\Presentación de campeón- Xayah - Jugabilidad - League of Legends.mp4", FileMode.Open, FileAccess.Read))
                {
                    var length = (int)video.Length;
                    byte[] aghhh = new byte[length];

                    video.Read(aghhh, 0, Math.Min(length, aghhh.Length));

                    Archivos archivo = new Archivos
                    {
                        TipoArchivo = Util.Portable.Enums.TipoArchivo.Video,
                        ArchivoContenido = aghhh
                    };

                    context.Archivos.Add(archivo);

                    await context.SaveChangesAsync();
                }
            }
        }
        
        [TestMethod]
        public async Task ArchivosBusiness_ModificarArchivoStream_ShouldModify()
        {
            using (FileStream video = File.Open(@"C:\Users\Bloodnero\Documents\Visual Studio 2017\Presentación de campeón- Xayah - Jugabilidad - League of Legends.mp4", FileMode.Open, FileAccess.Read))
            {
                ArchivosBusiness archivoServices = new ArchivosBusiness();

                var hola = await archivoServices.ModificarArchivoStream(97, (int)TipoArchivo.Video, null);
                
                Assert.IsNotNull(hola);
            }
        }

        [TestMethod]
        public async Task ArchivosBusiness_AsignarImagenPerfilPersona_ShouldAssign()
        {
            using (FileStream video = File.Open(@"C:\Users\Administrador.EXPINN-TEC-5\Documents\Visual Studio 2017\images.jpg", FileMode.Open, FileAccess.Read))
            {
                ArchivosBusiness archivoServices = new ArchivosBusiness();

                var hola = await archivoServices.AsignarImagenPerfilPersona(3, 2188, video);

                Assert.IsNotNull(hola);
            }
        }
    }
}
