using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Create an endpoint to GET ToDos
app.MapGet("ToDos", async (ToDoDbContext context) =>
{
    var todos = await context.ToDos.ToListAsync();
    return Results.Ok(todos);
});

// Create an endpoint to GET a ToDo
app.MapGet("ToDos/{id}", async (ToDoDbContext context, int id) =>
{
    var todo = await context.ToDos.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(todo);
});

// Create an endpoint to POST a ToDo
app.MapPost("ToDos", async (ToDoDbContext context, ToDo todo) =>
{
    await context.ToDos.AddAsync(todo);
    await context.SaveChangesAsync();
    return Results.Created($"ToDos/{todo.Id}", todo);
});

// Create an endpoint to PUT a ToDo
app.MapPut("ToDos/{id}", async (ToDoDbContext context, int id, ToDo todo) =>
{
    var existing = await context.ToDos.FindAsync(id);
    if (existing == null)
    {
        return Results.NotFound();
    }

    existing.Title = todo.Title;
    existing.IsDone = todo.IsDone;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Create an endpint to DELETE a ToDo
app.MapDelete("ToDos/{id}", async (ToDoDbContext context, int id) =>
{
    var todo = await context.ToDos.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }

    context.ToDos.Remove(todo);
    await context.SaveChangesAsync();
    return Results.NotFound();
});

app.Run();

// Craete a ToDo class
public class ToDo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsDone { get; set; }
}

// Create ToDoDbContext with EF
public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public DbSet<ToDo> ToDos { get; set; }
}