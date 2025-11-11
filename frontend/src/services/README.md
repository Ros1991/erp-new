# Services - Camada de Serviços

Esta pasta contém os serviços responsáveis pela comunicação com o backend.

## Arquivos

### `api.ts`
Configuração base do Axios com interceptors para:
- Adicionar automaticamente o token JWT nas requisições
- Adicionar o header `X-Company-Id` quando uma empresa está selecionada
- Redirecionar para login em caso de erro 401

**Configuração:**
```typescript
baseURL: 'https://localhost:8148/api'
```

**⚠️ Nota sobre HTTPS:**
O backend usa HTTPS com certificado auto-assinado. Em desenvolvimento, o navegador pode bloquear requisições. Para resolver:
1. Acesse `https://localhost:8148/health` no navegador
2. Aceite o certificado auto-assinado
3. Recarregue a aplicação frontend

### `authService.ts`
Serviço de autenticação com os seguintes métodos:

#### **Login**
```typescript
await authService.login({
  credential: 'email@exemplo.com', // ou telefone ou CPF
  password: 'senha123'
});
```

#### **Registro**
```typescript
await authService.register({
  email: 'email@exemplo.com',
  phone: '(11) 98765-4321',
  cpf: '123.456.789-00',
  password: 'senha123',
  confirmPassword: 'senha123'
});
```
**Nota:** Preencher pelo menos um dos três: email, phone ou cpf.

#### **Esqueci minha senha**
```typescript
await authService.forgotPassword({
  credential: 'email@exemplo.com' // ou telefone ou CPF
});
```

#### **Redefinir senha**
```typescript
await authService.resetPassword({
  token: 'ABC123', // Código recebido por email
  newPassword: 'novaSenha123',
  confirmPassword: 'novaSenha123'
});
```

#### **Logout**
```typescript
await authService.logout();
```

#### **Renovar token**
```typescript
const result = await authService.refreshToken();
// Retorna AuthResponse com novos tokens ou null se falhar
```

#### **Métodos auxiliares**
```typescript
// Verificar se está autenticado
authService.isAuthenticated(); // true/false

// Obter token
authService.getToken(); // string | null

// Obter usuário
authService.getUser(); // User | null

// Atualizar usuário no localStorage
authService.updateUser(user);
```

## Uso nas páginas

### Login
```typescript
import authService from '../services/authService';

try {
  await authService.login({
    credential: email,
    password: password
  });
  // Redirecionar para /companies
} catch (error) {
  // Tratar erro
}
```

### Register
```typescript
import authService from '../services/authService';

try {
  await authService.register({
    email: formData.email,
    phone: formData.phone,
    cpf: formData.cpf,
    password: formData.password
  });
  // Redirecionar para /login ou /companies
} catch (error) {
  // Tratar erro
}
```

### ForgotPassword
```typescript
import authService from '../services/authService';

try {
  await authService.forgotPassword({ email });
  // Mostrar mensagem de sucesso e redirecionar
} catch (error) {
  // Tratar erro
}
```

### ResetPassword
```typescript
import authService from '../services/authService';

try {
  await authService.resetPassword({ code, password });
  // Redirecionar para login
} catch (error) {
  // Tratar erro
}
```

## Integração com AuthContext

O `AuthContext` já está configurado para usar o `authService`. Use o hook `useAuth()` nas páginas:

```typescript
import { useAuth } from '../contexts/AuthContext';

function MyComponent() {
  const { login, logout, user, isAuthenticated } = useAuth();
  
  const handleLogin = async () => {
    try {
      await login(credential, password);
    } catch (error) {
      // Tratar erro
    }
  };
  
  return (
    // JSX
  );
}
```

## Tratamento de Erros

Todos os métodos podem lançar exceções. Sempre use try/catch:

```typescript
try {
  await authService.login(data);
} catch (error) {
  if (error.response) {
    // Erro da API
    console.error(error.response.data);
  } else {
    // Erro de rede ou outro
    console.error(error.message);
  }
}
```

## Endpoints do Backend (já implementados)

| Método | Endpoint | Body | Response |
|--------|----------|------|----------|
| POST | `/api/auth/login` | `{ credential, password }` | `{ data: AuthResponseDTO }` |
| POST | `/api/auth/register` | `{ email?, phone?, cpf?, password, confirmPassword }` | `{ data: AuthResponseDTO }` |
| POST | `/api/auth/forgot-password` | `{ credential }` | `{ success, message }` |
| POST | `/api/auth/reset-password` | `{ token, newPassword, confirmPassword }` | `{ success, message }` |
| POST | `/api/auth/logout` | `refreshToken (string)` | `{ success, message }` |
| POST | `/api/auth/refresh-token` | `refreshToken (string)` | `{ data: AuthResponseDTO }` |

### AuthResponseDTO
```typescript
{
  userId: number;
  email?: string;
  phone?: string;
  cpf?: string;
  token: string;
  refreshToken: string;
  expiresAt: string;
  refreshExpiresAt: string;
}
```

**Base URL:** `https://localhost:8148/api`
