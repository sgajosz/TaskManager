using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class ValuationRow
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Technology { get; set; }
        public string Errand { get; set; }
        public string Type { get; set; }
        public int Hours { get; set; }
        public int DoneHours { get; set; }
        public string Status { get; set; }
        public float OverallPrice { get; set; }

        public ValuationRow(string name, string username, string technology, string errand, string type, int hours, int doneHours, string status, float overallPrice)
        {
            this.Name = name;
            this.Username = username;
            this.Technology = technology;
            this.Errand = errand;
            this.Type = type;
            this.Hours = hours;
            this.DoneHours = doneHours;
            this.Status = status;
            this.OverallPrice = overallPrice;
        }
    }
}