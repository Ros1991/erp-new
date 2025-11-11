# Formatação de Campos - Telefone e CPF

## 📋 Resumo

Os campos de **telefone** e **CPF** agora possuem máscaras visuais nos formulários, mas **enviam dados sem formatação** para o backend.

## ✅ Implementação

### **Campos com Máscara Visual**

#### Register (Cadastro)
- ✅ Campo **Telefone**: Máscara `(00) 00000-0000`
- ✅ Campo **CPF**: Máscara `000.000.000-00`
- ✅ Envio ao backend: **sem formatação** (apenas números)

#### Login
- ✅ Campo **Credential**: Aceita email, telefone ou CPF
- ✅ Se for email (contém `@`): enviado como está
- ✅ Se for telefone/CPF: remove formatação antes de enviar

#### ForgotPassword (Esqueci Senha)
- ✅ Campo **Credential**: Aceita email, telefone ou CPF
- ✅ Se for email (contém `@`): enviado como está
- ✅ Se for telefone/CPF: remove formatação antes de enviar

## 🔧 Como Funciona

### Exemplo: Register

**Usuário digita:**
```
Telefone: (11) 98765-4321
CPF: 123.456.789-00
```

**Formulário exibe (com máscara):**
```
Telefone: (11) 98765-4321
CPF: 123.456.789-00
```

**Enviado ao backend:**
```json
{
  "phone": "11987654321",      // ✅ Sem formatação
  "cpf": "12345678900",         // ✅ Sem formatação
  "email": "user@example.com",
  "password": "senha123",
  "confirmPassword": "senha123"
}
```

### Exemplo: Login

**Usuário digita um dos três:**
```
1. email@exemplo.com       → Enviado: "email@exemplo.com" (mantém @)
2. (11) 98765-4321         → Enviado: "11987654321" (remove formatação)
3. 123.456.789-00          → Enviado: "12345678900" (remove formatação)
```

## 📁 Arquivos Modificados

### 1. `src/pages/auth/Register.tsx`
```typescript
// Antes de enviar ao backend
const cleanPhone = formData.phone ? formData.phone.replace(/\D/g, '') : undefined;
const cleanCpf = formData.cpf ? formData.cpf.replace(/\D/g, '') : undefined;

await authService.register({
  email: formData.email || undefined,
  phone: cleanPhone,    // ✅ Sem formatação
  cpf: cleanCpf,        // ✅ Sem formatação
  password: formData.password,
  confirmPassword: formData.confirmPassword,
});
```

### 2. `src/pages/auth/Login.tsx`
```typescript
// Se não for email (não tem @), remover formatação
const cleanCredential = credential.includes('@') 
  ? credential 
  : credential.replace(/\D/g, '');

await login(cleanCredential, password);
```

### 3. `src/pages/auth/ForgotPassword.tsx`
```typescript
// Se não for email (não tem @), remover formatação
const cleanCredential = credential.includes('@') 
  ? credential 
  : credential.replace(/\D/g, '');

await authService.forgotPassword({ credential: cleanCredential });
```

### 4. `src/utils/formatters.ts` (NOVO)
Utilitários de formatação criados:
- `removeNonNumeric()` - Remove caracteres não numéricos
- `formatCpf()` - Aplica máscara de CPF
- `formatPhone()` - Aplica máscara de telefone
- `cleanCredential()` - Limpa credential (mantém email, limpa telefone/CPF)
- `isValidCpfLength()` - Valida tamanho do CPF
- `isValidPhoneLength()` - Valida tamanho do telefone

## 💡 Uso dos Formatadores

### Exemplo de uso no formulário:

```typescript
import { formatPhone, formatCpf } from '../../utils/formatters';

// No onChange do campo
<Input
  value={phone}
  onChange={(e) => {
    const formatted = formatPhone(e.target.value);
    setPhone(formatted);
  }}
/>
```

### Exemplo antes de enviar:

```typescript
import { cleanCredential, removeNonNumeric } from '../../utils/formatters';

// Limpar credential (email, phone ou CPF)
const clean = cleanCredential(credential);

// Ou apenas remover formatação
const cleanPhone = removeNonNumeric(phone);
```

## ✅ Benefícios

1. **UX melhorada**: Usuário vê campos formatados
2. **Consistência**: Backend recebe sempre dados sem formatação
3. **Validação**: Mais fácil validar apenas números
4. **Flexibilidade**: Usuário pode digitar com ou sem formatação

## 🧪 Testes

### Teste de Registro
1. Preencher telefone: `(11) 98765-4321`
2. Preencher CPF: `123.456.789-00`
3. Ver no Network tab: `phone: "11987654321"`, `cpf: "12345678900"`

### Teste de Login
1. Login com email: `user@example.com` → enviado como está
2. Login com telefone: `(11) 98765-4321` → enviado como `11987654321`
3. Login com CPF: `123.456.789-00` → enviado como `12345678900`

### Teste de Forgot Password
1. Credential com email: `user@example.com` → enviado como está
2. Credential com telefone: `11987654321` → enviado como `11987654321`
3. Credential com CPF: `12345678900` → enviado como `12345678900`

## 📊 Compatibilidade com Backend

O backend espera:

**RegisterRequestDTO:**
```csharp
public string? Email { get; set; }        // Pode conter @
public string? Phone { get; set; }        // Apenas números (máx 20 chars)
public string? Cpf { get; set; }          // Apenas números (11 chars)
```

**LoginRequestDTO e ForgotPasswordRequestDTO:**
```csharp
public string Credential { get; set; }    // Email OU Phone OU CPF
```

✅ **Frontend agora envia exatamente no formato esperado!**
