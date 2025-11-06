namespace CAU.Application.DTOs.Base
{
    /// <summary>
    /// Resposta padronizada para todos os endpoints da API
    /// </summary>
    /// <typeparam name="T">Tipo de dados retornado</typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Dados retornados (DTO, lista, paginação, etc)
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Código HTTP de status
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Mensagem descritiva da operação
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Construtor para resposta de sucesso
        /// </summary>
        public BaseResponse(T data, int code = 200, string message = "Operação realizada com sucesso")
        {
            Data = data;
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Construtor para resposta de erro
        /// </summary>
        public BaseResponse(int code, string message)
        {
            Data = default;
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public BaseResponse() { }
    }
}
