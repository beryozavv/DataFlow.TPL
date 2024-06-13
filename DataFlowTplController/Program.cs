using DataFlowTplController.DataFlow;

namespace DataFlowTplController;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<ParallelAsyncFlow>();

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

        app.Lifetime.ApplicationStarted.Register(static state =>
        {
            var serviceProvider = state as IServiceProvider;
            var parallelAsyncFlow = serviceProvider!.GetRequiredService<ParallelAsyncFlow>();
            parallelAsyncFlow.InitFlow();
        }, app.Services);

        app.Lifetime.ApplicationStopping.Register(static state =>
        {
            var serviceProvider = state as IServiceProvider;
            var parallelAsyncFlow = serviceProvider!.GetRequiredService<ParallelAsyncFlow>();
            parallelAsyncFlow.CompleteFlow().GetAwaiter().GetResult();
        }, app.Services);

        app.Run();
    }
}