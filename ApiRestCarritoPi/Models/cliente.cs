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
    
    public partial class cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cliente()
        {
            this.preventa = new HashSet<preventa>();
        }
    
        public string cli_nit_cliente { get; set; }
        public string cli_nombre_cliente { get; set; }
        public string cli_direccion_cliente { get; set; }
        public Nullable<System.DateTime> cli_fecha_crea { get; set; }
        public Nullable<System.DateTime> cli_fecha_mod { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<preventa> preventa { get; set; }
    }
}