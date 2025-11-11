using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.CrossCutting.Filters;
using Microsoft.OpenApi.Models;
using ERP.CrossCutting.IoC;
using ERP.Configuration; // ADICIONADO: Nossa configuração personalizada do Swagger

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
})
.ConfigureApiBehaviorOptions(options =>
{
	// Customizar resposta de erro de validação do ModelState
	options.InvalidModelStateResponseFactory = context =>
	{
		var errors = context.ModelState
			.Where(x => x.Value.Errors.Count > 0)
			.ToDictionary(
				kvp => kvp.Key,
				kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
			);

		var response = new ERP.Application.DTOs.Base.BaseResponse<object>(
			code: 400,
			message: "One or more validation errors occurred.",
			errors: errors
		);

		return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
	};
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

// 6. JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// 7. Configurações adicionais
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

// 5. Authentication & Authorization
app.UseAuthentication();

// 6. Custom Middlewares (após autenticação, antes de autorização)
app.UseMiddleware<ERP.WebApi.Middlewares.JwtMiddleware>();
app.UseMiddleware<ERP.WebApi.Middlewares.CompanyContextMiddleware>();

app.UseAuthorization();

// 7. Controllers
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
