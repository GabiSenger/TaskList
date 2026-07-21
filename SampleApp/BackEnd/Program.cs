using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    // current workaround for port forwarding in codespaces
    // https://github.com/dotnet/aspnetcore/issues/57332
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

List<TaskItem> taskList = new List<TaskItem>();

app.MapPost("/AddTask", (CreateTaskItemDTO dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Text))
    {
        return Results.BadRequest(new {message="Texto não pode vir vazio"});
    }
    TaskItem newTask = new TaskItem{
        Id= taskList.Count > 0 ? taskList.Max((item) => item.Id) + 1 : 1,
        Text=dto.Text,
        Finished=false  
    };
    taskList.Add(newTask);
    return Results.Created($"/AddTask/{newTask.Id}", taskList);

});

app.MapGet("/GetTaks", () =>
{
    return Results.Ok(taskList);
});

app.MapDelete("/RemoveTask/{id}", (int Id) =>
{
    var uniqueTask = taskList.FirstOrDefault(t => t.Id == Id);
    if(uniqueTask == null)
    {
        return Results.NotFound(new {message = "tarefa não encontrada"});
    }
    taskList.Remove(uniqueTask);
    return Results.Ok(new {message="tarefa excluída com sucesso"});

});

app.Run();

public class TaskItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Finished { get; set; } 
}

public class CreateTaskItemDTO
{
    public string Text {get ; set;} = string.Empty;
}
