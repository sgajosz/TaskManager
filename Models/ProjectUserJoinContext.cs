using System.Data.Entity;

namespace TaskManager.Models
{
    public class ProjectUserJoinContext : DbContext
    {
        public DbSet<Project> ProjectUserJoins { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.ProjectUserJoinViewModel> ProjectUserJoinViewModels { get; set; }
    }
}