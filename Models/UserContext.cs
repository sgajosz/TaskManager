using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        //public System.Data.Entity.DbSet<TaskManager.ViewModels.RegistrationViewModel> RegistrationViewModels { get; set; }
    }
}