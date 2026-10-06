using DataInterfase;

namespace DataLayer
{
    // Guarda los archivos en una carpeta privada del servidor (fuera de wwwroot),
    // por lo que solo se pueden obtener a través de la aplicación.
    public class LocalDocumentoStorage : IDocumentoStorage
    {
        private readonly string _raiz;

        public LocalDocumentoStorage(string raiz)
        {
            _raiz = Path.GetFullPath(raiz);
            Directory.CreateDirectory(_raiz);
        }

        public async Task<string> GuardarAsync(Stream contenido, string carpeta, string extension)
        {
            var ruta = $"{carpeta}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";
            var destino = RutaFisica(ruta);
            Directory.CreateDirectory(Path.GetDirectoryName(destino)!);

            await using var archivo = new FileStream(destino, FileMode.CreateNew, FileAccess.Write);
            await contenido.CopyToAsync(archivo);
            return ruta;
        }

        public Task<Stream?> AbrirAsync(string ruta)
        {
            var origen = RutaFisica(ruta);
            Stream? stream = File.Exists(origen) ? new FileStream(origen, FileMode.Open, FileAccess.Read, FileShare.Read) : null;
            return Task.FromResult(stream);
        }

        public Task EliminarAsync(string ruta)
        {
            var origen = RutaFisica(ruta);
            if (File.Exists(origen)) File.Delete(origen);
            return Task.CompletedTask;
        }

        // Impide que una ruta manipulada salga de la carpeta raíz
        private string RutaFisica(string ruta)
        {
            var completa = Path.GetFullPath(Path.Combine(_raiz, ruta));
            if (!completa.StartsWith(_raiz + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                throw new InvalidOperationException("Ruta de documento no válida.");
            return completa;
        }
    }
}
