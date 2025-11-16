# 🔗 Sistema de Associação de Usuário a Empregado

## 📋 Objetivo

Permitir que empregados sejam associados a usuários do sistema, possibilitando:
1. Busca automática de usuário por email, telefone ou CPF
2. Criação de novo usuário caso não exista
3. Vinculação automática a empresa (CompanyUser) com cargo
4. Desassociação com opção de remover acesso à empresa

---

## 🗺️ Estrutura do Banco de Dados

### **Tabelas Envolvidas:**

```sql
tb_employee (
    employee_id BIGINT PRIMARY KEY,
    company_id BIGINT NOT NULL,
    user_id BIGINT NULL,  -- ← FK para tb_user
    employee_nickname VARCHAR(100) NOT NULL,
    employee_email VARCHAR(255) NULL,
    employee_phone VARCHAR(20) NULL,
    employee_cpf VARCHAR(11) NULL
)

tb_user (
    user_id BIGINT PRIMARY KEY,
    user_email VARCHAR(255) NULL,
    user_phone VARCHAR(20) NULL,
    user_cpf VARCHAR(11) NULL,
    user_password_hash VARCHAR(255) NOT NULL
)
-- Constraint: Pelo menos 1 identificador obrigatório (email OU phone OU cpf)

tb_company_user (
    company_user_id BIGINT PRIMARY KEY,
    company_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL,
    role_id BIGINT NULL
)
-- Unique: (company_id, user_id)
```

### **Relacionamentos:**
- `Employee.UserId` → `User.UserId` (nullable, allow multiple employees per user)
- `CompanyUser.UserId` → `User.UserId` (required)
- `CompanyUser.CompanyId` → `Company.CompanyId` (required)

---

## 🎯 Fluxos Funcionais

### **Fluxo 1: Associar Usuário (Employee SEM user_id)**

**1. Usuário clica em "Associar Usuário"**
   - Sistema busca automaticamente por:
     - `User.Email = Employee.Email` OR
     - `User.Phone = Employee.Phone` OR
     - `User.Cpf = Employee.Cpf`

**2A. Usuário ENCONTRADO + JÁ associado à empresa**
   - Exibir alert:
     ```
     ✅ Usuário encontrado!
     
     Email: usuario@exemplo.com
     Cargo atual: Gerente de Vendas
     
     Deseja associar este usuário ao empregado [Nickname]?
     
     [Cancelar] [Confirmar]
     ```
   - Se confirmar:
     - `UPDATE tb_employee SET user_id = [found_user_id]`

**2B. Usuário ENCONTRADO + NÃO associado à empresa**
   - Exibir dialog de seleção de cargo:
     ```
     ✅ Usuário encontrado, mas não tem acesso a esta empresa!
     
     Email: usuario@exemplo.com
     
     Selecione um cargo para dar acesso:
     [Dropdown de Cargos: Vendedor, Gerente, etc.]
     
     [Cancelar] [Confirmar]
     ```
   - Se confirmar:
     - `INSERT INTO tb_company_user (company_id, user_id, role_id, ...)`
     - `UPDATE tb_employee SET user_id = [found_user_id]`

**2C. Usuário NÃO ENCONTRADO**
   - Exibir dialog de criação de usuário:
     ```
     ❌ Nenhum usuário encontrado com os dados do empregado.
     
     Criar novo usuário:
     
     Email: [employee.email] (pré-preenchido)
     Telefone: [employee.phone] (pré-preenchido)
     CPF: [employee.cpf] (pré-preenchido)
     
     Senha: [input]
     Confirmar Senha: [input]
     
     Cargo: [Dropdown de Cargos]
     
     [Cancelar] [Criar e Associar]
     ```
   - Se confirmar:
     - `INSERT INTO tb_user (email, phone, cpf, password_hash)`
     - `INSERT INTO tb_company_user (company_id, user_id, role_id, ...)`
     - `UPDATE tb_employee SET user_id = [new_user_id]`

---

### **Fluxo 2: Desassociar Usuário (Employee COM user_id)**

**1. Usuário clica em "Desassociar Usuário"**
   - Exibir alert:
     ```
     ⚠️ Desassociar usuário do empregado?
     
     Empregado: [Nickname]
     Usuário: [Email ou Phone ou CPF]
     
     ❓ Deseja também REMOVER o acesso deste usuário à empresa?
     
     [Cancelar] [Apenas Desassociar] [Remover Acesso]
     ```

**2A. Apenas Desassociar:**
   - `UPDATE tb_employee SET user_id = NULL`
   - Usuário continua em `tb_company_user` (mantém acesso à empresa)

**2B. Remover Acesso:**
   - `UPDATE tb_employee SET user_id = NULL`
   - `DELETE FROM tb_company_user WHERE user_id = [user_id] AND company_id = [company_id]`
   - Usuário perde acesso à empresa

---

## 🛠️ Implementação Backend

### **1. Novos Endpoints (EmployeeController.cs)**

```csharp
[HttpPost("{employeeId}/searchUser")]
[RequirePermissions("employee.canEdit")]
public async Task<ActionResult<BaseResponse<UserSearchResultDTO>>> SearchUserForEmployee(long employeeId)
{
    // Busca usuário por email, phone ou cpf do employee
    // Retorna: usuário encontrado + se já tem acesso à empresa + cargo atual
}

[HttpPost("{employeeId}/associateUser")]
[RequirePermissions("employee.canEdit")]
public async Task<ActionResult<BaseResponse<EmployeeOutputDTO>>> AssociateUser(
    long employeeId, 
    AssociateUserDTO dto
)
{
    // dto.UserId: ID do usuário a ser associado
    // dto.RoleId: Cargo (se precisar criar CompanyUser)
    // dto.CreateCompanyUser: bool (se precisa criar associação com empresa)
}

[HttpPost("{employeeId}/createAndAssociateUser")]
[RequirePermissions("employee.canEdit")]
public async Task<ActionResult<BaseResponse<EmployeeOutputDTO>>> CreateAndAssociateUser(
    long employeeId,
    CreateUserAndAssociateDTO dto
)
{
    // dto.Email, Phone, Cpf
    // dto.Password
    // dto.RoleId
    // Cria User + CompanyUser + Associa a Employee
}

[HttpPost("{employeeId}/disassociateUser")]
[RequirePermissions("employee.canEdit")]
public async Task<ActionResult<BaseResponse<EmployeeOutputDTO>>> DisassociateUser(
    long employeeId,
    DisassociateUserDTO dto
)
{
    // dto.RemoveCompanyAccess: bool
    // Se true, remove de CompanyUser também
}
```

### **2. Novos DTOs**

```csharp
// DTOs/Employee/UserSearchResultDTO.cs
public class UserSearchResultDTO
{
    public long? UserId { get; set; }  // null se não encontrou
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Cpf { get; set; }
    public bool HasCompanyAccess { get; set; }
    public long? CurrentRoleId { get; set; }
    public string? CurrentRoleName { get; set; }
}

// DTOs/Employee/AssociateUserDTO.cs
public class AssociateUserDTO
{
    [Required(ErrorMessage = "ID do usuário é obrigatório")]
    public long UserId { get; set; }
    
    public long? RoleId { get; set; }  // Obrigatório se CreateCompanyUser = true
    
    public bool CreateCompanyUser { get; set; }  // Se precisa criar vínculo com empresa
}

// DTOs/Employee/CreateUserAndAssociateDTO.cs
public class CreateUserAndAssociateDTO
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Cpf { get; set; }
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Cargo é obrigatório")]
    public long RoleId { get; set; }
}

// DTOs/Employee/DisassociateUserDTO.cs
public class DisassociateUserDTO
{
    public bool RemoveCompanyAccess { get; set; }
}
```

### **3. Service Methods (EmployeeService.cs)**

```csharp
// Busca usuário por dados do employee
public async Task<UserSearchResultDTO> SearchUserForEmployeeAsync(long employeeId, long companyId)
{
    var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
    if (employee == null) throw new EntityNotFoundException("Employee", employeeId);
    
    // Buscar usuário por email, phone ou cpf
    User? foundUser = null;
    if (!string.IsNullOrEmpty(employee.Email))
        foundUser = await _unitOfWork.UserRepository.GetByEmailAsync(employee.Email);
    if (foundUser == null && !string.IsNullOrEmpty(employee.Phone))
        foundUser = await _unitOfWork.UserRepository.GetByPhoneAsync(employee.Phone);
    if (foundUser == null && !string.IsNullOrEmpty(employee.Cpf))
        foundUser = await _unitOfWork.UserRepository.GetByCpfAsync(employee.Cpf);
    
    if (foundUser == null)
        return new UserSearchResultDTO { UserId = null };
    
    // Verificar se tem acesso à empresa
    var companyUser = await _unitOfWork.CompanyUserRepository
        .GetByUserAndCompanyAsync(foundUser.UserId, companyId);
    
    return new UserSearchResultDTO
    {
        UserId = foundUser.UserId,
        Email = foundUser.Email,
        Phone = foundUser.Phone,
        Cpf = foundUser.Cpf,
        HasCompanyAccess = companyUser != null,
        CurrentRoleId = companyUser?.RoleId,
        CurrentRoleName = companyUser?.Role?.Name
    };
}

// Associar usuário existente
public async Task<EmployeeOutputDTO> AssociateUserAsync(
    long employeeId, 
    AssociateUserDTO dto, 
    long companyId, 
    long currentUserId)
{
    var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
    if (employee == null) throw new EntityNotFoundException("Employee", employeeId);
    
    // Verificar se o usuário existe
    var user = await _unitOfWork.UserRepository.GetOneByIdAsync(dto.UserId);
    if (user == null) throw new EntityNotFoundException("User", dto.UserId);
    
    // Se precisa criar CompanyUser
    if (dto.CreateCompanyUser)
    {
        if (dto.RoleId == null)
            throw new ValidationException("RoleId", "Cargo é obrigatório ao criar acesso à empresa");
        
        var companyUser = new CompanyUser(
            companyId,
            dto.UserId,
            dto.RoleId,
            currentUserId,
            null,
            DateTimeHelper.UtcNow,
            null
        );
        await _unitOfWork.CompanyUserRepository.CreateAsync(companyUser);
    }
    
    // Associar ao employee
    employee.UserId = dto.UserId;
    employee.AtualizadoPor = currentUserId;
    employee.AtualizadoEm = DateTimeHelper.UtcNow;
    
    await _unitOfWork.SaveChangesAsync();
    
    return await GetOneByIdAsync(employeeId);
}

// Criar e associar novo usuário
public async Task<EmployeeOutputDTO> CreateAndAssociateUserAsync(
    long employeeId,
    CreateUserAndAssociateDTO dto,
    long companyId,
    long currentUserId)
{
    var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
    if (employee == null) throw new EntityNotFoundException("Employee", employeeId);
    
    // Criar User
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new User(dto.Email, dto.Phone, dto.Cpf, passwordHash, null, null);
    var createdUser = await _unitOfWork.UserRepository.CreateAsync(user);
    await _unitOfWork.SaveChangesAsync();
    
    // Criar CompanyUser
    var companyUser = new CompanyUser(
        companyId,
        createdUser.UserId,
        dto.RoleId,
        currentUserId,
        null,
        DateTimeHelper.UtcNow,
        null
    );
    await _unitOfWork.CompanyUserRepository.CreateAsync(companyUser);
    
    // Associar ao employee
    employee.UserId = createdUser.UserId;
    employee.AtualizadoPor = currentUserId;
    employee.AtualizadoEm = DateTimeHelper.UtcNow;
    
    await _unitOfWork.SaveChangesAsync();
    
    return await GetOneByIdAsync(employeeId);
}

// Desassociar usuário
public async Task<EmployeeOutputDTO> DisassociateUserAsync(
    long employeeId,
    DisassociateUserDTO dto,
    long companyId,
    long currentUserId)
{
    var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
    if (employee == null) throw new EntityNotFoundException("Employee", employeeId);
    if (employee.UserId == null)
        throw new ValidationException("UserId", "Empregado não possui usuário associado");
    
    var userId = employee.UserId.Value;
    
    // Remover acesso à empresa se solicitado
    if (dto.RemoveCompanyAccess)
    {
        var companyUser = await _unitOfWork.CompanyUserRepository
            .GetByUserAndCompanyAsync(userId, companyId);
        if (companyUser != null)
        {
            await _unitOfWork.CompanyUserRepository.DeleteByIdAsync(companyUser.CompanyUserId);
        }
    }
    
    // Desassociar do employee
    employee.UserId = null;
    employee.AtualizadoPor = currentUserId;
    employee.AtualizadoEm = DateTimeHelper.UtcNow;
    
    await _unitOfWork.SaveChangesAsync();
    
    return await GetOneByIdAsync(employeeId);
}
```

### **4. Novos métodos em UserRepository**

```csharp
// IUserRepository interface
Task<User?> GetByEmailAsync(string email);
Task<User?> GetByPhoneAsync(string phone);
Task<User?> GetByCpfAsync(string cpf);

// UserRepository implementation
public async Task<User?> GetByEmailAsync(string email)
{
    return await _context.Set<User>()
        .FirstOrDefaultAsync(u => u.Email == email);
}

public async Task<User?> GetByPhoneAsync(string phone)
{
    return await _context.Set<User>()
        .FirstOrDefaultAsync(u => u.Phone == phone);
}

public async Task<User?> GetByCpfAsync(string cpf)
{
    return await _context.Set<User>()
        .FirstOrDefaultAsync(u => u.Cpf == cpf);
}
```

---

## 🎨 Implementação Frontend

### **1. Novos componentes**

**`AssociateUserDialog.tsx`**
- Dialog para associar usuário encontrado
- Mostra: Email, Phone, CPF do usuário
- Se não tem acesso: Dropdown de cargos
- Botões: Cancelar, Confirmar

**`CreateUserDialog.tsx`**
- Dialog para criar novo usuário
- Inputs: Email (pré-preenchido), Phone (pré-preenchido), CPF (pré-preenchido)
- Inputs: Senha, Confirmar Senha
- Dropdown: Cargos
- Botões: Cancelar, Criar e Associar

**`DisassociateUserAlert.tsx`**
- Alert para desassociar usuário
- Opções: Apenas Desassociar, Remover Acesso
- Botões: Cancelar, Confirmar

### **2. Atualizar employeeService.ts**

```typescript
// Buscar usuário por dados do employee
async searchUserForEmployee(employeeId: number): Promise<UserSearchResult> {
  const response = await api.post(`/api/employee/${employeeId}/searchUser`);
  return response.data.data;
}

// Associar usuário existente
async associateUser(employeeId: number, data: {
  userId: number;
  roleId?: number;
  createCompanyUser: boolean;
}): Promise<Employee> {
  const response = await api.post(`/api/employee/${employeeId}/associateUser`, data);
  return response.data.data;
}

// Criar e associar novo usuário
async createAndAssociateUser(employeeId: number, data: {
  email?: string;
  phone?: string;
  cpf?: string;
  password: string;
  roleId: number;
}): Promise<Employee> {
  const response = await api.post(`/api/employee/${employeeId}/createAndAssociateUser`, data);
  return response.data.data;
}

// Desassociar usuário
async disassociateUser(employeeId: number, removeCompanyAccess: boolean): Promise<Employee> {
  const response = await api.post(`/api/employee/${employeeId}/disassociateUser`, {
    removeCompanyAccess
  });
  return response.data.data;
}
```

### **3. Atualizar Employees.tsx**

**Desktop Table:**
- Adicionar coluna "Usuário" mostrando email/phone/cpf do usuário associado
- Adicionar botão de ação:
  - Se `employee.userId === null`: Botão "Associar Usuário" (ícone: UserPlus)
  - Se `employee.userId !== null`: Botão "Desassociar" (ícone: UserMinus)

**Mobile Cards:**
- Mostrar badge "Usuário: [email]" se associado
- Adicionar botão de ação no card

---

## 🧪 Casos de Teste

### **Cenário 1: Associar usuário existente com acesso**
1. Empregado sem user_id
2. Empregado tem email igual a usuário existente
3. Usuário já está em CompanyUser
4. ✅ Resultado: Apenas associa user_id, não cria CompanyUser

### **Cenário 2: Associar usuário existente sem acesso**
1. Empregado sem user_id
2. Empregado tem CPF igual a usuário existente
3. Usuário NÃO está em CompanyUser
4. Usuário seleciona cargo "Vendedor"
5. ✅ Resultado: Cria CompanyUser + Associa user_id

### **Cenário 3: Criar novo usuário**
1. Empregado sem user_id
2. Nenhum usuário encontrado
3. Usuário preenche senha e seleciona cargo
4. ✅ Resultado: Cria User + CompanyUser + Associa user_id

### **Cenário 4: Desassociar apenas**
1. Empregado com user_id
2. Usuário escolhe "Apenas Desassociar"
3. ✅ Resultado: user_id = null, CompanyUser mantém

### **Cenário 5: Desassociar e remover acesso**
1. Empregado com user_id
2. Usuário escolhe "Remover Acesso"
3. ✅ Resultado: user_id = null, CompanyUser deletado

---

## 📊 Validações

### **Backend:**
- ✅ Employee deve existir
- ✅ User deve existir (ao associar existente)
- ✅ RoleId obrigatório se CreateCompanyUser = true
- ✅ Password obrigatório e mínimo 6 caracteres (ao criar)
- ✅ Pelo menos 1 identificador obrigatório (email OU phone OU cpf) ao criar user
- ✅ Unique constraints: User.Email, User.Phone, User.Cpf

### **Frontend:**
- ✅ Senha e Confirmar Senha devem coincidir
- ✅ Cargo obrigatório ao criar usuário
- ✅ Pelo menos 1 identificador preenchido

---

## 🎯 Resumo da Implementação

### **Backend:**
1. ✅ 4 novos endpoints em EmployeeController
2. ✅ 4 novos DTOs
3. ✅ 4 novos métodos em EmployeeService
4. ✅ 3 novos métodos em UserRepository (GetByEmail, GetByPhone, GetByCpf)

### **Frontend:**
1. ✅ 3 novos componentes (dialogs/alerts)
2. ✅ 4 novos métodos em employeeService.ts
3. ✅ Atualizar Employees.tsx (adicionar coluna e botões)
4. ✅ Atualizar interface Employee (já tem userId e userEmail)

---

**Data:** 2025-11-14  
**Status:** 📝 Especificação Completa  
**Próximo:** Implementação step-by-step
