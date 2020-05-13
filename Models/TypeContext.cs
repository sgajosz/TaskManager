using System.Data.Entity;

namespace TaskManager.Models
{
    public class TypeContext : DbContext
    {
        public DbSet<Type> Types { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.TypeViewModel> TypeViewModels { get; set; }
    }
}