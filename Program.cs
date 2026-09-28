var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5177");

// 1. Настройка CORS для Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Добавляем поддержку сериализации JSON для DTO в Minimal API
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

// 2. Подключаем CORS строго до эндпоинтов
app.UseCors("AllowAngular");

string filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.txt");

// 3. POST эндпоинт (v1.0)
app.MapPost("/api/data", async (DataDto dto) =>
{
    await File.WriteAllTextAsync(filePath, dto.Text ?? string.Empty);
    return Results.Ok(new { message = "Данные успешно сохранены в data.txt" });
});

app.Run();

// DTO объявляется строго ПОСЛЕ app.Run()
record DataDto(string Text);