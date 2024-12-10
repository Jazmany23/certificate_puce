using certificated_unemi.Models.users;

namespace certificated_unemi.Models.role
{
    public class Role
    {
            public int Id { get; set; }
            public string Name { get; set; } 

            public ICollection<User> Users { get; set; } 
       
    }
}
