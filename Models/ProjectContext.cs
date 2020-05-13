using System.Data.Entity;

namespace TaskManager.Models
{
    public class ProjectContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.ProjectViewModel> ProjectViewModels { get; set; }
    }
}