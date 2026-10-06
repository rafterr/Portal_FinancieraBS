using BusinessInterfase;
using BusinessType;
using DataInterfase;

namespace BusinessLayer
{
    public class DocumentoProcessor : IDocumentoProcessor
    {
        // Extensión -> (content type, firma de los primeros bytes del archivo)
        private static readonly Dictionary<string, (string ContentType, byte[] Firma)> Formatos = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = ("application/pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 }),        // %PDF
            [".jpg"] = ("image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF }),
            [".jpeg"] = ("image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF }),
            [".png"] = ("image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 }),
        };

        private readonly IDocumentoRepository _documentoRepository;
        private readonly IDocumentoStorage _storage;
        private readonly DocumentoOptions _options;

        public DocumentoProcessor(IDocumentoRepository documentoRepository, IDocumentoStorage storage, DocumentoOptions options)
        {
            _documentoRepository = documentoRepository;
            _storage = storage;
            _options = options;
        }

        public ResultadoOperacion Validar(ArchivoSubido archivo)
        {
            if (archivo.Tamano <= 0) return ResultadoOperacion.Falla("El archivo está vacío.");
            if (archivo.Tamano > _options.TamanoMaximoBytes)
                return ResultadoOperacion.Falla($"El archivo '{archivo.NombreOriginal}' excede el tamaño máximo de {_options.TamanoMaximoBytes / (1024 * 1024)} MB.");
            if (!Formatos.ContainsKey(Path.GetExtension(archivo.NombreOriginal)))
                return ResultadoOperacion.Falla($"El archivo '{archivo.NombreOriginal}' no es PDF, JPG o PNG.");
            return ResultadoOperacion.Ok();
        }

        public async Task<ResultadoOperacion> SubirAsync(ArchivoSubido archivo, TipoDocumento tipo, int clienteId, int? prestamoId, string? usuarioId)
        {
            var validacion = Validar(archivo);
            if (!validacion.Exito) return validacion;

            if (tipo == TipoDocumento.ComprobanteDomicilio) prestamoId = null;
            else if (prestamoId == null) return ResultadoOperacion.Falla("El pagaré y la INE deben asociarse a un préstamo.");

            var extension = Path.GetExtension(archivo.NombreOriginal).ToLowerInvariant();
            var formato = Formatos[extension];

            // Se verifica el contenido real, no solo la extensión
            using var buffer = new MemoryStream();
            await archivo.Contenido.CopyToAsync(buffer);
            if (!buffer.ToArray().AsSpan().StartsWith(formato.Firma))
                return ResultadoOperacion.Falla($"El contenido de '{archivo.NombreOriginal}' no corresponde a un archivo {extension.TrimStart('.').ToUpperInvariant()} válido.");
            buffer.Position = 0;

            var ruta = await _storage.GuardarAsync(buffer, tipo.ToString().ToLowerInvariant(), extension);

            // Un solo documento por tipo: el comprobante por cliente; pagaré e INE por préstamo
            var anteriores = prestamoId == null
                ? (await _documentoRepository.GetByClienteIdAsync(clienteId)).Where(d => d.Tipo == tipo && d.PrestamoId == null)
                : (await _documentoRepository.GetByPrestamoIdAsync(prestamoId.Value)).Where(d => d.Tipo == tipo);

            await _documentoRepository.CreateAsync(new Documento
            {
                Tipo = tipo,
                ClienteId = clienteId,
                PrestamoId = prestamoId,
                NombreOriginal = Path.GetFileName(archivo.NombreOriginal),
                ContentType = formato.ContentType,
                Tamano = buffer.Length,
                Ruta = ruta,
                FechaCarga = DateTime.Now,
                UsuarioId = usuarioId
            });

            foreach (var anterior in anteriores.ToList())
                await EliminarAsync(anterior.Id);

            return ResultadoOperacion.Ok();
        }

        public async Task<List<Documento>> GetByClienteIdAsync(int clienteId)
        {
            return await _documentoRepository.GetByClienteIdAsync(clienteId);
        }

        public async Task<List<Documento>> GetByPrestamoIdAsync(int prestamoId)
        {
            return await _documentoRepository.GetByPrestamoIdAsync(prestamoId);
        }

        public async Task<Documento?> GetByIdAsync(int id)
        {
            return await _documentoRepository.GetByIdAsync(id);
        }

        public async Task<DocumentoAbierto?> AbrirAsync(int id)
        {
            var documento = await _documentoRepository.GetByIdAsync(id);
            if (documento == null) return null;

            var contenido = await _storage.AbrirAsync(documento.Ruta);
            return contenido == null ? null : new DocumentoAbierto(documento, contenido);
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id)
        {
            var documento = await _documentoRepository.GetByIdAsync(id);
            if (documento == null) return ResultadoOperacion.Falla("El documento no existe.");

            await _documentoRepository.DeleteAsync(id);
            await _storage.EliminarAsync(documento.Ruta);
            return ResultadoOperacion.Ok();
        }
    }
}
