using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Entities;
using System.Threading.Tasks;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;
using System.IO;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services.Tests
{
    [TestClass]
    public class ArchivosServicesTest
    {
        [TestMethod]
        public async Task ArchivosServices_BuscarArchivo_ShouldSearch()
        {
            ArchivosServices authenticateService = new ArchivosServices();

            Stream archivoBuscado = await authenticateService.BuscarArchivo(46);

            Assert.IsNotNull(archivoBuscado);
        }

        [TestMethod]
        public async Task ArchivosServices_CrearArchivoStream_ShouldCreate()
        {
            //using (FileStream video = File.Open(@"C:\Users\Bloodnero\Documents\Visual Studio 2017\Presentación de campeón- Xayah - Jugabilidad - League of Legends.mp4", FileMode.Open, FileAccess.Read))
            using (FileStream video = File.Open(@"C:\Users\Bloodnero\Documents\Visual Studio 2017\ahhhh.mp4", FileMode.Open, FileAccess.Read))
            {
                ArchivosServices archivoServices = new ArchivosServices();

                var hola = await archivoServices.CrearArchivoStream((int)TipoArchivo.Video, video);

                Assert.IsNotNull(hola);
            }
        }

        [TestMethod]
        public async Task ArchivosServices_AsignarImagenPerfilPersona_ShouldCreate()
        {
            using (Stream video = File.Open(@"C:\Users\Bloodnero\Documents\Visual Studio 2015\718156.jpg", FileMode.Open, FileAccess.Read))
            {
                ArchivosServices archivoServices = new ArchivosServices();

                var hola = await archivoServices.AsignarImagenPerfilPersona(3, 210, video);
            }
        }
    }
}
