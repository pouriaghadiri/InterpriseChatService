
using API.DependencyInjections;
using Application;
using Application.Common;
using Application.Features.UserUseCase.Validators;
using FluentValidation;
using MediatR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDependencyInjections(builder.Configuration);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // MediatR Configuration
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
        });
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI();
            app.UseSwagger();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}