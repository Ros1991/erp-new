# Padrão de Botão "Voltar" nas Páginas de Detalhe

## 🎯 Padrão Definido

Todas as páginas de detalhe/formulário (criar/editar) devem seguir o mesmo padrão visual de botão "Voltar".

---

## ✅ Padrão Correto (Baseado em RoleForm)

### **Estrutura:**

```tsx
<MainLayout>
  <div className="space-y-6">
    {/* Header */}
    <div className="mb-6">
      <Button
        variant="ghost"
        onClick={() => navigate('/rota-anterior')}
        className="mb-4"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar
      </Button>
      <h1 className="text-3xl font-bold text-gray-900">Título da Página</h1>
      <p className="text-gray-600 mt-1">
        Descrição da página
      </p>
    </div>

    {/* Conteúdo */}
    <Card>
      ...
    </Card>
  </div>
</MainLayout>
```

---

## 📋 Características do Botão

| Propriedade | Valor | Descrição |
|------------|-------|-----------|
| **variant** | `"ghost"` | Botão transparente |
| **className** | `"mb-4"` | Margem inferior de 1rem |
| **Ícone** | `<ArrowLeft className="h-4 w-4 mr-2" />` | Seta à esquerda do texto |
| **Texto** | `"Voltar"` | Texto do botão |
| **Posição** | Linha separada, acima do título | Não inline com o título |

---

## 🎨 Layout Visual

```
┌─────────────────────────────────┐
│ ← Voltar                        │  ← Botão em linha separada
│                                 │
│ Título da Página                │  ← Título grande (text-3xl)
│ Descrição da página             │  ← Descrição (text-gray-600)
│                                 │
│ ┌───────────────────────────┐   │
│ │ Card com conteúdo         │   │
│ └───────────────────────────┘   │
└─────────────────────────────────┘
```

---

## ❌ Padrão Antigo (Incorreto)

```tsx
// ❌ NÃO FAZER ASSIM
<div className="flex items-center gap-4">
  <Button
    variant="ghost"
    onClick={() => navigate('/rota')}
    className="h-9 w-9 p-0"  // ❌ Botão circular, só ícone
  >
    <ArrowLeft className="h-5 w-5" />  // ❌ Sem texto
  </Button>
  <div>
    <h1 className="text-2xl font-bold">Título</h1>
  </div>
</div>
```

**Problemas:**
- ❌ Botão circular (só ícone, sem texto)
- ❌ Inline com o título (mesma linha)
- ❌ Título menor (text-2xl em vez de text-3xl)
- ❌ Menos espaçamento

---

## 📝 Páginas Já Ajustadas

### ✅ **RoleForm** (padrão de referência)
- `/roles/new`
- `/roles/:id/edit`

### ✅ **AddUser**
- `/users/new`

### ✅ **EditUser**
- `/users/:companyUserId/edit`

---

## 🔧 Como Aplicar em Novas Páginas

### **Template:**

```tsx
import { useNavigate } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { ArrowLeft } from 'lucide-react';

export function MinhaPageDetalhe() {
  const navigate = useNavigate();

  return (
    <MainLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/rota-listagem')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">Título</h1>
          <p className="text-gray-600 mt-1">
            Descrição
          </p>
        </div>

        {/* Conteúdo */}
        <Card>
          <CardContent className="p-6">
            {/* Seu conteúdo aqui */}
          </CardContent>
        </Card>
      </div>
    </MainLayout>
  );
}
```

---

## 🎯 Benefícios do Padrão

1. ✅ **Consistência visual** em todo o sistema
2. ✅ **Melhor UX** (botão mais visível e clicável)
3. ✅ **Acessibilidade** (texto "Voltar" explícito)
4. ✅ **Espaçamento adequado** entre elementos
5. ✅ **Hierarquia clara** (botão → título → descrição)

---

## 📱 Responsividade

O padrão funciona bem em todas as resoluções:

**Desktop:**
```
← Voltar

Título da Página
Descrição da página
```

**Mobile:**
```
← Voltar

Título da Página
Descrição
```

---

## 🔍 Checklist de Implementação

Ao criar uma nova página de detalhe, verificar:

- [ ] Botão tem `variant="ghost"`
- [ ] Botão tem `className="mb-4"`
- [ ] Ícone `<ArrowLeft className="h-4 w-4 mr-2" />`
- [ ] Texto "Voltar" presente
- [ ] Botão em linha separada (não inline com título)
- [ ] Título usa `text-3xl font-bold text-gray-900`
- [ ] Descrição usa `text-gray-600 mt-1`
- [ ] Header envolto em `<div className="mb-6">`

---

## 🎨 Classes CSS Utilizadas

### **Container do Header:**
```tsx
<div className="mb-6">  // Margem inferior do header
```

### **Botão Voltar:**
```tsx
<Button
  variant="ghost"       // Estilo transparente
  className="mb-4"      // Margem inferior do botão
>
  <ArrowLeft className="h-4 w-4 mr-2" />  // Ícone 16px, margem direita
  Voltar
</Button>
```

### **Título:**
```tsx
<h1 className="text-3xl font-bold text-gray-900">  // 30px, negrito, cinza escuro
```

### **Descrição:**
```tsx
<p className="text-gray-600 mt-1">  // Cinza médio, margem superior pequena
```

---

## 🚀 Resultado Final

**Visual consistente em todas as páginas de detalhe:**

1. **RoleForm** → ✅ Padrão aplicado
2. **AddUser** → ✅ Padrão aplicado
3. **EditUser** → ✅ Padrão aplicado
4. **Futuras páginas** → Seguir este documento

---

## 📚 Referência

**Arquivo modelo:** `frontend/src/pages/roles/RoleForm.tsx` (linhas 185-213)

**Páginas ajustadas:**
- `frontend/src/pages/users/AddUser.tsx`
- `frontend/src/pages/users/EditUser.tsx`

**Documentação:** `tmp/BACK-BUTTON-STANDARD.md`
