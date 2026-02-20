using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using AILearningRoadmap.API.Models;

namespace AILearningRoadmap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadmapController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RoadmapController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // --- 1. FONKSİYON: ANA YOL HARİTASI ÇIKARMA ---
        // --- 1. FONKSİYON: ANA YOL HARİTASI ÇIKARMA ---
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateRoadmap([FromBody] UserRequest request)
        {
            if (string.IsNullOrEmpty(request.Goal)) return BadRequest("Lütfen yapmak istediğiniz projeyi anlatın.");

            string apiKey = _configuration["GeminiApiKey"];
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            string prompt = $@"
                Sen kıdemli bir yazılım mimarı ve eğitmensin. Kullanıcının şu projesini analiz et: '{request.Goal}'
                Kullanıcının bu alandaki mevcut bilgi seviyesi: '{request.Level}'. 
                Lütfen yol haritasını bu seviyeye kesinlikle uygun olarak hazırla.
                Çıktını SADECE aşağıdaki JSON formatında ver, başka hiçbir kelime veya açıklama yazma:
                {{
                  ""proje_adi"": ""..."",
                  ""seviye"": ""Başlangıç/Orta/İleri"",
                  ""adımlar"": [
                    {{
                      ""sira"": 1,
                      ""konu"": ""..."",
                      ""tahmini_sure"": ""Örn: 2 Hafta veya 15 Saat"", 
                      ""aciklama"": ""..."",
                      ""kaynaklar"": [ {{ ""tip"": ""video veya makale"", ""baslik"": ""..."", ""link"": ""..."" }} ]
                    }}
                  ]
                }}";

            var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };

            // YENİ: Ağ hatalarına karşı sunucuyu çökmekten koruyan Try-Catch bloğu
            try
            {
                using var httpClient = new HttpClient();
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Google'dan dönen hata: {errorDetails}");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                return Ok(responseData);
            }
            catch (HttpRequestException)
            {
                // İnternet anlık koparsa sistem çökmez, düzgün bir hata mesajı döner
                return StatusCode(503, "Google API sunucularına ulaşılamadı. Lütfen internet bağlantınızı kontrol edin.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }

        // --- 2. YENİ FONKSİYON: YAPAY ZEKA ÖZEL EĞİTMEN (DEEP DIVE) ---
        // --- 2. YENİ FONKSİYON: YAPAY ZEKA ÖZEL EĞİTMEN (DEEP DIVE) ---
        [HttpPost("deepdive")]
        public async Task<IActionResult> DeepDive([FromBody] DeepDiveRequest request)
        {
            if (string.IsNullOrEmpty(request.Topic)) return BadRequest("Konu boş olamaz.");

            string apiKey = _configuration["GeminiApiKey"];
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            string prompt = $"Sen sabırlı bir yazılım eğitmenisin. Öğrencin senden şu konuyu detaylıca açıklamanı istiyor: '{request.Topic}'. Lütfen bu konuyu akıcı bir dille, anlaşılır örnekler vererek ve gerekiyorsa kod örnekleriyle (kısa ve öz bir şekilde) açıkla.";

            var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };

            // YENİ: Ağ hatalarına karşı sunucuyu çökmekten koruyan Try-Catch bloğu
            try
            {
                using var httpClient = new HttpClient();
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode) return StatusCode(500, "Yapay zeka yanıt veremedi.");

                var responseData = await response.Content.ReadAsStringAsync();
                return Ok(responseData);
            }
            catch (HttpRequestException)
            {
                // Eğer Google'a ulaşılamazsa (DNS veya internet sorunu) sunucu çökmez, React'e bilgi verir
                return StatusCode(503, "Google API sunucularına ulaşılamadı. Lütfen anlık internet bağlantınızı veya VPN/Antivirüs ayarlarınızı kontrol edin.");
            }
            catch (Exception ex)
            {
                // Başka herhangi bir beklenmeyen hata olursa
                return StatusCode(500, $"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }
    }
}