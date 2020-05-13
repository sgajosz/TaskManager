using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class TaskContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }

        public System.Data.Entity.DbSet<TaskManager.ViewModels.TaskViewModel> TaskViewModels { get; set; }
    }
}