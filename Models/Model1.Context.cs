﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace artevol.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class artevolEntities : DbContext
    {
        public artevolEntities()
            : base("name=artevolEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<art_type> art_type { get; set; }
        public virtual DbSet<image> images { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<reqType> reqTypes { get; set; }
        public virtual DbSet<request> requests { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<sale_type> sale_type { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<bid> bids { get; set; }
        public virtual DbSet<order_prd> order_prd { get; set; }
        public virtual DbSet<order> orders { get; set; }
    }
}
