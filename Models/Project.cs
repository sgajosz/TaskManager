using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace TaskManager.Models
{
    [Table("projects")]
    public class Project
    {
        public int ID { get; set; }
        public int UserFK { get; set; }
        public string Name { get; set; }
        public DateTime Creation { get; set; }

        public Project() { }

        public Project(int id, int userFK, string name, DateTime creation)
        {
            this.ID = id;
            this.UserFK = userFK;
            this.Name = name;
            this.Creation = creation;
        }

        public Project(string name, DateTime creation)
        {
            this.Name = name;
            this.Creation = creation;
        }
    }
}