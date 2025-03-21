using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<SpeedGameApp>("speed-game-app");

builder.Build().Run();
