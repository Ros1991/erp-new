using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;
using System.Net;

namespace ERP.CrossCutting.Filters
{
    /// <summary>
    /// Global exception filter for centralized error handling across all controllers
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = CreateErrorResponse(context.Exception);
            
            // Log the exception
            LogException(context.Exception, context);

            context.Result = new ObjectResult(response)
            {
                StatusCode = response.Code
            };

            context.ExceptionHandled = true;
        }

        private BaseResponse<object> CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                EntityNotFoundException ex => new BaseResponse<object>
                (
                    code: 404,
                    message: ex.Message
                ),

                ValidationException ex => new BaseResponse<object>
                (
                    code: 400,
                    message: ex.Message,
                    errors: ex.Errors
                ),

                DuplicateEntityException ex => new BaseResponse<object>
                (
                    code: 409,
                    message: ex.Message
                ),

                BusinessRuleException ex => new BaseResponse<object>
                (
                    code: 400,
                    message: ex.Message
                ),

                UnauthorizedAccessException ex => new BaseResponse<object>
                (
                    code: 401,
                    message: ex.Message
                ),

                ForbiddenOperationException ex => new BaseResponse<object>
                (
                    code: 403,
                    message: ex.Message
                ),

                ArgumentNullException ex => new BaseResponse<object>
                (
                    code: 400,
                    message: "Parâmetro inválido fornecido."
                ),

                ArgumentException ex => new BaseResponse<object>
                (
                    code: 400,
                    message: ex.Message
                ),

                KeyNotFoundException ex => new BaseResponse<object>
                (
                    code: 404,
                    message: ex.Message
                ),

                InvalidOperationException ex => new BaseResponse<object>
                (
                    code: 409,
                    message: ex.Message
                ),

                _ => new BaseResponse<object>
                (
                    code: 500,
                    message: "Ocorreu um erro interno no servidor."
                )
            };
        }

        private void LogException(Exception exception, ExceptionContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var requestId = context.HttpContext.TraceIdentifier;

            switch (exception)
            {
                case EntityNotFoundException:
                case ValidationException:
                case BusinessRuleException:
                case ArgumentException:
                case KeyNotFoundException:
                    _logger.LogWarning(exception, 
                        "Client error in action {ActionName} (RequestId: {RequestId}): {Message}", 
                        actionName, requestId, exception.Message);
                    break;

                case DuplicateEntityException:
                case InvalidOperationException:
                    _logger.LogWarning(exception, 
                        "Conflict error in action {ActionName} (RequestId: {RequestId}): {Message}", 
                        actionName, requestId, exception.Message);
                    break;

                case UnauthorizedAccessException:
                case ForbiddenOperationException:
                    _logger.LogWarning(exception, 
                        "Security error in action {ActionName} (RequestId: {RequestId}): {Message}", 
                        actionName, requestId, exception.Message);
                    break;

                default:
                    _logger.LogError(exception, 
                        "Unhandled exception in action {ActionName} (RequestId: {RequestId})", 
                        actionName, requestId);
                    break;
            }
        }

    }
}
