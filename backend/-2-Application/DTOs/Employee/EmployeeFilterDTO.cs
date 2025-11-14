namespace ERP.Application.DTOs
{
    public class EmployeeFilterDTO
    {
        public string? Search { get; set; }
        public string? Nickname { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
        public long? EmployeeIdManager { get; set; }
        public long? UserId { get; set; }
        
        // Paginação
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        // Ordenação
        public string? OrderBy { get; set; } = "FullName";
        public bool IsAscending { get; set; } = true;
    }
}
