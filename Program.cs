using HelpDesk.Results.Models;
using HelpDesk.Results.Services;
using HelpDesk.Results.Results;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ================================================
// MIDDLEWARE ПОРЯДОК (ВАЖНО!)
// 1. Обработка исключений (должна быть первой)
// 2. Обработка HTTP-ошибок
// 3. Маршруты
// ================================================

// === ОБРАБОТКА ИСКЛЮЧЕНИЙ ===
if (app.Environment.IsDevelopment())
{
    // Development: подробная страница ошибки (stack trace)
    app.UseDeveloperExceptionPage();
}
else
{
    // Production: безопасный обработчик без деталей
    app.UseExceptionHandler("/error/exception");
}

// === ОБРАБОТКА HTTP-ОШИБОК (404, 401, 403 и т.д.) ===
app.UseStatusCodePagesWithReExecute("/error/status/{0}");

// === СТАТИЧЕСКИЕ ФАЙЛЫ ===
app.UseStaticFiles();

// ================================================
// МАРШРУТЫ
// ================================================

// --- Обработчики ошибок (должны быть объявлены до использования) ---

// Обработчик исключений для Production
app.MapGet("/error/exception", () =>
{
    return Results.Json(new
    {
        error = "Внутренняя ошибка сервера",
        message = "Произошла ошибка при обработке запроса. Пожалуйста, попробуйте позже.",
        statusCode = 500
    }, statusCode: 500);
});

// Обработчик HTTP-ошибок (404, 401, 403 и т.д.)
app.MapGet("/error/status/{code:int}", (int code) =>
{
    var message = code switch
    {
        400 => "Bad Request: Неверный запрос",
        401 => "Unauthorized: Требуется авторизация",
        403 => "Forbidden: Доступ запрещен",
        404 => "Not Found: Запрашиваемый ресурс не найден",
        418 => "I'm a teapot: Сервер отказывается заваривать кофе",
        500 => "Internal Server Error: Внутренняя ошибка сервера",
        _ => $"HTTP ошибка {code}"
    };
    
    return Results.Json(new { statusCode = code, message }, statusCode: code);
});

// --- Основные маршруты ---

// GET / - главная страница (через собственный IResult)
app.MapGet("/", (IResultExtensions resultExtensions) =>
{
    var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>HelpDesk.Results</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; background: #f5f5f5; }
        .container { max-width: 900px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        h1 { color: #2c3e50; }
        h2 { color: #3498db; border-bottom: 2px solid #3498db; padding-bottom: 5px; }
        .endpoint { background: #ecf0f1; padding: 10px; margin: 10px 0; border-radius: 5px; font-family: monospace; }
        .method { font-weight: bold; color: #27ae60; }
        .url { color: #2980b9; }
        .badge { display: inline-block; padding: 2px 8px; border-radius: 4px; font-size: 12px; margin-left: 10px; }
        .badge-get { background-color: #27ae60; color: white; }
        .badge-post { background-color: #3498db; color: white; }
    </style>
</head>
<body>
    <div class='container'>
        <h1> HelpDesk.Results</h1>
        <p>Учебный сервис заявок в службу поддержки. Демонстрация обработки исключений, HTTP-ошибок и Results API.</p>
        
        <h2> Маршруты API</h2>
        
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/</span>
            <span class='badge badge-get'>HTML</span> - Главная страница (собственный IResult)
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/about/text</span>
            <span class='badge badge-get'>Text</span> - Простой текстовый ответ
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/about/content</span>
            <span class='badge badge-get'>Content</span> - Текст с явным Content-Type
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/api/tickets</span>
            <span class='badge badge-get'>JSON</span> - Список всех заявок
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/api/tickets/{id}</span>
            <span class='badge badge-get'>JSON</span> - Заявка по ID
        </div>
        <div class='endpoint'>
            <span class='method'>POST</span> <span class='url'>/api/tickets/create?title=...&amp;priority=...</span>
            <span class='badge badge-post'>201</span> - Создание заявки
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/status/unauthorized</span>
            <span class='badge badge-get'>401</span> - Unauthorized
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/status/forbidden</span>
            <span class='badge badge-get'>403</span> - Forbidden
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/status/custom/418</span>
            <span class='badge badge-get'>418</span> - I'm a teapot
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/redirect/old-tickets</span>
            <span class='badge badge-get'>302</span> - Редирект на /api/tickets
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/redirect/ticket/{id}</span>
            <span class='badge badge-get'>302</span> - Редирект на заявку
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/files/readme</span>
            <span class='badge badge-get'>File</span> - Скачать инструкцию
        </div>
        <div class='endpoint'>
            <span class='method'>GET</span> <span class='url'>/throw</span>
            <span class='badge badge-get'>500</span> - Генерация исключения
        </div>
        
        <h2> Примеры запросов</h2>
        <code>curl http://localhost:5000/api/tickets</code><br>
        <code>curl http://localhost:5000/api/tickets/1</code><br>
        <code>curl -X POST 'http://localhost:5000/api/tickets/create?title=Проблема с Wi-Fi&amp;priority=1'</code>
    </div>
</body>
</html>";
    
    return resultExtensions.Html(html);
}).AllowAnonymous();

// GET /about/text - простой текст
app.MapGet("/about/text", () =>
{
    return Results.Text("HelpDesk.Results - сервис заявок в службу поддержки. Версия 1.0");
});

// GET /about/content - текст с явным Content-Type и кодировкой
app.MapGet("/about/content", () =>
{
    return Results.Content(
        content: "=== HelpDesk.Results ===\nСтатус: Активен\nВерсия: 1.0.0\nДата: " + DateTime.Now.ToShortDateString(),
        contentType: "text/plain; charset=utf-8"
    );
});

// GET /api/tickets - список всех заявок
app.MapGet("/api/tickets", (ITicketRepository repository) =>
{
    var tickets = repository.GetAll();
    return Results.Ok(tickets);
});

// GET /api/tickets/{id} - заявка по ID (именованный маршрут для редиректа)
app.MapGet("/api/tickets/{id:int}", (int id, ITicketRepository repository) =>
{
    var ticket = repository.GetById(id);
    return ticket is null 
        ? Results.NotFound(new { message = $"Заявка с ID {id} не найдена" })
        : Results.Ok(ticket);
}).WithName("ticket-details");

// POST /api/tickets/create - создание заявки
app.MapPost("/api/tickets/create", (string? title, int? priority, ITicketRepository repository) =>
{
    // Валидация
    if (string.IsNullOrWhiteSpace(title))
    {
        return Results.BadRequest(new { error = "Параметр 'title' обязателен", example = "/api/tickets/create?title=Проблема&priority=2" });
    }
    
    if (priority is null or < 1 or > 3)
    {
        return Results.BadRequest(new { error = "Параметр 'priority' должен быть от 1 до 3" });
    }
    
    var ticket = repository.Create(title, priority.Value);
    return Results.Created($"/api/tickets/{ticket.Id}", ticket);
});

// GET /status/unauthorized - 401 Unauthorized
app.MapGet("/status/unauthorized", () =>
{
    return Results.Unauthorized();
});

// GET /status/forbidden - 403 Forbidden
app.MapGet("/status/forbidden", () =>
{
    return Results.StatusCode(403);
});

// GET /status/custom/418 - произвольный статус-код (418 I'm a teapot)
app.MapGet("/status/custom/418", () =>
{
    return Results.StatusCode(418);
});

// GET /redirect/old-tickets - локальный редирект
app.MapGet("/redirect/old-tickets", () =>
{
    return Results.LocalRedirect("/api/tickets");
});

// GET /redirect/ticket/{id} - редирект на именованный маршрут
app.MapGet("/redirect/ticket/{id:int}", (int id) =>
{
    return Results.RedirectToRoute("ticket-details", new { id });
});

// GET /files/readme - отправка файла
app.MapGet("/files/readme", () =>
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "readme.txt");
    
    if (!File.Exists(filePath))
    {
        return Results.NotFound(new { message = "Файл не найден" });
    }
    
    return Results.File(filePath, "text/plain", "HelpDesk_Instruction.txt");
});

// GET /throw - генерация исключения
app.MapGet("/throw", () =>
{
    throw new InvalidOperationException("Это тестовое исключение для проверки обработчика ошибок");
});

app.Run();