# Proteção de Rotas por Permissão

## 🎯 Problema

Quando o usuário acessa uma rota diretamente pela URL (ex: `/users`) sem ter permissão de visualização, a página carrega normalmente e só mostra o conteúdo vazio ou com erro.

**Comportamento esperado:** Redirecionar para `/access-denied` automaticamente.

---

## ✅ Solução Implementada

### **1. Novo Componente: `PermissionProtectedRoute`**

Componente que verifica permissões e redireciona se não tiver acesso.

**Arquivo:** `frontend/src/components/permissions/PermissionProtectedRoute.tsx`

```typescript
import { Navigate } from 'react-router-dom';
import { usePermissions } from '../../contexts/PermissionContext';

export function PermissionProtectedRoute({
  children,
  requires,
  requiresAll
}: PermissionProtectedRouteProps) {
  const { hasAnyPermission, hasAllPermissions, permissions } = usePermissions();

  // Aguarda permissões carregarem
  if (!permissions) {
    return <div>Loading...</div>;
  }

  // Verificar permissões
  let hasAccess = true;

  if (requiresAll && requiresAll.length > 0) {
    hasAccess = hasAllPermissions(...requiresAll);
  } else if (requires) {
    const perms = Array.isArray(requires) ? requires : [requires];
    hasAccess = hasAnyPermission(...perms);
  }

  // Redireciona se não tiver acesso
  if (!hasAccess) {
    return <Navigate to="/access-denied" replace />;
  }

  return <>{children}</>;
}
```

---

### **2. Estrutura de Proteção de Rotas**

Agora as rotas têm **3 camadas de proteção**:

```typescript
<Route
  path="/users"
  element={
    <ProtectedRoute>              {/* 1️⃣ Autenticação */}
      <CompanyProtectedRoute>     {/* 2️⃣ Empresa selecionada */}
        <PermissionProtectedRoute requires="user.canView">  {/* 3️⃣ Permissão */}
          <Users />
        </PermissionProtectedRoute>
      </CompanyProtectedRoute>
    </ProtectedRoute>
  }
/>
```

**Camadas:**
1. **ProtectedRoute:** Verifica se está autenticado → Redireciona para `/login`
2. **CompanyProtectedRoute:** Verifica se tem empresa selecionada → Redireciona para `/companies`
3. **PermissionProtectedRoute:** Verifica se tem permissão → Redireciona para `/access-denied`

---

### **3. Rotas Protegidas**

#### **Roles (Cargos)**
```typescript
<Route path="/roles" element={
  <PermissionProtectedRoute requires="role.canView">
    <Roles />
  </PermissionProtectedRoute>
} />

<Route path="/roles/new" element={
  <PermissionProtectedRoute requires="role.canCreate">
    <RoleForm />
  </PermissionProtectedRoute>
} />

<Route path="/roles/:id/edit" element={
  <PermissionProtectedRoute requires="role.canEdit">
    <RoleForm />
  </PermissionProtectedRoute>
} />
```

#### **Users (Usuários)**
```typescript
<Route path="/users" element={
  <PermissionProtectedRoute requires="user.canView">
    <Users />
  </PermissionProtectedRoute>
} />

<Route path="/users/new" element={
  <PermissionProtectedRoute requires="user.canCreate">
    <AddUser />
  </PermissionProtectedRoute>
} />

<Route path="/users/:companyUserId/edit" element={
  <PermissionProtectedRoute requires="user.canEdit">
    <EditUser />
  </PermissionProtectedRoute>
} />
```

#### **Accounts (Contas)**
```typescript
<Route path="/accounts" element={
  <PermissionProtectedRoute requires="account.canView">
    <Accounts />
  </PermissionProtectedRoute>
} />
```

---

## 🔒 Diferença: Protected vs PermissionProtectedRoute

### **`<Protected>` (Componente de UI)**
- Usado **dentro** de páginas
- Controla **visibilidade** de elementos
- Se não tiver permissão: **esconde** o elemento

**Uso:**
```typescript
<Protected requires="role.canCreate">
  <Button>Criar Cargo</Button>
</Protected>
```

### **`<PermissionProtectedRoute>` (Proteção de Rota)**
- Usado nas **rotas** (routes/index.tsx)
- Controla **acesso** às páginas
- Se não tiver permissão: **redireciona** para `/access-denied`

**Uso:**
```typescript
<Route path="/roles" element={
  <PermissionProtectedRoute requires="role.canView">
    <Roles />
  </PermissionProtectedRoute>
} />
```

---

## 🎯 Fluxo de Acesso

### **Cenário 1: Usuário COM permissão**
```
Usuário digita: /users
  ↓
✅ Autenticado? Sim
  ↓
✅ Empresa selecionada? Sim
  ↓
✅ Tem permissão "user.canView"? Sim
  ↓
✅ Mostra página Users
```

### **Cenário 2: Usuário SEM permissão**
```
Usuário digita: /users
  ↓
✅ Autenticado? Sim
  ↓
✅ Empresa selecionada? Sim
  ↓
❌ Tem permissão "user.canView"? Não
  ↓
🚫 Redireciona para /access-denied
```

### **Cenário 3: Usuário NÃO autenticado**
```
Usuário digita: /users
  ↓
❌ Autenticado? Não
  ↓
🚫 Redireciona para /login
```

---

## 📝 Permissões por Módulo

| Módulo | Visualizar | Criar | Editar | Deletar |
|--------|------------|-------|--------|---------|
| **Roles** | `role.canView` | `role.canCreate` | `role.canEdit` | `role.canDelete` |
| **Users** | `user.canView` | `user.canCreate` | `user.canEdit` | `user.canDelete` |
| **Accounts** | `account.canView` | `account.canCreate` | `account.canEdit` | `account.canDelete` |

---

## 🚀 Benefícios

1. ✅ **Segurança:** Impede acesso direto pela URL sem permissão
2. ✅ **UX Melhor:** Usuário vê mensagem clara de "Acesso Negado"
3. ✅ **Centralizado:** Permissões verificadas em um único lugar (rotas)
4. ✅ **Reutilizável:** Componente pode ser usado em qualquer rota
5. ✅ **Consistente:** Mesmo padrão para todos os módulos

---

## 📚 Arquivos Criados/Modificados

### **Criados:**
- ✅ `PermissionProtectedRoute.tsx` - Componente de proteção de rota
- ✅ `components/permissions/index.ts` - Export centralizado

### **Modificados:**
- ✅ `routes/index.tsx` - Adicionado proteção em todas as rotas

---

## 🧪 Testes

### **Teste 1: Acesso com permissão**
1. Login com usuário que tem `user.canView`
2. Acessar `/users` diretamente
3. ✅ Página carrega normalmente

### **Teste 2: Acesso sem permissão**
1. Login com usuário SEM `user.canView`
2. Acessar `/users` diretamente
3. ✅ Redireciona para `/access-denied`

### **Teste 3: Acesso sem autenticação**
1. Não fazer login
2. Acessar `/users` diretamente
3. ✅ Redireciona para `/login`

---

## 💡 Padrão para Novos Módulos

Ao criar um novo módulo, sempre proteger as rotas:

```typescript
// Listagem
<Route path="/novo-modulo" element={
  <ProtectedRoute>
    <CompanyProtectedRoute>
      <PermissionProtectedRoute requires="modulo.canView">
        <ModuloList />
      </PermissionProtectedRoute>
    </CompanyProtectedRoute>
  </ProtectedRoute>
} />

// Criar
<Route path="/novo-modulo/new" element={
  <ProtectedRoute>
    <CompanyProtectedRoute>
      <PermissionProtectedRoute requires="modulo.canCreate">
        <ModuloForm />
      </PermissionProtectedRoute>
    </CompanyProtectedRoute>
  </ProtectedRoute>
} />

// Editar
<Route path="/novo-modulo/:id/edit" element={
  <ProtectedRoute>
    <CompanyProtectedRoute>
      <PermissionProtectedRoute requires="modulo.canEdit">
        <ModuloForm />
      </PermissionProtectedRoute>
    </CompanyProtectedRoute>
  </ProtectedRoute>
} />
```

---

## 🔐 Segurança em Camadas

**Frontend:**
- ✅ Roteamento protegido (PermissionProtectedRoute)
- ✅ Elementos protegidos (Protected)
- ✅ Botões/ações protegidos (Protected)

**Backend:**
- ✅ Endpoints protegidos (RequirePermissions)
- ✅ Validação de permissões no service
- ✅ Validação de contexto (CompanyId, UserId)

**Resultado:** Sistema robusto e seguro! 🛡️
