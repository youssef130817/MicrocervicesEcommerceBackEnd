using Microsoft.AspNetCore.Mvc;

namespace Product.API.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ImagesController(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    [HttpGet("{*imagePath}")]
    public IActionResult GetImage(string imagePath)
    {
        try
        {
            // Sécuriser le chemin pour éviter la traversée de répertoire
            var normalizedPath = imagePath?.Replace('/', Path.DirectorySeparatorChar);
            if (string.IsNullOrEmpty(normalizedPath))
            {
                return BadRequest("Chemin d'image non valide");
            }

            // Construire le chemin physique complet
            var imageFulPath = Path.Combine(_environment.WebRootPath, normalizedPath);

            // Vérifier si le fichier existe
            if (!System.IO.File.Exists(imageFulPath))
            {
                return NotFound("Image non trouvée");
            }

            // Déterminer le type MIME
            var contentType = GetContentType(imageFulPath);

            // Lire et retourner le fichier
            var imageBytes = System.IO.File.ReadAllBytes(imageFulPath);
            return File(imageBytes, contentType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur lors de la récupération de l'image: {ex.Message}");
        }
    }

    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}