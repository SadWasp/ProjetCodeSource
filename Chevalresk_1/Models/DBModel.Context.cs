//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chevalresk_1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBEntities : DbContext
    {
        public DBEntities()
            : base("name=DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Arme> Armes { get; set; }
        public virtual DbSet<Armure> Armures { get; set; }
        public virtual DbSet<Inventaire> Inventaires { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Panier> Paniers { get; set; }
        public virtual DbSet<Potion> Potions { get; set; }
        public virtual DbSet<Ressource> Ressources { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
    }
}
