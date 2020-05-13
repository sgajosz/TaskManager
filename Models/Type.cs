using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    [Table("types")]
    public class Type
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int ProjectFK { get; set; }

        public Type() { }

        public Type(int id, string name, int projectFK)
        {
            this.ID = id;
            this.Name = name;
            this.ProjectFK = projectFK;
        }

        public Type(string name, int projectFK)
        {
            this.Name = name;
            this.ProjectFK = projectFK;
        }
    }
}