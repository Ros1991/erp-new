# Padronização de Toasts de Erro

## 🎯 Problema Resolvido

**Antes:** Toasts mostravam mensagens com títulos e prefixos técnicos:
```
┌─────────────────────────────────────┐
│ ❌ Erro                             │
│ Validation error in field           │
│ 'CompanyUser': Não é possível       │
│ remover o dono da empresa.          │
└─────────────────────────────────────┘
```

**Depois:** Toast limpo, sem título, apenas a mensagem:
```
┌─────────────────────────────────────┐
│ ❌ Não é possível remover o dono    │
│    da empresa.                      │
└─────────────────────────────────────┘
```

---

## ✅ Solução Implementada

### **1. Função Padronizada no ToastContext**

**Arquivo:** `frontend/src/contexts/ToastContext.tsx`

```typescript
// Função padronizada para lidar com erros do backend
const handleBackendError = useCallback((error: any) => {
  const { message } = parseBackendError(error);
  showToast('error', message); // Sem título, apenas a mensagem
}, [showToast]);
```

**Adicionado ao contexto:**
```typescript
interface ToastContextType {
  // ... outros métodos
  handleBackendError: (error: any) => void; // ✅ NOVO
}
```

---

### **2. Títulos Removidos por Padrão**

```typescript
// ANTES
const showSuccess = useCallback((message: string, title = 'Sucesso') => {
  showToast('success', message, title);
}, [showToast]);

const showError = useCallback((message: string, title = 'Erro') => {
  showToast('error', message, title);
}, [showToast]);

// DEPOIS
const showSuccess = useCallback((message: string, title?: string) => {
  showToast('success', message, title);  // Sem título padrão
}, [showToast]);

const showError = useCallback((message: string, title?: string) => {
  showToast('error', message, title);  // Sem título padrão
}, [showToast]);
```

---

### **3. errorHandler Melhorado**

**Arquivo:** `frontend/src/utils/errorHandler.ts`

```typescript
// Erros de validação (errors na raiz do BaseResponse)
if (response.errors && Object.keys(response.errors).length > 0) {
  // ✅ Extrair a primeira mensagem de erro limpa
  const firstField = Object.keys(response.errors)[0];
  const firstErrorMessage = response.errors[firstField][0];
  
  return {
    hasValidationErrors: true,
    validationErrors: response.errors,
    message: firstErrorMessage || response.message || 'Erro de validação',
  };
}
```

**Antes:** `message = response.message` (com prefixo técnico)
**Depois:** `message = firstErrorMessage` (apenas a mensagem limpa)

---

## 📊 Uso Padronizado

### **ANTES (manual, repetitivo):**

```typescript
const { showError, showSuccess, showValidationErrors } = useToast();

try {
  await companyUserService.delete(id);
  showSuccess('Usuário removido com sucesso!');
} catch (err: any) {
  const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
  
  if (hasValidationErrors && validationErrors) {
    showValidationErrors(validationErrors);  // Mostra múltiplos toasts
  } else {
    showError(message);  // Mostra 1 toast
  }
}
```

**Problemas:**
- ❌ Muito código repetitivo
- ❌ Fácil esquecer de importar parseBackendError
- ❌ Inconsistente (às vezes esquece o if/else)

---

### **DEPOIS (padronizado, simples):**

```typescript
const { showSuccess, handleBackendError } = useToast();

try {
  await companyUserService.delete(id);
  showSuccess('Usuário removido com sucesso!');
} catch (err: any) {
  handleBackendError(err);  // ✅ Uma linha!
}
```

**Benefícios:**
- ✅ 1 linha vs 8 linhas
- ✅ Sempre correto (parseBackendError interno)
- ✅ Mensagem sempre limpa
- ✅ Sem título "Erro"

---

## 🎯 Quando Usar Cada Método

### **`handleBackendError(error)` - Use para erros do backend:**

```typescript
try {
  await apiCall();
} catch (err: any) {
  handleBackendError(err);  // ✅ Qualquer erro de API
}
```

**Trata automaticamente:**
- ✅ Erros de validação (campo, mensagem)
- ✅ Erros de negócio (ValidationException)
- ✅ Erros de rede (sem conexão)
- ✅ Erros 404, 500, etc.

---

### **`showError(message)` - Use para validações frontend:**

```typescript
if (!email) {
  showError('Email é obrigatório');  // ✅ Validação no frontend
  return;
}

if (password.length < 6) {
  showError('Senha deve ter no mínimo 6 caracteres');  // ✅ Validação no frontend
  return;
}
```

**Quando usar:**
- ✅ Validações antes de enviar ao backend
- ✅ Mensagens de erro customizadas
- ✅ Erros de lógica do frontend

---

## 📝 Arquivos Atualizados

### **1. ToastContext.tsx**

```typescript
// Mudanças:
- Adicionado import do parseBackendError
- Criado método handleBackendError
- Removido títulos padrão de showSuccess, showError, etc.
- Adicionado handleBackendError no provider
```

### **2. errorHandler.ts**

```typescript
// Mudança:
- Extrair mensagem limpa do array errors ao invés de usar response.message
```

### **3. Users.tsx**

```typescript
// ANTES:
import { parseBackendError } from '../../utils/errorHandler';
const { showError, showSuccess } = useToast();

catch (err: any) {
  const { message } = parseBackendError(err);
  showError(message);
}

// DEPOIS:
const { showSuccess, handleBackendError } = useToast();

catch (err: any) {
  handleBackendError(err);
}
```

### **4. AddUser.tsx**

```typescript
// ANTES:
import { parseBackendError } from '../../utils/errorHandler';
const { showError, showSuccess, showValidationErrors } = useToast();

catch (error: any) {
  const { hasValidationErrors, validationErrors, message } = parseBackendError(error);
  
  if (hasValidationErrors && validationErrors) {
    showValidationErrors(validationErrors);
  } else {
    showError(message);
  }
}

// DEPOIS:
const { showSuccess, showError, handleBackendError } = useToast();

catch (error: any) {
  handleBackendError(error);
}

// Mantém showError para validações frontend:
if (!email && !phone && !cpf) {
  showError('Preencha pelo menos um: E-mail, Telefone ou CPF');
  return;
}
```

---

## 🔄 Migração de Código Existente

### **Passo 1: Importar handleBackendError**

```typescript
// ANTES
const { showError, showSuccess } = useToast();

// DEPOIS
const { showSuccess, handleBackendError } = useToast();

// Se tiver validações frontend, manter showError:
const { showSuccess, showError, handleBackendError } = useToast();
```

---

### **Passo 2: Remover import parseBackendError**

```typescript
// ANTES
import { parseBackendError } from '../../utils/errorHandler';

// DEPOIS
// (remover linha)
```

---

### **Passo 3: Substituir catch blocks de API**

```typescript
// ANTES
catch (err: any) {
  const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
  
  if (hasValidationErrors && validationErrors) {
    showValidationErrors(validationErrors);
  } else {
    showError(message);
  }
}

// DEPOIS
catch (err: any) {
  handleBackendError(err);
}
```

---

## ✅ Checklist de Padronização

- [x] ToastContext com handleBackendError
- [x] Títulos removidos dos toasts por padrão
- [x] errorHandler extrai mensagem limpa
- [x] Users.tsx usando handleBackendError
- [x] AddUser.tsx usando handleBackendError
- [ ] EditUser.tsx migrar
- [ ] Roles.tsx migrar
- [ ] RoleForm.tsx migrar
- [ ] Login.tsx migrar (já está bom)
- [ ] Register.tsx migrar (já está bom)
- [ ] Outras telas...

---

## 💡 Exemplos de Mensagens

### **Resposta do Backend:**

```json
{
  "success": false,
  "message": "Validation error in field 'CompanyUser': Não é possível remover o dono da empresa.",
  "errors": {
    "CompanyUser": ["Não é possível remover o dono da empresa."]
  }
}
```

### **Toast Exibido:**

**ANTES:**
```
┌─────────────────────────────────────┐
│ ❌ Erro                             │
│ Validation error in field           │
│ 'CompanyUser': Não é possível       │
│ remover o dono da empresa.          │
└─────────────────────────────────────┘
```

**DEPOIS:**
```
┌─────────────────────────────────────┐
│ ❌ Não é possível remover o dono    │
│    da empresa.                      │
└─────────────────────────────────────┘
```

---

## 🎊 Resultado

**Toasts agora são:**
- ✅ **Limpos:** Sem títulos desnecessários
- ✅ **Concisos:** Apenas a mensagem relevante
- ✅ **User-friendly:** Sem jargão técnico
- ✅ **Padronizados:** Uma linha de código
- ✅ **Consistentes:** Sempre o mesmo comportamento

**Código agora é:**
- ✅ **Simples:** 1 linha vs 8 linhas
- ✅ **Fácil:** Não precisa lembrar de parseBackendError
- ✅ **Seguro:** Sempre trata erros corretamente
- ✅ **Manutenível:** Mudança centralizada no ToastContext

---

## 📚 Documentação

**Arquivos:**
- `frontend/src/contexts/ToastContext.tsx` - Contexto com handleBackendError
- `frontend/src/utils/errorHandler.ts` - Parser de erros do backend
- `tmp/TOAST-STANDARDIZATION.md` - Esta documentação
