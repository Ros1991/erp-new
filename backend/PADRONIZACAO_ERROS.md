# Padronização de Respostas de Erro

## 📋 Problema Identificado

Anteriormente, havia **dois formatos diferentes** de resposta de erro:

### 1. Erros de ModelState (ASP.NET padrão)
```json
{
    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
        "Password": ["Password deve ter no mínimo 6 caracteres"]
    },
    "traceId": "00-3d9fe940504155325b7f92078278e313-690dcb89508b4267-00"
}
```

### 2. ValidationException customizada
```json
{
    "data": {
        "errors": {
            "Credentials": ["Credenciais inválidas."]
        }
    },
    "code": 400,
    "message": "Validation error in field 'Credentials': Credenciais inválidas."
}
```

❌ **Problema:** A coleção `errors` vinha dentro de `data` em um caso e na raiz no outro.

## ✅ Solução Implementada

Agora **TODOS os erros** seguem o padrão **BaseResponse** com `errors` na **raiz**:

```json
{
    "data": null,
    "code": 400,
    "message": "One or more validation errors occurred.",
    "errors": {
        "Password": ["Password deve ter no mínimo 6 caracteres"],
        "Credential": ["Email, Phone ou CPF é obrigatório"]
    }
}
```

## 🔧 Mudanças Realizadas

### 1. **BaseResponse.cs** - Adicionada propriedade `Errors`

```csharp
public class BaseResponse<T>
{
    public T Data { get; set; }
    public int Code { get; set; }
    public string Message { get; set; }
    public Dictionary<string, List<string>>? Errors { get; set; } // ✅ NOVO

    // Construtor para erros com validação
    public BaseResponse(int code, string message, Dictionary<string, List<string>> errors)
    {
        Data = default;
        Code = code;
        Message = message;
        Errors = errors; // ✅ Errors na raiz
    }
}
```

### 2. **GlobalExceptionFilter.cs** - Ajustado ValidationException

**Antes:**
```csharp
ValidationException ex => new BaseResponse<object>(
    code: 400,
    message: ex.Message
)
{
    Data = new { errors = ex.Errors } // ❌ Errors dentro de Data
}
```

**Depois:**
```csharp
ValidationException ex => new BaseResponse<object>(
    code: 400,
    message: ex.Message,
    errors: ex.Errors // ✅ Errors na raiz
)
```

### 3. **Program.cs** - Customizado ModelState Validation

Adicionada configuração para customizar resposta de erros do ModelState:

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

        var response = new BaseResponse<object>(
            code: 400,
            message: "One or more validation errors occurred.",
            errors: errors // ✅ Errors na raiz
        );

        return new BadRequestObjectResult(response);
    };
});
```

## 🎯 Formato Final Padronizado

### Resposta de Sucesso
```json
{
    "data": { /* dados */ },
    "code": 200,
    "message": "Operação realizada com sucesso",
    "errors": null
}
```

### Resposta de Erro Simples
```json
{
    "data": null,
    "code": 404,
    "message": "Recurso não encontrado",
    "errors": null
}
```

### Resposta de Erro com Validações
```json
{
    "data": null,
    "code": 400,
    "message": "One or more validation errors occurred.",
    "errors": {
        "Email": ["Email é obrigatório", "Email inválido"],
        "Password": ["Password deve ter no mínimo 6 caracteres"]
    }
}
```

## 📱 Frontend - Ajustes

### errorHandler.ts

```typescript
interface BackendResponse {
  data?: any;
  code: number;
  message: string;
  errors?: Record<string, string[]>; // ✅ Sempre na raiz
}

export function parseBackendError(error: any) {
  const response: BackendResponse = error.response.data;

  // Verificar se tem errors na raiz
  if (response.errors && Object.keys(response.errors).length > 0) {
    return {
      hasValidationErrors: true,
      validationErrors: response.errors,
      message: response.message,
    };
  }

  // Erro simples
  return {
    hasValidationErrors: false,
    message: response.message,
  };
}
```

### Tratamento nos Componentes

```typescript
try {
  await authService.login(data);
} catch (err: any) {
  const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
  
  if (hasValidationErrors && validationErrors) {
    // ✅ Mostra apenas os erros, NÃO mostra o message
    showValidationErrors(validationErrors);
  } else {
    // ✅ Mostra apenas o message
    showError(message);
  }
}
```

## 📊 Comportamento dos Toasts

### Quando há `errors` (validação)
- ✅ Exibe **um toast para cada erro**
- ✅ Cada toast mostra a mensagem de erro específica
- ❌ **NÃO exibe** o `message` geral

**Exemplo:**
```
Toast 1: "Password deve ter no mínimo 6 caracteres"
Toast 2: "Email é obrigatório"
```

### Quando NÃO há `errors` (erro simples)
- ✅ Exibe **um único toast** com o `message`

**Exemplo:**
```
Toast: "Credenciais inválidas"
```

### Em caso de Sucesso
- ✅ Exibe toast de sucesso com o `message`

**Exemplo:**
```
Toast Success: "Login realizado com sucesso!"
```

## ✅ Benefícios

1. **Consistência:** Mesmo formato para todos os erros
2. **Simplicidade:** Frontend trata todos os erros da mesma forma
3. **Clareza:** `errors` sempre na raiz, nunca em `data`
4. **UX:** Toasts mostram apenas informações relevantes
5. **Manutenibilidade:** Código mais limpo e fácil de manter

## 🧪 Testando

### Teste 1: Erro de ModelState
```bash
POST /api/auth/login
{
  "credential": "",
  "password": "123"
}
```

**Resposta esperada:**
```json
{
    "data": null,
    "code": 400,
    "message": "One or more validation errors occurred.",
    "errors": {
        "Credential": ["Credential é obrigatório"],
        "Password": ["Password deve ter no mínimo 6 caracteres"]
    }
}
```

### Teste 2: ValidationException
```bash
POST /api/auth/login
{
  "credential": "invalid@email.com",
  "password": "wrongpass"
}
```

**Resposta esperada:**
```json
{
    "data": null,
    "code": 400,
    "message": "Validation error in field 'Credentials': Credenciais inválidas.",
    "errors": {
        "Credentials": ["Credenciais inválidas."]
    }
}
```

### Teste 3: Sucesso
```bash
POST /api/auth/login
{
  "credential": "user@email.com",
  "password": "senha123"
}
```

**Resposta esperada:**
```json
{
    "data": {
        "userId": 1,
        "email": "user@email.com",
        "token": "...",
        "refreshToken": "..."
    },
    "code": 200,
    "message": "Login realizado com sucesso",
    "errors": null
}
```

## 📝 Resumo

✅ **BaseResponse** agora tem propriedade `Errors`
✅ **Errors** sempre na **raiz**, nunca em `data`
✅ **ModelState** validation usa formato BaseResponse
✅ **ValidationException** usa formato BaseResponse
✅ **Frontend** trata todos os erros uniformemente
✅ **Toasts** mostram apenas erros quando há validação, apenas message quando não há
