using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class TaskViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Technology { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Errand { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Task")]
        public string Type { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public int Hours { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Done hours")]
        public int DoneHours { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Status { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public TaskViewModel() { }

        public TaskViewModel(string name, string technology, string errand, string type, int hours, string status, string description)
        {
            this.Name = name;
            this.Technology = technology;
            this.Errand = errand;
            this.Type = type;
            this.Hours = hours;
            this.Status = status;
            this.Description = description;
        }

        public TaskViewModel(string user, string name, string technology, string errand, string type, int hours, string status, string description)
        {
            this.User = user;
            this.Name = name;
            this.Technology = technology;
            this.Errand = errand;
            this.Type = type;
            this.Hours = hours;
            this.Status = status;
            this.Description = description;
        }
    }
}