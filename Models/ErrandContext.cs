using System.Data.Entity;

namespace TaskManager.Models
{
    public class ErrandContext : DbContext
    {
        public DbSet<Errand> Errands { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.ErrandViewModel> ErrandViewModels { get; set; }
    }
}