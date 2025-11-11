# Integração Frontend com Backend - Notas de Implementação

## ✅ Alterações Realizadas

### 1. Configuração da API Base
- **Arquivo:** `src/services/api.ts`
- **Mudança:** Atualizado baseURL de `http://localhost:5000/api` para `https://localhost:8148/api`
- **Nota:** Backend usa HTTPS com certificado auto-assinado

### 2. AuthService Atualizado
- **Arquivo:** `src/services/authService.ts`
- **Mudanças principais:**
  - Interfaces atualizadas para corresponder aos DTOs do backend C#
  - `AuthResponse` agora inclui `refreshToken`, `expiresAt` e `refreshExpiresAt`
  - `RegisterData` agora requer `confirmPassword`
  - `ForgotPasswordData` usa `credential` em vez de `email`
  - `ResetPasswordData` usa `token`, `newPassword` e `confirmPassword`
  - Método `refreshToken()` implementado para renovação de tokens
  - Respostas da API agora extraem dados de `response.data.data` (padrão BaseResponse do backend)

### 3. Páginas de Autenticação Atualizadas
Todas as páginas foram atualizadas para corresponder aos DTOs do backend:

#### Login (`src/pages/auth/Login.tsx`)
- ✅ Já estava correto - usa `credential` e `password`
- Integrado via `useAuth()` hook

#### Register (`src/pages/auth/Register.tsx`)
- ✅ Adicionado `confirmPassword` ao payload
- Redireciona para `/companies` após sucesso

#### ForgotPassword (`src/pages/auth/ForgotPassword.tsx`)
- ✅ Alterado de `{ email }` para `{ credential: email }`
- Redireciona para `/reset-password` após sucesso

#### ResetPassword (`src/pages/auth/ResetPassword.tsx`)
- ✅ Atualizado payload para `{ token, newPassword, confirmPassword }`
- Suporta código via URL query parameter: `?code=ABC123`

### 4. AuthContext Atualizado
- **Arquivo:** `src/contexts/AuthContext.tsx`
- **Mudanças:**
  - Interface `User` atualizada: `userId` em vez de `id`
  - Método `login()` extrai dados do usuário corretamente de `AuthResponse`
  - `checkAuth()` usa dados do localStorage em vez de chamada API
  - Método `logout()` agora é async e envia `refreshToken`

## 🔧 Configuração Necessária

### 1. Aceitar Certificado HTTPS
O backend usa HTTPS com certificado auto-assinado. Para que o frontend funcione:

1. Abra o navegador
2. Acesse: `https://localhost:8148/health`
3. Aceite o aviso de segurança do certificado
4. Você verá: `{"status":"Healthy","timestamp":"...","version":"1.0"}`
5. Recarregue a aplicação frontend

### 2. Verificar Backend está Rodando
```bash
# Teste o health check
curl -k https://localhost:8148/health

# Teste o endpoint de info da API
curl -k https://localhost:8148/api/info
```

## 📋 Correspondência de DTOs

### Backend → Frontend

| Backend DTO | Frontend Interface | Uso |
|-------------|-------------------|-----|
| `LoginRequestDTO` | `LoginData` | Login |
| `RegisterRequestDTO` | `RegisterData` | Registro |
| `ForgotPasswordRequestDTO` | `ForgotPasswordData` | Esqueci senha |
| `ResetPasswordRequestDTO` | `ResetPasswordData` | Reset senha |
| `AuthResponseDTO` | `AuthResponse` | Resposta auth |

### Campos Importantes

**AuthResponse (Backend → Frontend):**
```typescript
{
  userId: number;          // Backend: long UserId
  email?: string;          // Backend: string? Email
  phone?: string;          // Backend: string? Phone
  cpf?: string;            // Backend: string? Cpf
  token: string;           // Backend: string Token
  refreshToken: string;    // Backend: string RefreshToken
  expiresAt: string;       // Backend: DateTime ExpiresAt
  refreshExpiresAt: string;// Backend: DateTime RefreshExpiresAt
}
```

## 🧪 Como Testar

### 1. Iniciar Backend
```bash
cd backend
dotnet run
# Backend iniciará em https://localhost:8148
```

### 2. Iniciar Frontend
```bash
cd frontend
npm run dev
# Frontend iniciará em http://localhost:5173
```

### 3. Fluxo de Teste Completo

#### A. Registro de Novo Usuário
1. Acesse `http://localhost:5173/register`
2. Preencha pelo menos um: email, phone ou CPF
3. Defina uma senha (mínimo 6 caracteres)
4. Confirme a senha
5. Clique em "Criar Conta"
6. Deve redirecionar para `/companies`

#### B. Login
1. Acesse `http://localhost:5173/login`
2. Digite email/phone/CPF cadastrado
3. Digite a senha
4. Clique em "Entrar"
5. Deve redirecionar para `/companies`

#### C. Esqueci Minha Senha
1. Acesse `http://localhost:5173/forgot-password`
2. Digite seu email
3. Clique em "Enviar código de verificação"
4. Deve redirecionar para `/reset-password`

#### D. Redefinir Senha
1. Acesse `http://localhost:5173/reset-password`
2. Digite o código recebido (ou use URL: `?code=ABC123`)
3. Digite nova senha e confirmação
4. Clique em "Redefinir senha"
5. Deve redirecionar para `/login`

#### E. Logout
1. Quando logado, clique em "Sair"
2. Deve limpar tokens e redirecionar para `/login`

## 🐛 Troubleshooting

### Erro: "Network Error" ou "ERR_CERT_AUTHORITY_INVALID"
**Causa:** Certificado HTTPS não foi aceito no navegador
**Solução:** Acesse `https://localhost:8148/health` e aceite o certificado

### Erro: "Access-Control-Allow-Origin"
**Causa:** Problema de CORS
**Solução:** Verificar se backend tem `AllowAll` CORS policy configurada (já está no `Program.cs`)

### Erro: "Cannot find module" ou TypeScript errors
**Causa:** Cache do TypeScript desatualizado
**Solução:**
```bash
# Limpar cache
rm -rf node_modules/.vite
rm -rf node_modules/.tmp

# Reiniciar TypeScript Server no VS Code
Ctrl+Shift+P → "TypeScript: Restart TS Server"
```

### Login/Register não funciona
**Verificar:**
1. Console do navegador (F12) para erros
2. Network tab para ver requisições
3. Response da API para mensagens de erro
4. Backend logs para detalhes do erro

### Tokens não são salvos
**Verificar:**
1. `localStorage` no Developer Tools → Application → Local Storage
2. Deve ter: `token`, `refreshToken`, `user`

## 📝 Próximos Passos

1. ✅ Autenticação básica integrada
2. ⏳ Implementar seleção de empresas
3. ⏳ Implementar refresh token automático antes de expirar
4. ⏳ Adicionar loading states mais robustos
5. ⏳ Implementar tratamento de erros mais específico
6. ⏳ Adicionar testes unitários para authService
7. ⏳ Implementar persistência de sessão (remember me)

## 🔐 Segurança

**Tokens armazenados:**
- `token` (JWT) - Access token
- `refreshToken` - Para renovar o access token
- `user` - Dados básicos do usuário

**Nota:** Em produção, considerar usar `httpOnly cookies` para maior segurança dos tokens.

**Interceptors configurados:**
- Request: Adiciona automaticamente `Authorization: Bearer {token}` 
- Response: Redireciona para `/login` em caso de 401 Unauthorized
