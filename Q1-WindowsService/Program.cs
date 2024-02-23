using Q1_WindowsService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddWindowsService(o => o.ServiceName = "WorkloadCollectorService");

var host = builder.Build();
host.Run();
