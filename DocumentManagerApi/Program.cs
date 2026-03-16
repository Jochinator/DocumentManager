using System.Text.Json;
using System.Text.Json.Serialization;
using DocumentManager.DocumentProcessor;
using DocumentManagerApi;
using DocumentManagerModel;
using DocumentManagerPersistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerFileOperationFilter>();
});

builder.Services.AddScoped<DocumentRepository>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<ContactRepository>();
builder.Services.AddScoped<FilePersistence>();

builder.Services.AddScoped<DocumentProcessor>();
builder.Services.AddScoped<FileSystemDocumentFileFactory>();

builder.Services.AddHostedService<DataMigrationService>();
builder.Services.AddScoped<IDataMigration, ScopeBasedFileStructureMigration>();
builder.Services.AddScoped<IDataMigration, DeduplicateTagsMigration>();
builder.Services.AddScoped<IDataMigration, SenderNameToContactMigration>();
builder.Services.AddScoped<FilesystemViewService>();

builder.Services.Configure<PersistenceDefinitions>(
    builder.Configuration.GetSection("PersistenceDefinitions"));

var definitions = builder.Configuration
    .GetSection("PersistenceDefinitions")
    .Get<PersistenceDefinitions>();

Directory.CreateDirectory(definitions!.DataRootFolder);
Directory.CreateDirectory(Path.Combine(definitions.DataRootFolder, definitions.DocumentFolder));
Directory.CreateDirectory(Path.Combine(definitions.DataRootFolder, definitions.ImportFolder));
Directory.CreateDirectory(Path.Combine(definitions.DataRootFolder, definitions.DeletedFolder));
Directory.CreateDirectory(Path.Combine(definitions.DataRootFolder, definitions.FailedFolder));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    await next.Invoke();

    if (context.Response.StatusCode == 404 && context.Request.Path.Value != null && !context.Request.Path.Value.Contains("/api"))
    {
        context.Request.Path = new PathString("/index.html");
        await next.Invoke();
    }
});
app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("initializing document Repository");
    var documentRepository = scope.ServiceProvider.GetRequiredService<DocumentRepository>();
    documentRepository.Init();
}

app.Run();