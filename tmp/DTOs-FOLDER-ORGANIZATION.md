# Organização dos DTOs em Pastas

## 📁 Nova Estrutura

Os DTOs foram organizados em pastas por módulo, mantendo o namespace único `ERP.Application.DTOs`:

```
backend/-2-Application/DTOs/
├── Account/
│   ├── AccountFilterDTO.cs
│   ├── AccountInputDTO.cs
│   └── AccountOutputDTO.cs
├── Auth/
│   ├── AuthResponseDTO.cs
│   ├── ForgotPasswordRequestDTO.cs
│   ├── LoginRequestDTO.cs
│   ├── RegisterRequestDTO.cs
│   └── ResetPasswordRequestDTO.cs
├── Base/
│   ├── PagedRequest.cs
│   ├── PagedResult.cs
│   └── BaseResponse.cs
├── Company/
│   ├── CompanyFilterDTO.cs
│   ├── CompanyInputDTO.cs
│   └── CompanyOutputDTO.cs
├── CompanyUser/
│   ├── CompanyUserFilterDTO.cs
│   ├── CompanyUserInputDTO.cs
│   └── CompanyUserOutputDTO.cs
├── Employee/
│   ├── EmployeeFilterDTO.cs
│   ├── EmployeeInputDTO.cs
│   └── EmployeeOutputDTO.cs
├── Role/
│   ├── RoleFilterDTO.cs
│   ├── RoleInputDTO.cs
│   └── RoleOutputDTO.cs
└── User/
    ├── UserFilterDTO.cs
    ├── UserInputDTO.cs
    └── UserOutputDTO.cs
```

## 🔑 Namespace Único

**TODOS os DTOs** usam o mesmo namespace: `ERP.Application.DTOs`

**Por quê?**
- ✅ Sem necessidade de atualizar referências em todo o código
- ✅ Import único: `using ERP.Application.DTOs;`
- ✅ Organização visual por pastas
- ✅ Facilita navegação no VS Code/Visual Studio
- ✅ Mantém compatibilidade total

## 📝 Exemplo de DTO

**Arquivo:** `backend/-2-Application/DTOs/Account/AccountFilterDTO.cs`

```csharp
using ERP.Application.DTOs.Base;

namespace ERP.Application.DTOs  // ← Namespace único para todos!
{
    /// <summary>
    /// Filtros específicos para Account
    /// </summary>
    public class AccountFilterDTO : PagedRequest
    {
        
    }
}
```

## 🔗 Referências nos Outros Arquivos

**Nenhuma alteração necessária!**

### Mappers (exemplo)
```csharp
using ERP.Application.DTOs;  // ← Já funciona!
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class AccountMapper
    {
        public static AccountOutputDTO ToAccountOutputDTO(Account entity)
        {
            // ...
        }
    }
}
```

### Services (exemplo)
```csharp
using ERP.Application.DTOs;  // ← Já funciona!

namespace ERP.Application.Services
{
    public class AccountService : IAccountService
    {
        public async Task<AccountOutputDTO> GetByIdAsync(long id)
        {
            // ...
        }
    }
}
```

### Controllers (exemplo)
```csharp
using ERP.Application.DTOs;  // ← Já funciona!
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<AccountOutputDTO>>> GetByIdAsync(long id)
        {
            // ...
        }
    }
}
```

## ✅ Benefícios

1. **Organização Visual**
   - Fácil encontrar DTOs relacionados
   - Estrutura clara por módulo
   - Melhor navegação no explorador de arquivos

2. **Sem Breaking Changes**
   - Namespace único mantém compatibilidade
   - Nenhuma referência precisa ser atualizada
   - Código antigo continua funcionando

3. **Escalabilidade**
   - Fácil adicionar novos módulos
   - Padrão claro para novos DTOs
   - Consistência em todo o projeto

4. **Manutenção**
   - Arquivos relacionados ficam juntos
   - Fácil refatoração futura
   - Menos chance de conflitos em merges

## 📋 Padrão para Novos Módulos

Ao criar um novo módulo (ex: `Product`):

1. **Criar pasta:**
   ```
   backend/-2-Application/DTOs/Product/
   ```

2. **Criar DTOs com namespace único:**
   ```csharp
   namespace ERP.Application.DTOs  // ← Sempre este namespace!
   {
       public class ProductFilterDTO : PagedRequest { }
       public class ProductInputDTO { }
       public class ProductOutputDTO { }
   }
   ```

3. **Usar normalmente:**
   ```csharp
   using ERP.Application.DTOs;  // ← Import único!
   
   public class ProductService
   {
       public async Task<ProductOutputDTO> GetByIdAsync(long id)
       {
           // Acesso direto a todos os DTOs!
       }
   }
   ```

## 🎯 Resultado Final

- ✅ **8 módulos** organizados em pastas
- ✅ **23 arquivos** de DTOs reorganizados
- ✅ **0 referências** quebradas
- ✅ **Compilação** OK (apenas warnings de nullability)
- ✅ **Compatibilidade** 100% mantida

## 🚀 Como Usar

**Nada muda no dia a dia!**

```csharp
// Continua importando apenas:
using ERP.Application.DTOs;

// E tendo acesso a TODOS os DTOs:
AccountFilterDTO
AccountInputDTO
AccountOutputDTO
CompanyFilterDTO
CompanyInputDTO
CompanyOutputDTO
EmployeeFilterDTO
EmployeeInputDTO
EmployeeOutputDTO
RoleFilterDTO
RoleInputDTO
RoleOutputDTO
UserFilterDTO
UserInputDTO
UserOutputDTO
// ... e todos os outros!
```

---

**Data:** 12/11/2025  
**Motivação:** Organização visual sem breaking changes  
**Status:** ✅ Completo e testado
