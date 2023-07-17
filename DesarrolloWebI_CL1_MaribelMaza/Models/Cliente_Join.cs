using System.ComponentModel.DataAnnotations;

namespace DesarrolloWebI_CL1_MaribelMaza.Models
{
    public class Cliente_Join
    {
        [Display(Name = "ID Cliente")]
        public int idCliente { get; set; }
        [Display(Name = "Razon Social")]
        public string? razonSocial { get; set; }
        [Display(Name = "Direccion")]
        public string? direccion { get; set; }
        [Display(Name = "Telefono")]
        public string? telefono { get; set; }
        [Display(Name = "Categoria")]
        public string? nombreCategoriaCliente { get; set; }
    }
}
