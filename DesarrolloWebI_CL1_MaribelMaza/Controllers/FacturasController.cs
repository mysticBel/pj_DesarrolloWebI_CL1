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


        // PREGUNTA 3
        // Traemos la lista de Categoria de Clientes
        IEnumerable<Categoria_Cliente> GetCategoriasCliente()
        {
            List<Categoria_Cliente> categoriasCliente = new List<Categoria_Cliente>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                SqlCommand command = new SqlCommand("sp_GetCategoriasCliente", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    categoriasCliente.Add(new Categoria_Cliente()
                    {
                        idCategoriaCliente = dr.GetInt32(0),
                        nombreCategoria = dr.GetString(1)
                    });
                }

            }

            return categoriasCliente;
        }

        public async Task<IActionResult> InsertCliente()
        {
            ViewBag.categoriasCliente = new SelectList(
                    await Task.Run(() => GetCategoriasCliente()),
                    "idCategoriaCliente",
                    "nombreCategoria"
                );

            return View(new Cliente());
        }

        [HttpPost]
        public async Task<IActionResult> InsertCliente(Cliente cliente)
        {

            ViewBag.categoriasCliente = new SelectList(
                await Task.Run(() => GetCategoriasCliente()),
                "idCategoriaCliente",
                "nombreCategoria",
                cliente.idCategoriaCliente
            );


            //REcibiremos un objeto cliente, y preguntamos si no es valido, para registrarlo
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            //Boton Guardar
            using (SqlConnection connection = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand command = new SqlCommand("sp_InsertCliente", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    //agregamos los parametros del sp
                    command.Parameters.AddWithValue("@prmstrRazonSocial", cliente.razonSocial); 
                    command.Parameters.AddWithValue("@prmstrDireccion", cliente.direccion);
                    command.Parameters.AddWithValue("@prmstrTelefono", cliente.telefono);
                    command.Parameters.AddWithValue("@prmintIdCategoria", cliente.idCategoriaCliente);

                    connection.Open();
                  
                    int filasAfectadas = command.ExecuteNonQuery();
                    ViewBag.mensaje = $"Se ha insertado {filasAfectadas} cliente.";


                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = ex.Message;
                }

            }

            return View(cliente);
        }


        // vista de Cliente
        IEnumerable<Cliente_Join> GetClientesPorRazonSocial(string razonSocial)
        {
            List<Cliente_Join> clientes = new List<Cliente_Join>();

            using (SqlConnection connection = new SqlConnection(this.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_GetClientesPorRazonSocial", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@prmstrRazonSocial", razonSocial);
 

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    clientes.Add(new Cliente_Join()
                    {
                        idCliente = dr.GetInt32(0),
                        razonSocial = dr.GetString(1),
                        direccion = dr.GetString(2),
                        telefono = dr.GetString(3),
                        nombreCategoriaCliente = dr.GetString(4)
                    });
                }
            }
            return clientes;
        }

        // su ActionRESULT  + PAGINACION
        public async Task<IActionResult> FiltroClientes(string razonSocial = "", int pagina = 0)
        {
            //validacion por si no se envia nada
            if (razonSocial == null)
                razonSocial = "";

            IEnumerable<Cliente_Join> clientes = GetClientesPorRazonSocial(razonSocial);
            int filasPagina = 5;  // indico que tome 5 items por pagina
            int totalFilas = clientes.Count();
            int numeroPaginas = totalFilas % filasPagina == 0 ? totalFilas / filasPagina : (totalFilas / filasPagina) + 1;

            ViewBag.pagina = pagina;
            ViewBag.numeroPaginas = numeroPaginas;
            ViewBag.razonSocial = razonSocial;

            return View(await Task.Run(() => clientes.Skip(pagina * filasPagina).Take(filasPagina)));

        }
    }
}
