using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// API Controller'larımızı (RoadmapController gibi) sisteme tanıtıyoruz
builder.Services.AddControllers();

// Swagger (Test Arayüzü) servislerini ekliyoruz
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS Politikası(React'e izin veriyoruz)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Eğer geliştirme aşamasındaysak Swagger arayüzünü göster diyoruz
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eğitim Asistanı API V1");
        c.RoutePrefix = "swagger"; // Adresin sonuna /swagger ekleyince açılması için
    });

// YENİ EKLE: Bu satır, kimlik doğrulama ekranını (şifreyi) yazılımsal olarak bypass etmeye yardımcı olabilir
app.MapControllers().AllowAnonymous();


//app.UseHttpsRedirection();

// YENİ EKLENEN KISIM: CORS'u aktif et (UseAuthorization'dan önce olmalı!)
app.UseCors("AllowReact");

app.UseAuthorization();
app.MapControllers(); // Gelen istekleri Controller'lara yönlendirir

app.Run();