using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class ErrandViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}