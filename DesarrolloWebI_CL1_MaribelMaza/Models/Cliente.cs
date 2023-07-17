namespace DesarrolloWebI_CL1_MaribelMaza.Models
{
    public class Cliente

    {
        public int idCliente{ get; set; }
        public string? razonSocial { get; set; }
        public string? direccion { get; set; }
        public string? telefono { get; set; }
        public int idCategoriaCliente { get; set; }
    }
}
