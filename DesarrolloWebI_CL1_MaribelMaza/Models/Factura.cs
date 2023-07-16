using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DesarrolloWebI_CL1_MaribelMaza.Models
{
    public class Factura
    {
        [Display(Name = "ID Factura")]
        public int idFactura{ get; set; }

        [Display(Name = "Fecha de Emision")]
        public DateTime fechaEmision { get; set; }

        [Display(Name = "Nombre")]
        public string? nombreProducto { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal precioUnitario { get; set; }

        [Display(Name = "Cantidad")]
        public decimal cantidad { get; set; }

        [Display(Name = "Monto")]
        public decimal monto { get; set; }
    }
}
