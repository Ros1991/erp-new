# 3 Camadas de Proteção de Rotas - Explicação

## 🎯 Por que 3 Camadas?

O sistema ERP possui **3 níveis de segurança** que devem ser verificados em sequência:

1. **Autenticação** - O usuário fez login?
2. **Contexto de Empresa** - O usuário selecionou uma empresa?
3. **Permissões** - O usuário tem permissão para acessar este módulo?

Cada camada resolve um problema diferente!

---

## 1️⃣ ProtectedRoute (Autenticação)

### **O que faz:**
Verifica se o usuário está **autenticado** (fez login).

### **Código:**
```typescript
function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();
  
  if (isLoading) {
    return <div>Loading...</div>;  // Aguarda verificar autenticação
  }
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;  // ❌ Não autenticado → /login
  }
  
  return <>{children}</>;  // ✅ Autenticado → continua
}
```

### **Quando redireciona:**
- Usuário **não está logado**
- Token expirou
- Sessão inválida

### **Para onde redireciona:**
→ `/login`

### **Exemplo:**
```
Usuário tenta acessar: /users
  ↓
❌ Não está logado
  ↓
🚫 Redireciona para /login
```

---

## 2️⃣ CompanyProtectedRoute (Contexto de Empresa)

### **O que faz:**
Verifica se o usuário **selecionou uma empresa** para trabalhar.

### **Por que é necessário:**
O sistema é **multi-empresa** (multi-tenant). O usuário pode ter acesso a várias empresas, mas precisa escolher uma para trabalhar.

### **Código:**
```typescript
function CompanyProtectedRoute({ children }: { children: React.ReactNode }) {
  const { selectedCompany } = useAuth();
  
  if (!selectedCompany) {
    return <Navigate to="/companies" replace />;  // ❌ Sem empresa → /companies
  }
  
  return <>{children}</>;  // ✅ Empresa selecionada → continua
}
```

### **Quando redireciona:**
- Usuário está autenticado **MAS** não selecionou uma empresa
- Primeiro login (ainda não escolheu empresa)
- Mudou de empresa mas não selecionou nova

### **Para onde redireciona:**
→ `/companies` (tela de seleção de empresa)

### **Exemplo:**
```
Usuário tenta acessar: /users
  ↓
✅ Está logado
  ↓
❌ Não selecionou empresa
  ↓
🚫 Redireciona para /companies
```

---

## 3️⃣ PermissionProtectedRoute (Permissões)

### **O que faz:**
Verifica se o usuário tem **permissão** para acessar aquele módulo/funcionalidade.

### **Por que é necessário:**
Mesmo dentro de uma empresa, usuários têm **cargos diferentes** com **permissões diferentes**:
- Dono tem acesso total
- Gerente tem acesso limitado
- Vendedor tem acesso restrito

### **Código:**
```typescript
function PermissionProtectedRoute({ children, requires }) {
  const { hasAnyPermission, permissions } = usePermissions();
  
  if (!permissions) {
    return <div>Loading...</div>;  // Aguarda carregar permissões
  }
  
  const perms = Array.isArray(requires) ? requires : [requires];
  const hasAccess = hasAnyPermission(...perms);
  
  if (!hasAccess) {
    return <Navigate to="/access-denied" replace />;  // ❌ Sem permissão → /access-denied
  }
  
  return <>{children}</>;  // ✅ Tem permissão → continua
}
```

### **Quando redireciona:**
- Usuário está autenticado
- Usuário selecionou empresa
- **MAS** não tem permissão para acessar aquela página

### **Para onde redireciona:**
→ `/access-denied`

### **Exemplo:**
```
Usuário Vendedor tenta acessar: /roles (gerenciar cargos)
  ↓
✅ Está logado
  ↓
✅ Empresa selecionada
  ↓
❌ Não tem permissão "role.canView" (só Dono/Gerente têm)
  ↓
🚫 Redireciona para /access-denied
```

---

## 🔗 Como Funcionam Juntas

### **Estrutura de Rota:**
```typescript
<Route path="/users" element={
  <ProtectedRoute>                    // 1️⃣ Primeiro: está logado?
    <CompanyProtectedRoute>           // 2️⃣ Depois: selecionou empresa?
      <PermissionProtectedRoute       // 3️⃣ Por fim: tem permissão?
        requires="user.canView"
      >
        <Users />                     // ✅ Se passou por tudo, mostra página
      </PermissionProtectedRoute>
    </CompanyProtectedRoute>
  </ProtectedRoute>
} />
```

### **Fluxo de Verificação (Sequencial):**

```
1. ProtectedRoute verifica:
   └─ Está autenticado?
      ├─ ❌ NÃO → Redireciona para /login
      └─ ✅ SIM → Passa para próxima camada

2. CompanyProtectedRoute verifica:
   └─ Selecionou empresa?
      ├─ ❌ NÃO → Redireciona para /companies
      └─ ✅ SIM → Passa para próxima camada

3. PermissionProtectedRoute verifica:
   └─ Tem permissão "user.canView"?
      ├─ ❌ NÃO → Redireciona para /access-denied
      └─ ✅ SIM → Mostra a página <Users />
```

---

## 📊 Comparação

| Camada | Verifica | Redireciona para | Quando usar |
|--------|----------|------------------|-------------|
| **ProtectedRoute** | Autenticação | `/login` | Todas as rotas internas |
| **CompanyProtectedRoute** | Empresa selecionada | `/companies` | Rotas que precisam de contexto de empresa |
| **PermissionProtectedRoute** | Permissões do cargo | `/access-denied` | Rotas com controle de acesso |

---

## 🎯 Exemplos Práticos

### **Exemplo 1: Usuário Não Logado**
```
GET /users
  ↓
ProtectedRoute: ❌ não autenticado
  ↓
Redirect: /login
```

### **Exemplo 2: Usuário Logado, Sem Empresa**
```
GET /users
  ↓
ProtectedRoute: ✅ autenticado
  ↓
CompanyProtectedRoute: ❌ sem empresa
  ↓
Redirect: /companies
```

### **Exemplo 3: Usuário Logado, Com Empresa, Sem Permissão**
```
GET /users
  ↓
ProtectedRoute: ✅ autenticado
  ↓
CompanyProtectedRoute: ✅ empresa ID=27
  ↓
PermissionProtectedRoute: ❌ sem "user.canView"
  ↓
Redirect: /access-denied
```

### **Exemplo 4: Usuário Logado, Com Empresa, Com Permissão**
```
GET /users
  ↓
ProtectedRoute: ✅ autenticado
  ↓
CompanyProtectedRoute: ✅ empresa ID=27
  ↓
PermissionProtectedRoute: ✅ tem "user.canView"
  ↓
Render: <Users /> ✅
```

---

## 🚫 Exceções (Rotas que NÃO usam todas as camadas)

### **Rotas Públicas (nenhuma camada):**
- `/` (Landing)
- `/login`
- `/register`
- `/forgot-password`
- `/reset-password`

### **Rotas Autenticadas mas sem Empresa (só ProtectedRoute):**
- `/companies` (seleção de empresa)
- `/company/:id/settings` (configurações da empresa)

### **Rotas com Empresa mas sem Permissão (ProtectedRoute + CompanyProtectedRoute):**
- `/dashboard` (todos têm acesso)
- `/access-denied` (página de erro)

### **Rotas Completas (todas as 3 camadas):**
- `/roles`, `/roles/new`, `/roles/:id/edit`
- `/users`, `/users/new`, `/users/:id/edit`
- `/accounts`
- Todos os módulos de negócio

---

## 💡 Regra de Ouro

**Quanto mais "interno" e "sensível" for o módulo, mais camadas de proteção ele precisa!**

```
Público → 0 camadas
Autenticado → 1 camada (ProtectedRoute)
Autenticado + Empresa → 2 camadas (+ CompanyProtectedRoute)
Autenticado + Empresa + Permissão → 3 camadas (+ PermissionProtectedRoute)
```

---

## 🔐 Segurança em Profundidade (Defense in Depth)

Cada camada é uma **barreira adicional** de segurança:

1. **Autenticação:** Garante que é um usuário válido
2. **Empresa:** Garante contexto correto (multi-tenant)
3. **Permissões:** Garante acesso autorizado (RBAC)

**Resultado:** Sistema robusto, seguro e escalável! 🛡️
