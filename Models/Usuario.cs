using System.ComponentModel.DataAnnotations;

public class Usuario
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Correo { get; set; }

    [Required, DataType(DataType.Password)]
    public string Contraseña { get; set; }
}