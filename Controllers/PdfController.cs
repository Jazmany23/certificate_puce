using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace certificated_unemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        [HttpPost("convert-to-images")]
        public IActionResult ConvertPdfToImages([FromForm] IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return BadRequest("El archivo PDF es requerido.");
            }

            try
            {
                // Guardar el archivo PDF temporalmente
                var pdfPath = Path.Combine(Path.GetTempPath(), pdfFile.FileName);
                using (var stream = new FileStream(pdfPath, FileMode.Create))
                {
                    pdfFile.CopyTo(stream);
                }

                // Convertir PDF a imágenes
                var images = ConvertPdfToImages(pdfPath);

                // Devolver las imágenes en base64
                var imageBase64List = new List<string>();
                foreach (var image in images)
                {
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                        imageBase64List.Add(Convert.ToBase64String(ms.ToArray()));
                    }
                    image.Dispose(); // Liberar recursos
                }

                // Eliminar el archivo PDF temporal
                System.IO.File.Delete(pdfPath);

                return Ok(imageBase64List);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar el PDF: {ex.Message}");
            }
        }

        private List<Image> ConvertPdfToImages(string pdfPath)
        {
            var images = new List<Image>();
            using (var pdfDocument = PdfiumViewer.PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    var image = pdfDocument.Render(i, 300, 300, true);
                    images.Add(image);
                }
            }
            return images;
        }
    }
}
