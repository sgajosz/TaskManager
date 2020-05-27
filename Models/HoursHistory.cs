using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class HoursHistory
    {
        public int ID { get; set; }
        public int UserFK { get; set; }
        public int TaskFK { get; set; }
        public DateTime EditedAt { get; set; }
        public int Hours { get; set; }

        public HoursHistory(int iD, int userFK, int taskFK, DateTime editedAt, int hours)
        {
            this.ID = iD;
            this.UserFK = userFK;
            this.TaskFK = taskFK;
            this.EditedAt = editedAt;
            this.Hours = hours;
        }
    }
}