using JobMesh.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/submitjob", (JobSubmission submission) =>
{
    // Here you would add logic to handle the job submission, such as validating the input,
    // saving it to a database, or queuing it for processing.
    
    // For demonstration purposes, we'll just return a success message with the submitted data.
    return Results.Ok(new { Message = "Job submitted successfully!", Submission = submission });
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
