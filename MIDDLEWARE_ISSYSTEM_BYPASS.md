# Middleware: IsSystem Bypass - Documentação

## 📋 Objetivo

Usuários com **role do sistema** (`IsSystem = true`) devem ter **acesso total** e **bypass de todas as verificações de permissão**, independente do módulo ou ação solicitada.

---

## ✅ Implementação

### **Modificação no PermissionService**

**Arquivo:** `backend/-5-CrossCutting/Services/PermissionService.cs`

---

### **1. UserHasPermissionAsync**

**ANTES:**
```csharp
public async Task<bool> UserHasPermissionAsync(long userId, long companyId, string module, string action)
{
    var permissions = await GetUserPermissionsAsync(userId, companyId);

    if (permissions == null)
    {
        _logger.LogWarning("Permissões não encontradas...");
        return false;
    }

    // Admin tem acesso total
    if (permissions.IsAdmin)
    {
        _logger.LogInformation("Usuário {UserId} é admin - acesso permitido", userId);
        return true;
    }

    // Verificar permissões específicas...
}
```

**DEPOIS:**
```csharp
public async Task<bool> UserHasPermissionAsync(long userId, long companyId, string module, string action)
{
    // ✅ NOVO: Verificar se o usuário tem role do sistema (IsSystem)
    var role = await _unitOfWork.CompanyUserRepository.GetUserRoleInCompanyAsync(userId, companyId);
    
    if (role != null && role.IsSystem)
    {
        _logger.LogInformation("Usuário {UserId} tem role do sistema (IsSystem=true) - acesso total permitido", userId);
        return true; // ← BYPASS COMPLETO
    }

    var permissions = await GetUserPermissionsAsync(userId, companyId);

    if (permissions == null)
    {
        _logger.LogWarning("Permissões não encontradas...");
        return false;
    }

    // Admin tem acesso total
    if (permissions.IsAdmin)
    {
        _logger.LogInformation("Usuário {UserId} é admin - acesso permitido", userId);
        return true;
    }

    // Verificar permissões específicas...
}
```

**Mudanças:**
1. ✅ Busca a role do usuário na empresa **antes** de verificar permissões
2. ✅ Se `role.IsSystem == true`, retorna `true` **imediatamente**
3. ✅ **Bypass completo** de todas as verificações de permissão
4. ✅ Log indicando que é role do sistema

---

### **2. GetUserPermissionsAsync**

**ANTES:**
```csharp
public async Task<RolePermissions> GetUserPermissionsAsync(long userId, long companyId)
{
    var role = await _unitOfWork.CompanyUserRepository.GetUserRoleInCompanyAsync(userId, companyId);

    if (role == null)
    {
        _logger.LogWarning("Role não encontrada...");
        return null;
    }

    try
    {
        var permissions = JsonSerializer.Deserialize<RolePermissions>(role.Permissions, ...);
        return permissions;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao desserializar permissões...");
        return new RolePermissions();
    }
}
```

**DEPOIS:**
```csharp
public async Task<RolePermissions> GetUserPermissionsAsync(long userId, long companyId)
{
    var role = await _unitOfWork.CompanyUserRepository.GetUserRoleInCompanyAsync(userId, companyId);

    if (role == null)
    {
        _logger.LogWarning("Role não encontrada...");
        return null;
    }

    // ✅ NOVO: Se for role do sistema, retornar permissões de admin total
    if (role.IsSystem)
    {
        _logger.LogInformation("Role do sistema detectada (IsSystem=true) para UserId={UserId}, RoleId={RoleId} - retornando permissões totais", userId, role.RoleId);
        return new RolePermissions
        {
            IsAdmin = true,
            AllowedEndpoints = new List<string> { "*" },
            Modules = new Dictionary<string, ModulePermissions>()
        };
    }

    try
    {
        var permissions = JsonSerializer.Deserialize<RolePermissions>(role.Permissions, ...);
        return permissions;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao desserializar permissões...");
        return new RolePermissions();
    }
}
```

**Mudanças:**
1. ✅ Verifica se `role.IsSystem == true`
2. ✅ Se sim, retorna um objeto `RolePermissions` com:
   - `IsAdmin = true`
   - `AllowedEndpoints = ["*"]`
   - Módulos vazios (não precisa pois IsAdmin já dá acesso total)
3. ✅ Log indicando retorno de permissões totais

---

## 🔄 Fluxo de Verificação de Permissão

### **Com IsSystem = true (Role "Dono"):**

```
1. API recebe requisição
   GET /api/products
   Headers: Authorization, X-Company-Id
   ↓
2. JwtMiddleware valida token
   Extrai UserId
   ↓
3. CompanyContextMiddleware
   Valida CompanyId no header
   Verifica se usuário tem acesso à empresa
   ↓
4. Controller/Action requer permissão
   [RequirePermission("product", "view")]
   ↓
5. PermissionService.UserHasPermissionAsync
   ↓
6. Busca role do usuário
   role = GetUserRoleInCompanyAsync(userId, companyId)
   ↓
7. ✅ Verifica IsSystem
   if (role.IsSystem == true)
       return true; // ← BYPASS COMPLETO
   ↓
8. ✅ Acesso permitido sem verificar permissões!
```

### **Com IsSystem = false (Role customizada):**

```
1-6. [mesmos passos]
   ↓
7. Verifica IsSystem
   if (role.IsSystem == false)
       continua verificação normal
   ↓
8. GetUserPermissionsAsync
   Deserializa permissões do JSON
   ↓
9. Verifica IsAdmin
   if (permissions.IsAdmin)
       return true;
   ↓
10. Verifica permissão específica do módulo
    permissions.Modules["product"].CanView
    ↓
11. Retorna true ou false baseado na permissão
```

---

## 🎯 Casos de Uso

### **1. Usuário Owner (Dono)**

```csharp
// Usuário com role "Dono" (IsSystem = true)
UserId: 1
CompanyId: 1
Role: "Dono" (IsSystem = true)

// Requisição qualquer
GET /api/products
GET /api/financial/transactions
POST /api/users
DELETE /api/roles/5

✅ Todos passam!
✅ IsSystem = true → Bypass completo
```

### **2. Usuário com Role Customizada**

```csharp
// Usuário com role "Vendedor" (IsSystem = false)
UserId: 2
CompanyId: 1
Role: "Vendedor" (IsSystem = false)
Permissions: {
    "product": { "canView": true, "canCreate": false },
    "financial": { "canView": false }
}

// Requisições
GET /api/products        ✅ Permitido (canView = true)
POST /api/products       ❌ Negado (canCreate = false)
GET /api/financial       ❌ Negado (canView = false)
```

---

## 📊 Comparação: Antes x Depois

| Cenário | ANTES | DEPOIS |
|---------|-------|--------|
| **Owner acessa endpoint** | Verifica permissões JSON | ✅ Bypass imediato (IsSystem) |
| **Performance para Owner** | Deserializa JSON sempre | ✅ Mais rápido (sem deserialização) |
| **Owner edita role "Dono"** | Permitido | ❌ Bloqueado (RoleService) |
| **Owner deleta role "Dono"** | Permitido | ❌ Bloqueado (RoleService) |
| **User customizado** | Verifica permissões | Verifica permissões (igual) |
| **Logs de debug** | "IsAdmin" | "IsSystem = true" (mais claro) |

---

## 🛡️ Vantagens da Abordagem

### **1. Performance**
- ✅ Owner não precisa deserializar JSON de permissões
- ✅ Verificação mais rápida (só checa `role.IsSystem`)
- ✅ Menos processamento

### **2. Segurança**
- ✅ Role do sistema identificada no **nível da entidade** (não só JSON)
- ✅ Impossível editar/deletar role "Dono" (validação no RoleService)
- ✅ Flag `IsSystem` no banco (não pode ser modificada via API normal)

### **3. Manutenibilidade**
- ✅ Lógica centralizada no PermissionService
- ✅ Fácil identificar roles do sistema
- ✅ Logs claros indicando bypass

### **4. Escalabilidade**
- ✅ Fácil adicionar outras roles do sistema (Admin, SuperAdmin, etc.)
- ✅ Não depende do JSON de permissões
- ✅ Consistente entre módulos

---

## 🧪 Testes

### **1. Criar empresa e verificar Owner:**
```http
POST /api/companies
Authorization: Bearer {token}
Body: { "name": "Teste LTDA", "userId": 1 }

✅ Empresa criada
✅ Role "Dono" criada (IsSystem = true)
✅ Usuário 1 associado à role "Dono"
```

### **2. Owner acessa qualquer endpoint:**
```http
GET /api/products
Authorization: Bearer {token}
X-Company-Id: 1

✅ Log: "Usuário 1 tem role do sistema (IsSystem=true) - acesso total permitido"
✅ Acesso permitido
```

### **3. Owner tenta editar role "Dono":**
```http
PUT /api/roles/1
Authorization: Bearer {token}
X-Company-Id: 1
Body: { "name": "Novo Nome" }

❌ Erro 400: "Roles do sistema (Owner/Admin) não podem ser editadas."
✅ Bloqueado no RoleService (não chega no middleware)
```

### **4. User customizado acessa:**
```http
GET /api/financial/transactions
Authorization: Bearer {token_user2}
X-Company-Id: 1

✅ Log: "Validação de permissão: UserId=2, Module=financial, Action=view, HasPermission=false"
❌ Acesso negado (permissão específica)
```

---

## 📝 Logs Gerados

### **Owner (IsSystem = true):**
```
[INFO] Usuário 1 tem role do sistema (IsSystem=true) - acesso total permitido
[INFO] Validação de permissão: UserId=1, Module=product, Action=view, HasPermission=true
```

### **User Customizado (IsSystem = false):**
```
[INFO] Permissões carregadas para UserId=2, RoleId=5
[INFO] Validação de permissão: UserId=2, Module=product, Action=view, HasPermission=true
[WARN] Validação de permissão: UserId=2, Module=financial, Action=view, HasPermission=false
```

---

## ⚠️ Considerações Importantes

### **1. IsSystem é apenas leitura pela API**
- Não existe endpoint para modificar `IsSystem`
- Só pode ser definido:
  - Automaticamente ao criar empresa (role "Dono")
  - Via migração SQL (para roles existentes)

### **2. Hierarquia de Verificação**
```
1º → IsSystem = true? → ✅ Permitir
2º → IsAdmin = true? → ✅ Permitir
3º → Verificar permissão específica do módulo
```

### **3. Roles do Sistema**
Atualmente apenas:
- ✅ **"Dono"** (criada automaticamente)

Podem ser adicionadas no futuro:
- "Admin"
- "SuperAdmin"
- "Suporte"

---

## 📁 Arquivo Modificado

```
backend/
└── -5-CrossCutting/
    └── Services/
        └── PermissionService.cs    ← Modificado (2 métodos)
```

**Métodos alterados:**
1. ✅ `UserHasPermissionAsync` - Bypass imediato se IsSystem
2. ✅ `GetUserPermissionsAsync` - Retorna permissões totais se IsSystem

---

## ✅ Checklist de Implementação

- [x] Adicionar verificação IsSystem no `UserHasPermissionAsync`
- [x] Adicionar verificação IsSystem no `GetUserPermissionsAsync`
- [x] Retornar `RolePermissions` com `IsAdmin = true` para roles do sistema
- [x] Adicionar logs informativos
- [x] Testar com usuário Owner
- [x] Testar com usuário customizado
- [x] Documentar alteração

---

## 🎉 Resultado Final

**Usuários com role do sistema (`IsSystem = true`) agora têm:**
- ✅ **Bypass completo** de verificação de permissões
- ✅ **Acesso total** a todos os endpoints e módulos
- ✅ **Performance melhorada** (sem deserialização JSON)
- ✅ **Logs claros** indicando acesso privilegiado
- ✅ **Proteção** contra edição/deleção da própria role

**Sistema de permissões robusto e performático implementado!** 🚀
