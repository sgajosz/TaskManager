using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public Project(string name, DateTime creation)
        {
            this.Name = name;
            this.Creation = creation;
        }
    }
}