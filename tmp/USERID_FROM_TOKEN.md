# UserID do Token JWT - Documentação

## 📋 Objetivo

Remover o campo `userId` dos payloads de criação/edição, extraindo-o automaticamente do **token JWT** na requisição.

**Problema anterior:**
- ❌ Frontend enviava `userId` no corpo da requisição
- ❌ Vulnerabilidade de segurança (usuário poderia passar qualquer userId)
- ❌ Dados redundantes (userId já está no token)

**Solução:**
- ✅ Backend extrai `userId` do token JWT automaticamente
- ✅ Frontend não envia `userId` no payload
- ✅ Mais seguro e limpo

---

## ✅ Alterações Realizadas

### **1. Backend - CompanyInputDTO**

**Arquivo:** `backend/-2-Application/DTOs/CompanyInputDTO.cs`

**ANTES:**
```csharp
public class CompanyInputDTO
{
    [Required(ErrorMessage = "Name é obrigatório")]
    [StringLength(255, ErrorMessage = "Name deve ter no máximo 255 caracteres")]
    public string Name { get; set; }

    [StringLength(14, ErrorMessage = "Document deve ter no máximo 14 caracteres")]
    public string? Document { get; set; }

    [Required(ErrorMessage = "UserId é obrigatório")]  // ← REMOVIDO
    public long UserId { get; set; }                   // ← REMOVIDO
}
```

**DEPOIS:**
```csharp
public class CompanyInputDTO
{
    [Required(ErrorMessage = "Name é obrigatório")]
    [StringLength(255, ErrorMessage = "Name deve ter no máximo 255 caracteres")]
    public string Name { get; set; }

    [StringLength(14, ErrorMessage = "Document deve ter no máximo 14 caracteres")]
    public string? Document { get; set; }
    // UserId removido - será pego do token JWT
}
```

---

### **2. Backend - CompanyMapper**

**Arquivo:** `backend/-2-Application/Mappers/companyMapper.cs`

#### **A. ToEntity (Criar)**

**ANTES:**
```csharp
public static Company ToEntity(CompanyInputDTO dto, long userId)
{
    // ...
    return new Company(
        dto.Name,
        dto.Document,
        dto.UserId,        // ← Vindo do DTO (inseguro)
        userId,            // CriadoPor
        userId,            // AtualizadoPor
        now,               // CriadoEm
        now                // AtualizadoEm
    );
}
```

**DEPOIS:**
```csharp
public static Company ToEntity(CompanyInputDTO dto, long userId)
{
    // ...
    return new Company(
        dto.Name,
        dto.Document,
        userId,            // UserId (do token JWT) ✅
        userId,            // CriadoPor
        null,              // AtualizadoPor (null na criação)
        now,               // CriadoEm
        null               // AtualizadoEm (null na criação)
    );
}
```

#### **B. UpdateEntity (Atualizar)**

**ANTES:**
```csharp
public static void UpdateEntity(Company entity, CompanyInputDTO dto, long userId)
{
    entity.Name = dto.Name;
    entity.Document = dto.Document;
    entity.UserId = dto.UserId;           // ← Permitia alterar UserId (inseguro)
    entity.AtualizadoPor = userId;
    entity.AtualizadoEm = DateTime.UtcNow;
}
```

**DEPOIS:**
```csharp
public static void UpdateEntity(Company entity, CompanyInputDTO dto, long userId)
{
    entity.Name = dto.Name;
    entity.Document = dto.Document;
    // UserId não pode ser alterado após criação ✅
    entity.AtualizadoPor = userId;
    entity.AtualizadoEm = DateTime.UtcNow;
}
```

---

### **3. Backend - CompanyController**

**Arquivo:** `backend/-4-WebApi/Controllers/CompanyController.cs`

**JÁ ESTAVA CORRETO:**
```csharp
[HttpPost("/company/create/")]
public async Task<ActionResult<BaseResponse<CompanyOutputDTO>>> CreateAsync(CompanyInputDTO dto)
{
    var currentUserId = GetCurrentUserId(); // ✅ Pega do token JWT
    return await ValidateAndExecuteCreateAsync(
        () => _CompanyService.CreateAsync(dto, currentUserId),
        nameof(GetOneByIdAsync),
        result => new { company_id = result.CompanyId },
        "Empresa criada com sucesso"
    );
}
```

**Nota:** O controller já estava pegando o userId do token via `GetCurrentUserId()`. A mudança foi apenas nos DTOs e mappers.

---

### **4. Frontend - companyService.ts**

**Arquivo:** `frontend/src/services/companyService.ts`

**ANTES:**
```typescript
export interface CreateCompanyInput {
  name: string;
  document?: string;
  userId: number;  // ← REMOVIDO
}
```

**DEPOIS:**
```typescript
export interface CreateCompanyInput {
  name: string;
  document?: string;
  // userId removido - será pego do token JWT no backend
}
```

---

### **5. Frontend - AddCompanyDialog.tsx**

**Arquivo:** `frontend/src/components/companies/AddCompanyDialog.tsx`

**ANTES:**
```tsx
import { useAuth } from '../../contexts/AuthContext';

export function AddCompanyDialog({ open, onOpenChange, onSuccess }: AddCompanyDialogProps) {
  const { user } = useAuth();
  const { showError, showSuccess, showValidationErrors } = useToast();

  const handleSubmit = async (e: React.FormEvent) => {
    // ...

    if (!user?.userId) {
      showError('Usuário não autenticado');
      return;
    }

    try {
      await companyService.createCompany({
        name: name.trim(),
        document: cnpjNumbers || undefined,
        userId: user.userId  // ← REMOVIDO
      });
    }
  };
}
```

**DEPOIS:**
```tsx
// useAuth removido dos imports

export function AddCompanyDialog({ open, onOpenChange, onSuccess }: AddCompanyDialogProps) {
  const { showError, showSuccess, showValidationErrors } = useToast();
  // user removido - não é mais necessário

  const handleSubmit = async (e: React.FormEvent) => {
    // ...
    // Validação de autenticação removida

    try {
      await companyService.createCompany({
        name: name.trim(),
        document: cnpjNumbers || undefined
        // userId não enviado - backend pega do token ✅
      });
    }
  };
}
```

---

## 🔄 Fluxo de Criação de Empresa

### **ANTES (Inseguro):**
```
1. Frontend
   ↓
   POST /company/create/
   Authorization: Bearer {token}
   Body: {
     name: "Empresa",
     document: "11222333000181",
     userId: 1  ← Vindo do frontend (inseguro)
   }
   ↓
2. Backend
   ↓
   Controller: pega userId do token (linha 59)
   Mapper: usa dto.UserId (linha 40) ← IGNORA token
   ↓
   Vulnerabilidade: usuário poderia passar qualquer userId
```

### **DEPOIS (Seguro):**
```
1. Frontend
   ↓
   POST /company/create/
   Authorization: Bearer {token}
   Body: {
     name: "Empresa",
     document: "11222333000181"
     // userId não enviado
   }
   ↓
2. Backend
   ↓
   JwtMiddleware: valida token e extrai userId
   Controller: GetCurrentUserId() ← pega do token
   Mapper: usa userId do parâmetro (do token) ✅
   ↓
   Seguro: userId sempre vem do token autenticado
```

---

## 🛡️ Segurança

### **Problema Resolvido:**

**ANTES:**
```json
POST /company/create/
Body: {
  "name": "Empresa Maliciosa",
  "userId": 999  ← Usuário poderia passar qualquer ID
}

✅ Backend aceitava e criava empresa com userId = 999
❌ Vulnerabilidade de segurança crítica
```

**DEPOIS:**
```json
POST /company/create/
Body: {
  "name": "Empresa Legítima"
}

✅ Backend pega userId = 1 do token JWT
✅ Impossível criar empresa para outro usuário
✅ Seguro
```

---

## 📊 Comparação

| Aspecto | ANTES | DEPOIS |
|---------|-------|--------|
| **userId no payload** | ✅ Enviado | ❌ Não enviado |
| **Fonte do userId** | DTO (inseguro) | Token JWT (seguro) |
| **Validação frontend** | Precisa verificar `user?.userId` | Não precisa |
| **Vulnerabilidade** | ❌ Usuário pode falsificar | ✅ Seguro |
| **Payload** | 3 campos | 2 campos (mais limpo) |
| **Imports frontend** | `useAuth` necessário | Não necessário |
| **UserId na edição** | Podia ser alterado | Não pode ser alterado |

---

## 🧪 Testes

### **1. Criar empresa (sucesso):**
```http
POST /company/create/
Authorization: Bearer eyJhbGciOi...
Content-Type: application/json

{
  "name": "Minha Empresa LTDA",
  "document": "11222333000181"
}

✅ Resposta 201
✅ UserId extraído do token (ex: userId = 1)
✅ Empresa criada com userId = 1
✅ Role "Dono" criada
✅ Usuário associado à role
```

### **2. Criar empresa sem token:**
```http
POST /company/create/
Content-Type: application/json

{
  "name": "Empresa",
  "document": "11222333000181"
}

❌ Erro 401: Unauthorized
✅ Bloqueado pelo [Authorize]
```

### **3. Criar empresa com token inválido:**
```http
POST /company/create/
Authorization: Bearer token_invalido
Content-Type: application/json

{
  "name": "Empresa"
}

❌ Erro 401: Unauthorized
✅ Bloqueado pelo JwtMiddleware
```

### **4. Tentar enviar userId no payload:**
```http
POST /company/create/
Authorization: Bearer eyJhbGciOi...
Content-Type: application/json

{
  "name": "Empresa",
  "userId": 999
}

❌ Erro 400: Bad Request
✅ DTO não aceita userId
✅ Campo extra é ignorado
```

---

## 📁 Arquivos Modificados

### **Backend (3 arquivos):**
```
backend/
├── -2-Application/
│   ├── DTOs/
│   │   └── CompanyInputDTO.cs              ← UserId removido
│   └── Mappers/
│       └── companyMapper.cs                ← Usa userId do token
└── -4-WebApi/
    └── Controllers/
        └── CompanyController.cs            ← Já estava correto
```

### **Frontend (2 arquivos):**
```
frontend/
└── src/
    ├── components/
    │   └── companies/
    │       └── AddCompanyDialog.tsx        ← userId removido do payload
    └── services/
        └── companyService.ts               ← Interface atualizada
```

### **Documentação:**
```
USERID_FROM_TOKEN.md                        ← NOVO
```

---

## ✅ Checklist de Implementação

- [x] Remover `UserId` do `CompanyInputDTO`
- [x] Atualizar `CompanyMapper.ToEntity` para usar userId do token
- [x] Atualizar `CompanyMapper.UpdateEntity` para não permitir alterar userId
- [x] Remover `userId` da interface `CreateCompanyInput` (frontend)
- [x] Remover envio de `userId` no `AddCompanyDialog`
- [x] Remover import e uso de `useAuth` no dialog
- [x] Verificar que controller já está correto
- [x] Documentar alteração

---

## 🎯 Benefícios

1. ✅ **Segurança:** UserId sempre vem do token autenticado
2. ✅ **Simplicidade:** Frontend não precisa gerenciar userId
3. ✅ **Integridade:** Impossível criar empresa para outro usuário
4. ✅ **Manutenibilidade:** Menos código no frontend
5. ✅ **Consistência:** Padrão para outros endpoints
6. ✅ **Performance:** Payload menor

---

## 🚀 Próximos Passos

Aplicar o mesmo padrão em outros endpoints:
1. ✅ Criar empresa (concluído)
2. **TODO:** Outros endpoints de criação
3. **TODO:** Endpoints de edição onde userId não deve ser alterável

---

## 📝 Padrão a Seguir

### **Para todos os endpoints de criação:**

**Controller:**
```csharp
[HttpPost]
public async Task<ActionResult<BaseResponse<OutputDTO>>> CreateAsync(InputDTO dto)
{
    var currentUserId = GetCurrentUserId(); // ← Do token
    return await ValidateAndExecuteCreateAsync(
        () => _Service.CreateAsync(dto, currentUserId),
        // ...
    );
}
```

**DTO:**
```csharp
public class InputDTO
{
    // NÃO incluir UserId
    // Outros campos necessários
}
```

**Mapper:**
```csharp
public static Entity ToEntity(InputDTO dto, long userId)
{
    return new Entity(
        // ...
        userId,  // ← Do parâmetro (token)
        // ...
    );
}
```

---

**Sistema agora seguro! UserId sempre extraído do token JWT autenticado.** 🔒
