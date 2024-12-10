using certificated_unemi.Models.users;

namespace certificated_unemi.Models.socialauth
{
    public class SocialAuth
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Clave foránea
        public string Provider { get; set; } // Ej: "LinkedIn", "Google", "Facebook"
        public string AccessToken { get; set; }
        public string ProviderId { get; set; } // ID único del usuario en el proveedor
        public string RefreshToken { get; set; } // (opcional) Token para renovar el acceso
        public DateTime? ExpiryDate { get; set; } // (opcional) Fecha de expiración del token

        // Relación con User
        public User User { get; set; }
    }
}
