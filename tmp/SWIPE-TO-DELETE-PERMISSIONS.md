# SwipeToDelete com Controle de Permissões

## 📱 Componente Mobile com Proteção de Permissões

O `SwipeToDelete` agora suporta controle granular de permissões para ações de editar e excluir.

## 🎯 Props

```typescript
interface SwipeToDeleteProps {
  children: ReactNode;
  onDelete: () => void;              // Função chamada ao excluir
  onTap?: () => void;                // Função chamada ao tocar (opcional)
  disabled?: boolean;                // Desabilita todas as interações
  showDeleteButton?: boolean;        // Controla se o botão de delete aparece (padrão: true)
}
```

## ⚙️ Comportamentos

### 1. **Long Press (500ms)**
- Mostra botão de excluir **APENAS se** `showDeleteButton = true`
- Se `showDeleteButton = false`, long press não faz nada

### 2. **Tap Rápido (<300ms)**
- Executa `onTap()` se fornecido
- Se `onTap` for `undefined`, não faz nada

### 3. **Disabled**
- Bloqueia **TODAS** as interações (tap e long press)
- Card fica visualmente desabilitado

---

## 📝 Exemplo de Uso com Permissões

```typescript
import { SwipeToDelete } from '../../components/ui/SwipeToDelete';
import { usePermissions } from '../../contexts/PermissionContext';

function RolesList() {
  const { hasPermission } = usePermissions();
  
  return roles.map((role) => {
    const canEdit = hasPermission('role.canEdit');
    const canDelete = hasPermission('role.canDelete');
    
    // Desabilita se for role de sistema OU sem nenhuma permissão
    const isDisabled = role.isSystem || (!canEdit && !canDelete);
    
    return (
      <SwipeToDelete
        key={role.id}
        // Se NÃO tem permissão de delete, passa função vazia
        onDelete={canDelete ? () => handleDelete(role) : () => {}}
        // Se NÃO tem permissão de edit, passa undefined (não faz nada)
        onTap={canEdit ? () => navigate(`/edit/${role.id}`) : undefined}
        // Desabilita se for sistema OU sem permissões
        disabled={isDisabled}
        // SÓ mostra botão se tem permissão E não é sistema
        showDeleteButton={canDelete && !role.isSystem}
      >
        <Card className={isDisabled ? 'opacity-60 cursor-not-allowed' : 'cursor-pointer'}>
          {/* Conteúdo do card */}
        </Card>
      </SwipeToDelete>
    );
  });
}
```

---

## 🎨 Feedback Visual

### Card Normal (com permissões)
```typescript
<Card className="hover:shadow-md active:bg-gray-50 cursor-pointer">
```

### Card Desabilitado (sem permissões)
```typescript
<Card className="opacity-60 cursor-not-allowed">
```

---

## 🔐 Cenários de Permissão

### 1. **Admin/System** (todas permissões)
- ✅ Pode tocar (editar)
- ✅ Pode long press (excluir)
- ✅ Botão de delete aparece

### 2. **Somente canEdit**
- ✅ Pode tocar (editar)
- ❌ Long press NÃO mostra botão
- ❌ Botão de delete oculto

### 3. **Somente canDelete**
- ❌ Tap não faz nada
- ✅ Long press mostra botão
- ✅ Pode excluir

### 4. **Sem permissões**
- ❌ Card desabilitado
- ❌ Nenhuma interação funciona
- ❌ Visual: opacity-60

### 5. **Role de Sistema**
- ❌ Card desabilitado (ninguém pode editar/deletar)
- 🔒 Proteção especial

---

## 🚀 Vantagens

| Antes | Depois |
|-------|--------|
| ❌ Botão aparecia mesmo sem permissão | ✅ Botão só aparece com permissão |
| ❌ Long press sempre ativo | ✅ Long press condicional |
| ❌ Confuso para o usuário | ✅ Feedback visual claro |

---

## 🐛 Problema Resolvido

**Antes:**
```
Usuário com apenas canEdit:
- Toca: ✅ Edita
- Long press: ❌ Mostra botão de delete (mas não funciona) ⚠️ CONFUSO!
```

**Depois:**
```
Usuário com apenas canEdit:
- Toca: ✅ Edita
- Long press: ✅ Não mostra nada 🎉 CLARO!
```

---

## 📚 Arquivos Relacionados

- **Componente:** `frontend/src/components/ui/SwipeToDelete.tsx`
- **Uso:** `frontend/src/pages/roles/Roles.tsx`
- **Context:** `frontend/src/contexts/PermissionContext.tsx`
