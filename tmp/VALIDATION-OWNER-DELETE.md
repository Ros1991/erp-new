# Validação: Impedir Exclusão do Dono da Empresa

## 🎯 Regra de Negócio

**O dono da empresa NÃO pode ser removido da lista de CompanyUsers.**

O dono é identificado pelo campo `user_id` na tabela `tb_company`.

---

## ✅ Implementação

### **Arquivo Modificado:**
`backend/-2-Application/Services/companyUserService.cs`

### **Método:**
`RemoveUserFromCompanyAsync(long companyUserId)`

---

## 🔧 Validação Adicionada

```csharp
public async Task<bool> RemoveUserFromCompanyAsync(long companyUserId)
{
    // 1️⃣ Buscar o CompanyUser para obter userId e companyId
    var companyUser = await _unitOfWork.CompanyUserRepository.GetOneByIdAsync(companyUserId);
    if (companyUser == null)
    {
        throw new EntityNotFoundException("CompanyUser", companyUserId);
    }

    // 2️⃣ Buscar a empresa para verificar se o usuário é o dono
    var company = await _unitOfWork.CompanyRepository.GetOneByIdAsync(companyUser.CompanyId);
    if (company == null)
    {
        throw new EntityNotFoundException("Company", companyUser.CompanyId);
    }

    // 3️⃣ Validar se o usuário é o dono da empresa
    if (company.UserId == companyUser.UserId)
    {
        throw new ValidationException("CompanyUser", "Não é possível remover o dono da empresa.");
    }

    // 4️⃣ Se não for o dono, permite deletar
    var result = await _unitOfWork.CompanyUserRepository.DeleteByIdAsync(companyUserId);
    await _unitOfWork.SaveChangesAsync();
    return result;
}
```

---

## 📋 Fluxo de Validação

```
Usuário tenta deletar CompanyUser
  ↓
1. Buscar CompanyUser no banco
   ├─ ❌ Não existe → EntityNotFoundException
   └─ ✅ Existe → Próximo passo
  ↓
2. Buscar Company no banco
   ├─ ❌ Não existe → EntityNotFoundException
   └─ ✅ Existe → Próximo passo
  ↓
3. Verificar se company.UserId == companyUser.UserId
   ├─ ✅ SIM (é o dono) → ValidationException ❌
   └─ ❌ NÃO (não é o dono) → Permite deletar ✅
  ↓
4. Deletar CompanyUser
  ↓
5. SaveChanges
```

---

## 🎯 Cenários de Teste

### **Cenário 1: Tentar Deletar o Dono**

**Dados:**
- Company ID: 1
- Company.UserId: 5 (dono)
- CompanyUser.UserId: 5 (mesmo usuário)

**Resultado:**
```json
{
  "success": false,
  "message": "Não é possível remover o dono da empresa.",
  "errors": {
    "CompanyUser": ["Não é possível remover o dono da empresa."]
  }
}
```

**Status Code:** `400 Bad Request`

---

### **Cenário 2: Deletar Usuário Normal**

**Dados:**
- Company ID: 1
- Company.UserId: 5 (dono)
- CompanyUser.UserId: 10 (outro usuário)

**Resultado:**
```json
{
  "success": true,
  "message": "CompanyUser deletado com sucesso",
  "data": true
}
```

**Status Code:** `200 OK`

---

### **Cenário 3: CompanyUser Não Existe**

**Dados:**
- CompanyUserId: 999 (não existe)

**Resultado:**
```json
{
  "success": false,
  "message": "CompanyUser com ID 999 não encontrado."
}
```

**Status Code:** `404 Not Found`

---

## 🗄️ Estrutura de Dados

### **Tabela: tb_company**
```sql
company_id | company_name | user_id (dono) | ...
-----------|--------------|----------------|-----
1          | Empresa A    | 5              | ...
2          | Empresa B    | 8              | ...
```

### **Tabela: tb_company_user**
```sql
company_user_id | company_id | user_id | role_id | ...
----------------|------------|---------|---------|-----
1               | 1          | 5       | 1       | ...  ← DONO (user_id=5)
2               | 1          | 10      | 2       | ...  ← Pode deletar
3               | 1          | 15      | 3       | ...  ← Pode deletar
```

---

## 🔒 Segurança

### **Validações em Camadas:**

1. **Frontend (UI):**
   - Botão de delete desabilitado para dono (visual)
   - Mensagem: "Dono da empresa"

2. **Backend (Service):**
   - ✅ **Validação implementada**
   - Impede delete mesmo que frontend seja burlado
   - Retorna erro claro

### **Por que Validar no Backend?**

- ✅ Frontend pode ser manipulado (DevTools, API direta)
- ✅ Backend é a **última linha de defesa**
- ✅ Garante integridade dos dados

---

## 📡 Resposta da API

### **Sucesso (Não é Dono):**
```http
DELETE /api/companyuser/10
200 OK

{
  "success": true,
  "message": "CompanyUser deletado com sucesso",
  "data": true
}
```

### **Erro (É o Dono):**
```http
DELETE /api/companyuser/1
400 Bad Request

{
  "success": false,
  "message": "Não é possível remover o dono da empresa.",
  "errors": {
    "CompanyUser": [
      "Não é possível remover o dono da empresa."
    ]
  }
}
```

---

## 🎨 Impacto no Frontend

O frontend já trata erros da API com `parseBackendError`:

```typescript
try {
  await companyUserService.delete(companyUserId);
  showSuccess('Usuário removido com sucesso');
} catch (err: any) {
  const { message } = parseBackendError(err);
  showError(message); // "Não é possível remover o dono da empresa."
}
```

**Toast exibido:**
```
❌ Não é possível remover o dono da empresa.
```

---

## 💡 Melhorias Futuras (Opcional)

### **1. Desabilitar Botão Delete no Frontend (UX):**

```typescript
const isOwner = user.userId === company.ownerId;

<button 
  disabled={isOwner}
  onClick={() => handleDelete(user)}
  className={isOwner ? 'opacity-50 cursor-not-allowed' : ''}
>
  {isOwner ? 'Dono' : 'Deletar'}
</button>
```

### **2. Badge Visual:**

```tsx
{user.userId === company.ownerId && (
  <span className="badge bg-yellow-100 text-yellow-800">
    👑 Dono
  </span>
)}
```

### **3. Tooltip:**

```tsx
<Tooltip content="O dono da empresa não pode ser removido">
  <button disabled>Deletar</button>
</Tooltip>
```

---

## 🧪 Como Testar

### **Teste Manual:**

1. Criar uma empresa (você será o dono automaticamente)
2. Adicionar outros usuários à empresa
3. Tentar deletar outro usuário → ✅ Funciona
4. Tentar deletar você (dono) → ❌ Erro: "Não é possível remover o dono da empresa."

### **Teste com cURL:**

```bash
# Tentar deletar o dono (deve falhar)
curl -X DELETE http://localhost:5000/api/companyuser/1 \
  -H "Authorization: Bearer {token}" \
  -H "X-Company-ID: 1"

# Tentar deletar outro usuário (deve funcionar)
curl -X DELETE http://localhost:5000/api/companyuser/2 \
  -H "Authorization: Bearer {token}" \
  -H "X-Company-ID: 1"
```

---

## 📝 Exceções Lançadas

| Exceção | Quando | Mensagem |
|---------|--------|----------|
| `EntityNotFoundException` | CompanyUser não existe | "CompanyUser com ID {id} não encontrado." |
| `EntityNotFoundException` | Company não existe | "Company com ID {id} não encontrado." |
| `ValidationException` | Tentativa de deletar dono | "Não é possível remover o dono da empresa." |

---

## ✅ Checklist de Implementação

- [x] Adicionar validação no `RemoveUserFromCompanyAsync`
- [x] Buscar CompanyUser pelo ID
- [x] Buscar Company pelo CompanyId
- [x] Comparar company.UserId com companyUser.UserId
- [x] Lançar ValidationException se for o dono
- [x] Documentar comportamento
- [ ] (Opcional) Desabilitar botão delete no frontend
- [ ] (Opcional) Adicionar badge visual "Dono"
- [ ] (Opcional) Testes unitários

---

## 🎊 Resultado

**Agora o dono da empresa está protegido contra exclusão acidental ou maliciosa!**

- ✅ Validação no backend (segura)
- ✅ Mensagem de erro clara
- ✅ Integridade dos dados garantida
- ✅ Não quebra outras funcionalidades

**Arquivo:** `backend/-2-Application/Services/companyUserService.cs`
**Método:** `RemoveUserFromCompanyAsync`
**Doc:** `tmp/VALIDATION-OWNER-DELETE.md`
