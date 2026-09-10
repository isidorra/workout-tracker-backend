using DotNetEnv;

Env.TraversePath().NoClobber().Load();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run();
