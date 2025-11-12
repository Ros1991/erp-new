using System.Text.Json;
using ERP.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ERP.CrossCutting.Services
{
    /// <summary>
    /// Serviço para gerenciar a configuração de módulos e permissões do sistema
    /// </summary>
    public interface IModuleConfigurationService
    {
        /// <summary>
        /// Obtém a configuração completa de módulos
        /// </summary>
        SystemModulesConfiguration GetConfiguration();

        /// <summary>
        /// Obtém apenas os módulos ativos
        /// </summary>
        List<ModuleConfiguration> GetActiveModules();

        /// <summary>
        /// Obtém um módulo específico por chave
        /// </summary>
        ModuleConfiguration GetModule(string moduleKey);

        /// <summary>
        /// Obtém os endpoints permitidos para uma permissão específica
        /// </summary>
        List<string> GetAllowedEndpoints(string moduleKey, string permissionKey);

        /// <summary>
        /// Verifica se um endpoint está permitido para uma permissão (sem método HTTP)
        /// </summary>
        bool IsEndpointAllowed(string moduleKey, string permissionKey, string endpoint);

        /// <summary>
        /// Verifica se uma rota específica (método HTTP + path) está permitida
        /// </summary>
        bool IsRouteAllowed(string moduleKey, string permissionKey, string method, string path);
    }

    public class ModuleConfigurationService : IModuleConfigurationService
    {
        private readonly SystemModulesConfiguration _configuration;
        private readonly ILogger<ModuleConfigurationService> _logger;

        public ModuleConfigurationService(ILogger<ModuleConfigurationService> logger)
        {
            _logger = logger;
            _configuration = LoadConfiguration();
        }

        private SystemModulesConfiguration LoadConfiguration()
        {
            try
            {
                // Tentar múltiplos caminhos possíveis
                var possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "modules-configuration.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "-4-WebApi", "Configuration", "modules-configuration.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Configuration", "modules-configuration.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "-4-WebApi", "Configuration", "modules-configuration.json")
                };

                string configPath = possiblePaths.FirstOrDefault(File.Exists);
                
                if (configPath == null)
                {
                    _logger.LogWarning($"Arquivo de configuração de módulos não encontrado. Caminhos tentados: {string.Join(", ", possiblePaths)}");
                    return CreateDefaultConfiguration();
                }

                _logger.LogInformation($"Arquivo de configuração encontrado: {configPath}");

                var jsonContent = File.ReadAllText(configPath);
                var configuration = JsonSerializer.Deserialize<SystemModulesConfiguration>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (configuration == null || !configuration.Modules.Any())
                {
                    _logger.LogWarning("Configuração de módulos está vazia. Usando configuração padrão.");
                    return CreateDefaultConfiguration();
                }

                _logger.LogInformation($"Configuração de módulos carregada com sucesso: {configuration.Modules.Count} módulos");
                return configuration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar configuração de módulos. Usando configuração padrão.");
                return CreateDefaultConfiguration();
            }
        }

        private SystemModulesConfiguration CreateDefaultConfiguration()
        {
            // Configuração mínima de fallback
            return new SystemModulesConfiguration
            {
                Modules = new List<ModuleConfiguration>
                {
                    new ModuleConfiguration
                    {
                        Key = "role",
                        Name = "Cargos",
                        Description = "Gerenciar cargos e permissões",
                        IsActive = true,
                        Order = 1,
                        Permissions = new List<PermissionConfiguration>
                        {
                            new PermissionConfiguration { Key = "canView", Name = "Visualizar", Order = 1 },
                            new PermissionConfiguration { Key = "canCreate", Name = "Criar", Order = 2 },
                            new PermissionConfiguration { Key = "canEdit", Name = "Editar", Order = 3 },
                            new PermissionConfiguration { Key = "canDelete", Name = "Excluir", Order = 4 }
                        }
                    }
                }
            };
        }

        public SystemModulesConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public List<ModuleConfiguration> GetActiveModules()
        {
            return _configuration.Modules
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .ToList();
        }

        public ModuleConfiguration GetModule(string moduleKey)
        {
            return _configuration.Modules
                .FirstOrDefault(m => m.Key.Equals(moduleKey, StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetAllowedEndpoints(string moduleKey, string permissionKey)
        {
            var module = GetModule(moduleKey);
            if (module == null) return new List<string>();

            var permission = module.Permissions
                .FirstOrDefault(p => p.Key.Equals(permissionKey, StringComparison.OrdinalIgnoreCase));

            // Retorna paths de AllowedRoutes e AllowedEndpoints (para retrocompatibilidade)
            var endpoints = new List<string>();
            
            if (permission != null)
            {
                if (permission.AllowedEndpoints != null)
                    endpoints.AddRange(permission.AllowedEndpoints);
                    
                if (permission.AllowedRoutes != null)
                    endpoints.AddRange(permission.AllowedRoutes.Select(r => $"{r.Method} {r.Path}"));
            }

            return endpoints;
        }

        public bool IsEndpointAllowed(string moduleKey, string permissionKey, string endpoint)
        {
            var module = GetModule(moduleKey);
            if (module == null) return false;

            var permission = module.Permissions
                .FirstOrDefault(p => p.Key.Equals(permissionKey, StringComparison.OrdinalIgnoreCase));

            if (permission == null) return false;

            // Tentar match com AllowedEndpoints (retrocompatibilidade - sem método HTTP)
            if (permission.AllowedEndpoints != null)
            {
                foreach (var allowedEndpoint in permission.AllowedEndpoints)
                {
                    if (MatchPath(allowedEndpoint, endpoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifica se uma rota específica (método + path) está permitida
        /// </summary>
        public bool IsRouteAllowed(string moduleKey, string permissionKey, string method, string path)
        {
            var module = GetModule(moduleKey);
            if (module == null) return false;

            var permission = module.Permissions
                .FirstOrDefault(p => p.Key.Equals(permissionKey, StringComparison.OrdinalIgnoreCase));

            if (permission == null) return false;

            // Verificar AllowedRoutes com método HTTP
            if (permission.AllowedRoutes != null)
            {
                foreach (var route in permission.AllowedRoutes)
                {
                    if (route.Method.Equals(method, StringComparison.OrdinalIgnoreCase) && MatchPath(route.Path, path))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool MatchPath(string pattern, string path)
        {
            // Suporta wildcards simples
            if (pattern.Contains("*"))
            {
                var regexPattern = pattern.Replace("*", ".*");
                if (System.Text.RegularExpressions.Regex.IsMatch(path, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            // Suporta parâmetros de rota {id}, {userId}, etc
            else if (pattern.Contains("{") && pattern.Contains("}"))
            {
                var regexPattern = System.Text.RegularExpressions.Regex.Replace(pattern, @"\{[^}]+\}", @"[^/]+");
                if (System.Text.RegularExpressions.Regex.IsMatch(path, $"^{regexPattern}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            // Match exato
            else if (pattern.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
