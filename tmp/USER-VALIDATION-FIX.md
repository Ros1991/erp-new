# Correção: Validação de Criação de Usuário

## 🐛 Problema

Ao criar um usuário preenchendo **apenas o email**, o backend retornava erros dizendo que Phone e CPF eram obrigatórios:

```
❌ The Cpf field is required.
❌ The Phone field is required.
```

**Isso estava ERRADO!** A validação deve ser igual à do Register (autenticação):
- **Pelo menos UM** dos três (Email, Phone ou CPF) deve estar preenchido
- **NÃO** todos são obrigatórios

---

## ✅ Solução Implementada

### **1. UserInputDTO - Tornar Campos Opcionais**

**Arquivo:** `backend/-2-Application/DTOs/UserInputDTO.cs`

**ANTES (errado):**
```csharp
public class UserInputDTO
{
    public string Email { get; set; }          // ❌ Não-nullable
    public string Phone { get; set; }          // ❌ Não-nullable
    public string Cpf { get; set; }            // ❌ Não-nullable
    
    [Required]
    public string PasswordHash { get; set; }   // ❌ Nome errado
}
```

**DEPOIS (correto):**
```csharp
public class UserInputDTO
{
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string? Email { get; set; }         // ✅ Opcional (nullable)
    
    [StringLength(20, ErrorMessage = "Phone deve ter no máximo 20 caracteres")]
    public string? Phone { get; set; }         // ✅ Opcional (nullable)
    
    [StringLength(11, ErrorMessage = "Cpf deve ter no máximo 11 caracteres")]
    public string? Cpf { get; set; }           // ✅ Opcional (nullable)
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; }       // ✅ Nome correto, obrigatória
}
```

**Mudanças:**
1. ✅ Todos os identificadores agora são **nullable** (`string?`)
2. ✅ `PasswordHash` renomeado para `Password`
3. ✅ Adicionado `[MinLength(6)]` na senha
4. ✅ Igual ao `RegisterRequestDTO`

---

### **2. UserMapper - Atualizar Referências**

**Arquivo:** `backend/-2-Application/Mappers/userMapper.cs`

**ANTES (errado):**
```csharp
public static User ToEntity(UserInputDTO dto)
{
    return new User(
        dto.Email,
        dto.Phone,
        dto.Cpf,
        dto.PasswordHash,  // ❌ Campo não existe mais
        null,
        null
    );
}

public static void UpdateEntity(User entity, UserInputDTO dto)
{
    entity.Email = dto.Email;
    entity.Phone = dto.Phone;
    entity.Cpf = dto.Cpf;
    entity.PasswordHash = dto.PasswordHash;  // ❌ Campo não existe mais
}
```

**DEPOIS (correto):**
```csharp
public static User ToEntity(UserInputDTO dto)
{
    return new User(
        dto.Email,
        dto.Phone,
        dto.Cpf,
        dto.Password,  // ✅ Campo correto
        null,
        null
    );
}

public static void UpdateEntity(User entity, UserInputDTO dto)
{
    entity.Email = dto.Email;
    entity.Phone = dto.Phone;
    entity.Cpf = dto.Cpf;
    entity.PasswordHash = dto.Password;  // ✅ Campo correto
}
```

---

### **3. UserService - Validação Já Existe**

**Arquivo:** `backend/-2-Application/Services/userService.cs`

A validação de "pelo menos um identificador" **já existia**:

```csharp
public async Task<UserOutputDTO> CreateAsync(UserInputDTO dto)
{
    if (dto == null)
        throw new ValidationException(nameof(dto), "Dados são obrigatórios.");

    ValidateAtLeastOneContact(dto);  // ✅ Validação já existia

    var entity = UserMapper.ToEntity(dto);
    var createdEntity = await _unitOfWork.UserRepository.CreateAsync(entity);
    await _unitOfWork.SaveChangesAsync();
    return UserMapper.ToUserOutputDTO(createdEntity);
}

private void ValidateAtLeastOneContact(UserInputDTO dto)
{
    bool hasValidEmail = !string.IsNullOrWhiteSpace(dto.Email) && dto.Email.Length >= 5;
    bool hasValidPhone = !string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone.Length >= 8;
    bool hasValidCpf = !string.IsNullOrWhiteSpace(dto.Cpf) && dto.Cpf.Length >= 11;

    if (!hasValidEmail && !hasValidPhone && !hasValidCpf)
    {
        throw new ValidationException(
            "ContactInfo",
            "Pelo menos um dos campos Email, Phone ou Cpf deve ser preenchido com valor válido."
        );
    }
}
```

**Não precisou alterar nada no service!** ✅

---

### **4. Frontend - Atualizar userService**

**Arquivo:** `frontend/src/services/userService.ts`

**ANTES (errado):**
```typescript
async create(data: { email?: string; phone?: string; cpf?: string; password: string }): Promise<User> {
  const response = await api.post('/user/create', {
    email: data.email,
    phone: data.phone,
    cpf: data.cpf,
    passwordHash: data.password  // ❌ Campo errado
  });
  return response.data.data;
}
```

**DEPOIS (correto):**
```typescript
async create(data: { email?: string; phone?: string; cpf?: string; password: string }): Promise<User> {
  const response = await api.post('/user/create', {
    email: data.email,
    phone: data.phone,
    cpf: data.cpf,
    password: data.password  // ✅ Campo correto
  });
  return response.data.data;
}
```

---

## 📊 Comparação com RegisterRequestDTO

Agora `UserInputDTO` está **igual** ao `RegisterRequestDTO`:

| Campo | RegisterRequestDTO | UserInputDTO (Antes) | UserInputDTO (Depois) |
|-------|-------------------|---------------------|---------------------|
| **Email** | `string?` (opcional) | `string` (obrigatório) ❌ | `string?` (opcional) ✅ |
| **Phone** | `string?` (opcional) | `string` (obrigatório) ❌ | `string?` (opcional) ✅ |
| **Cpf** | `string?` (opcional) | `string` (obrigatório) ❌ | `string?` (opcional) ✅ |
| **Password** | `string` (obrigatório) | `PasswordHash` ❌ | `Password` ✅ |
| **MinLength** | `[MinLength(6)]` | Nenhum ❌ | `[MinLength(6)]` ✅ |
| **Validação** | Pelo menos 1 | Pelo menos 1 ✅ | Pelo menos 1 ✅ |

---

## 🎯 Cenários de Teste

### **Cenário 1: Apenas Email ✅**

**Request:**
```json
{
  "email": "joao@empresa.com",
  "password": "123456"
}
```

**Resultado:**
```
✅ 200 OK
Usuário criado com sucesso!
```

---

### **Cenário 2: Apenas Telefone ✅**

**Request:**
```json
{
  "phone": "11999999999",
  "password": "123456"
}
```

**Resultado:**
```
✅ 200 OK
Usuário criado com sucesso!
```

---

### **Cenário 3: Apenas CPF ✅**

**Request:**
```json
{
  "cpf": "12345678900",
  "password": "123456"
}
```

**Resultado:**
```
✅ 200 OK
Usuário criado com sucesso!
```

---

### **Cenário 4: Email + Telefone + CPF ✅**

**Request:**
```json
{
  "email": "joao@empresa.com",
  "phone": "11999999999",
  "cpf": "12345678900",
  "password": "123456"
}
```

**Resultado:**
```
✅ 200 OK
Usuário criado com sucesso!
```

---

### **Cenário 5: Nenhum Identificador ❌**

**Request:**
```json
{
  "password": "123456"
}
```

**Resultado:**
```
❌ 400 Bad Request
{
  "success": false,
  "message": "Pelo menos um dos campos Email, Phone ou Cpf deve ser preenchido com valor válido.",
  "errors": {
    "ContactInfo": ["Pelo menos um dos campos Email, Phone ou Cpf deve ser preenchido com valor válido."]
  }
}
```

---

### **Cenário 6: Senha Curta ❌**

**Request:**
```json
{
  "email": "joao@empresa.com",
  "password": "123"
}
```

**Resultado:**
```
❌ 400 Bad Request
{
  "success": false,
  "errors": {
    "Password": ["Senha deve ter no mínimo 6 caracteres"]
  }
}
```

---

### **Cenário 7: Sem Senha ❌**

**Request:**
```json
{
  "email": "joao@empresa.com"
}
```

**Resultado:**
```
❌ 400 Bad Request
{
  "success": false,
  "errors": {
    "Password": ["Senha é obrigatória"]
  }
}
```

---

## 📝 Arquivos Modificados

1. ✅ `backend/-2-Application/DTOs/UserInputDTO.cs`
   - Email, Phone, Cpf agora são nullable
   - PasswordHash → Password
   - Adicionado MinLength(6)

2. ✅ `backend/-2-Application/Mappers/userMapper.cs`
   - ToEntity: `dto.PasswordHash` → `dto.Password`
   - UpdateEntity: `dto.PasswordHash` → `dto.Password`

3. ✅ `frontend/src/services/userService.ts`
   - `passwordHash: data.password` → `password: data.password`

---

## ✅ Checklist de Validações

- [x] Pelo menos 1 identificador obrigatório (Email, Phone OU Cpf)
- [x] Email opcional (nullable)
- [x] Phone opcional (nullable)
- [x] Cpf opcional (nullable)
- [x] Senha obrigatória
- [x] Senha mínimo 6 caracteres
- [x] Validação no service (`ValidateAtLeastOneContact`)
- [x] Frontend envia `password` (não `passwordHash`)
- [x] Mapper usa `dto.Password`

---

## 🎊 Resultado

**Agora a validação está EXATAMENTE igual ao Register:**
- ✅ Apenas 1 identificador necessário (não todos)
- ✅ Email, Phone e Cpf são opcionais
- ✅ Senha obrigatória com mínimo 6 caracteres
- ✅ Mensagens de erro claras
- ✅ Frontend alinhado com backend

**Arquivos:**
- `backend/-2-Application/DTOs/UserInputDTO.cs`
- `backend/-2-Application/Mappers/userMapper.cs`
- `frontend/src/services/userService.ts`

**Doc:** `tmp/USER-VALIDATION-FIX.md`
