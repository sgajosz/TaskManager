using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class ProjectContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.ProjectViewModel> ProjectViewModels { get; set; }
    }
}