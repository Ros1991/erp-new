namespace ERP.Application.DTOs
{
    public class CompanyUserFilterDTO
    {
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "userEmail";
        public string? SortDirection { get; set; } = "asc";
    }
}
