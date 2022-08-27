using SmartCityTechnologies.CognitiveServices.Builders;
using SmartCityTechnologies.CognitiveServices.Models;
using SmartCityTechnologies.CognitiveServices.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ISpeechBuilder, SpeechConfigBuilder>();
builder.Services.Configure<TextSpeechOptions>(builder.Configuration.GetSection(nameof(TextSpeechOptions)));
builder.Services.Configure<FaceDetectionOptions>(builder.Configuration.GetSection(nameof(FaceDetectionOptions)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
