//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiRestCarritoPi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class detalle_preventa
    {
        public string dep_cod_dte_preventa { get; set; }
        public string pre_cod_preventa { get; set; }
        public string pro_correlativo { get; set; }
        public Nullable<int> dep_cantidad_venta { get; set; }
        public Nullable<decimal> dep_precio_venta { get; set; }
        public Nullable<decimal> dep_descuento_prod { get; set; }
    
        public virtual preventa preventa { get; set; }
        public virtual producto producto { get; set; }
    }
}