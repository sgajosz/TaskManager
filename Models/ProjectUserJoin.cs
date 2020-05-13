using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    [Table("project_user_join")]
    public class ProjectUserJoin
    {
        public int ID { get; set; }
        public int ProjectFK { get; set; }
        public int UserFK { get; set; }

        public ProjectUserJoin() { }

        public ProjectUserJoin(int projectFK, int userFK)
        {
            this.ProjectFK = projectFK;
            this.UserFK = userFK;
        }
    }
}