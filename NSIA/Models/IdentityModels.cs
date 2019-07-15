using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace NSIA.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        //public bool IsCoperate { get; set; }

        //[Required]
        //public string CompanyName { get; set; }

        //[Required]
        //public string Surname { get; set; }

        //[Required]
        //public string Firstname { get; set; }

        //[Required]
        //public DateTime RegisterationDate { get; set; }

        //public bool IsDeleted { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<UserClaims> UserClaims { get; set; }

        //public DbSet<Vehicle> Vehicle { get; set; }

        //public DbSet<UserPolicy> UserPolicy { get; set; }

        //public DbSet<VehicleDetail> vehicleDetail { get; set; }

        //public DbSet<Client> Client { get; set; }

        //public DbSet<ClaimsDocument> ClaimsDocument { get; set; }

        //public DbSet<PaymentDetail> PaymentDetail { get; set; }

        //public DbSet<Quote> Quote { get; set; }

        //public DbSet<Renewal> Renewal { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
            //modelBuilder.Entity<Quote>()
            //      .ToTable("Quote");
            //modelBuilder.Entity<UserPolicy>()
            //     .ToTable("UserPolicy");
            //modelBuilder.Entity<Vehicle>()
            //    .ToTable("Vehicle");
            //modelBuilder.Entity<VehicleDetail>()
            //    .ToTable("VehicleDetail");
            //modelBuilder.Entity<Renewal>()
            //    .ToTable("Renewal");
            //modelBuilder.Entity<ClaimsDocument>()
            //    .ToTable("ClaimsDocument");
            //modelBuilder.Entity<Client>()
            //    .ToTable("Client");
            //modelBuilder.Entity<PaymentDetail>()
            //    .ToTable("PaymentDetail");

       // }

    }
}