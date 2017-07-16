using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIndowsAuthCommon.Models;

namespace WIndowsAuthCommon
{
    public class AuthContext : IdentityDbContext<CustomUser>
    {
        public AuthContext(DbConnection connection)
        : base(connection, true)
        {
        }

        /// <summary>
        /// constructor load context based on connectionstring in app.config
        /// </summary>
        public AuthContext() : base("name=WindowsAuthSQL")
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(new CreateDatabaseIfNotExists<AuthContext>());
        }

        private const int STRINGCOLUMNLENGTH = 400;
        private const int DESCRIPTIONCOLUMNNLENGTH = 4000;
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //set all datetime fields to use datetime2 sql type (best practice)
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            //set default max length for string types
            modelBuilder.Properties<String>().Configure(c => c.HasMaxLength(STRINGCOLUMNLENGTH));
            //set nvarcharmax for description fields
            modelBuilder.Properties<String>().Where(p => p.Name.EndsWith("Description",System.StringComparison.OrdinalIgnoreCase)).Configure(c => c.HasMaxLength(DESCRIPTIONCOLUMNNLENGTH));

            //otherwise will get errors like:EntityType 'IdentityUserRole' has no key defined. Define the key for this EntityType.
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }
    }
}
