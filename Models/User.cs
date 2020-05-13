using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    [Table("users")]
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }

        public User() {}

        public User(int id, string name, string surname)
        {
            this.ID = id;
            this.Name = name;
            this.Surname = surname;
        }

        public User(string email, string name, string surname, string password)
        {
            this.Email = email;
            this.Name = name;
            this.Surname = surname;
            this.Password = password;
        }
    }
}