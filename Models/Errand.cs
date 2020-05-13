using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    [Table("errands")]
    public class Errand
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int ProjectFK { get; set; }

        public Errand() { }

        public Errand(int id, string name, int projectFK)
        {
            this.ID = id;
            this.Name = name;
            this.ProjectFK = projectFK;
        }

        public Errand(string name, int projectFK)
        {
            this.Name = name;
            this.ProjectFK = projectFK;
        }
    }
}