using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ERP.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ErpContext _context;

        public EmployeeRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync(long companyId)
        {
            return await _context.Set<Employee>()
                .Include(e => e.EmployeeManager)
                .Include(e => e.User)
                .Where(e => e.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(long companyId, EmployeeFilterDTO filters)
        {
            var query = _context.Set<Employee>()
                .Include(e => e.EmployeeManager)
                .Include(e => e.User)
                .Where(e => e.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var searchLower = filters.Search.ToLower();
                var onlyDigits = Regex.Replace(searchLower, @"[^\d]", "");
                
                query = query.Where(e => 
                    e.Nickname.ToLower().Contains(searchLower) || 
                    e.FullName.ToLower().Contains(searchLower) ||
                    (e.Email != null && e.Email.ToLower().Contains(searchLower)) ||
                    // Busca em Phone e CPF apenas se houver dígitos
                    (!string.IsNullOrEmpty(onlyDigits) && e.Phone != null && e.Phone.Contains(onlyDigits)) ||
                    (!string.IsNullOrEmpty(onlyDigits) && e.Cpf != null && e.Cpf.Contains(onlyDigits))
                );
            }
            
            // Filtros específicos
            if (!string.IsNullOrWhiteSpace(filters.Nickname))
            {
                query = query.Where(e => e.Nickname.ToLower().Contains(filters.Nickname.ToLower()));
            }
            
            if (!string.IsNullOrWhiteSpace(filters.FullName))
            {
                query = query.Where(e => e.FullName.ToLower().Contains(filters.FullName.ToLower()));
            }
            
            if (!string.IsNullOrWhiteSpace(filters.Email))
            {
                query = query.Where(e => e.Email != null && e.Email.ToLower().Contains(filters.Email.ToLower()));
            }
            
            if (!string.IsNullOrWhiteSpace(filters.Phone))
            {
                var phoneDigits = Regex.Replace(filters.Phone, @"[^\d]", "");
                if (!string.IsNullOrEmpty(phoneDigits))
                {
                    query = query.Where(e => e.Phone != null && e.Phone.Contains(phoneDigits));
                }
            }
            
            if (!string.IsNullOrWhiteSpace(filters.Cpf))
            {
                var cpfDigits = Regex.Replace(filters.Cpf, @"[^\d]", "");
                if (!string.IsNullOrEmpty(cpfDigits))
                {
                    query = query.Where(e => e.Cpf != null && e.Cpf.Contains(cpfDigits));
                }
            }
            
            if (filters.EmployeeIdManager.HasValue)
            {
                query = query.Where(e => e.EmployeeIdManager == filters.EmployeeIdManager.Value);
            }
            
            if (filters.UserId.HasValue)
            {
                query = query.Where(e => e.UserId == filters.UserId.Value);
            }

            // Contar total antes da paginação
            var total = await query.CountAsync();

            // Aplicar ordenação dinâmica
            if (!string.IsNullOrWhiteSpace(filters.OrderBy))
            {
                query = query.OrderByProperty(filters.OrderBy, filters.IsAscending);
            }
            else
            {
                query = query.OrderBy(e => e.FullName); // Ordenação padrão por nome
            }

            // Aplicar paginação
            var skip = (filters.Page - 1) * filters.PageSize;
            var items = await query
                .Skip(skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<Employee>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<Employee> GetOneByIdAsync(long employeeId)
        {
            return await _context.Set<Employee>()
                .Include(e => e.EmployeeManager)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> CreateAsync(Employee entity)
        {
            await _context.Set<Employee>().AddAsync(entity);
            return entity;
        }

        public async Task<Employee> UpdateByIdAsync(long employeeId, Employee entity)
        {
            if (employeeId <= 0)
                throw new ValidationException(nameof(employeeId), "EmployeeId deve ser maior que zero.");

            var existing = await _context.Set<Employee>().FindAsync(employeeId);
            
            if (existing == null)
                throw new EntityNotFoundException("Employee", employeeId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long employeeId)
        {
            if (employeeId <= 0)
                throw new ValidationException(nameof(employeeId), "EmployeeId deve ser maior que zero.");

            var existing = await _context.Set<Employee>().FindAsync(employeeId);
            
            if (existing == null)
                throw new EntityNotFoundException("Employee", employeeId);

            _context.Set<Employee>().Remove(existing);
            return true;
        }
    }
}
