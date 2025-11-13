using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.CrossCutting.Helpers;
using ERP.WebApi.Attributes;
using ERP.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebApi.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de transações financeiras
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/financial-transaction")]
    [Tags("Financeiro - Transações Financeiras")]
    [ApiExplorerSettings(GroupName = "Financeiro - Transações Financeiras")]
    public class FinancialTransactionController : BaseController
    {
        private readonly IFinancialTransactionService _financialTransactionService;

        public FinancialTransactionController(IFinancialTransactionService financialTransactionService, ILogger<FinancialTransactionController> logger) : base(logger)
        {
            _financialTransactionService = financialTransactionService;
        }

        [HttpGet("getAll")]
        [RequirePermissions("financialTransaction.canView")]
        public async Task<ActionResult<BaseResponse<List<FinancialTransactionOutputDTO>>>> GetAllAsync()
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(() => _financialTransactionService.GetAllAsync(companyId), "Transações encontradas com sucesso");
        }

        [HttpGet("getPaged")]
        [RequirePermissions("financialTransaction.canView")]
        public async Task<ActionResult<BaseResponse<PagedResult<FinancialTransactionOutputDTO>>>> GetPagedAsync([FromQuery] FinancialTransactionFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(() => _financialTransactionService.GetPagedAsync(companyId, filters), "Transações encontradas com sucesso");
        }

        [HttpGet("{financialTransactionId}")]
        [RequirePermissions("financialTransaction.canView")]
        public async Task<ActionResult<BaseResponse<FinancialTransactionOutputDTO>>> GetOneByIdAsync(long financialTransactionId)
        {
            ValidateId(financialTransactionId, nameof(financialTransactionId));
            return await ExecuteAsync(() => _financialTransactionService.GetOneByIdAsync(financialTransactionId), "Transação encontrada com sucesso");
        }

        [HttpPost("create")]
        [RequirePermissions("financialTransaction.canCreate")]
        public async Task<ActionResult<BaseResponse<FinancialTransactionOutputDTO>>> CreateAsync(FinancialTransactionInputDTO dto)
        {
            var companyId = GetCompanyId();
            var currentUserId = GetCurrentUserId();
            return await ValidateAndExecuteCreateAsync(
                () => _financialTransactionService.CreateAsync(dto, companyId, currentUserId),
                nameof(GetOneByIdAsync),
                result => new { financialTransactionId = result.FinancialTransactionId },
                "Transação criada com sucesso"
            );
        }

        [HttpPut("{financialTransactionId}")]
        [RequirePermissions("financialTransaction.canEdit")]
        public async Task<ActionResult<BaseResponse<FinancialTransactionOutputDTO>>> UpdateByIdAsync(long financialTransactionId, FinancialTransactionInputDTO dto)
        {
            ValidateId(financialTransactionId, nameof(financialTransactionId));
            var currentUserId = GetCurrentUserId();
            return await ValidateAndExecuteAsync(() => _financialTransactionService.UpdateByIdAsync(financialTransactionId, dto, currentUserId), "Transação atualizada com sucesso");
        }

        [HttpDelete("{financialTransactionId}")]
        [RequirePermissions("financialTransaction.canDelete")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long financialTransactionId)
        {
            ValidateId(financialTransactionId, nameof(financialTransactionId));
            return await ValidateAndExecuteAsync(() => _financialTransactionService.DeleteByIdAsync(financialTransactionId), "Transação excluída com sucesso");
        }
    }
}
