using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HR_System.Web.Data
{
    public class HR_SystemWebContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public HR_SystemWebContext() : base("name=HR_SystemWebContext")
        {
        }

        public DbSet<HR_System.Data.Company> Companies { get; set; }

        public System.Data.Entity.DbSet<HR_System.Data.Region> Regions { get; set; }

        public System.Data.Entity.DbSet<HR_System.Data.Department> Departments { get; set; }

        public System.Data.Entity.DbSet<HR_System.Data.Person> People { get; set; }

        public System.Data.Entity.DbSet<HR_System.Data.Status> Status { get; set; }
    }
}
