using certificated_unemi.Models.certificate;
using certificated_unemi.Models.socialauth;

namespace certificated_unemi.Models.users
{
    public class User
    {
        public int Id { get; set; }
        public string IdentificationNumber { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int RoleId { get; set; }
        public ICollection<Certificate> Certificates { get; set; } // Relación 1:N con Certificate

        //public Role Role { get; set; } 
        //public ICollection<Certificate> Certificates { get; set; } 
        public ICollection<SocialAuth> SocialAuths { get; set; } // 1:N relationship with S
    }
}
