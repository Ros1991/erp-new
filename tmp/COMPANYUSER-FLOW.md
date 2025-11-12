# Fluxo de Adicionar/Editar Usuários à Empresa

## 🎯 Objetivo

Permitir adicionar usuários da tabela `tb_user` à empresa (`tb_company_user`) com validação de duplicação e seleção de cargo.

---

## 📋 Fluxo Completo

### **1. Adicionar Usuário à Empresa**

#### **Passo 1: Buscar Usuário Existente**
- Usuário acessa `/users/new`
- Busca usuários existentes por email, telefone ou CPF
- Busca com **debounce de 500ms** (não sobrecarrega backend)
- Backend faz busca com **formatação removida** (mesma lógica do login)

**Frontend:**
```typescript
const result = await userService.getPaged({
  searchTerm: searchTerm.trim(),
  pageSize: 10
});
```

**Backend:**
```csharp
// UserRepository.GetPagedAsync
var cleanSearch = Regex.Replace(searchLower, @"[^\d]", "");
query = query.Where(x => 
    (x.Email != null && x.Email.ToLower().Contains(searchLower)) ||
    (x.Phone != null && x.Phone.Contains(cleanSearch)) ||
    (x.Cpf != null && x.Cpf.Contains(cleanSearch))
);
```

#### **Passo 2: Criar Novo Usuário (Opcional)**
- Se não encontrar o usuário, pode criar novo
- Modal com campos: Email, Telefone, CPF, Senha
- **Pelo menos 1 identificador obrigatório** (email, telefone ou CPF)
- Senha **obrigatória**

**Frontend:**
```typescript
await userService.create({
  email: newUserData.email,
  phone: newUserData.phone,
  cpf: newUserData.cpf,
  password: newUserData.password
});
```

#### **Passo 3: Selecionar Cargo**
- Após selecionar/criar usuário, mostra grid de cargos
- Usuário escolhe um cargo
- Cargos de sistema aparecem com badge roxo

#### **Passo 4: Salvar Vínculo**
- Chama API para criar vínculo `CompanyUser`
- **Backend valida duplicação** (usuário já na empresa?)
- **Backend valida cargo** (pertence à empresa?)

**Frontend:**
```typescript
await companyUserService.create({
  userId: selectedUser.userId,
  roleId: selectedRole
});
```

**Backend:**
```csharp
// CompanyUserService.AddUserToCompanyAsync
var existingLink = await _unitOfWork.CompanyUserRepository
    .GetByUserAndCompanyAsync(dto.UserId, companyId);

if (existingLink != null)
{
    throw new ValidationException("UserId", "Usuário já está associado a esta empresa.");
}
```

---

### **2. Editar Cargo do Usuário**

#### **Fluxo:**
1. Usuário clica em **Editar** na lista
2. Abre página `/users/:companyUserId/edit`
3. Mostra informações do usuário (email/telefone/CPF)
4. Mostra cargo atual
5. Permite selecionar novo cargo
6. **NÃO permite editar dados do usuário** (apenas cargo)

**Frontend:**
```typescript
// EditUser.tsx
const user = await companyUserService.getById(companyUserId);
const rolesResult = await roleService.getRoles({ pageSize: 100 });

// Atualizar
await companyUserService.update(companyUserId, {
  userId: companyUser.userId,
  roleId: selectedRole
});
```

**Backend:**
```csharp
// CompanyUserService.UpdateUserRoleAsync
var role = await _unitOfWork.RoleRepository.GetOneByIdAsync(dto.RoleId);
if (role == null || role.CompanyId != existingEntity.CompanyId)
{
    throw new ValidationException("RoleId", "Role inválida ou não pertence a esta empresa.");
}
```

---

## 🔧 Implementação Técnica

### **Backend**

#### **1. UserFilterDTO**
```csharp
public class UserFilterDTO : PagedRequest
{
    public string? SearchTerm { get; set; }
}
```

#### **2. UserRepository**
```csharp
public async Task<PagedResult<User>> GetPagedAsync(UserFilterDTO filters)
{
    var query = _context.Set<User>().AsQueryable();

    if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
    {
        var searchLower = filters.SearchTerm.ToLower();
        var cleanSearch = Regex.Replace(searchLower, @"[^\d]", "");
        
        query = query.Where(x => 
            (x.Email != null && x.Email.ToLower().Contains(searchLower)) ||
            (x.Phone != null && x.Phone.Contains(cleanSearch)) ||
            (x.Cpf != null && x.Cpf.Contains(cleanSearch))
        );
    }

    var total = await query.CountAsync();
    var items = await query
        .Skip(filters.Skip)
        .Take(filters.PageSize)
        .ToListAsync();

    return new PagedResult<User>(items, filters.Page, filters.PageSize, total);
}
```

#### **3. CompanyUserService (Validações)**
```csharp
// Validação de duplicação
var existingLink = await _unitOfWork.CompanyUserRepository
    .GetByUserAndCompanyAsync(dto.UserId, companyId);

if (existingLink != null)
{
    throw new ValidationException("UserId", "Usuário já está associado a esta empresa.");
}

// Validação de cargo
var role = await _unitOfWork.RoleRepository.GetOneByIdAsync(dto.RoleId);
if (role == null || role.CompanyId != companyId)
{
    throw new ValidationException("RoleId", "Role inválida ou não pertence a esta empresa.");
}
```

---

### **Frontend**

#### **1. userService.ts**
```typescript
export interface User {
  userId: number;
  email?: string;
  phone?: string;
  cpf?: string;
}

const userService = {
  async getPaged(filters: UserFilters): Promise<PagedResponse<User>> { ... },
  async create(data: { email?: string; phone?: string; cpf?: string; password: string }): Promise<User> { ... },
  async getById(userId: number): Promise<User> { ... }
};
```

#### **2. AddUser.tsx**
- Busca de usuários com debounce
- Modal para criar novo usuário
- Seleção de cargo
- Validações (usuário selecionado, cargo selecionado)

**Estrutura:**
```typescript
const [searchTerm, setSearchTerm] = useState('');
const [users, setUsers] = useState<User[]>([]);
const [selectedUser, setSelectedUser] = useState<User | null>(null);
const [selectedRole, setSelectedRole] = useState<number | null>(null);
const [showNewUserModal, setShowNewUserModal] = useState(false);
```

#### **3. EditUser.tsx**
- Carrega dados do CompanyUser
- Carrega lista de cargos
- Permite apenas alterar cargo
- Mostra identificador do usuário (email/telefone/CPF)

**Estrutura:**
```typescript
const [companyUser, setCompanyUser] = useState<CompanyUser | null>(null);
const [roles, setRoles] = useState<Role[]>([]);
const [selectedRole, setSelectedRole] = useState<number | null>(null);
```

#### **4. Rotas**
```typescript
<Route path="/users" element={<Users />} />
<Route path="/users/new" element={<AddUser />} />
<Route path="/users/:companyUserId/edit" element={<EditUser />} />
```

---

## 🎨 UX/UI

### **AddUser.tsx**

**Desktop:**
```
┌──────────────────────────────────────────────────────────────┐
│ ← Adicionar Usuário à Empresa           [+ Novo Usuário]    │
│                                                               │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ 🔍 Buscar usuário por email, telefone ou CPF...        │ │
│ └─────────────────────────────────────────────────────────┘ │
│                                                               │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ 👤 joao@empresa.com                                     │ │
│ │    📧 joao@empresa.com  📱 (11) 99999-9999              │ │
│ └─────────────────────────────────────────────────────────┘ │
│                                                               │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ Selecione o Cargo                                        │ │
│ │ Usuário selecionado: joao@empresa.com                    │ │
│ │                                                           │ │
│ │ [Dono] [Gerente] [Vendedor] [Financeiro]                │ │
│ │                                         [Adicionar]       │ │
│ └─────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

**Mobile:**
```
┌─────────────────────────────────┐
│ ← Adicionar Usuário à Empresa  │
│                                  │
│ 🔍 Buscar usuário...             │
│ [+ Novo]                        │
│                                  │
│ ┌─────────────────────────────┐ │
│ │ 👤 joao@empresa.com         │ │
│ │    📧 joao@empresa.com      │ │
│ └─────────────────────────────┘ │
│                                  │
│ Selecione o Cargo:              │
│ ┌─────┐ ┌─────┐                │
│ │Dono │ │Ger. │                │
│ └─────┘ └─────┘                │
│                [Adicionar]      │
└─────────────────────────────────┘
```

### **EditUser.tsx**

**Desktop:**
```
┌──────────────────────────────────────────────────────────────┐
│ ← Editar Cargo do Usuário                                    │
│   Usuário: joao@empresa.com                                  │
│                                                               │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ Selecione o Cargo                                        │ │
│ │ Cargo atual: Gerente                                     │ │
│ │                                                           │ │
│ │ [Dono] [Gerente ✓] [Vendedor] [Financeiro]              │ │
│ │                                  [Cancelar] [Salvar]     │ │
│ └─────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

---

## ✅ Validações Implementadas

### **Backend**

1. ✅ **Duplicação:** Usuário não pode estar 2x na mesma empresa
2. ✅ **Cargo válido:** Cargo deve pertencer à empresa
3. ✅ **Dados obrigatórios:** UserId e RoleId são required

### **Frontend**

1. ✅ **Usuário selecionado:** Não permite salvar sem usuário
2. ✅ **Cargo selecionado:** Não permite salvar sem cargo
3. ✅ **Novo usuário:** Pelo menos 1 identificador obrigatório
4. ✅ **Senha obrigatória:** Ao criar novo usuário

---

## 📊 Fluxo de Dados

```
┌─────────────┐
│   tb_user   │ ← Usuários do sistema (global)
└─────────────┘
       │
       ▼
┌─────────────┐
│  AddUser    │ → Busca usuário OU cria novo
└─────────────┘
       │
       ▼ (usuário selecionado)
┌─────────────┐
│ Seleciona   │ → Escolhe cargo da empresa
│   Cargo     │
└─────────────┘
       │
       ▼
┌─────────────────┐
│ tb_company_user │ ← Vínculo usuário ↔ empresa ↔ cargo
└─────────────────┘
       │
       ▼
┌─────────────┐
│  EditUser   │ → Edita apenas o cargo
└─────────────┘
```

---

## 🎯 Casos de Uso

### **1. Adicionar usuário existente**
1. Abrir `/users/new`
2. Buscar "joao@empresa.com"
3. Selecionar usuário encontrado
4. Escolher cargo "Gerente"
5. Clicar "Adicionar à Empresa"
6. ✅ Vínculo criado

### **2. Criar novo usuário e adicionar**
1. Abrir `/users/new`
2. Buscar "maria@empresa.com" (não encontrado)
3. Clicar "+ Novo Usuário"
4. Preencher: email, telefone, CPF, senha
5. Criar usuário
6. Usuário aparece selecionado
7. Escolher cargo "Vendedor"
8. Clicar "Adicionar à Empresa"
9. ✅ Usuário criado + Vínculo criado

### **3. Editar cargo**
1. Na lista de usuários, clicar "Editar"
2. Abrir `/users/123/edit`
3. Ver cargo atual: "Vendedor"
4. Selecionar novo cargo: "Gerente"
5. Clicar "Salvar"
6. ✅ Cargo atualizado

### **4. Tentar adicionar duplicado**
1. Buscar usuário já vinculado
2. Selecionar usuário
3. Escolher cargo
4. Clicar "Adicionar"
5. ❌ Erro: "Usuário já está associado a esta empresa."

---

## 📚 Arquivos Criados/Modificados

### **Backend**
- ✅ `UserFilterDTO.cs` - Adicionado SearchTerm
- ✅ `userRepository.cs` - Busca otimizada com formatação
- ✅ `companyUserService.cs` - Validações já existiam

### **Frontend**
- ✅ `userService.ts` - NOVO
- ✅ `AddUser.tsx` - NOVO
- ✅ `EditUser.tsx` - NOVO
- ✅ `companyUserService.ts` - Adicionado getById e update
- ✅ `routes/index.tsx` - Rotas /users/new e /users/:id/edit
- ✅ `Users.tsx` - Navegação corrigida (companyUserId)

---

## 🚀 Próximos Passos (Opcional)

1. **Validação de email único** ao criar usuário
2. **Upload de foto** do usuário
3. **Histórico de alterações** de cargo
4. **Notificação por email** quando adicionado à empresa
5. **Importação em massa** de usuários (CSV/Excel)
