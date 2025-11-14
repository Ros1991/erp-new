using ERP.CrossCutting.Services;
using ERP.Domain.Models;
using ERP.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;

namespace ERP.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModuleConfigurationController : BaseController
    {
        private readonly IModuleConfigurationService _moduleConfigurationService;

        public ModuleConfigurationController(
            IModuleConfigurationService moduleConfigurationService,
            ILogger<ModuleConfigurationController> logger) : base(logger)
        {
            _moduleConfigurationService = moduleConfigurationService;
        }

        /// <summary>
        /// Obtém a configuração completa de módulos e permissões do sistema
        /// </summary>
        [HttpGet]
        public ActionResult<BaseResponse<SystemModulesConfiguration>> GetConfiguration()
        {
            var configuration = _moduleConfigurationService.GetConfiguration();
            return Ok(new BaseResponse<SystemModulesConfiguration>(configuration, 200, "Configuração de módulos obtida com sucesso"));
        }

        /// <summary>
        /// Obtém apenas os módulos ativos
        /// </summary>
        [HttpGet("active")]
        public ActionResult<BaseResponse<List<ModuleConfiguration>>> GetActiveModules()
        {
            var modules = _moduleConfigurationService.GetActiveModules();
            return Ok(new BaseResponse<List<ModuleConfiguration>>(modules, 200, "Módulos ativos obtidos com sucesso"));
        }

        /// <summary>
        /// Obtém um módulo específico por chave
        /// </summary>
        [HttpGet("{moduleKey}")]
        public ActionResult<BaseResponse<ModuleConfiguration>> GetModule(string moduleKey)
        {
            var module = _moduleConfigurationService.GetModule(moduleKey);
            
            if (module == null)
            {
                return NotFound(new BaseResponse<ModuleConfiguration>(400, $"Módulo '{moduleKey}' não encontrado"));
            }

            return Ok(new BaseResponse<ModuleConfiguration>(module, 200, "Módulo obtido com sucesso"));
        }

        /// <summary>
        /// Obtém os endpoints permitidos para uma permissão específica
        /// </summary>
        [HttpGet("{moduleKey}/permissions/{permissionKey}/endpoints")]
        public ActionResult<BaseResponse<List<string>>> GetAllowedEndpoints(string moduleKey, string permissionKey)
        {
            var endpoints = _moduleConfigurationService.GetAllowedEndpoints(moduleKey, permissionKey);
            return Ok(new BaseResponse<List<string>>(endpoints, 200, "Endpoints permitidos obtidos com sucesso"));
        }
    }
}
