using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.CrossCutting.Exceptions;
using ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Application.Services
{
    public class CompanySettingService : ICompanySettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanySettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CompanySettingOutputDTO> GetByCompanyIdAsync(long companyId)
        {
            var entity = await _unitOfWork.CompanySettingRepository.GetByCompanyIdAsync(companyId);
            
            if (entity == null)
            {
                // Retornar configurações padrão se não existir
                return new CompanySettingOutputDTO
                {
                    CompanyId = companyId,
                    TimeToleranceMinutes = 10,
                    PayrollDay = 5,
                    PayrollClosingDay = 25,
                    VacationDaysPerYear = 30,
                    MinHoursForLunchBreak = 6,
                    MaxOvertimeHoursPerMonth = 44,
                    AllowWeekendWork = false,
                    RequireJustificationAfterHours = 2,
                    WeeklyHoursDefault = 44,
                    DefaultCostCenterDistributions = new List<DefaultCostCenterDistributionDTO>()
                };
            }

            // Carregar rateio padrão
            var distributions = await _unitOfWork.DefaultCostCenterDistributionRepository.GetByCompanyIdAsync(companyId);

            return new CompanySettingOutputDTO
            {
                CompanySettingId = entity.CompanySettingId,
                CompanyId = entity.CompanyId,
                EmployeeIdGeneralManager = entity.EmployeeIdGeneralManager,
                TimeToleranceMinutes = entity.TimeToleranceMinutes,
                PayrollDay = entity.PayrollDay,
                PayrollClosingDay = entity.PayrollClosingDay,
                VacationDaysPerYear = entity.VacationDaysPerYear,
                MinHoursForLunchBreak = entity.MinHoursForLunchBreak,
                MaxOvertimeHoursPerMonth = entity.MaxOvertimeHoursPerMonth,
                AllowWeekendWork = entity.AllowWeekendWork,
                RequireJustificationAfterHours = entity.RequireJustificationAfterHours,
                WeeklyHoursDefault = entity.WeeklyHoursDefault,
                DefaultCostCenterDistributions = distributions.Select(d => new DefaultCostCenterDistributionDTO
                {
                    DefaultCostCenterDistributionId = d.DefaultCostCenterDistributionId,
                    CostCenterId = d.CostCenterId,
                    CostCenterName = d.CostCenter?.Name,
                    Percentage = d.Percentage
                }).ToList(),
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public async Task<CompanySettingOutputDTO> SaveAsync(CompanySettingInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar rateio padrão se informado
            if (dto.DefaultCostCenterDistributions != null && dto.DefaultCostCenterDistributions.Count > 0)
            {
                var totalPercentage = dto.DefaultCostCenterDistributions.Sum(d => d.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("DefaultCostCenterDistributions", "A soma das porcentagens deve ser 100%.");
                }
            }

            var now = DateTime.UtcNow;
            var existingEntity = await _unitOfWork.CompanySettingRepository.GetByCompanyIdAsync(companyId);

            CompanySetting entity;
            if (existingEntity == null)
            {
                // Criar novo
                entity = new CompanySetting
                {
                    CompanyId = companyId,
                    EmployeeIdGeneralManager = dto.EmployeeIdGeneralManager,
                    TimeToleranceMinutes = dto.TimeToleranceMinutes,
                    PayrollDay = dto.PayrollDay,
                    PayrollClosingDay = dto.PayrollClosingDay,
                    VacationDaysPerYear = dto.VacationDaysPerYear,
                    MinHoursForLunchBreak = dto.MinHoursForLunchBreak,
                    MaxOvertimeHoursPerMonth = dto.MaxOvertimeHoursPerMonth,
                    AllowWeekendWork = dto.AllowWeekendWork,
                    RequireJustificationAfterHours = dto.RequireJustificationAfterHours,
                    WeeklyHoursDefault = dto.WeeklyHoursDefault,
                    CriadoPor = currentUserId,
                    CriadoEm = now
                };
                await _unitOfWork.CompanySettingRepository.CreateAsync(entity);
            }
            else
            {
                // Atualizar existente
                existingEntity.EmployeeIdGeneralManager = dto.EmployeeIdGeneralManager;
                existingEntity.TimeToleranceMinutes = dto.TimeToleranceMinutes;
                existingEntity.PayrollDay = dto.PayrollDay;
                existingEntity.PayrollClosingDay = dto.PayrollClosingDay;
                existingEntity.VacationDaysPerYear = dto.VacationDaysPerYear;
                existingEntity.MinHoursForLunchBreak = dto.MinHoursForLunchBreak;
                existingEntity.MaxOvertimeHoursPerMonth = dto.MaxOvertimeHoursPerMonth;
                existingEntity.AllowWeekendWork = dto.AllowWeekendWork;
                existingEntity.RequireJustificationAfterHours = dto.RequireJustificationAfterHours;
                existingEntity.WeeklyHoursDefault = dto.WeeklyHoursDefault;
                existingEntity.AtualizadoPor = currentUserId;
                existingEntity.AtualizadoEm = now;
                entity = existingEntity;
            }

            // Atualizar rateio padrão (delete all + insert)
            await _unitOfWork.DefaultCostCenterDistributionRepository.DeleteByCompanyIdAsync(companyId);
            
            if (dto.DefaultCostCenterDistributions != null && dto.DefaultCostCenterDistributions.Count > 0)
            {
                foreach (var dist in dto.DefaultCostCenterDistributions)
                {
                    var distribution = new DefaultCostCenterDistribution
                    {
                        CompanyId = companyId,
                        CostCenterId = dist.CostCenterId,
                        Percentage = dist.Percentage,
                        CriadoPor = currentUserId,
                        CriadoEm = now
                    };
                    await _unitOfWork.DefaultCostCenterDistributionRepository.CreateAsync(distribution);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return await GetByCompanyIdAsync(companyId);
        }

        public async Task<List<DefaultCostCenterDistributionDTO>> GetDefaultDistributionsAsync(long companyId)
        {
            var distributions = await _unitOfWork.DefaultCostCenterDistributionRepository.GetByCompanyIdAsync(companyId);
            
            return distributions.Select(d => new DefaultCostCenterDistributionDTO
            {
                DefaultCostCenterDistributionId = d.DefaultCostCenterDistributionId,
                CostCenterId = d.CostCenterId,
                CostCenterName = d.CostCenter?.Name,
                Percentage = d.Percentage
            }).ToList();
        }
    }
}
