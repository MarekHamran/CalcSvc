using CalcSvc;
using CalcSvc.Messenger;
using CalcSvc.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.Services.AddSingleton<IMessenger, Messenger>();
builder.Services.AddHostedService<MQReader>();
builder.Services.AddDbContext<ArticlesDbContext>();

var app = builder.Build();

// wait until pg/rabbit containers start
await Task.Delay(3000);
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<ArticlesDbContext>();
	context.Database.Migrate();
}

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
