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
/// Controller para gerenciamento de fornecedores e clientes
/// </summary>
[Authorize]
[ApiController]
[Route("api/supplier-customer")]
[Tags("Cadastros - Fornecedores/Clientes")]
[ApiExplorerSettings(GroupName = "Cadastros - Fornecedores/Clientes")]
public class SupplierCustomerController : BaseController
{
    private readonly ISupplierCustomerService _service;

    public SupplierCustomerController(ISupplierCustomerService service, ILogger<SupplierCustomerController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("supplierCustomer.canView")]
    public async Task<ActionResult<BaseResponse<List<SupplierCustomerOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Fornecedores/Clientes listados com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("supplierCustomer.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<SupplierCustomerOutputDTO>>>> GetPagedAsync([FromQuery] SupplierCustomerFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Fornecedores/Clientes listados com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("supplierCustomer.canView")]
    public async Task<ActionResult<BaseResponse<SupplierCustomerOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Fornecedor e Cliente encontrado com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("supplierCustomer.canCreate")]
    public async Task<ActionResult<BaseResponse<SupplierCustomerOutputDTO>>> CreateAsync(SupplierCustomerInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.SupplierCustomerId },
            "Fornecedor e Cliente criado com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("supplierCustomer.canEdit")]
    public async Task<ActionResult<BaseResponse<SupplierCustomerOutputDTO>>> UpdateByIdAsync(long id, SupplierCustomerInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Fornecedor e Cliente atualizado com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("supplierCustomer.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Fornecedor e Cliente deletado com sucesso",
            "Fornecedor e Cliente não encontrado ou não pôde ser deletado"
        );
    }
}
