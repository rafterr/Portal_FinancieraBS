namespace DataLayer
{
    internal static class Busqueda
    {
        // "juan perez 55" -> ["juan", "perez", "55"]; cada término debe coincidir con algún campo
        public static string[] Terminos(string? texto) =>
            string.IsNullOrWhiteSpace(texto)
                ? Array.Empty<string>()
                : texto.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // "#12" o "12" -> 12
        public static int? ComoId(string termino) =>
            int.TryParse(termino.TrimStart('#'), out var id) ? id : null;
    }
}
