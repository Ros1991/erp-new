namespace ERP.Domain.Models
{
    /// <summary>
    /// Modelo de permissões de uma Role (armazenado como JSON no campo role_permissions)
    /// </summary>
    public class RolePermissions
    {
        /// <summary>
        /// Lista de endpoints permitidos para esta role
        /// </summary>
        public List<string> AllowedEndpoints { get; set; } = new();

        /// <summary>
        /// Permissões por módulo
        /// </summary>
        public Dictionary<string, ModulePermissions> Modules { get; set; } = new();

        /// <summary>
        /// É administrador? (acesso total)
        /// </summary>
        public bool IsAdmin { get; set; } = false;
    }

    /// <summary>
    /// Permissões de um módulo específico
    /// </summary>
    public class ModulePermissions
    {
        public bool CanView { get; set; } = false;
        public bool CanCreate { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool CanExport { get; set; } = false;
    }

    /// <summary>
    /// Constantes de módulos do sistema
    /// </summary>
    public static class Modules
    {
        public const string Company = "company";
        public const string Account = "account";
        public const string User = "user";
        public const string Role = "role";
        public const string Product = "product";
        public const string Order = "order";
        public const string Financial = "financial";
        public const string Report = "report";
    }

    /// <summary>
    /// Constantes de ações
    /// </summary>
    public static class Actions
    {
        public const string View = "view";
        public const string Create = "create";
        public const string Edit = "edit";
        public const string Delete = "delete";
        public const string Export = "export";
    }
}
