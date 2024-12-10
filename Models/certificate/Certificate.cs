using certificated_unemi.Models.users;

namespace certificated_unemi.Models.certificate
{
    public class Certificate
    {
        public int Id { get; set; }
        public string Title { get; set; } // Ejemplo: "Certificado de Liderazgo"
        public string PdfLink { get; set; } // Link al PDF
        public double SizeInMb { get; set; } // Tamaño del archivo en MB
        public DateTime UploadDate { get; set; } // Fecha de subida del certificado
        public int UserId { get; set; } // Relación con el usuario

        // Relación con User
        public User User { get; set; }
    }
}
