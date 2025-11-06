using Microsoft.EntityFrameworkCore;
using CAU.Infrastructure.Data;
using CAU.CrossCutting.Filters;
using Microsoft.OpenApi.Models;
using CAU.CrossCutting.IoC;
using CAU.Configuration; // ADICIONADO: Nossa configuração personalizada do Swagger

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DE SERVIÇOS =====

// 1. Entity Framework / Database Context
builder.Services.AddDbContext<ErpContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("ErpConnection")));

// 2. Dependency Injection - Repositories, Services, UnitOfWork
builder.Services.AddApplicationServices();

// 3. Controllers com filtros personalizados
builder.Services.AddControllers(options =>
{
	// Adiciona o filtro global de exceções
	options.Filters.Add<GlobalExceptionFilter>();
});

// 4. CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

// 5. Swagger/OpenAPI - CONFIGURAÇÃO PERSONALIZADA INTEGRADA
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation(); // SUBSTITUÍDO: Nossa configuração personalizada

// 6. Configurações adicionais
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// ===== BUILD DA APLICAÇÃO =====
var app = builder.Build();

// ===== CONFIGURAÇÃO DO PIPELINE =====

// 1. Swagger (Development) - MIDDLEWARE PERSONALIZADO INTEGRADO
if (app.Environment.IsDevelopment())
{
	app.UseSwaggerDocumentation(); // SUBSTITUÍDO: Nosso middleware personalizado
	app.UseDeveloperExceptionPage();
}

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. CORS
app.UseCors("AllowAll");

// 4. Routing
app.UseRouting();

// 5. Authorization (se necessário no futuro)
// app.UseAuthentication();
// app.UseAuthorization();

// 6. Controllers
app.MapControllers();

// ===== CONFIGURAÇÃO DE ROTAS PERSONALIZADAS =====

// Health Check endpoint
app.MapGet("/health", () => Results.Ok(new
{
	Status = "Healthy",
	Timestamp = DateTime.UtcNow,
	Version = "1.0"
}));

// API Info endpoint
app.MapGet("/api/info", () => Results.Ok(new
{
	ApiName = "API ERP",
	Version = "1.0",
	Environment = app.Environment.EnvironmentName,
	Framework = ".NET 9",
	Architecture = "Clean Architecture + DDD"
}));

// ===== INICIALIZAÇÃO =====
app.Run();
