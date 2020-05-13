using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class TechnologyViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^\d+.?\d{0,2}$", ErrorMessage = "This value has to be in 0,00 format")]
        [Display(Name = "Price per hour")]
        public float Price { get; set; }
    }
}