using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    [Table("tasks")]
    public class Task
    {
        public int ID { get; set; }
        public int ProjecFK { get; set; }
        public int UserFK { get; set; }
        public string Name { get; set; }
        public string Technology { get; set; }
        public string Errand { get; set; }
        public string Type { get; set; }
        public int Hours { get; set; }
        public int DoneHours { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public Task() { }

        public Task(string name, string technology, string errand, string type, int hours, string status, string description)
        {
            this.Name = name;
            this.Technology = technology;
            this.Errand = errand;
            this.Type = type;
            this.Hours = hours;
            this.Status = status;
            this.Description = description;
        }

        public Task(int userFK, string name, string technology, string errand, string type, int hours, int doneHours, string status, string description)
        {
            this.UserFK = userFK;
            this.Name = name;
            this.Technology = technology;
            this.Errand = errand;
            this.Type = type;
            this.Hours = hours;
            this.DoneHours = doneHours;
            this.Status = status;
            this.Description = description;
        }

        public Task(int id, int projectFK, int userFK, string name, string technology, string errand, string type, int hours, int doneHours, string status, string description)
        {
            this.ID = id;
            this.ProjecFK = projectFK;
            this.UserFK = userFK;
            this.Name = name;
            this.Technology = technology;
            this.Errand = errand;
            this.Type = type;
            this.Hours = hours;
            this.DoneHours = doneHours;
            this.Status = status;
            this.Description = description;
        }
    }
}