# Busca Inteligente - Remoção Automática de Formatação

## 🎯 Funcionalidade

A busca remove automaticamente caracteres especiais do termo de busca para **telefone e CPF**, pois eles são **salvos SEM formatação no banco**.

**Phone e CPF no banco:** `"11999998888"` e `"12345678900"` (sem parênteses, traços, pontos)

Isso permite que o usuário busque:
- `"11999998888"` → Encontra `"11999998888"`
- `"(11) 99999-8888"` → Remove formatação → Encontra `"11999998888"`
- `"123.456.789-00"` → Remove formatação → Encontra `"12345678900"`
- `"joao@email.com"` → Busca normalmente no email

---

## 🔧 Implementação

### Backend (CompanyUserRepository)

```csharp
var searchLower = filters.SearchTerm.ToLower();

// Phone e CPF são salvos SEM formatação, então remove caracteres especiais do termo
var cleanSearch = Regex.Replace(searchLower, @"[^\d]", "");
// Remove: ( ) - . espaço e qualquer não-dígito

query = query.Where(cu => 
    // Email: busca normal com ToLower
    (cu.User.Email != null && cu.User.Email.ToLower().Contains(searchLower)) ||
    
    // Phone: SEMPRE busca sem formatação (banco não tem formatação)
    (cu.User.Phone != null && cu.User.Phone.Contains(cleanSearch)) ||
    
    // CPF: SEMPRE busca sem formatação (banco não tem formatação)
    (cu.User.Cpf != null && cu.User.Cpf.Contains(cleanSearch)) ||
    
    // Cargo: busca normal com ToLower
    (cu.Role != null && cu.Role.Name.ToLower().Contains(searchLower))
);
```

**Por que é simples?**
- Phone e CPF no banco = `"11999998888"` e `"12345678900"` (SEM formatação)
- Basta remover formatação do termo de busca e comparar diretamente!

---

## 📊 Exemplos de Busca

### 1. **Busca por Email**
```
Busca: "joao@"
Termo usado: "joao@" (ToLower)
Encontra: 
  ✅ Email "joao@empresa.com"
  ✅ Email "maria_joao@teste.com"
```

### 2. **Busca por Telefone**
```
Busca: "11999998888"
Termo usado: "11999998888" (apenas dígitos)
Banco tem: "11999998888" (sem formatação)
Encontra: ✅
  
Busca: "(11) 99999-8888"
Termo usado: "11999998888" (remove formatação)
Banco tem: "11999998888" (sem formatação)
Encontra: ✅

Busca: "99999"
Termo usado: "99999" (apenas dígitos)
Banco tem: "11999998888"
Encontra: ✅ (contém 99999)
```

### 3. **Busca por CPF**
```
Busca: "12345678900"
Termo usado: "12345678900" (apenas dígitos)
Banco tem: "12345678900" (sem formatação)
Encontra: ✅
  
Busca: "123.456.789-00"
Termo usado: "12345678900" (remove formatação)
Banco tem: "12345678900" (sem formatação)
Encontra: ✅

Busca: "123456"
Termo usado: "123456" (apenas dígitos)
Banco tem: "12345678900"
Encontra: ✅ (contém 123456)
```

### 4. **Busca por Cargo**
```
Busca: "gerente"
Termo usado: "gerente" (ToLower)
Encontra: 
  ✅ Usuários com cargo "Gerente"
```

---

## 🧮 Regex Usado

### Remover caracteres não-numéricos:
```csharp
Regex.Replace(searchLower, @"[^\d]", "")
```

**O que remove:**
- `(` `)` → Parênteses
- `-` → Traço/hífen
- `.` → Ponto
- ` ` → Espaço
- Letras, símbolos, etc.

**Mantém apenas:**
- `0-9` → Dígitos

---

## 🎨 SQL Gerado (Aproximado)

```sql
SELECT cu.*, u.*, r.*
FROM tb_company_user cu
INNER JOIN tb_user u ON cu.user_id = u.user_id
LEFT JOIN tb_role r ON cu.role_id = r.role_id
WHERE cu.company_id = 27
  AND (
    -- Email: busca com ToLower
    LOWER(u.user_email) LIKE '%termo%'
    OR
    -- Phone: banco já está sem formatação, busca direta
    u.user_phone LIKE '%11999998888%'
    OR
    -- CPF: banco já está sem formatação, busca direta
    u.user_cpf LIKE '%12345678900%'
    OR
    -- Cargo: busca com ToLower
    LOWER(r.role_name) LIKE '%termo%'
  );
```

**Por que é eficiente?**
- Não precisa de `REPLACE` no banco (phone e cpf já estão limpos)
- Apenas remove formatação do termo de busca na aplicação
- Query SQL mais simples e rápida

---

## 🔍 Similaridade com Login

### Login (Frontend)
```typescript
// Remove formatação se não for email
const cleanCredential = credential.includes('@') 
  ? credential 
  : credential.replace(/\D/g, '');
```

### Busca (Backend)
```csharp
// SEMPRE remove formatação do termo para phone/cpf
var cleanSearch = Regex.Replace(searchLower, @"[^\d]", "");

// Phone e CPF: busca com termo limpo
cu.User.Phone.Contains(cleanSearch)
cu.User.Cpf.Contains(cleanSearch)

// Email: busca com termo original
cu.User.Email.ToLower().Contains(searchLower)
```

**Por que é mais simples na busca?**
- **Login:** Precisa detectar @ porque email pode ter ou não
- **Busca:** Phone e CPF no banco SEMPRE estão sem formatação
- Resultado: Apenas remove formatação do termo, sem lógica condicional

---

## ✅ Vantagens

1. **UX Melhor:** Usuário não precisa saber a formatação exata
2. **Flexível:** Aceita `11999998888`, `(11) 99999-8888`, `11 99999-8888`
3. **Simples:** Apenas remove formatação do termo, sem condicionais
4. **Performance:** Query SQL otimizada (sem REPLACE no banco)
5. **Consistente:** Phone e CPF sempre sem formatação no banco

---

## 📝 Casos de Teste

| Termo de Busca | Termo Limpo | Encontra no Banco |
|----------------|-------------|-------------------|
| `joao@empresa.com` | `"joao@empresa.com"` | Email `"joao@empresa.com"` ✅ |
| `11999998888` | `"11999998888"` | Phone `"11999998888"` ✅ |
| `(11) 99999-8888` | `"11999998888"` | Phone `"11999998888"` ✅ |
| `123.456.789-00` | `"12345678900"` | CPF `"12345678900"` ✅ |
| `12345678900` | `"12345678900"` | CPF `"12345678900"` ✅ |
| `gerente` | `"gerente"` | Cargo `"Gerente"` ✅ |

---

## 🚀 Benefícios Reais

**Cenário:** Banco tem phone = `"11999998888"` (sem formatação)

**Busca 1:** `"11999998888"`
```
Remove formatação: "11999998888"
Banco tem: "11999998888"
Resultado: ✅ Encontrado!
```

**Busca 2:** `"(11) 99999-8888"`
```
Remove formatação: "11999998888"
Banco tem: "11999998888"
Resultado: ✅ Encontrado!
```

**Busca 3:** `"11 99999-8888"`
```
Remove formatação: "11999998888"
Banco tem: "11999998888"
Resultado: ✅ Encontrado!
```

**Conclusão:** Usuário pode digitar COM ou SEM formatação, sempre encontra!

---

## 📚 Arquivo Modificado

- ✅ `backend/-3-Infrastructure/Repositories/companyUserRepository.cs`
  - Método `GetPagedAsync`
  - Linhas 38-67
