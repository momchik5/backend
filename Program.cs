var builder = WebApplication.CreateBuilder(args);

// 1. Добавляем сервис CORS, чтобы разрешить запросы с Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Адрес вашего Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 2. Включаем CORS
app.UseCors("AllowAngular");


string filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.txt");

app.MapPost("/api/data", async (DataDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Text))
    {
        return Results.BadRequest(new { message = "Текст не должен быть пустым" });
    }

    await File.AppendAllTextAsync(filePath, dto.Text + Environment.NewLine);
    return Results.Ok(new { message = "Данные успешно сохранены в файл!" });
});


app.MapGet("/api/data", async () =>
{
    if (!File.Exists(filePath))
    {
        return Results.Ok(new { text = "Файл пока пуст или не создан." });
    }

    string content = await File.ReadAllTextAsync(filePath);
    return Results.Ok(new { text = content });
});

app.Run();

// DTO для получения JSON из POST-запроса
record DataDto(string Text);