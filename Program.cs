var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка CORS для Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

record DataDto(string Text);

// v1.0: Сохранение данных в файл data.txt
app.MapPost("/api/data", async (DataDto dto) =>
{
    await File.WriteAllTextAsync("data.txt", dto.Text ?? string.Empty);
    return Results.Ok(new { message = "Данные успешно сохранены в data.txt" });
});

app.Run();