using BusinessInterfase;
using BusinessLayer;
using BusinessType;

namespace FinancieraBS.Tests
{
    public class DocumentoProcessorTests
    {
        private static readonly byte[] Pdf = { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31 };
        private static readonly byte[] Png = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A };

        private readonly FakeDocumentoRepository _repo = new();
        private readonly FakeDocumentoStorage _storage = new();
        private readonly DocumentoProcessor _processor;

        public DocumentoProcessorTests()
        {
            _processor = new DocumentoProcessor(_repo, _storage, new DocumentoOptions { TamanoMaximoBytes = 1024 });
        }

        private static ArchivoSubido Archivo(string nombre, byte[] contenido) => new(new MemoryStream(contenido), nombre, contenido.Length);

        [Fact]
        public async Task Subir_GuardaArchivoValido()
        {
            var resultado = await _processor.SubirAsync(Archivo("ine.pdf", Pdf), TipoDocumento.Ine, 1, 10, "u1");

            Assert.True(resultado.Exito);
            var doc = Assert.Single(_repo.Documentos);
            Assert.Equal("application/pdf", doc.ContentType);
            Assert.Equal(10, doc.PrestamoId);
            Assert.Single(_storage.Archivos);
        }

        [Theory]
        [InlineData("virus.exe")]
        [InlineData("pagina.html")]
        public void Validar_RechazaExtensionesNoPermitidas(string nombre)
        {
            Assert.False(_processor.Validar(Archivo(nombre, Pdf)).Exito);
        }

        [Fact]
        public void Validar_RechazaArchivoDemasiadoGrande()
        {
            Assert.False(_processor.Validar(Archivo("ine.pdf", new byte[2048])).Exito);
        }

        [Fact]
        public async Task Subir_RechazaContenidoQueNoCoincideConLaExtension()
        {
            var resultado = await _processor.SubirAsync(Archivo("ine.pdf", Png), TipoDocumento.Ine, 1, 10, null);

            Assert.False(resultado.Exito);
            Assert.Empty(_storage.Archivos);
        }

        [Fact]
        public async Task Subir_ReemplazaElDocumentoDelMismoTipo()
        {
            await _processor.SubirAsync(Archivo("pagare1.pdf", Pdf), TipoDocumento.Pagare, 1, 10, null);
            await _processor.SubirAsync(Archivo("pagare2.png", Png), TipoDocumento.Pagare, 1, 10, null);

            var doc = Assert.Single(_repo.Documentos);
            Assert.Equal("pagare2.png", doc.NombreOriginal);
            Assert.Single(_storage.Archivos);
        }

        [Fact]
        public async Task Subir_ComprobanteNoSeLigaAPrestamo()
        {
            await _processor.SubirAsync(Archivo("comp.png", Png), TipoDocumento.ComprobanteDomicilio, 1, 10, null);

            Assert.Null(Assert.Single(_repo.Documentos).PrestamoId);
        }

        [Fact]
        public async Task Subir_IneSinPrestamoEsRechazada()
        {
            Assert.False((await _processor.SubirAsync(Archivo("ine.pdf", Pdf), TipoDocumento.Ine, 1, null, null)).Exito);
        }
    }
}
