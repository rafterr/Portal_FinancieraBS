namespace DataInterfase
{
    // Dónde viven físicamente los archivos (disco del hosting, BD, nube...).
    // Cambiar de almacenamiento solo requiere otra implementación.
    public interface IDocumentoStorage
    {
        Task<string> GuardarAsync(Stream contenido, string carpeta, string extension);
        Task<Stream?> AbrirAsync(string ruta);
        Task EliminarAsync(string ruta);
    }
}
