using restapi;

var builder = WebApplication.CreateBuilder(args);

builder.AddDependenciesForRestApi();


var app = builder.Build();

app.AddMiddlewareForRestApi();

app.Run();
