using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiRestCarritoPi.Models
{
    public class ResponseProductos
    {
        public int CodRes { get; set; }
        public string msgRes { get; set; }
        public string GuidVenta { get; set; }
        public List<Producto> lstProductos { get; set; }
    }

    public class Producto
    {
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadCompra { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal DescuentoProducto { get; set; }
        public string ImagenProducto { get; set; }
    }
}