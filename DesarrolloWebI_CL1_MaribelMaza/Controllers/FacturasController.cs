using Microsoft.AspNetCore.Mvc;

//para la cadena de conexion
using Microsoft.Extensions.Configuration;
//
using Microsoft.Data.SqlClient;
using DesarrolloWebI_CL1_MaribelMaza.Models;
//
using System.Data; //para los sp
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesarrolloWebI_CL1_MaribelMaza.Controllers
{
    public class FacturasController : Controller
    {

        public readonly IConfiguration _config;
        public string cadena;

        public FacturasController(IConfiguration _config)
        {
            this._config = _config;  
            this.cadena = _config["ConnectionStrings:connection"]; 
        }


        /*public IActionResult Index()
        {
            return View();
        }*/

        // PREGUNTA 1
        IEnumerable<Factura> GetFacturasPorProducto(string nombreProducto)
        {
            List<Factura> facturas = new List<Factura>();

            using (SqlConnection connection = new SqlConnection(this.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_GetFacturasPorProducto", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@prmstrNombreProducto", nombreProducto);

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    facturas.Add(new Factura()
                    {
                        idFactura = dr.GetInt32(0),
                        fechaEmision = dr.GetDateTime(1),
                        nombreProducto = dr.GetString(2),
                        precioUnitario = dr.GetDecimal(3),
                        cantidad = dr.GetDecimal(4),
                        monto= dr.GetDecimal(5),
                    });
                }
                connection.Close();
            }
            return facturas;
        }

        public async Task<IActionResult> GetFacturasPorProducto(string nombreProducto = "", int pagina = 0)
        {
            //validacion por si no se envia nada
            if (nombreProducto == null)
                nombreProducto = "";

            IEnumerable<Factura> facturas = GetFacturasPorProducto(nombreProducto);
            int filasPagina = 5;  // indico que tome 5 items por pagina
            int totalFilas = facturas.Count();
            int numeroPaginas = totalFilas % filasPagina == 0 ? totalFilas / filasPagina : (totalFilas / filasPagina) + 1;

            ViewBag.pagina = pagina;
            ViewBag.numeroPaginas = numeroPaginas;
            ViewBag.nombreProducto = nombreProducto;

            return View(await Task.Run(() => facturas.Skip(pagina * filasPagina).Take(filasPagina)));

        }


        // PREGUNTA 2
        IEnumerable<Factura> GetFacturasPorAnioCliente(int anio, string razonSocial)
        {
            List<Factura> facturasAxC = new List<Factura>();

            using (SqlConnection connection = new SqlConnection(this.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_GetFacturasPorAnioCliente", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@prmintAnio", anio);
                cmd.Parameters.AddWithValue("@prmstrRazonSocial", razonSocial);

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    facturasAxC.Add(new Factura()
                    {
                        idFactura = dr.GetInt32(0),
                        fechaEmision = dr.GetDateTime(1),
                        razonSocial = dr.GetString(2),
                        total = dr.GetDecimal(3),
                        
                    });
                }
                connection.Close();
            }
            return facturasAxC;
        }

        public async Task<IActionResult> GetFacturasPorAnioCliente(int anio = 0, string razonSocial = "", int pagina = 0)
        {
            //validacion por si no se envia nada
            if (razonSocial == null)
            {
                razonSocial = "";

            }
                
            IEnumerable<Factura> facturasAxC = GetFacturasPorAnioCliente(anio, razonSocial);
            int filasPagina = 5;  
            int totalFilas = facturasAxC.Count();
            int numeroPaginas = totalFilas % filasPagina == 0 ? totalFilas / filasPagina : (totalFilas / filasPagina) + 1;

            ViewBag.pagina = pagina;
            ViewBag.numeroPaginas = numeroPaginas;
            ViewBag.anio = anio;
            ViewBag.razonSocial = razonSocial;

            return View(await Task.Run(() => facturasAxC.Skip(pagina * filasPagina).Take(filasPagina)));

        }




        

    }
}
