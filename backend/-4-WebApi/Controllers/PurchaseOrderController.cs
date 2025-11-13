using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de ordens de compra
/// </summary>
[Authorize]
[ApiController]
[Route("api/purchase-order")]
[Tags("Compras - Ordens de Compra")]
[ApiExplorerSettings(GroupName = "Compras - Ordens de Compra")]
public class PurchaseOrderController : BaseController
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrderController(IPurchaseOrderService service, ILogger<PurchaseOrderController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("purchaseOrder.canView")]
    public async Task<ActionResult<BaseResponse<List<PurchaseOrderOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Ordens de compra listadas com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("purchaseOrder.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<PurchaseOrderOutputDTO>>>> GetPagedAsync([FromQuery] PurchaseOrderFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Ordens de compra listadas com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("purchaseOrder.canView")]
    public async Task<ActionResult<BaseResponse<PurchaseOrderOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Ordem de compra encontrada com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("purchaseOrder.canCreate")]
    public async Task<ActionResult<BaseResponse<PurchaseOrderOutputDTO>>> CreateAsync(PurchaseOrderInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.PurchaseOrderId },
            "Ordem de compra criada com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("purchaseOrder.canEdit")]
    public async Task<ActionResult<BaseResponse<PurchaseOrderOutputDTO>>> UpdateByIdAsync(long id, PurchaseOrderInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Ordem de compra atualizada com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("purchaseOrder.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Ordem de compra deletada com sucesso",
            "Ordem de compra não encontrada ou não pôde ser deletada"
        );
    }
}
