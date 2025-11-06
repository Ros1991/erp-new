namespace CAU.Application.DTOs.Base
{
    /// <summary>
    /// Parâmetros de paginação e filtros para requisições
    /// </summary>
    public class PagedRequest
    {
        private int _page = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        /// <summary>
        /// Número da página (mínimo 1)
        /// </summary>
        public int Page 
        { 
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Quantidade de itens por página (mínimo 1, máximo 100)
        /// </summary>
        public int PageSize 
        { 
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }

        /// <summary>
        /// Campo para ordenação (ex: "Name", "CreatedAt")
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Direção da ordenação (asc ou desc)
        /// </summary>
        public string? OrderDirection { get; set; } = "asc";

        /// <summary>
        /// Texto de busca/filtro geral
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Verifica se a ordenação é ascendente
        /// </summary>
        public bool IsAscending => string.IsNullOrWhiteSpace(OrderDirection) || 
                                    OrderDirection.ToLower() == "asc";

        /// <summary>
        /// Calcula o número de registros a pular
        /// </summary>
        public int Skip => (Page - 1) * PageSize;
    }
}
