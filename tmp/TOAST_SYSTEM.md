# Sistema de Toast - Notificações

## 📋 Visão Geral

Sistema de notificações toast implementado para substituir alertas inline nas páginas de autenticação. Os toasts aparecem no canto superior direito da tela e podem ser fechados automaticamente ou por clique.

## 🎨 Componentes Criados

### 1. **Toast.tsx**
Componente visual individual do toast.

**Localização:** `src/components/ui/Toast.tsx`

**Propriedades:**
```typescript
interface ToastProps {
  id: string;              // ID único do toast
  type: ToastType;         // 'success' | 'error' | 'warning' | 'info'
  title?: string;          // Título opcional
  message: string;         // Mensagem principal
  duration?: number;       // Duração em ms (padrão: 5000)
  onClose: (id: string) => void;  // Callback de fechamento
}
```

**Características:**
- ✅ Fecha automaticamente após `duration` ms
- ✅ Pode ser fechado clicando no toast ou no botão X
- ✅ Animação de hover com scale
- ✅ Cores diferenciadas por tipo
- ✅ Ícones apropriados (CheckCircle, AlertCircle, etc.)

### 2. **ToastContext.tsx**
Contexto global para gerenciamento de toasts.

**Localização:** `src/contexts/ToastContext.tsx`

**Métodos disponíveis:**
```typescript
interface ToastContextType {
  showToast(type, message, title?, duration?): void;
  showSuccess(message, title?): void;
  showError(message, title?): void;
  showWarning(message, title?): void;
  showInfo(message, title?): void;
  showValidationErrors(errors): void;
}
```

## 🔧 Configuração

### 1. App.tsx
O `ToastProvider` foi adicionado ao App:

```typescript
<Router>
  <ToastProvider>
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  </ToastProvider>
</Router>
```

### 2. Uso nas Páginas

Todas as páginas de autenticação foram atualizadas:
- ✅ Login
- ✅ Register
- ✅ ForgotPassword
- ✅ ResetPassword

## 📖 Como Usar

### Importar o Hook

```typescript
import { useToast } from '../../contexts/ToastContext';

function MyComponent() {
  const { showSuccess, showError, showValidationErrors } = useToast();
  
  // ...
}
```

### Exibir Toast Simples

```typescript
// Sucesso
showSuccess('Operação realizada com sucesso!');

// Erro
showError('Ocorreu um erro ao processar sua solicitação');

// Aviso
showWarning('Atenção: dados incompletos');

// Informação
showInfo('Este recurso está em beta');
```

### Exibir Toast com Título Customizado

```typescript
showSuccess('Conta criada com sucesso!', 'Bem-vindo');
showError('Senha incorreta', 'Erro de Autenticação');
```

### Exibir Toast com Duração Customizada

```typescript
showToast('info', 'Mensagem importante', 'Atenção', 10000); // 10 segundos
```

### Exibir Erros de Validação do Backend

```typescript
try {
  await authService.login(data);
} catch (err: any) {
  const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
  
  if (hasValidationErrors && validationErrors) {
    showValidationErrors(validationErrors); // ✅ Múltiplos toasts, um por erro
  } else {
    showError(message);
  }
}
```

## 🎯 Parser de Erros do Backend

### errorHandler.ts

**Localização:** `src/utils/errorHandler.ts`

Função principal: `parseBackendError(error)`

**Estrutura de erro do backend:**
```typescript
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Password": ["Password deve ter no mínimo 6 caracteres"],
    "Credential": ["Email, Phone ou CPF é obrigatório"]
  },
  "traceId": "00-..."
}
```

**Retorno da função:**
```typescript
{
  hasValidationErrors: boolean;
  validationErrors?: Record<string, string[]>;
  message: string;
}
```

## 🎨 Estilos por Tipo

### Success (Verde)
- Fundo: `bg-green-50`
- Borda: `border-green-500`
- Ícone: `CheckCircle` verde

### Error (Vermelho)
- Fundo: `bg-red-50`
- Borda: `border-red-500`
- Ícone: `AlertCircle` vermelho

### Warning (Amarelo)
- Fundo: `bg-yellow-50`
- Borda: `border-yellow-500`
- Ícone: `AlertTriangle` amarelo

### Info (Azul)
- Fundo: `bg-blue-50`
- Borda: `border-blue-500`
- Ícone: `Info` azul

## 📍 Posicionamento

Toasts aparecem no **canto superior direito** da tela:
```css
position: fixed;
top: 16px;
right: 16px;
z-index: 50;
```

Empilhamento: Toasts aparecem um abaixo do outro com `space-y-2`.

## ✨ Funcionalidades

### Auto-close
- **Duração padrão:** 5 segundos
- **Customizável:** Pode ser alterado por toast
- **Desabilitar:** Passar `duration={0}`

### Fechamento Manual
- **Clicar no toast:** Fecha o toast
- **Botão X:** Botão específico de fechar
- **StopPropagation:** Botão X não propaga o clique

### Animações
- **Hover:** Scale 1.05 + shadow aumentada
- **Transições:** `transition-all duration-300`

## 🔄 Exemplo Completo de Uso

```typescript
import { useToast } from '../../contexts/ToastContext';
import { parseBackendError } from '../../utils/errorHandler';
import authService from '../../services/authService';

function Login() {
  const { showValidationErrors, showError, showSuccess } = useToast();
  const [credential, setCredential] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validações locais
    if (!credential) {
      showError('Por favor, insira seu e-mail, telefone ou CPF');
      return;
    }

    try {
      await authService.login({ credential, password });
      showSuccess('Login realizado com sucesso!');
    } catch (err: any) {
      const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
      
      if (hasValidationErrors && validationErrors) {
        // Exibe um toast para cada erro de validação
        showValidationErrors(validationErrors);
      } else {
        // Exibe um único toast de erro
        showError(message);
      }
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Formulário sem alertas inline */}
    </form>
  );
}
```

## 📊 Benefícios

✅ **Melhor UX:** Notificações não-invasivas
✅ **Consistência:** Mesmo estilo em toda aplicação
✅ **Flexibilidade:** Múltiplos toasts simultâneos
✅ **Acessibilidade:** Pode ser fechado por clique ou automaticamente
✅ **Validação:** Suporte nativo para erros do backend
✅ **Clean Code:** Sem alertas inline poluindo JSX

## 🚀 Próximos Passos

- [ ] Adicionar suporte a ações nos toasts (ex: "Desfazer")
- [ ] Adicionar limite de toasts simultâneos
- [ ] Adicionar posicionamento configurável (top-left, bottom-right, etc.)
- [ ] Adicionar sons de notificação (opcional)
- [ ] Adicionar persistência de toasts importantes
