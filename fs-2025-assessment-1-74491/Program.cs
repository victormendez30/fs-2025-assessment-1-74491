using fs_2025_assessment_1_74491.Endpoints;
using fs_2025_assessment_1_74491.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddStationEndPointsV1();
app.AddStationEndPointsV2();

app.Run();
