using PAW.MinimalAPI.Models;
using PAW.MinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Dependency Injection / IoC
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddHttpClient<ITipoCambioService, TipoCambioService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();   // sirve wwwroot/index.html
app.UseStaticFiles();

// ---------- Tasks (CRUD) ----------
var tasks = app.MapGroup("/api/tasks");

tasks.MapGet("/", async (ITaskService svc) =>
    Results.Ok(await svc.GetAllAsync()));

tasks.MapGet("/{id:int}", async (int id, ITaskService svc) =>
    await svc.GetByIdAsync(id) is TaskItem t ? Results.Ok(t) : Results.NotFound());

tasks.MapPost("/", async (TaskItem task, ITaskService svc) =>
{
    if (string.IsNullOrWhiteSpace(task.Title))
        return Results.BadRequest("El título es requerido.");
    var created = await svc.CreateAsync(task);
    return Results.Created($"/api/tasks/{created.Id}", created);
});

tasks.MapPut("/{id:int}", async (int id, TaskItem task, ITaskService svc) =>
    await svc.UpdateAsync(id, task) ? Results.NoContent() : Results.NotFound());

tasks.MapDelete("/{id:int}", async (int id, ITaskService svc) =>
    await svc.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());

// ---------- Tipo de cambio ----------
app.MapGet("/api/tipocambio", async (ITipoCambioService svc) =>
    await svc.GetAsync() is TipoCambio tc ? Results.Ok(tc) : Results.StatusCode(503));

app.Run();
