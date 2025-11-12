namespace ERP.Domain.Models
{
    /// <summary>
    /// Configuração completa dos módulos e permissões do sistema
    /// </summary>
    public class SystemModulesConfiguration
    {
        public List<ModuleConfiguration> Modules { get; set; } = new();
    }

    /// <summary>
    /// Configuração de um módulo específico
    /// </summary>
    public class ModuleConfiguration
    {
        /// <summary>
        /// Chave única do módulo (ex: "role", "user", "account")
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Nome amigável do módulo (ex: "Cargos", "Usuários")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descrição do módulo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ícone do módulo (opcional)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Ordem de exibição
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Módulo está ativo?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Permissões disponíveis para este módulo
        /// </summary>
        public List<PermissionConfiguration> Permissions { get; set; } = new();
    }

    /// <summary>
    /// Configuração de uma permissão específica
    /// </summary>
    public class PermissionConfiguration
    {
        /// <summary>
        /// Chave da permissão (ex: "canView", "canCreate")
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Nome amigável da permissão (ex: "Visualizar", "Criar")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descrição da permissão
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Endpoints que esta permissão libera acesso
        /// Suporta wildcards: "/role/*", "/role/{id}"
        /// </summary>
        public List<string>? AllowedEndpoints { get; set; }

        /// <summary>
        /// Rotas com métodos HTTP específicos
        /// Formato: "GET /role/{id}", "POST /role/create", "PUT /role/{id}", "DELETE /role/{id}"
        /// </summary>
        public List<EndpointRoute>? AllowedRoutes { get; set; }

        /// <summary>
        /// Ordem de exibição
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Cor da permissão (opcional, para UI)
        /// </summary>
        public string Color { get; set; }
    }

    /// <summary>
    /// Representa uma rota com método HTTP específico
    /// </summary>
    public class EndpointRoute
    {
        /// <summary>
        /// Método HTTP (GET, POST, PUT, DELETE, PATCH, etc)
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Path do endpoint (ex: "/role/{id}", "/role/create")
        /// </summary>
        public string Path { get; set; }
    }
}
