using Microsoft.AspNetCore.Mvc;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;
using ERP.WebApi.Extensions;
using System.Diagnostics;

namespace ERP.WebApi.Controllers.Base
{
    /// <summary>
    /// Base controller with common error handling and helper methods
    /// </summary>
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtém o ID do usuário autenticado
        /// </summary>
        protected long GetCurrentUserId()
        {
            return HttpContext.GetUserId();
        }

        /// <summary>
        /// Obtém o email do usuário autenticado
        /// </summary>
        protected string? GetCurrentUserEmail()
        {
            return HttpContext.GetUserEmail();
        }

        /// <summary>
        /// Obtém o telefone do usuário autenticado
        /// </summary>
        protected string? GetCurrentUserPhone()
        {
            return HttpContext.GetUserPhone();
        }

        /// <summary>
        /// Obtém o CPF do usuário autenticado
        /// </summary>
        protected string? GetCurrentUserCpf()
        {
            return HttpContext.GetUserCpf();
        }

        /// <summary>
        /// Verifica se o usuário está autenticado
        /// </summary>
        protected bool IsUserAuthenticated()
        {
            return HttpContext.IsAuthenticated();
        }

        /// <summary>
        /// Obtém o CompanyId do contexto da requisição (multi-tenant)
        /// </summary>
        protected long GetCompanyId()
        {
            return HttpContext.GetCompanyId();
        }

        /// <summary>
        /// Verifica se existe um CompanyId no contexto
        /// </summary>
        protected bool HasCompanyContext()
        {
            return HttpContext.HasCompanyContext();
        }

        /// <summary>
        /// Validates ModelState and throws ValidationException if invalid
        /// </summary>
        protected void ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                throw new ValidationException(errors);
            }
        }

        /// <summary>
        /// Validates required parameter and throws ValidationException if null or empty
        /// </summary>
        protected void ValidateRequired(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogWarning("Validation failed: {ParameterName} is required", parameterName);
                throw new ValidationException(parameterName, $"Parameter '{parameterName}' is required and cannot be null or empty.");
            }
        }

        /// <summary>
        /// Validates required parameter and throws ValidationException if null
        /// </summary>
        protected void ValidateRequired<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                _logger.LogWarning("Validation failed: {ParameterName} is required", parameterName);
                throw new ValidationException(parameterName, $"Parameter '{parameterName}' is required and cannot be null.");
            }
        }

        /// <summary>
        /// Validates ID parameter and throws ValidationException if invalid
        /// </summary>
        protected void ValidateId(long id, string parameterName = "id")
        {
            if (id <= 0)
            {
                _logger.LogWarning("Validation failed: {ParameterName} must be greater than 0, received: {Value}", parameterName, id);
                throw new ValidationException(parameterName, $"Parameter '{parameterName}' must be greater than 0.");
            }
        }

        /// <summary>
        /// Retorna resposta de sucesso com dados
        /// </summary>
        protected ActionResult<BaseResponse<T>> SuccessResponse<T>(T data, string message = "Operação realizada com sucesso")
        {
            return Ok(new BaseResponse<T>(data, 200, message));
        }

        /// <summary>
        /// Retorna resposta de criação (201) com location header
        /// </summary>
        protected ActionResult<BaseResponse<T>> CreatedResponse<T>(string actionName, object routeValues, T data, string message = "Registro criado com sucesso")
        {
            return CreatedAtAction(actionName, routeValues, new BaseResponse<T>(data, 201, message));
        }

        /// <summary>
        /// Retorna resposta para operações booleanas (delete, update)
        /// </summary>
        protected ActionResult<BaseResponse<bool>> BooleanResponse(bool success, string successMessage, string failureMessage)
        {
            if (success)
            {
                return Ok(new BaseResponse<bool>(true, 200, successMessage));
            }
            else
            {
                return Ok(new BaseResponse<bool>(false, 200, failureMessage));
            }
        }

        /// <summary>
        /// Executa operação assíncrona com tratamento automático e logging de performance
        /// </summary>
        protected async Task<ActionResult<BaseResponse<T>>> ExecuteAsync<T>(
            Func<Task<T>> operation, 
            string message = "Operação realizada com sucesso",
            string operationName = null)
        {
            var sw = Stopwatch.StartNew();
            var op = operationName ?? operation.Method.Name;
            
            _logger.LogInformation("Iniciando operação: {Operation}", op);
            
            var result = await operation();
            
            sw.Stop();
            _logger.LogInformation("Operação {Operation} concluída em {ElapsedMs}ms", op, sw.ElapsedMilliseconds);
            
            return SuccessResponse(result, message);
        }

        /// <summary>
        /// Executa operação de criação com tratamento automático e logging
        /// </summary>
        protected async Task<ActionResult<BaseResponse<T>>> ExecuteCreateAsync<T>(
            Func<Task<T>> operation,
            string actionName,
            Func<T, object> routeValuesFactory,
            string message = "Registro criado com sucesso")
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Criando novo registro via: {Action}", actionName);
            
            var result = await operation();
            var routeValues = routeValuesFactory(result);
            
            sw.Stop();
            _logger.LogInformation("Registro criado com sucesso em {ElapsedMs}ms", sw.ElapsedMilliseconds);
            
            return CreatedResponse(actionName, routeValues, result, message);
        }

        /// <summary>
        /// Executa operação booleana com tratamento automático
        /// </summary>
        protected async Task<ActionResult<BaseResponse<bool>>> ExecuteBooleanAsync(
            Func<Task<bool>> operation,
            string successMessage,
            string failureMessage,
            string operationName = null)
        {
            var op = operationName ?? operation.Method.Name;
            _logger.LogInformation("Executando operação booleana: {Operation}", op);
            
            var result = await operation();
            
            _logger.LogInformation("Operação {Operation} retornou: {Result}", op, result);
            
            return BooleanResponse(result, successMessage, failureMessage);
        }

        /// <summary>
        /// Valida request e executa operação
        /// </summary>
        protected async Task<ActionResult<BaseResponse<T>>> ValidateAndExecuteAsync<T>(Func<Task<T>> operation, string message = "Operação realizada com sucesso")
        {
            ValidateModelState();
            return await ExecuteAsync(operation, message);
        }

        /// <summary>
        /// Valida request e executa operação de criação
        /// </summary>
        protected async Task<ActionResult<BaseResponse<T>>> ValidateAndExecuteCreateAsync<T>(
            Func<Task<T>> operation,
            string actionName,
            Func<T, object> routeValuesFactory,
            string message = "Registro criado com sucesso")
        {
            ValidateModelState();
            return await ExecuteCreateAsync(operation, actionName, routeValuesFactory, message);
        }
    }
}
