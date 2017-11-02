using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using Apo_ChanService.DataObjects;

namespace Apo_ChanService.Models
{
    public class Apo_ChanContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string connectionStringName = "Name=MS_TableConnectionString";

        public Apo_ChanContext() : base(connectionStringName)
        {
        }

        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<ReportItem> ReportItems { get; set; }
        public DbSet<GroupItem> GroupItems { get; set; }
        public DbSet<GroupUserItem> GroupUserItems { get; set; }
        public DbSet<ReportGroupItem> ReportGroupItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));

            // Add
            modelBuilder.Conventions.Add(new DecimalPrecisionAttributeConvention());
        }
    }

}
