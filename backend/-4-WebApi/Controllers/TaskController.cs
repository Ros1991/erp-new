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
/// Controller para gerenciamento de tarefas
/// </summary>
[Authorize]
[ApiController]
[Route("api/task")]
[Tags("Tarefas - Gerenciamento")]
[ApiExplorerSettings(GroupName = "Tarefas - Gerenciamento")]
public class TaskController : BaseController
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service, ILogger<TaskController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("task.canView")]
    public async Task<ActionResult<BaseResponse<List<TaskOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Tarefas listadas com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("task.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<TaskOutputDTO>>>> GetPagedAsync([FromQuery] TaskFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Tarefas listadas com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("task.canView")]
    public async Task<ActionResult<BaseResponse<TaskOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Tarefa encontrada com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("task.canCreate")]
    public async Task<ActionResult<BaseResponse<TaskOutputDTO>>> CreateAsync(TaskInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.TaskId },
            "Tarefa criada com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("task.canEdit")]
    public async Task<ActionResult<BaseResponse<TaskOutputDTO>>> UpdateByIdAsync(long id, TaskInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Tarefa atualizada com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("task.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Tarefa deletada com sucesso",
            "Tarefa não encontrada ou não pôde ser deletada"
        );
    }
}
