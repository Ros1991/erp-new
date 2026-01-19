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

        /// <summary>
        /// É role do sistema? (IsSystem=true, bypass total)
        /// </summary>
        public bool IsSystemRole { get; set; } = false;
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
        
        /// <summary>
        /// Permissões extras específicas do módulo (ex: canProcess, canClose, canReopen)
        /// </summary>
        public Dictionary<string, bool>? ExtraPermissions { get; set; }
    }

    /// <summary>
    /// Constantes de módulos do sistema
    /// </summary>
    public static class Modules
    {
        public const string Role = "role";
        public const string User = "user";
        public const string Account = "account";
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
    }
}
