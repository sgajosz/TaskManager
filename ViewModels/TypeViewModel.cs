using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class TypeViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}