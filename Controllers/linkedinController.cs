using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace certificated_unemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class linkedinController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public linkedinController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpPost("exchange-token-and-get-profile")]
        public async Task<IActionResult> ExchangeTokenAndGetProfile([FromBody] TokenRequest request)
        {
            // Datos de configuración de LinkedIn
            const string clientId = "77x5da5gotkniv";
            const string clientSecret = "WPL_AP1.asJfSqn9PQmbQKIk.jw9Z/A==";
            const string redirectUri = "http://localhost:3000/auth-linkedin"; // Asegúrate de usar tu URI configurada en LinkedIn
            const string tokenUrl = "https://www.linkedin.com/oauth/v2/accessToken";
            const string linkedInProfileUrl = "https://api.linkedin.com/v2/userinfo";

            using (var _httpClient = new HttpClient())
            {
                try
                {
                    // Construir el cuerpo de la solicitud para intercambiar el código por un token de acceso
                    var requestBody = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", request.Code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

                    // Realizar la solicitud al endpoint de LinkedIn para obtener el token de acceso
                    var tokenResponse = await _httpClient.PostAsync(tokenUrl, requestBody);

                    if (!tokenResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                        return StatusCode((int)tokenResponse.StatusCode, new
                        {
                            error = "Failed to exchange token",
                            details = errorContent
                        });
                    }

                    var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<LinkedInTokenResponse>(tokenContent);

                    // Configurar el encabezado de autorización con el token de acceso
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

                    // Obtener el perfil del usuario desde LinkedIn
                    var profileResponse = await _httpClient.GetAsync(linkedInProfileUrl);

                    if (!profileResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await profileResponse.Content.ReadAsStringAsync();
                        return StatusCode((int)profileResponse.StatusCode, new
                        {
                            error = "Failed to fetch LinkedIn profile",
                            details = errorContent
                        });
                    }

                    // Leer y deserializar el contenido del perfil del usuario
                    var profileContent = await profileResponse.Content.ReadAsStringAsync();
                    var profileData = JsonConvert.DeserializeObject<LinkedInProfile>(profileContent);

                    // Retornar el perfil del usuario
                    return Ok(new
                    {
                        accessToken = tokenData.AccessToken,
                        idToken = tokenData.AccessToken, // Si necesitas manejar el ID Token
                        profile = profileData
                    });
                }
                catch (Exception ex)
                {
                    // Manejar errores y retornar una respuesta adecuada
                    return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
                }
            }
        }

        [HttpPost("get-user-data")]
        public async Task<IActionResult> GetUserData([FromBody] TokenRequest request)
        {
            var linkedInProfileUrl = "https://api.linkedin.com/v2/userinfo";

            using (var _httpClient = new HttpClient())
            {
                // Configura el encabezado de autorización
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Code);

                var profileResponse = await _httpClient.GetAsync(linkedInProfileUrl);

                if (!profileResponse.IsSuccessStatusCode)
                {
                    var errorContent = await profileResponse.Content.ReadAsStringAsync();
                    return StatusCode(400, new { error = "Failed to fetch LinkedIn profile", details = errorContent });
                }

                var profileContent = await profileResponse.Content.ReadAsStringAsync();
                // Procesa el contenido del perfil según tus necesidades
                return Ok(profileContent);
            }
        }

        [HttpPost("share-text")]
        public async Task<IActionResult> ShareTextPost([FromBody] ShareTextRequest request)
        {
            var url = "https://api.linkedin.com/v2/ugcPosts";

            // Cuerpo de la solicitud
            var body = new
            {
                author = $"urn:li:person:{request.PersonId}",
                lifecycleState = "PUBLISHED",
                specificContent = new Dictionary<string, object>
            {
                {
                "com.linkedin.ugc.ShareContent", new
                    {
                    shareCommentary = new
                    {
                        text = request.Message
                    },
                    shareMediaCategory = "NONE"
                    }
                }   
            },
                visibility = new Dictionary<string, string>
                {
            { "com.linkedin.ugc.MemberNetworkVisibility", "PUBLIC" }
                }
            };

            using (var httpClient = new HttpClient())
            {
                // Configurar encabezados
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);
                httpClient.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

                // Serializar el cuerpo a JSON
                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Realizar la solicitud POST
                var response = await httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "Failed to share post on LinkedIn",
                        details = errorContent
                    });
                }

                return Ok(await response.Content.ReadAsStringAsync());
            }
        }

        [HttpPost("revoke-user")]
        public async Task<IActionResult> RevokeUser([FromBody] TokenRequest request)
        {
            string clientId = "77x5da5gotkniv";
            string clientSecret = "WPL_AP1.asJfSqn9PQmbQKIk.jw9Z/A==";
            string RevokeTokenEndpoint = "https://www.linkedin.com/oauth/v2/revoke";

            using var httpClient = new HttpClient();
            try
            {
                // Preparar el contenido de la solicitud
                var content = new StringContent(
                    $"client_id={clientId}&client_secret={clientSecret}&token={request.Code}",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded"
                );

                // Enviar la solicitud POST
                var response = await httpClient.PostAsync(RevokeTokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Token revocado exitosamente.");
                }

                return StatusCode((int)response.StatusCode, "No se pudo revocar el token.");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error al conectarse al servidor de LinkedIn: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }
        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromBody] LinkedInPost post)
        {
            try
            {
                var accessToken = post.Token;

                using (var httpClient = new HttpClient())
                {
              
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", post.Token);
                    httpClient.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

                    // Paso 1: Registrar la imagen
                    var registerUploadContent = new
                    {
                        registerUploadRequest = new
                        {
                            recipes = new string[] { "urn:li:digitalmediaRecipe:feedshare-image" },
                            owner = $"urn:li:person:{post.PersonUrn}",
                                            serviceRelationships = new[]
                                            {
                            new
                            {
                                relationshipType = "OWNER",
                                identifier = "urn:li:userGeneratedContent"
                            }
                        }
                        }
                    };

                    var uploadUrlResponse = await httpClient.PostAsync(
                        "https://api.linkedin.com/v2/assets?action=registerUpload",
                        new StringContent(JsonConvert.SerializeObject(registerUploadContent), Encoding.UTF8, "application/json")
                    );

                    if (!uploadUrlResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await uploadUrlResponse.Content.ReadAsStringAsync();
                        return BadRequest($"Error al registrar la imagen: {errorContent}");
                    }

                    var uploadUrlResponseContent = await uploadUrlResponse.Content.ReadAsStringAsync();
                    dynamic uploadUrlData = JsonConvert.DeserializeObject(uploadUrlResponseContent);
                    string uploadUrl = uploadUrlData.value.uploadMechanism["com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest"].uploadUrl;
                    string assetId = uploadUrlData.value.asset;

                    // Paso 2: Descargar la imagen desde la URL proporcionada (post.ImageUrl)
                    byte[] imageBytes;
                    using (var client = new HttpClient())
                    {
                        imageBytes = await client.GetByteArrayAsync(post.ImageUrl); // Descarga los bytes de la imagen
                    }

                    // Paso 3: Subir la imagen al servidor de LinkedIn
                    using (var content = new ByteArrayContent(imageBytes))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // Especificar el tipo de contenido
                        var imageResponse = await httpClient.PutAsync(uploadUrl, content);

                        if (!imageResponse.IsSuccessStatusCode)
                        {
                            var errorContent = await imageResponse.Content.ReadAsStringAsync();
                            return BadRequest($"Error al subir la imagen: {errorContent}");
                        }
                    }


                  

                    var postContent = new
                    {
                        author = $"urn:li:person:{post.PersonUrn}",
                        lifecycleState = "PUBLISHED",
                                            specificContent = new Dictionary<string, object>
                        {
                            {
                                "com.linkedin.ugc.ShareContent", new
                                {
                                    shareCommentary = new { text = post.Text },
                                    shareMediaCategory = "IMAGE",
                                    media = new[]
                                    {
                                        new
                                        {
                                            media = assetId,
                                            status = "READY",
                                            description = new { text = "Descripción de la imagen" },
                                            title = new { text = "Título de la imagen" }
                                        }
                                    }
                                }
                            }
                        },
                                            visibility = new Dictionary<string, string>
                        {
                            { "com.linkedin.ugc.MemberNetworkVisibility", "PUBLIC" }
                        }
                     };


                    var jsonBody = JsonConvert.SerializeObject(postContent);

                    var postResponse = await httpClient.PostAsync(
                        "https://api.linkedin.com/v2/ugcPosts",
                        new StringContent(jsonBody, Encoding.UTF8, "application/json")
                    );

                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await postResponse.Content.ReadAsStringAsync();
                        return BadRequest($"Error al crear la publicación: {errorContent}");
                    }

                    var postResponseContent = await postResponse.Content.ReadAsStringAsync();
                    dynamic postData = JsonConvert.DeserializeObject(postResponseContent); // Deserializa a un objeto dinámico
                    string id = postData.id; // Extrae el campo 'id'

                    // Divide el ID y toma el último segmento
                    string shareId = id.Split(':')[^1]; // Usa la sintaxis ^1 para tomar el último elemento

                    // Construir el enlace a la publicación

                    // Construir el enlace a la publicación
                    string postUrl = $"https://www.linkedin.com/feed/update/urn:li:share:{shareId}";

                    return Ok(new
                    {
                        Message = "Publicación creada con éxito",
                        Link = postUrl
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al publicar en LinkedIn: {ex.Message}");
            }
        }


    

        public class LinkedInPost
        {
            public string PersonUrn { get; set; }
            public string Text { get; set; }
            public string ImageUrl { get; set; }
            public string Token { get; set; }
        }

        // Clase para recibir el token del cliente
        public class TokenRequest
        {
            public string Code { get; set; }
        }

        public class LinkedInTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
        public class LinkedInProfile
        {
            public string Sub { get; set; }
            public bool EmailVerified { get; set; }
            public string Name { get; set; }
            public Locale Locale { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
            public string Email { get; set; }
            public string Picture { get; set; }
        }

        public class Locale
        {
            public string Country { get; set; }
            public string Language { get; set; }
        }
        public class ShareTextRequest
        {
            public string AccessToken { get; set; } // Token de acceso del usuario
            public string PersonId { get; set; }   // URN del usuario ("urn:li:person:<ID>")
            public string Message { get; set; }    // Mensaje de texto para compartir
        }


    }
}
