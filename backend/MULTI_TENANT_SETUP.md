# 🏢 Sistema Multi-Tenant por Company

## 📋 Conceito

Todos os dados do sistema são isolados por `CompanyId`. Cada requisição (exceto Auth e Company) **deve** incluir o header `X-Company-Id`.

## 🔧 Configuração no Program.cs

```csharp
using ERP.WebApi.Middlewares;

var app = builder.Build();

// ORDEM IMPORTANTE DOS MIDDLEWARES:
app.UseMiddleware<JwtMiddleware>();           // 1️⃣ Valida JWT e anexa UserId
app.UseMiddleware<CompanyContextMiddleware>(); // 2️⃣ Valida e anexa CompanyId
app.UseAuthentication();                       // 3️⃣ Authentication
app.UseAuthorization();                        // 4️⃣ Authorization

app.MapControllers();
app.Run();
```

## 🚫 Endpoints Excluídos (não precisam de CompanyId)

| Endpoint | Motivo |
|----------|--------|
| `/api/auth/*` | Autenticação pública |
| `/api/company/*` | Gerencia as próprias companies |
| `/swagger` | Documentação |
| `/health` | Health check |

## ✅ Endpoints que PRECISAM de CompanyId

Todos os outros endpoints **DEVEM** incluir o header:

```http
X-Company-Id: 1
```

Exemplos:
- `/api/account/*` ✅ Precisa
- `/api/user/*` ✅ Precisa
- `/api/products/*` ✅ Precisa
- `/api/orders/*` ✅ Precisa

## 📡 Exemplos de Requisições

### ✅ Requisição Correta

```http
GET /api/account/getAll
Authorization: Bearer eyJhbGc...
X-Company-Id: 1
```

**Resposta: 200 OK**

### ❌ Sem CompanyId

```http
GET /api/account/getAll
Authorization: Bearer eyJhbGc...
(sem X-Company-Id)
```

**Resposta: 400 Bad Request**
```json
{
  "code": 400,
  "message": "CompanyId é obrigatório. Envie via header 'X-Company-Id'.",
  "data": null
}
```

### ❌ CompanyId Inválido

```http
GET /api/account/getAll
Authorization: Bearer eyJhbGc...
X-Company-Id: 0
```

**Resposta: 400 Bad Request**
```json
{
  "code": 400,
  "message": "CompanyId deve ser maior que zero.",
  "data": null
}
```

## 💻 Uso no Controller

```csharp
[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : BaseController
{
    [HttpGet("getAll")]
    public async Task<ActionResult> GetAll()
    {
        // ✅ Obtém CompanyId do contexto (já validado pelo middleware)
        var companyId = GetCompanyId();
        
        // ✅ Passar para o service
        var accounts = await _service.GetAllByCompanyAsync(companyId);
        
        return Ok(accounts);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult> Create(AccountInputDTO dto)
    {
        var companyId = GetCompanyId();
        var userId = GetCurrentUserId();
        
        // ✅ Força CompanyId do contexto (segurança)
        var account = await _service.CreateAsync(dto, companyId, userId);
        
        return Ok(account);
    }
}
```

## 🔒 Segurança - Validação de Acesso

### TODO: Implementar validação se usuário tem acesso à Company

No `CompanyContextMiddleware.cs`, adicione:

```csharp
// TODO: Validar se o usuário tem acesso a essa Company
var userId = context.GetUserId();
var hasAccess = await ValidateUserCompanyAccess(userId, companyId);

if (!hasAccess) 
{
    context.Response.StatusCode = 403;
    await context.Response.WriteAsJsonAsync(new 
    { 
        code = 403,
        message = "Você não tem acesso a esta empresa.",
        data = (object)null
    });
    return;
}
```

### Implementação Sugerida

Criar um service para validar:

```csharp
public interface ICompanyAccessService
{
    Task<bool> UserHasAccessToCompany(long userId, long companyId);
}

public class CompanyAccessService : ICompanyAccessService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<bool> UserHasAccessToCompany(long userId, long companyId)
    {
        // Verifica na tabela CompanyUser se o usuário tem acesso
        var companyUsers = await _unitOfWork.CompanyUserRepository.GetAllAsync();
        return companyUsers.Any(cu => cu.UserId == userId && cu.CompanyId == companyId);
    }
}
```

## 📊 Fluxo Completo

```
┌─────────┐                ┌──────────────────┐                ┌──────────┐
│ Cliente │                │   Middlewares    │                │   API    │
└────┬────┘                └────────┬─────────┘                └─────┬────┘
     │                              │                                │
     │  GET /api/account/getAll     │                                │
     │  Authorization: Bearer ...   │                                │
     │  X-Company-Id: 1            │                                │
     ├─────────────────────────────>│                                │
     │                              │                                │
     │                              │  JwtMiddleware                 │
     │                              │  ├─ Valida JWT                 │
     │                              │  └─ Anexa UserId               │
     │                              │                                │
     │                              │  CompanyContextMiddleware      │
     │                              │  ├─ Extrai X-Company-Id        │
     │                              │  ├─ Valida CompanyId           │
     │                              │  ├─ Valida Acesso (TODO)       │
     │                              │  └─ Anexa CompanyId            │
     │                              │                                │
     │                              ├───────────────────────────────>│
     │                              │                                │
     │                              │          Controller            │
     │                              │          ├─ GetCompanyId()     │
     │                              │          ├─ GetCurrentUserId() │
     │                              │          └─ Buscar Dados       │
     │                              │                                │
     │        Data (200 OK)         │                                │
     │<─────────────────────────────┴────────────────────────────────┤
     │                                                               │
```

## 🎯 Benefícios

1. ✅ **Isolamento de Dados** - Cada company vê apenas seus dados
2. ✅ **Segurança** - Impossível acessar dados de outra company
3. ✅ **Simplicidade** - Header único para todas as requisições
4. ✅ **Rastreabilidade** - CompanyId logado em todas as operações
5. ✅ **Escalabilidade** - Fácil adicionar novas companies

## 🧪 Testando

### Com Postman

1. Faça login:
```http
POST /api/auth/login
{ "credential": "user@example.com", "password": "senha123" }
```

2. Copie o token da resposta

3. Configure headers:
```
Authorization: Bearer {seu_token}
X-Company-Id: 1
```

4. Teste endpoints:
```http
GET /api/account/getAll
GET /api/user/getAll
POST /api/account/create
```

### Com cURL

```bash
curl -X GET "http://localhost:5000/api/account/getAll" \
  -H "Authorization: Bearer eyJhbGc..." \
  -H "X-Company-Id: 1"
```

### Com JavaScript/Axios

```javascript
// Configurar interceptor global
axios.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  const companyId = localStorage.getItem('companyId');
  
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  
  if (companyId) {
    config.headers['X-Company-Id'] = companyId;
  }
  
  return config;
});

// Fazer requisições normalmente
const accounts = await axios.get('/api/account/getAll');
```

## 📝 Checklist de Implementação

- [x] Criar `CompanyContextMiddleware`
- [x] Adicionar extensions `GetCompanyId()` no HttpContext
- [x] Adicionar methods `GetCompanyId()` no BaseController
- [x] Registrar middleware no `Program.cs`
- [ ] Implementar validação de acesso usuário-company
- [ ] Atualizar Services para receber `companyId`
- [ ] Adicionar filtros automáticos nos Repositories
- [ ] Criar testes de integração
- [ ] Documentar no Swagger

## 🚨 Importante

- **NUNCA** confie no `CompanyId` enviado no DTO
- **SEMPRE** use o `CompanyId` do contexto (obtido via `GetCompanyId()`)
- **VALIDE** se o usuário tem acesso à company (implementar)

