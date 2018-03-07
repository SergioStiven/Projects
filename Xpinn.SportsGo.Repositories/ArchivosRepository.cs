using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using System.Data.SqlTypes;
using Xpinn.SportsGo.Util.Portable;
using System.Data.SqlClient;
using System.IO;

namespace Xpinn.SportsGo.Repositories
{
    public class ArchivosRepository
    {
        SportsGoEntities _context;
        const string RowDataStatement = @"SELECT ArchivoContenido.PathName() AS 'Path', GET_FILESTREAM_TRANSACTION_CONTEXT() AS 'Transaction' FROM Archivos WHERE Consecutivo = @Consecutivo";
        const string RowDataLenghtStatement = @"SELECT SUM(DATALENGTH(ArchivoContenido)) FROM Archivos where Consecutivo = @Consecutivo";

        public ArchivosRepository(SportsGoEntities context)
        {
            _context = context;
        }

        public void CrearArchivo(Archivos archivoParaCrear)
        {
            _context.Archivos.Add(archivoParaCrear);
        }

        public async Task ModificarArchivoContenidoStream(int consecutivoArchivo, Stream sourceStream)
        {
            FileStreamRowData rowData = await _context.Database
                                                      .SqlQuery<FileStreamRowData>(RowDataStatement, new SqlParameter("Consecutivo", consecutivoArchivo))
                                                      .FirstOrDefaultAsync();

            using (SqlFileStream dest = new SqlFileStream(rowData.Path, rowData.Transaction, FileAccess.ReadWrite))
            {
                await sourceStream.CopyToAsync(dest);
            }
        }

        public void ModificarArchivo(Archivos archivoParaModificar)
        {
            _context.Archivos.Attach(archivoParaModificar);

            DbEntityEntry entry = _context.Entry(archivoParaModificar);
            entry.State = EntityState.Modified;
            entry.Property("RowGUID").IsModified = false;
        }

        public void ModificarCodigoTipoArchivoDeUnArchivo(Archivos archivoParaModificar)
        {
            _context.Archivos.Attach(archivoParaModificar);

            DbEntityEntry entry = _context.Entry(archivoParaModificar);
            entry.Property(nameof(archivoParaModificar.CodigoTipoArchivo)).IsModified = true;
        }

        public async Task<Stream> StreamArchivo(Archivos archivoParaBuscar)
        {
            FileStreamRowData rowData = await _context.Database
                                                      .SqlQuery<FileStreamRowData>(RowDataStatement, new SqlParameter("Consecutivo", archivoParaBuscar.Consecutivo))
                                                      .FirstOrDefaultAsync();

            return new SqlFileStream(rowData.Path, rowData.Transaction, FileAccess.Read);
        }

        public async Task<long> StreamArchivoGetLength(Archivos archivoParaBuscar)
        {
            long lenght = await _context.Database
                                        .SqlQuery<long>(RowDataLenghtStatement, new SqlParameter("Consecutivo", archivoParaBuscar.Consecutivo))
                                        .FirstOrDefaultAsync();

            return lenght;
        }

        public void EliminarArchivo(Archivos archivoParaEliminar)
        {
            _context.Archivos.Attach(archivoParaEliminar);
            _context.Archivos.Remove(archivoParaEliminar);
        }
    }
}