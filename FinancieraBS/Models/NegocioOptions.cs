namespace FinancieraBS.Models
{
    // Datos del negocio que aparecen en el portal y en los comprobantes (sección "Negocio" de appsettings)
    public class NegocioOptions
    {
        public string Nombre { get; set; } = "Financiera BS";
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        // Ruta relativa a la carpeta del proyecto web
        public string LogoRuta { get; set; } = "wwwroot/img/logo.svg";
        // Lada internacional para armar el enlace de WhatsApp con teléfonos de 10 dígitos
        public string CodigoPaisWhatsApp { get; set; } = "52";
    }
}

namespace FinancieraBS.Models
{
    public class ComprobanteViewModel
    {
        public BusinessInterfase.ComprobantePago Comprobante { get; set; } = null!;
        public string NombreNegocio { get; set; } = string.Empty;
        public string WhatsAppUrl { get; set; } = string.Empty;
        public string MensajeCompartir { get; set; } = string.Empty;
        public bool Nuevo { get; set; }
    }
}
