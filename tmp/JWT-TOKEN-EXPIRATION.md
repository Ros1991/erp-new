# Configuração de Expiração do Token JWT

## 🎯 Problema

Token de autenticação expirava muito rápido (a cada 2 horas), forçando o usuário a fazer login constantemente.

---

## ✅ Solução Implementada

### **Mudança na Configuração:**

**Arquivo:** `backend/-4-WebApi/appsettings.Auth.json`

```json
{
  "Jwt": {
    "Secret": "your-very-secure-secret-key-min-32-characters-long-change-this-in-production",
    "Issuer": "ERP.API",
    "Audience": "ERP.Client",
    "ExpiresInHours": "8760"  // ← ANTES: "2" | DEPOIS: "8760" (1 ano)
  }
}
```

---

## 📊 Comparação

| Configuração | Antes | Depois |
|-------------|-------|--------|
| **Duração** | 2 horas | 8760 horas |
| **Equivalente** | 2 horas | 1 ano |
| **Expira em** | ~120 minutos | ~365 dias |

---

## 🔧 Como Funciona

### **Geração do Token (TokenService.cs):**

```csharp
var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"] ?? "ERP.API",
    audience: _configuration["Jwt:Audience"] ?? "ERP.Client",
    claims: claims,
    expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiresInHours"] ?? "2")),
    //                                                 ↑ Lê do appsettings.Auth.json
    signingCredentials: credentials
);
```

**Fluxo:**
1. Backend lê `Jwt:ExpiresInHours` do `appsettings.Auth.json`
2. Valor padrão é "2" se não estiver configurado
3. Token é gerado com `expires = DateTime.UtcNow.AddHours(8760)`
4. Token válido por **1 ano**

---

## ⚙️ Para Aplicar a Mudança

### **1. Reiniciar o Backend**

A mudança só entra em vigor após reiniciar o servidor:

```bash
# Parar o backend (Ctrl+C no terminal)
# Iniciar novamente
dotnet run
```

### **2. Fazer Login Novamente**

Os tokens antigos (com 2 horas) ainda vão expirar. Após reiniciar:
1. Fazer logout
2. Fazer login novamente
3. Novo token terá **1 ano** de validade ✅

---

## 🎯 Opções de Duração

Se quiser ajustar para outro valor:

| Duração Desejada | Valor em Horas | Config |
|------------------|----------------|--------|
| 1 dia | 24 | `"ExpiresInHours": "24"` |
| 1 semana | 168 | `"ExpiresInHours": "168"` |
| 1 mês | 720 | `"ExpiresInHours": "720"` |
| 6 meses | 4380 | `"ExpiresInHours": "4380"` |
| **1 ano (atual)** | **8760** | `"ExpiresInHours": "8760"` |
| 10 anos | 87600 | `"ExpiresInHours": "87600"` |
| "Para sempre" | 876000 | `"ExpiresInHours": "876000"` (100 anos) |

---

## 🔒 Validação do Token (JwtConfiguration.cs)

```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidIssuer = configuration["Jwt:Issuer"] ?? "ERP.API",
    ValidateAudience = true,
    ValidAudience = configuration["Jwt:Audience"] ?? "ERP.Client",
    ValidateLifetime = true,  // ← Valida se token expirou
    ClockSkew = TimeSpan.Zero // Sem tolerância de tempo
};
```

---

## 🧪 Como Verificar

### **1. Decodificar o Token JWT:**

1. Fazer login no sistema
2. Abrir DevTools (F12) → Application → LocalStorage
3. Copiar o valor do token
4. Acessar: https://jwt.io/
5. Colar o token
6. Verificar o campo `exp` (expiração em timestamp Unix)

**Exemplo:**
```json
{
  "UserId": "1",
  "email": "admin@empresa.com",
  "exp": 1762876800,  // ← Timestamp de expiração
  "iss": "ERP.API",
  "aud": "ERP.Client"
}
```

**Converter timestamp para data:**
```javascript
new Date(1762876800 * 1000)
// Resultado: ~1 ano a partir de hoje
```

### **2. Verificar no Backend Log:**

Quando o backend inicia, ele carrega a configuração do JWT.

---

## ⚠️ Considerações de Segurança

### **Desenvolvimento:**
- ✅ Token longo é OK
- ✅ Evita interrupções durante desenvolvimento
- ✅ Mais produtivo

### **Produção:**
- ⚠️ Tokens muito longos são menos seguros
- ⚠️ Se o token vazar, fica válido por muito tempo
- 💡 Considerar usar **Refresh Tokens** em produção

### **Melhor Prática em Produção:**
```json
{
  "Jwt": {
    "ExpiresInHours": "8",      // Access Token: 8 horas
    "RefreshTokenDays": "30"    // Refresh Token: 30 dias
  }
}
```

Com refresh tokens, o usuário não precisa fazer login constantemente, mas o access token expira mais rápido (mais seguro).

---

## 📝 Arquivos Envolvidos

1. ✅ **`appsettings.Auth.json`** - Configuração de expiração
2. ✅ **`TokenService.cs`** - Geração do token
3. ✅ **`JwtConfiguration.cs`** - Validação do token
4. ✅ **`JwtMiddleware.cs`** - Middleware de autenticação

---

## 🎊 Resultado

Agora o token dura **1 ano (8760 horas)**, e o usuário não precisa fazer login toda hora! 🚀

**Após reiniciar o backend:**
- ✅ Tokens novos: válidos por 1 ano
- ✅ Login uma vez, trabalhar o ano todo
- ✅ Sem interrupções constantes

---

## 🔄 Refresh Token (Futuro)

Se quiser implementar refresh token no futuro:

**TokenService.cs já tem o método:**
```csharp
public string GenerateRefreshToken()
{
    var randomNumber = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
}
```

**Fluxo com Refresh Token:**
1. Access Token expira em 8 horas
2. Frontend detecta expiração
3. Envia Refresh Token para backend
4. Backend gera novo Access Token
5. Usuário continua logado sem interrupção

**Vantagem:** Mais seguro que token de 1 ano, mas sem pedir login constantemente.
