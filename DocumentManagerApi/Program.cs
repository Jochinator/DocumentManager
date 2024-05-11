using DocumentManager.DocumentProcessor;
using DocumentManagerApi;
using DocumentManagerPersistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerFileOperationFilter>();
});
builder.Services.AddSingleton<PersistenceDefinitions>();
builder.Services.AddSingleton<DocumentRepository>();
builder.Services.AddSingleton<TagRepository>();

builder.Services.AddScoped<DocumentProcessor>();
builder.Services.AddScoped<FilePersistence>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (HttpContext context, Func<Task> next) =>
{
    await next.Invoke();

    if (context.Response.StatusCode == 404 && !context.Request.Path.Value.Contains("/api"))
    {
        context.Request.Path = new PathString("/index.html");
        await next.Invoke();
    }
});
app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

var documentRepository = app.Services.GetService<DocumentRepository>();
documentRepository.Init();

app.Run();