using DotNetEnv;

using Microsoft.AspNetCore.Identity;

using Wolverine;
using Wolverine.FluentValidation;

using WorkoutTracker.Api;
using WorkoutTracker.Application;
using WorkoutTracker.Infrastructure;
using WorkoutTracker.Infrastructure.Identity;
using WorkoutTracker.Infrastructure.Persistence;

Env.TraversePath().NoClobber().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(AssemblyReference).Assembly);
    opts.UseFluentValidation();
    opts.UseRuntimeCompilation();

    // UserManager depends on IServiceProvider, so Wolverine can't build it inline. Resolving it and
    // AppDbContext from the message's scope keeps Identity and the repositories on one DbContext.
    opts.CodeGeneration.AlwaysUseServiceLocationFor<UserManager<ApplicationUser>>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
