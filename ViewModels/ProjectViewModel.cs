using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class ProjectViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        public ProjectViewModel() { }

        public ProjectViewModel(string name)
        {
            this.Name = name;
        }
    }
}