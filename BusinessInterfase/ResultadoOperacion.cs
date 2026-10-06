namespace BusinessInterfase
{
    public record ResultadoOperacion(bool Exito, string? Error = null)
    {
        public static ResultadoOperacion Ok() => new(true);
        public static ResultadoOperacion Falla(string error) => new(false, error);
    }
}
