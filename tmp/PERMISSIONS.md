# 🔐 Sistema de Permissões - Frontend

Documentação completa do sistema de permissões do frontend.

## 📋 Índice

1. [Configuração Inicial](#configuração-inicial)
2. [Context de Permissões](#context-de-permissões)
3. [Hook usePermission](#hook-usepermission)
4. [Componente Protected](#componente-protected)
5. [Componente ProtectedRoute](#componente-protectedroute)
6. [Exemplos Práticos](#exemplos-práticos)

---

## 🚀 Configuração Inicial

### 1. Adicionar PermissionProvider no App

```tsx
// src/App.tsx
import { PermissionProvider } from './contexts/PermissionContext';

function App() {
  return (
    <PermissionProvider>
      <Router>
        {/* resto da aplicação */}
      </Router>
    </PermissionProvider>
  );
}
```

### 2. Carregar Permissões ao Selecionar Empresa

```tsx
// Quando usuário seleciona empresa
const { loadPermissions } = usePermissions();

const handleSelectCompany = async (companyId: number) => {
  // ... lógica de seleção de empresa
  await loadPermissions(); // Carrega permissões do backend
};
```

### 3. Limpar Permissões ao Logout

```tsx
const { clearPermissions } = usePermissions();

const handleLogout = () => {
  clearPermissions();
  // ... resto do logout
};
```

---

## 🎯 Context de Permissões

### Propriedades Disponíveis

```tsx
const {
  permissions,          // UserPermissions | null
  loading,              // boolean
  hasPermission,        // (perm: string) => boolean
  hasAnyPermission,     // (...perms: string[]) => boolean
  hasAllPermissions,    // (...perms: string[]) => boolean
  loadPermissions,      // () => Promise<void>
  clearPermissions      // () => void
} = usePermissions();
```

### Formato das Permissões

```typescript
interface UserPermissions {
  isAdmin: boolean;        // Acesso total
  isSystemRole: boolean;   // Role do sistema (bypass)
  modules: {
    role: {
      canView: boolean;
      canCreate: boolean;
      canEdit: boolean;
      canDelete: boolean;
    },
    user: { ... },
    account: { ... }
  }
}
```

---

## 🪝 Hook usePermission

Hook simplificado para verificar permissões.

### Uso Básico

```tsx
import { usePermission } from '../hooks/usePermission';

function MyComponent() {
  const canCreateRole = usePermission('role.canCreate');
  const canEditUser = usePermission('user.canEdit');

  return (
    <div>
      {canCreateRole && <button>Criar Cargo</button>}
      {canEditUser && <button>Editar Usuário</button>}
    </div>
  );
}
```

### Múltiplas Permissões (OR)

```tsx
// Mostra se tiver QUALQUER UMA das permissões
const canManageRoles = usePermission(['role.canCreate', 'role.canEdit']);
```

---

## 🛡️ Componente Protected

Controla visibilidade de elementos baseado em permissões.

### Exemplos de Uso

#### 1. Permissão Simples

```tsx
import { Protected } from '../components/permissions/Protected';

<Protected requires="role.canCreate">
  <button onClick={handleCreate}>
    Criar Novo Cargo
  </button>
</Protected>
```

#### 2. Múltiplas Permissões (OR)

```tsx
// Mostra se tiver QUALQUER UMA das permissões
<Protected requires={["role.canEdit", "role.canCreate"]}>
  <button>Editar</button>
</Protected>
```

#### 3. Todas as Permissões (AND)

```tsx
// Mostra APENAS se tiver TODAS as permissões
<Protected requiresAll={["role.canView", "role.canEdit"]}>
  <button>Visualizar e Editar</button>
</Protected>
```

#### 4. Com Fallback

```tsx
<Protected 
  requires="role.canView"
  fallback={<div>Você não tem permissão para visualizar</div>}
>
  <DataTable />
</Protected>
```

#### 5. Hierarquia (todas do módulo)

```tsx
// Apenas admins completos do módulo role
<Protected requires="role.*">
  <AdminPanel />
</Protected>
```

---

## 🚧 Componente ProtectedRoute

Protege rotas inteiras redirecionando para página de acesso negado.

### Uso no Routes

```tsx
import { ProtectedRoute } from '../components/permissions/ProtectedRoute';

// routes/index.tsx
<Routes>
  {/* Rota protegida simples */}
  <Route 
    path="/roles" 
    element={
      <ProtectedRoute requires="role.canView">
        <Roles />
      </ProtectedRoute>
    } 
  />

  {/* Múltiplas permissões (OR) */}
  <Route 
    path="/roles/new" 
    element={
      <ProtectedRoute requires={["role.canCreate", "role.canEdit"]}>
        <RoleForm />
      </ProtectedRoute>
    } 
  />

  {/* Todas as permissões (AND) */}
  <Route 
    path="/admin" 
    element={
      <ProtectedRoute requiresAll={["role.*", "user.*", "account.*"]}>
        <AdminPanel />
      </ProtectedRoute>
    } 
  />

  {/* Redirect customizado */}
  <Route 
    path="/super-admin" 
    element={
      <ProtectedRoute 
        requires="role.*" 
        redirectTo="/dashboard"
      >
        <SuperAdmin />
      </ProtectedRoute>
    } 
  />

  {/* Página de acesso negado */}
  <Route path="/access-denied" element={<AccessDenied />} />
</Routes>
```

---

## 💡 Exemplos Práticos

### Exemplo 1: Página de Cargos

```tsx
import { Protected } from '../components/permissions/Protected';
import { ProtectedRoute } from '../components/permissions/ProtectedRoute';
import { usePermission } from '../hooks/usePermission';

function Roles() {
  const canCreate = usePermission('role.canCreate');
  const canEdit = usePermission('role.canEdit');
  const canDelete = usePermission('role.canDelete');

  return (
    <div>
      {/* Botão de criar visível apenas com permissão */}
      <Protected requires="role.canCreate">
        <button onClick={handleCreate}>
          Criar Novo Cargo
        </button>
      </Protected>

      {/* Tabela sempre visível para quem tem canView */}
      <table>
        {roles.map(role => (
          <tr key={role.id}>
            <td>{role.name}</td>
            <td>
              {/* Botão editar */}
              <Protected requires="role.canEdit">
                <button onClick={() => handleEdit(role.id)}>
                  Editar
                </button>
              </Protected>

              {/* Botão deletar */}
              <Protected requires="role.canDelete">
                <button onClick={() => handleDelete(role.id)}>
                  Deletar
                </button>
              </Protected>
            </td>
          </tr>
        ))}
      </table>
    </div>
  );
}

// Proteger a rota no routes/index.tsx
<Route 
  path="/roles" 
  element={
    <ProtectedRoute requires="role.canView">
      <Roles />
    </ProtectedRoute>
  } 
/>
```

### Exemplo 2: Form de Usuário

```tsx
function UserForm() {
  const canEditRole = usePermission('user.canEdit');
  const canEditStatus = usePermission(['user.canEdit', 'user.canCreate']);

  return (
    <form>
      <input name="name" />
      <input name="email" />

      {/* Apenas admins podem mudar o cargo */}
      <Protected requires="user.canEdit">
        <select name="role">
          <option>Admin</option>
          <option>User</option>
        </select>
      </Protected>

      {/* Apenas quem pode editar ou criar pode mudar status */}
      <Protected requires={["user.canEdit", "user.canCreate"]}>
        <input type="checkbox" name="active" />
      </Protected>

      <button type="submit">Salvar</button>
    </form>
  );
}
```

### Exemplo 3: Dashboard com Widgets

```tsx
function Dashboard() {
  return (
    <div className="grid grid-cols-3 gap-4">
      {/* Widget de Cargos */}
      <Protected 
        requires="role.canView"
        fallback={<LockedWidget title="Cargos" />}
      >
        <RolesWidget />
      </Protected>

      {/* Widget de Usuários */}
      <Protected 
        requires="user.canView"
        fallback={<LockedWidget title="Usuários" />}
      >
        <UsersWidget />
      </Protected>

      {/* Widget de Contas */}
      <Protected 
        requires="account.canView"
        fallback={<LockedWidget title="Contas" />}
      >
        <AccountsWidget />
      </Protected>
    </div>
  );
}
```

### Exemplo 4: Menu Lateral

```tsx
import { usePermission } from '../hooks/usePermission';

function Sidebar() {
  const canViewRoles = usePermission('role.canView');
  const canViewUsers = usePermission('user.canView');
  const canViewAccounts = usePermission('account.canView');

  return (
    <nav>
      <Link to="/dashboard">Dashboard</Link>

      {canViewRoles && (
        <Link to="/roles">Cargos</Link>
      )}

      {canViewUsers && (
        <Link to="/users">Usuários</Link>
      )}

      {canViewAccounts && (
        <Link to="/accounts">Contas</Link>
      )}
    </nav>
  );
}
```

---

## 🔍 Formato de Permissões

### Sintaxe

```
module.permission
```

### Exemplos

```typescript
"role.canView"      // Visualizar cargos
"role.canCreate"    // Criar cargos
"role.canEdit"      // Editar cargos
"role.canDelete"    // Deletar cargos
"role.*"            // TODAS as permissões de cargos

"user.canView"      // Visualizar usuários
"user.*"            // TODAS as permissões de usuários

"account.canCreate" // Criar contas
"account.*"         // TODAS as permissões de contas
```

---

## ⚡ Performance

- **Cache automático**: Permissões são armazenadas em `localStorage`
- **Cache por request**: Evita múltiplas verificações
- **Lazy loading**: Componentes `Protected` não renderizam se sem permissão

---

## 🎯 Boas Práticas

### ✅ DO

- Use `<Protected>` para esconder elementos
- Use `<ProtectedRoute>` para proteger páginas inteiras
- Use `usePermission()` para lógica condicional
- Carregue permissões ao selecionar empresa
- Limpe permissões ao fazer logout

### ❌ DON'T

- Não confie apenas no frontend para segurança
- Backend SEMPRE deve validar permissões
- Não hardcode permissões, use o sistema
- Não deixe de proteger rotas sensíveis

---

## 🔒 Segurança

**IMPORTANTE:** O sistema de permissões do frontend é apenas para UX.

**NUNCA confie apenas no frontend para segurança!**

O backend SEMPRE deve validar permissões usando `[RequirePermissions]`.

Frontend = Esconder botões e rotas  
Backend = Bloquear acesso de verdade

---

## 📚 Referências

- Context: `src/contexts/PermissionContext.tsx`
- Hook: `src/hooks/usePermission.ts`
- Componente Protected: `src/components/permissions/Protected.tsx`
- Componente ProtectedRoute: `src/components/permissions/ProtectedRoute.tsx`
- Página Access Denied: `src/pages/AccessDenied.tsx`
