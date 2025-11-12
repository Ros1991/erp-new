# Correção: Busca em Phone/CPF Apenas Quando Houver Dígitos

## 🐛 Problema Identificado

### **Cenário:**
```
Usuário digita: "teste"
  ↓
Regex remove tudo que não é dígito: @"[^\d]"
  ↓
onlyDigits = ""  (string vazia)
  ↓
Query: cpf.Contains("")
  ↓
❌ Retorna TRUE para QUALQUER CPF não-null!
```

### **Causa Raiz:**
O regex `@"[^\d]"` significa "tudo que NÃO é dígito", então:
- `"teste"` → `""` (vazio)
- `"11999998888"` → `"11999998888"` (ok)
- `"(11) 99999-9999"` → `"11999999999"` (ok)

**Problema:** `string.Contains("")` sempre retorna `true` em C#!

---

## ✅ Solução Implementada

### **Lógica Corrigida:**

```csharp
var searchLower = filters.SearchTerm.ToLower();
// Extrair APENAS dígitos do termo
var onlyDigits = Regex.Replace(searchLower, @"[^\d]", "");

query = query.Where(cu => 
    // Email: busca normal
    (cu.User.Email != null && cu.User.Email.ToLower().Contains(searchLower)) ||
    
    // Phone: busca APENAS se houver dígitos no termo
    (!string.IsNullOrEmpty(onlyDigits) && cu.User.Phone != null && cu.User.Phone.Contains(onlyDigits)) ||
    
    // CPF: busca APENAS se houver dígitos no termo
    (!string.IsNullOrEmpty(onlyDigits) && cu.User.Cpf != null && cu.User.Cpf.Contains(onlyDigits)) ||
    
    // Cargo: busca normal
    (cu.Role != null && cu.Role.Name.ToLower().Contains(searchLower))
);
```

### **Diferença:**

**ANTES (errado):**
```csharp
(cu.User.Phone != null && cu.User.Phone.Contains(cleanSearch))
```
- Se `cleanSearch = ""`, retorna TRUE para TODOS os phones não-null

**DEPOIS (correto):**
```csharp
(!string.IsNullOrEmpty(onlyDigits) && cu.User.Phone != null && cu.User.Phone.Contains(onlyDigits))
```
- Se `onlyDigits = ""`, a condição já é FALSE (não busca em phone)

---

## 📋 Exemplos de Comportamento

### **Exemplo 1: Busca por Email (texto)**
```
Input: "teste"
  ↓
searchLower = "teste"
onlyDigits = ""
  ↓
Busca em:
✅ Email: email.Contains("teste")
❌ Phone: NÃO busca (onlyDigits está vazio)
❌ CPF: NÃO busca (onlyDigits está vazio)
✅ Cargo: role.Contains("teste")
```

### **Exemplo 2: Busca por Telefone (com formatação)**
```
Input: "(11) 99999-9999"
  ↓
searchLower = "(11) 99999-9999"
onlyDigits = "11999999999"
  ↓
Busca em:
✅ Email: email.Contains("(11) 99999-9999")
✅ Phone: phone.Contains("11999999999")  ← FUNCIONA!
✅ CPF: cpf.Contains("11999999999")
✅ Cargo: role.Contains("(11) 99999-9999")
```

### **Exemplo 3: Busca por Telefone (sem formatação)**
```
Input: "11999998888"
  ↓
searchLower = "11999998888"
onlyDigits = "11999998888"
  ↓
Busca em:
✅ Email: email.Contains("11999998888")
✅ Phone: phone.Contains("11999998888")  ← FUNCIONA!
✅ CPF: cpf.Contains("11999998888")
✅ Cargo: role.Contains("11999998888")
```

### **Exemplo 4: Busca por CPF (com formatação)**
```
Input: "123.456.789-00"
  ↓
searchLower = "123.456.789-00"
onlyDigits = "12345678900"
  ↓
Busca em:
✅ Email: email.Contains("123.456.789-00")
✅ Phone: phone.Contains("12345678900")
✅ CPF: cpf.Contains("12345678900")  ← FUNCIONA!
✅ Cargo: role.Contains("123.456.789-00")
```

### **Exemplo 5: Busca por Cargo**
```
Input: "Gerente"
  ↓
searchLower = "gerente"
onlyDigits = ""
  ↓
Busca em:
✅ Email: email.Contains("gerente")
❌ Phone: NÃO busca (onlyDigits está vazio)
❌ CPF: NÃO busca (onlyDigits está vazio)
✅ Cargo: role.Contains("gerente")  ← FUNCIONA!
```

---

## 🎯 Lógica de Decisão

```
Termo de busca tem dígitos?
  ├─ SIM → Busca em: Email, Phone, CPF, Cargo
  └─ NÃO → Busca em: Email, Cargo (não busca em Phone/CPF)
```

**Por quê?**
- **Email e Cargo:** podem conter texto → sempre buscar
- **Phone e CPF:** são APENAS números → buscar APENAS se termo tiver números

---

## 📊 Comparação: Antes vs Depois

| Busca | Antes (bug) | Depois (correto) |
|-------|-------------|------------------|
| `"teste"` | Retorna TODOS os users com phone/cpf não-null | Retorna apenas emails/cargos com "teste" ✅ |
| `"11999998888"` | Funciona | Funciona ✅ |
| `"(11) 99999-9999"` | Funciona | Funciona ✅ |
| `"Gerente"` | Retorna TODOS os users com phone/cpf não-null | Retorna apenas cargos "Gerente" ✅ |

---

## 🔧 Arquivos Modificados

1. ✅ `companyUserRepository.cs` - Busca de CompanyUser
2. ✅ `userRepository.cs` - Busca de User

**Ambos com a mesma lógica:** só buscar em phone/cpf SE houver dígitos no termo.

---

## ✅ Benefícios

1. ✅ **Correção do bug:** Não retorna mais todos os usuários ao buscar texto
2. ✅ **Performance:** Menos comparações desnecessárias
3. ✅ **Lógica clara:** Se não tem dígitos, não busca em campos numéricos
4. ✅ **Flexibilidade:** Usuário pode buscar com ou sem formatação

---

## 🧪 Testes

### **Teste 1: Buscar por nome de cargo**
```
Input: "Gerente"
Esperado: Apenas usuários com cargo "Gerente"
✅ Passa
```

### **Teste 2: Buscar por email**
```
Input: "joao@empresa.com"
Esperado: Usuário com esse email
✅ Passa
```

### **Teste 3: Buscar por telefone com formatação**
```
Input: "(11) 99999-9999"
Esperado: Usuário com telefone 11999999999
✅ Passa
```

### **Teste 4: Buscar por telefone sem formatação**
```
Input: "11999998888"
Esperado: Usuário com telefone 11999998888
✅ Passa
```

### **Teste 5: Buscar por CPF com formatação**
```
Input: "123.456.789-00"
Esperado: Usuário com CPF 12345678900
✅ Passa
```

### **Teste 6: Buscar texto qualquer (bug anterior)**
```
Input: "teste"
Antes: Retornava TODOS os usuários com phone/cpf não-null ❌
Depois: Retorna apenas emails/cargos contendo "teste" ✅
```

---

## 💡 Lição Aprendida

**String.Contains("")** sempre retorna `true` em C#!

Sempre verificar se a string de busca não está vazia antes de usar Contains:
```csharp
// ❌ ERRADO
if (field.Contains(searchTerm))

// ✅ CORRETO
if (!string.IsNullOrEmpty(searchTerm) && field.Contains(searchTerm))
```

---

## 🎊 Resultado

Agora a busca funciona perfeitamente:
- ✅ Busca por email
- ✅ Busca por telefone (com ou sem formatação)
- ✅ Busca por CPF (com ou sem formatação)
- ✅ Busca por cargo
- ✅ NÃO retorna falsos positivos!
