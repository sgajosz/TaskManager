using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    [Table("technologies")]
    public class Technology
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int ProjectFK { get; set; }

        public float Price { get; set; }

        public Technology() { }

        public Technology(int id, string name, int projectFK, float price)
        {
            this.ID = id;
            this.Name = name;
            this.ProjectFK = projectFK;
            this.Price = price;
        }

        public Technology(string name, int projectFK, float price)
        {
            this.Name = name;
            this.ProjectFK = projectFK;
            this.Price = price;
        }
    }
}