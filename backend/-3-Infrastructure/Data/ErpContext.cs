using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CAU.Domain.Entities;

namespace CAU.Infrastructure.Data
{
	public partial class ErpContext : DbContext
	{
		private readonly IHttpContextAccessor httpAccessor;

		public ErpContext(IHttpContextAccessor httpAccessor, DbContextOptions<ErpContext> options):base(options)
		{
			this.httpAccessor = httpAccessor;
		}

		public ErpContext(IHttpContextAccessor httpAccessor)
		{
			this.httpAccessor = httpAccessor;
		}

		partial void OnModelBuilding(ModelBuilder builder);

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configurar esquema padrão como 'erp'
			builder.HasDefaultSchema("erp");

			builder.Entity<Account>()
				.HasOne(i => i.Company)
				.WithMany(i => i.AccountList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<AccountPayableReceivable>()
				.HasOne(i => i.Company)
				.WithMany(i => i.AccountPayableReceivableList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<CompanySetting>()
				.HasOne(i => i.Company)
				.WithMany(i => i.CompanySettingList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<CompanyUser>()
				.HasOne(i => i.Company)
				.WithMany(i => i.CompanyUserList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<CompanyUser>()
				.HasOne(i => i.Role)
				.WithMany(i => i.CompanyUserList)
				.HasForeignKey(i => i.RoleId)
				.HasPrincipalKey(i => i.RoleId);
			builder.Entity<CompanyUser>()
				.HasOne(i => i.User)
				.WithMany(i => i.CompanyUserList)
				.HasForeignKey(i => i.UserId)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<Contract>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.ContractList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<ContractBenefitDiscount>()
				.HasOne(i => i.Contract)
				.WithMany(i => i.ContractBenefitDiscountList)
				.HasForeignKey(i => i.ContractId)
				.HasPrincipalKey(i => i.ContractId);
			builder.Entity<ContractCostCenter>()
				.HasOne(i => i.Contract)
				.WithMany(i => i.ContractCostCenterList)
				.HasForeignKey(i => i.ContractId)
				.HasPrincipalKey(i => i.ContractId);
			builder.Entity<ContractCostCenter>()
				.HasOne(i => i.CostCenter)
				.WithMany(i => i.ContractCostCenterList)
				.HasForeignKey(i => i.CostCenterId)
				.HasPrincipalKey(i => i.CostCenterId);
			builder.Entity<CostCenter>()
				.HasOne(i => i.Company)
				.WithMany(i => i.CostCenterList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<Employee>()
				.HasOne(i => i.Company)
				.WithMany(i => i.EmployeeList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<Employee>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.EmployeeList)
				.HasForeignKey(i => i.EmployeeIdManager)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<Employee>()
				.HasOne(i => i.User)
				.WithMany(i => i.EmployeeList)
				.HasForeignKey(i => i.UserId)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<EmployeeAllowedLocation>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.EmployeeAllowedLocationList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<EmployeeAllowedLocation>()
				.HasOne(i => i.Location)
				.WithMany(i => i.EmployeeAllowedLocationList)
				.HasForeignKey(i => i.LocationId)
				.HasPrincipalKey(i => i.LocationId);
			builder.Entity<FinancialTransaction>()
				.HasOne(i => i.Account)
				.WithMany(i => i.FinancialTransactionList)
				.HasForeignKey(i => i.AccountId)
				.HasPrincipalKey(i => i.AccountId);
			builder.Entity<FinancialTransaction>()
				.HasOne(i => i.AccountPayableReceivable)
				.WithMany(i => i.FinancialTransactionList)
				.HasForeignKey(i => i.AccountPayableReceivableId)
				.HasPrincipalKey(i => i.AccountPayableReceivableId);
			builder.Entity<FinancialTransaction>()
				.HasOne(i => i.Company)
				.WithMany(i => i.FinancialTransactionList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<FinancialTransaction>()
				.HasOne(i => i.PurchaseOrder)
				.WithMany(i => i.FinancialTransactionList)
				.HasForeignKey(i => i.PurchaseOrderId)
				.HasPrincipalKey(i => i.PurchaseOrderId);
			builder.Entity<Justification>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.JustificationList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<Justification>()
				.HasOne(i => i.User)
				.WithMany(i => i.JustificationList)
				.HasForeignKey(i => i.UserIdApprover)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<LoanAdvance>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.LoanAdvanceList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<Location>()
				.HasOne(i => i.Company)
				.WithMany(i => i.LocationList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<Payroll>()
				.HasOne(i => i.Company)
				.WithMany(i => i.PayrollList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<PayrollEmployee>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.PayrollEmployeeList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<PayrollEmployee>()
				.HasOne(i => i.Payroll)
				.WithMany(i => i.PayrollEmployeeList)
				.HasForeignKey(i => i.PayrollId)
				.HasPrincipalKey(i => i.PayrollId);
			builder.Entity<PayrollItem>()
				.HasOne(i => i.PayrollEmployee)
				.WithMany(i => i.PayrollItemList)
				.HasForeignKey(i => i.PayrollEmployeeId)
				.HasPrincipalKey(i => i.PayrollEmployeeId);
			builder.Entity<PurchaseOrder>()
				.HasOne(i => i.Company)
				.WithMany(i => i.PurchaseOrderList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<PurchaseOrder>()
				.HasOne(i => i.User)
				.WithMany(i => i.PurchaseOrderList)
				.HasForeignKey(i => i.UserIdApprover)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<PurchaseOrder>()
				.HasOne(i => i.User)
				.WithMany(i => i.PurchaseOrderList)
				.HasForeignKey(i => i.UserIdRequester)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<Task>()
				.HasOne(i => i.Company)
				.WithMany(i => i.TaskList)
				.HasForeignKey(i => i.CompanyId)
				.HasPrincipalKey(i => i.CompanyId);
			builder.Entity<Task>()
				.HasOne(i => i.Task)
				.WithMany(i => i.TaskList)
				.HasForeignKey(i => i.TaskIdBlocking)
				.HasPrincipalKey(i => i.TaskId);
			builder.Entity<Task>()
				.HasOne(i => i.Task)
				.WithMany(i => i.TaskList)
				.HasForeignKey(i => i.TaskIdParent)
				.HasPrincipalKey(i => i.TaskId);
			builder.Entity<TaskComment>()
				.HasOne(i => i.Task)
				.WithMany(i => i.TaskCommentList)
				.HasForeignKey(i => i.TaskId)
				.HasPrincipalKey(i => i.TaskId);
			builder.Entity<TaskComment>()
				.HasOne(i => i.User)
				.WithMany(i => i.TaskCommentList)
				.HasForeignKey(i => i.UserId)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<TaskEmployee>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.TaskEmployeeList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<TaskEmployee>()
				.HasOne(i => i.Task)
				.WithMany(i => i.TaskEmployeeList)
				.HasForeignKey(i => i.TaskId)
				.HasPrincipalKey(i => i.TaskId);
			builder.Entity<TaskStatusHistory>()
				.HasOne(i => i.TaskEmployee)
				.WithMany(i => i.TaskStatusHistoryList)
				.HasForeignKey(i => i.TaskEmployeeId)
				.HasPrincipalKey(i => i.TaskEmployeeId);
			builder.Entity<TimeEntry>()
				.HasOne(i => i.Employee)
				.WithMany(i => i.TimeEntryList)
				.HasForeignKey(i => i.EmployeeId)
				.HasPrincipalKey(i => i.EmployeeId);
			builder.Entity<TimeEntry>()
				.HasOne(i => i.Location)
				.WithMany(i => i.TimeEntryList)
				.HasForeignKey(i => i.LocationId)
				.HasPrincipalKey(i => i.LocationId);
			builder.Entity<TransactionCostCenter>()
				.HasOne(i => i.CostCenter)
				.WithMany(i => i.TransactionCostCenterList)
				.HasForeignKey(i => i.CostCenterId)
				.HasPrincipalKey(i => i.CostCenterId);
			builder.Entity<TransactionCostCenter>()
				.HasOne(i => i.FinancialTransaction)
				.WithMany(i => i.TransactionCostCenterList)
				.HasForeignKey(i => i.FinancialTransactionId)
				.HasPrincipalKey(i => i.FinancialTransactionId);
			builder.Entity<UserToken>()
				.HasOne(i => i.User)
				.WithMany(i => i.UserTokenList)
				.HasForeignKey(i => i.UserId)
				.HasPrincipalKey(i => i.UserId);
			builder.Entity<Account>()
				.Property(p => p.AccountId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Account>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Account>()
				.Property(p => p.Name)
				.HasColumnType("string");
			builder.Entity<Account>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<Account>()
				.Property(p => p.InitialBalance)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Account>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Account>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Account>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Account>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.AccountPayableReceivableId)
				.HasColumnType("Int64");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.Description)
				.HasColumnType("string");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.Amount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.DueDate)
				.HasColumnType("DateTime");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.IsPaid)
				.HasColumnType("bool");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<AccountPayableReceivable>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Company>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Company>()
				.Property(p => p.Name)
				.HasColumnType("string");
			builder.Entity<Company>()
				.Property(p => p.Document)
				.HasColumnType("string");
			builder.Entity<Company>()
				.Property(p => p.UserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Company>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Company>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Company>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Company>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<CompanySetting>()
				.Property(p => p.CompanySettingId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.EmployeeIdGeneralManager)
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.TimeToleranceMinutes)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.PayrollDay)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.PayrollClosingDay)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.VacationDaysPerYear)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.MinHoursForLunchBreak)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<CompanySetting>()
				.Property(p => p.MaxOvertimeHoursPerMonth)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<CompanySetting>()
				.Property(p => p.AllowWeekendWork)
				.HasColumnType("bool");
			builder.Entity<CompanySetting>()
				.Property(p => p.RequireJustificationAfterHours)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.WeeklyHoursDefault)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<CompanySetting>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<CompanySetting>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<CompanySetting>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<CompanyUser>()
				.Property(p => p.CompanyUserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanyUser>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanyUser>()
				.Property(p => p.UserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanyUser>()
				.Property(p => p.RoleId)
				.HasColumnType("Int64");
			builder.Entity<CompanyUser>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CompanyUser>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<CompanyUser>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<CompanyUser>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Contract>()
				.Property(p => p.ContractId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Contract>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Contract>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<Contract>()
				.Property(p => p.Value)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Contract>()
				.Property(p => p.IsPayroll)
				.HasColumnType("bool");
			builder.Entity<Contract>()
				.Property(p => p.HasInss)
				.HasColumnType("bool");
			builder.Entity<Contract>()
				.Property(p => p.HasIrrf)
				.HasColumnType("bool");
			builder.Entity<Contract>()
				.Property(p => p.HasFgts)
				.HasColumnType("bool");
			builder.Entity<Contract>()
				.Property(p => p.StartDate)
				.HasColumnType("DateTime");
			builder.Entity<Contract>()
				.Property(p => p.EndDate)
				.HasColumnType("DateTime?");
			builder.Entity<Contract>()
				.Property(p => p.WeeklyHours)
				.HasColumnType("Int64");
			builder.Entity<Contract>()
				.Property(p => p.IsActive)
				.HasColumnType("bool");
			builder.Entity<Contract>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Contract>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Contract>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Contract>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.ContractBenefitDiscountId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.ContractId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.Description)
				.HasColumnType("string");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.Application)
				.HasColumnType("string");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.Amount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<ContractBenefitDiscount>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.ContractCostCenterId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.ContractId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.CostCenterId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.Percentage)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<ContractCostCenter>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<CostCenter>()
				.Property(p => p.CostCenterId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CostCenter>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CostCenter>()
				.Property(p => p.Name)
				.HasColumnType("string");
			builder.Entity<CostCenter>()
				.Property(p => p.Description)
				.HasColumnType("string?");
			builder.Entity<CostCenter>()
				.Property(p => p.IsActive)
				.HasColumnType("bool");
			builder.Entity<CostCenter>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<CostCenter>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<CostCenter>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<CostCenter>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Employee>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Employee>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Employee>()
				.Property(p => p.UserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Employee>()
				.Property(p => p.EmployeeIdManager)
				.HasColumnType("Int64");
			builder.Entity<Employee>()
				.Property(p => p.Nickname)
				.HasColumnType("string");
			builder.Entity<Employee>()
				.Property(p => p.FullName)
				.HasColumnType("string");
			builder.Entity<Employee>()
				.Property(p => p.Email)
				.HasColumnType("string");
			builder.Entity<Employee>()
				.Property(p => p.Phone)
				.HasColumnType("string");
			builder.Entity<Employee>()
				.Property(p => p.Cpf)
				.HasColumnType("string");
			builder.Entity<Employee>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Employee>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Employee>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Employee>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.EmployeeAllowedLocationId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.LocationId)
				.HasColumnType("Int64");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<EmployeeAllowedLocation>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.FinancialTransactionId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.AccountId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.PurchaseOrderId)
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.AccountPayableReceivableId)
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.Description)
				.HasColumnType("string");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.Amount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.TransactionDate)
				.HasColumnType("DateTime");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<FinancialTransaction>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Justification>()
				.Property(p => p.JustificationId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Justification>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Justification>()
				.Property(p => p.ReferenceDate)
				.HasColumnType("DateTime");
			builder.Entity<Justification>()
				.Property(p => p.Reason)
				.HasColumnType("string");
			builder.Entity<Justification>()
				.Property(p => p.AttachmentUrl)
				.HasColumnType("string?");
			builder.Entity<Justification>()
				.Property(p => p.HoursGranted)
				.HasColumnType("decimal?");
			builder.Entity<Justification>()
				.Property(p => p.UserIdApprover)
				.HasColumnType("Int64");
			builder.Entity<Justification>()
				.Property(p => p.Status)
				.HasColumnType("string");
			builder.Entity<Justification>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Justification>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Justification>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Justification>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<LoanAdvance>()
				.Property(p => p.LoanAdvanceId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<LoanAdvance>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<LoanAdvance>()
				.Property(p => p.Amount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<LoanAdvance>()
				.Property(p => p.Installments)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<LoanAdvance>()
				.Property(p => p.DiscountSource)
				.HasColumnType("string");
			builder.Entity<LoanAdvance>()
				.Property(p => p.StartDate)
				.HasColumnType("DateTime");
			builder.Entity<LoanAdvance>()
				.Property(p => p.IsApproved)
				.HasColumnType("bool");
			builder.Entity<LoanAdvance>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<LoanAdvance>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<LoanAdvance>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<LoanAdvance>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Location>()
				.Property(p => p.LocationId)
				.HasColumnType("Int64");
			builder.Entity<Location>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Location>()
				.Property(p => p.Name)
				.HasColumnType("string");
			builder.Entity<Location>()
				.Property(p => p.Address)
				.HasColumnType("string?");
			builder.Entity<Location>()
				.Property(p => p.Latitude)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Location>()
				.Property(p => p.Longitude)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Location>()
				.Property(p => p.RadiusMeters)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Location>()
				.Property(p => p.IsActive)
				.HasColumnType("bool");
			builder.Entity<Location>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Location>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Location>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Location>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Payroll>()
				.Property(p => p.PayrollId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Payroll>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Payroll>()
				.Property(p => p.PeriodStartDate)
				.HasColumnType("DateTime");
			builder.Entity<Payroll>()
				.Property(p => p.PeriodEndDate)
				.HasColumnType("DateTime");
			builder.Entity<Payroll>()
				.Property(p => p.TotalGrossPay)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Payroll>()
				.Property(p => p.TotalDeductions)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Payroll>()
				.Property(p => p.TotalNetPay)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<Payroll>()
				.Property(p => p.IsClosed)
				.HasColumnType("bool");
			builder.Entity<Payroll>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Payroll>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Payroll>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Payroll>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.PayrollEmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.PayrollId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.IsOnVacation)
				.HasColumnType("bool");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.VacationDays)
				.HasColumnType("Int64");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.VacationAdvanceAmount)
				.HasColumnType("decimal?");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.TotalGrossPay)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.TotalDeductions)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.TotalNetPay)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<PayrollEmployee>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<PayrollItem>()
				.Property(p => p.PayrollItemId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollItem>()
				.Property(p => p.PayrollEmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollItem>()
				.Property(p => p.Description)
				.HasColumnType("string");
			builder.Entity<PayrollItem>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<PayrollItem>()
				.Property(p => p.Category)
				.HasColumnType("string");
			builder.Entity<PayrollItem>()
				.Property(p => p.Amount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<PayrollItem>()
				.Property(p => p.ReferenceId)
				.HasColumnType("Int64");
			builder.Entity<PayrollItem>()
				.Property(p => p.CalculationBasis)
				.HasColumnType("decimal?");
			builder.Entity<PayrollItem>()
				.Property(p => p.CalculationDetails)
				.HasColumnType("any");
			builder.Entity<PayrollItem>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PayrollItem>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<PayrollItem>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<PayrollItem>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.PurchaseOrderId)
				.HasColumnType("Int64");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.UserIdRequester)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.UserIdApprover)
				.HasColumnType("Int64");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.Description)
				.HasColumnType("string");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.TotalAmount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.Status)
				.HasColumnType("string");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<PurchaseOrder>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Role>()
				.Property(p => p.RoleId)
				.HasColumnType("Int64");
			builder.Entity<Role>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Role>()
				.Property(p => p.Name)
				.HasColumnType("string");
			builder.Entity<Role>()
				.Property(p => p.Permissions)
				.HasColumnType("any");
			builder.Entity<Role>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Role>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Role>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Role>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<Task>()
				.Property(p => p.TaskId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.CompanyId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.TaskIdParent)
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.TaskIdBlocking)
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.Title)
				.HasColumnType("string");
			builder.Entity<Task>()
				.Property(p => p.Description)
				.HasColumnType("string?");
			builder.Entity<Task>()
				.Property(p => p.Priority)
				.HasColumnType("string");
			builder.Entity<Task>()
				.Property(p => p.FrequencyDays)
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.AllowSunday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.AllowMonday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.AllowTuesday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.AllowWednesday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.AllowThursday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.AllowFriday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.AllowSaturday)
				.HasColumnType("bool");
			builder.Entity<Task>()
				.Property(p => p.StartDate)
				.HasColumnType("DateTime?");
			builder.Entity<Task>()
				.Property(p => p.EndDate)
				.HasColumnType("DateTime?");
			builder.Entity<Task>()
				.Property(p => p.OverallStatus)
				.HasColumnType("string");
			builder.Entity<Task>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<Task>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<Task>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<TaskComment>()
				.Property(p => p.TaskCommentId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskComment>()
				.Property(p => p.TaskId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskComment>()
				.Property(p => p.UserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskComment>()
				.Property(p => p.Comment)
				.HasColumnType("string");
			builder.Entity<TaskComment>()
				.Property(p => p.AttachmentUrl)
				.HasColumnType("string?");
			builder.Entity<TaskComment>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskComment>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<TaskComment>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<TaskComment>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<TaskEmployee>()
				.Property(p => p.TaskEmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskEmployee>()
				.Property(p => p.TaskId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskEmployee>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskEmployee>()
				.Property(p => p.Status)
				.HasColumnType("string");
			builder.Entity<TaskEmployee>()
				.Property(p => p.EstimatedHours)
				.HasColumnType("decimal?");
			builder.Entity<TaskEmployee>()
				.Property(p => p.ActualHours)
				.HasColumnType("decimal?");
			builder.Entity<TaskEmployee>()
				.Property(p => p.StartDate)
				.HasColumnType("string?");
			builder.Entity<TaskEmployee>()
				.Property(p => p.EndDate)
				.HasColumnType("string?");
			builder.Entity<TaskEmployee>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskEmployee>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<TaskEmployee>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<TaskEmployee>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.TaskStatusHistoryId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.TaskEmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.OldStatus)
				.HasColumnType("string?");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.NewStatus)
				.HasColumnType("string");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.ChangeReason)
				.HasColumnType("string?");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<TaskStatusHistory>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<TimeEntry>()
				.Property(p => p.TimeEntryId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TimeEntry>()
				.Property(p => p.EmployeeId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TimeEntry>()
				.Property(p => p.Type)
				.HasColumnType("string");
			builder.Entity<TimeEntry>()
				.Property(p => p.Timestamp)
				.HasColumnType("string");
			builder.Entity<TimeEntry>()
				.Property(p => p.Latitude)
				.HasColumnType("decimal?");
			builder.Entity<TimeEntry>()
				.Property(p => p.Longitude)
				.HasColumnType("decimal?");
			builder.Entity<TimeEntry>()
				.Property(p => p.LocationId)
				.HasColumnType("Int64");
			builder.Entity<TimeEntry>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TimeEntry>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<TimeEntry>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<TimeEntry>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.TransactionCostCenterId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.FinancialTransactionId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.CostCenterId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.Amount)
				.HasDefaultValueSql("((0))")
				.HasColumnType("decimal");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.CriadoPor)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.AtualizadoPor)
				.HasColumnType("Int64");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.CriadoEm)
				.HasColumnType("string");
			builder.Entity<TransactionCostCenter>()
				.Property(p => p.AtualizadoEm)
				.HasColumnType("string?");
			builder.Entity<User>()
				.Property(p => p.UserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<User>()
				.Property(p => p.Email)
				.HasColumnType("string");
			builder.Entity<User>()
				.Property(p => p.Phone)
				.HasColumnType("string");
			builder.Entity<User>()
				.Property(p => p.Cpf)
				.HasColumnType("string");
			builder.Entity<User>()
				.Property(p => p.PasswordHash)
				.HasColumnType("string");
			builder.Entity<User>()
				.Property(p => p.ResetToken)
				.HasColumnType("string?");
			builder.Entity<User>()
				.Property(p => p.ResetTokenExpiresAt)
				.HasColumnType("string?");
			builder.Entity<UserToken>()
				.Property(p => p.UserTokenId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<UserToken>()
				.Property(p => p.UserId)
				.HasDefaultValueSql("((0))")
				.HasColumnType("Int64");
			builder.Entity<UserToken>()
				.Property(p => p.Token)
				.HasColumnType("string");
			builder.Entity<UserToken>()
				.Property(p => p.RefreshToken)
				.HasColumnType("string?");
			builder.Entity<UserToken>()
				.Property(p => p.ExpiresAt)
				.HasColumnType("string");
			builder.Entity<UserToken>()
				.Property(p => p.RefreshExpiresAt)
				.HasColumnType("string?");
			builder.Entity<UserToken>()
				.Property(p => p.IsRevoked)
				.HasColumnType("bool");
			this.OnModelBuilding(builder);
		}
		
		public DbSet<Account> Account { get; set; }
		public DbSet<AccountPayableReceivable> AccountPayableReceivable { get; set; }
		public DbSet<Company> Company { get; set; }
		public DbSet<CompanySetting> CompanySetting { get; set; }
		public DbSet<CompanyUser> CompanyUser { get; set; }
		public DbSet<Contract> Contract { get; set; }
		public DbSet<ContractBenefitDiscount> ContractBenefitDiscount { get; set; }
		public DbSet<ContractCostCenter> ContractCostCenter { get; set; }
		public DbSet<CostCenter> CostCenter { get; set; }
		public DbSet<Employee> Employee { get; set; }
		public DbSet<EmployeeAllowedLocation> EmployeeAllowedLocation { get; set; }
		public DbSet<FinancialTransaction> FinancialTransaction { get; set; }
		public DbSet<Justification> Justification { get; set; }
		public DbSet<LoanAdvance> LoanAdvance { get; set; }
		public DbSet<Location> Location { get; set; }
		public DbSet<Payroll> Payroll { get; set; }
		public DbSet<PayrollEmployee> PayrollEmployee { get; set; }
		public DbSet<PayrollItem> PayrollItem { get; set; }
		public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
		public DbSet<Role> Role { get; set; }
		public DbSet<Task> Task { get; set; }
		public DbSet<TaskComment> TaskComment { get; set; }
		public DbSet<TaskEmployee> TaskEmployee { get; set; }
		public DbSet<TaskStatusHistory> TaskStatusHistory { get; set; }
		public DbSet<TimeEntry> TimeEntry { get; set; }
		public DbSet<TransactionCostCenter> TransactionCostCenter { get; set; }
		public DbSet<User> User { get; set; }
		public DbSet<UserToken> UserToken { get; set; }
	}
}
