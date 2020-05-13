using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class ProjectUserJoinViewModel
    {
        [Required]
        [Display(Name = "User")]
        public string UserString { get; set; }

        public ProjectUserJoinViewModel() { }

        public ProjectUserJoinViewModel(string userString)
        {
            this.UserString = userString;
        }
    }
}