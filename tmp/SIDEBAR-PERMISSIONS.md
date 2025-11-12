# Sidebar com Controle de Permissões

## 📋 Como Funciona

O Sidebar agora filtra automaticamente os itens de menu baseado nas permissões do usuário. Apenas os módulos que o usuário tem permissão de **visualizar** (`canView`) aparecem no menu.

## ✅ Estrutura de um Item de Menu

```typescript
interface MenuItem {
  icon: any;           // Ícone do Lucide React
  label: string;       // Texto exibido no menu
  path: string;        // Rota do React Router
  permission?: string; // Permissão necessária (opcional)
}
```

## 🎯 Exemplos

### Item SEM permissão (sempre visível)
```typescript
{ 
  icon: Home, 
  label: 'Dashboard', 
  path: '/dashboard' 
}
```
**Quando usar:** Para páginas que TODOS os usuários autenticados podem acessar (ex: Dashboard, Perfil).

---

### Item COM permissão (condicional)
```typescript
{ 
  icon: Shield, 
  label: 'Cargos', 
  path: '/roles', 
  permission: 'role.canView' 
}
```
**Quando usar:** Para módulos que apenas usuários com permissões específicas podem acessar.

---

## 📝 Como Adicionar um Novo Item

1. **Escolha o ícone** (do Lucide React)
2. **Defina o path** (rota existente)
3. **Defina a permissão** (formato: `module.canView`)

### Exemplo: Adicionar "Relatórios"
```typescript
import { FileText } from 'lucide-react';

const menuItems: MenuItem[] = [
  { icon: Home, label: 'Dashboard', path: '/dashboard' },
  { icon: Shield, label: 'Cargos', path: '/roles', permission: 'role.canView' },
  { icon: Users, label: 'Usuários', path: '/users', permission: 'user.canView' },
  { icon: Wallet, label: 'Conta Correntes', path: '/accounts', permission: 'account.canView' },
  // ✅ NOVO ITEM
  { icon: FileText, label: 'Relatórios', path: '/reports', permission: 'report.canView' },
];
```

---

## 🔐 Convenção de Permissões

| Módulo | Permissão para Sidebar |
|--------|------------------------|
| Cargos | `role.canView` |
| Usuários | `user.canView` |
| Contas Correntes | `account.canView` |
| Dashboard | *(sem permissão)* |

**Regra:** Sempre usar `{module}.canView` para controlar a visibilidade no menu.

---

## 🚀 Comportamento

### Usuário com Admin/System Role
- ✅ Vê **TODOS** os itens do menu (bypass automático)

### Usuário com Permissões Limitadas
- ✅ Vê apenas os módulos onde tem `canView = true`
- ❌ Não vê módulos onde `canView = false`

### Exemplo Prático

**Usuário "Vendedor"** com permissões:
```json
{
  "modules": {
    "account": { "canView": true, ... },
    "role": { "canView": false, ... }
  }
}
```

**Sidebar mostrará:**
- ✅ Dashboard
- ✅ Conta Correntes
- ❌ Cargos (oculto)

---

## 🔒 Segurança

⚠️ **IMPORTANTE:** O Sidebar é apenas a **primeira camada** de proteção (UI).

**Camadas de Segurança Completas:**
1. ✅ **Sidebar:** Oculta itens (UX)
2. ✅ **ProtectedRoute:** Bloqueia navegação direta (React Router)
3. ✅ **RequirePermissions:** Valida no backend (API)

**Nunca confie apenas na UI!** Sempre proteja as rotas e endpoints.

---

## 📚 Referências

- **Contexto de Permissões:** `frontend/src/contexts/PermissionContext.tsx`
- **Protected Route:** `frontend/src/components/permissions/ProtectedRoute.tsx`
- **Backend Attribute:** `backend/-4-WebApi/Attributes/RequirePermissionsAttribute.cs`
- **Documentação Completa:** `frontend/PERMISSIONS.md`
