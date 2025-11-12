# User vs CompanyUser

## 🔍 Conceitos Fundamentais

### **User** (Usuário do Sistema)
Representa um **usuário único** no sistema inteiro (multi-tenant).

**Tabela:** `tb_user`
**Campos principais:**
- `user_id`: ID único do usuário
- `user_email`: Email de login
- `user_password_hash`: Senha criptografada
- `user_cpf`, `user_phone`: Dados pessoais

**Controller:** `UserController` → `/api/user/`
**Service:** `IUserService`

**Uso:** Autenticação, gerenciamento de credenciais, perfil do usuário.

---

### **CompanyUser** (Vínculo Usuário-Empresa)
Representa o **vínculo** entre um usuário e uma empresa, incluindo o cargo/role naquela empresa específica.

**Tabela:** `tb_company_user`
**Campos principais:**
- `company_user_id`: ID único do vínculo
- `user_id`: Referência ao usuário
- `company_id`: Referência à empresa
- `role_id`: Cargo do usuário **nesta empresa**

**Controller:** `CompanyUserController` → `/api/companyuser/`
**Service:** `ICompanyUserService`

**Uso:** Gerenciar quais usuários pertencem a uma empresa e seus cargos/permissões.

---

## 🎯 Diferenças Práticas

| Aspecto | User | CompanyUser |
|---------|------|-------------|
| **Escopo** | Global (sistema) | Por empresa (tenant) |
| **ID** | `userId` | `companyUserId` |
| **Tabela** | `tb_user` | `tb_company_user` |
| **Relacionamento** | 1 usuário para N empresas | N vínculos (1 por empresa) |
| **Contém Role?** | ❌ Não | ✅ Sim (`roleId`) |
| **Controller** | `/api/user/` | `/api/companyuser/` |
| **Tela Frontend** | (não tem listagem) | `/users` (lista da empresa) |

---

## 📊 Exemplo Real

### Cenário:
João trabalha em 3 empresas diferentes com cargos diferentes.

```
User (tb_user):
- user_id: 123
- user_email: "joao@email.com"
- user_password_hash: "..."

CompanyUser (tb_company_user):
1. company_user_id: 1000, user_id: 123, company_id: 10, role_id: 5 (Vendedor)
2. company_user_id: 1001, user_id: 123, company_id: 20, role_id: 1 (Dono)
3. company_user_id: 1002, user_id: 123, company_id: 30, role_id: 8 (Gerente)
```

- **User único:** João tem apenas 1 registro na `tb_user`
- **3 vínculos:** João tem 3 registros na `tb_company_user`, cada um com cargo diferente

---

## 🛠️ Quando Usar Cada Um?

### Use **UserController** (`/api/user/`) para:
- ✅ Registro de novo usuário (sign up)
- ✅ Autenticação (login)
- ✅ Alterar senha
- ✅ Atualizar perfil pessoal
- ✅ Buscar usuários do sistema (para adicionar à empresa)

### Use **CompanyUserController** (`/api/companyuser/`) para:
- ✅ Listar usuários **da empresa atual**
- ✅ Adicionar usuário existente à empresa
- ✅ Atribuir/alterar cargo do usuário
- ✅ Remover usuário da empresa
- ✅ Ver permissões por empresa

---

## 🔐 Impacto nas Permissões

### Permissões são por CompanyUser, NÃO por User!

```typescript
// ❌ ERRADO: Buscar permissões por userId
const permissions = getPermissions(userId);

// ✅ CORRETO: Buscar permissões por userId + companyId
const permissions = getPermissions(userId, companyId);
```

**Por quê?**
- João pode ser **Dono** na Empresa A (todas permissões)
- João pode ser **Vendedor** na Empresa B (permissões limitadas)

As permissões dependem do **cargo na empresa**, não do usuário em si.

---

## 📝 Endpoints Implementados

### UserController (`/api/user/`)
```
GET    /api/user/getAll              // Todos os users do sistema
GET    /api/user/getPaged            // Users paginados
GET    /api/user/{userId}            // User específico
POST   /api/user/create              // Criar novo user
PUT    /api/user/{userId}            // Atualizar user
DELETE /api/user/{userId}            // Deletar user
```

### CompanyUserController (`/api/companyuser/`)
```
GET    /api/companyuser/getAll       // Users da empresa atual
GET    /api/companyuser/getPaged     // Users paginados com filtros
GET    /api/companyuser/{companyUserId}  // Vínculo específico
POST   /api/companyuser/create       // Adicionar user à empresa
PUT    /api/companyuser/{companyUserId} // Atualizar cargo
DELETE /api/companyuser/{companyUserId} // Remover da empresa
```

---

## 🎨 Frontend

### Service: `companyUserService.ts`
```typescript
// Busca users DA EMPRESA ATUAL (usa header X-Company-Id)
const users = await companyUserService.getPaged({
  page: 1,
  pageSize: 10,
  searchTerm: 'joao'
});

// Cada user tem:
interface CompanyUser {
  companyUserId: number;  // ← ID do VÍNCULO (não do user)
  userId: number;         // ← ID do user
  companyId: number;      // ← ID da empresa
  roleId: number;         // ← Cargo NESTA empresa
  userEmail: string;
  roleName: string;
}
```

### Página: `/users`
- ✅ Lista apenas os **CompanyUsers da empresa atual**
- ✅ Mostra email + cargo
- ✅ Delete remove o **vínculo**, não o usuário
- ✅ Edit altera o cargo **nesta empresa**

---

## ⚠️ Cuidados Importantes

### 1. Delete é Soft!
```csharp
// ❌ NÃO deleta o User do sistema
DELETE /api/companyuser/{companyUserId}

// ✅ Remove apenas o vínculo user-empresa
// O usuário continua existindo e pode estar em outras empresas
```

### 2. IDs Diferentes!
```typescript
// Na listagem
user.companyUserId  // ← Para editar/deletar vínculo
user.userId         // ← ID real do usuário (não usar no CRUD)
```

### 3. Multi-Tenant
```typescript
// Backend SEMPRE usa o companyId do contexto
const companyId = GetCompanyId(); // do header X-Company-Id

// Frontend SEMPRE envia o header
api.defaults.headers.common['X-Company-Id'] = company.id;
```

---

## 📚 Arquivos Relacionados

### Backend
- `Controllers/UserController.cs` - Gerencia users do sistema
- `Controllers/CompanyUserController.cs` - Gerencia vínculos empresa-user
- `Services/CompanyUserService.cs` - Lógica de negócio
- `Entities/companyUser.cs` - Model do vínculo

### Frontend
- `services/companyUserService.ts` - Chamadas API
- `pages/users/Users.tsx` - Tela de listagem
- Contexto: `CompanyId` enviado via header

---

## 🎯 Resumo Rápido

```
User = Pessoa que usa o sistema
CompanyUser = Pessoa trabalhando NESTA empresa

Tela /users = Lista CompanyUsers (não Users)
Delete = Remove da empresa (não do sistema)
Cargo/Permissões = Por CompanyUser (não por User)
```
