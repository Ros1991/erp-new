namespace ERP.Application.DTOs.Base
{
    /// <summary>
    /// Resultado paginado para listagens
    /// </summary>
    /// <typeparam name="T">Tipo dos itens retornados</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Lista de itens da página atual
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Número da página atual (baseado em 1)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de registros disponíveis
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Total de páginas disponíveis
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);

        /// <summary>
        /// Indica se há página anterior
        /// </summary>
        public bool HasPrevious => Page > 1;

        /// <summary>
        /// Indica se há próxima página
        /// </summary>
        public bool HasNext => Page < TotalPages;

        /// <summary>
        /// Construtor
        /// </summary>
        public PagedResult(List<T> items, int page, int pageSize, int total)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            Total = total;
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public PagedResult() 
        {
            Items = new List<T>();
        }
    }
}
