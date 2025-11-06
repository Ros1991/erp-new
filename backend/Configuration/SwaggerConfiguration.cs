using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CAU.Configuration;

/// <summary>
/// Configuração do Swagger/OpenAPI para documentação detalhada da API
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Configura o Swagger com XML comments e informações detalhadas da API
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>Services collection configurado</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Informações gerais da API
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "API ERP",
                Version = "1.0",
                Description = @"**API RESTful - API ERP**",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "erp",
                    Email = "developer@gvinci.com.br"
                }
            });

            // Incluir XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Configurar descrições de enum como strings
            c.SchemaFilter<EnumSchemaFilter>();
            
            // Configurar descrições automáticas para responses
            c.DescribeAllParametersInCamelCase();
            
            // Permitir que controllers usem [Tags] personalizados
            c.DocInclusionPredicate((name, api) => true);

            Console.WriteLine("Lista de endpoints");
            Console.WriteLine("===============================================================================================");

			//Configurar ordenação de operações usando chave composta
			//O OperationFilter já modificou o OperationId para incluir ordenação
			c.OrderActionsBy((apiDesc) =>
			{
                if (apiDesc.GroupName != null)
                {
                    Console.WriteLine($"{apiDesc.GroupName} ({apiDesc.HttpMethod}) /{apiDesc.RelativePath}");
                }
                return apiDesc.ActionDescriptor.DisplayName;
			});

			Console.WriteLine("===============================================================================================");

		});

        return services;
    }

    /// <summary>
    /// Configura o middleware do Swagger UI
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder configurado</returns>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ERP 1.0");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "API ERP - Documentação";
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            c.DefaultModelsExpandDepth(-1); // Ocultar modelos por padrão
            c.DisplayRequestDuration();
            c.EnableFilter();
            c.EnableDeepLinking();
            c.ShowExtensions();
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/swagger");
                return;
            }
            await next();
        });

        return app;
    }

}

/// <summary>
/// Schema filter para melhor exibição de enums no Swagger
/// </summary>
public class EnumSchemaFilter : Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiSchema schema, Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (string enumName in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(enumName));
            }
        }
    }
}
