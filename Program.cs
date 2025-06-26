using Hengeler.API.Middleware;
using Hengeler.Application.Interfaces;
using Hengeler.Application.Services;
using Hengeler.Domain;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Entities.Interfaces;
using Hengeler.Domain.Interfaces;
using Hengeler.Domain.Services;
using Hengeler.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? [];

builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthDomainService, AuthDomainService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingDomainService, BookingDomainService>();
builder.Services.AddScoped<IContactDomainService, ContactDomainService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ITranslationDomainService, TranslationDomainService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IEventDomainService, EventDomainService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISlideDomainService, SlideDomainService>();
builder.Services.AddScoped<ISlideService, SlideService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomDomainService, RoomDomainService>();
builder.Services.AddScoped<ISliderDomainService, SliderDomainService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddAuthentication("jwe")
    .AddScheme<AuthenticationSchemeOptions, DummyAuthHandler>("jwe", null);

builder.Services.AddScoped<IEmailDomainService>(sp =>
{
    var config = builder.Configuration.GetSection("EmailSettings");
    EmailSettings emailOptions = new()
    {
        SmtpServer = config["SmtpServer"]!,
        Port = int.Parse(config["Port"]!),
        SenderName = config["SenderName"]!,
        SenderEmail = config["SenderEmail"]!,
        Username = config["Username"]!,
        Password = config["Password"]!
    };
    return new EmailDomainService(emailOptions);

});

builder.Services.AddScoped<IBookingService>(sp =>
{
    var config = builder.Configuration.GetSection("Stripe");
    var bookingDomainService = sp.GetRequiredService<IBookingDomainService>();
    var emailDomainService = sp.GetRequiredService<IEmailDomainService>();
    var authDomainService = sp.GetRequiredService<IAuthDomainService>();
    return new BookingService(
        config["ApiKey"]!,
        config["SuccessUrl"]!,
        config["CancelUrl"]!,
        config["WebhookSecret"]!,
        emailDomainService,
        authDomainService,
        bookingDomainService
    );
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddScoped<ContactSeeder>();

var nextAuthSecret = builder.Configuration["NextAuth:Secret"];
if (string.IsNullOrEmpty(nextAuthSecret))
{
    throw new Exception("NextAuth secret is not configured");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.WebHost.UseWebRoot("wwwroot");
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ContactSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JweAuthenticationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();


