﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db_preventa_superEntities : DbContext
    {
        public db_preventa_superEntities()
            : base("name=db_preventa_superEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<carrito> carrito { get; set; }
        public virtual DbSet<cliente> cliente { get; set; }
        public virtual DbSet<detalle_preventa> detalle_preventa { get; set; }
        public virtual DbSet<nps> nps { get; set; }
        public virtual DbSet<preventa> preventa { get; set; }
        public virtual DbSet<producto> producto { get; set; }
    }
}
