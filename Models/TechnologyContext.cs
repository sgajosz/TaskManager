using System.Data.Entity;

namespace TaskManager.Models
{
    public class TechnologyContext : DbContext
    {
        public DbSet<Technology> Technologies { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.TechnologyViewModel> TechnologyViewModels { get; set; }
    }
}