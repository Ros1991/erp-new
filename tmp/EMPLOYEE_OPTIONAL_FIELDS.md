# ✅ Campos Opcionais em Employee - Apenas Nickname Obrigatório

## 🎯 Objetivo

Tornar **apenas o nickname obrigatório** na tabela de empregados, permitindo empresas cadastrarem funcionários com informações mínimas.

---

## 📊 Mudanças Realizadas

### **✅ Banco de Dados**

#### **Schema (erp.sql)**
```sql
-- Apenas nickname é NOT NULL
"employee_nickname"     varchar (100)  NOT NULL,
"employee_full_name"    varchar (255)  NULL,
"employee_email"        varchar (255)  NULL,
"employee_phone"        varchar (20)   NULL,
"employee_cpf"          varchar (11)   NULL,
```

#### **Constraints UNIQUE com suporte a NULL**
```sql
-- Permite múltiplos NULL, mas valores não-NULL devem ser únicos
CREATE UNIQUE INDEX "uk_employee_cpf" 
ON "erp"."tb_employee"("employee_cpf") 
WHERE "employee_cpf" IS NOT NULL;

CREATE UNIQUE INDEX "uk_employee_email" 
ON "erp"."tb_employee"("employee_email") 
WHERE "employee_email" IS NOT NULL;

CREATE UNIQUE INDEX "uk_employee_phone" 
ON "erp"."tb_employee"("employee_phone") 
WHERE "employee_phone" IS NOT NULL;

-- Nickname continua único por empresa (obrigatório)
ALTER TABLE "erp"."tb_employee" 
ADD CONSTRAINT "uk_employee_nickname" 
UNIQUE("company_id","employee_nickname");
```

#### **Migration 004**
📄 `backend/-1-Domain/database/migrations/004_make_employee_fields_optional.sql`

**Executa:**
1. Remove constraints UNIQUE antigas
2. Torna colunas nullable
3. Cria UNIQUE INDEX com `WHERE NOT NULL`
4. Adiciona comentários explicativos

---

### **✅ Backend**

#### **Entity (employee.cs)**
```csharp
public string Nickname { get; set; }      // Obrigatório
public string? FullName { get; set; }     // Opcional
public string? Email { get; set; }        // Opcional
public string? Phone { get; set; }        // Opcional
public string? Cpf { get; set; }          // Opcional
```

#### **DTO (EmployeeInputDTO.cs)**
```csharp
[Required(ErrorMessage = "Apelido é obrigatório")]
public string Nickname { get; set; }

// Removido [Required] - agora opcional
public string? FullName { get; set; }
public string? Email { get; set; }
public string? Phone { get; set; }
public string? Cpf { get; set; }
```

---

### **✅ Frontend**

#### **Service (employeeService.ts)**
```typescript
export interface Employee {
  nickname: string;      // Obrigatório
  fullName?: string;     // Opcional
  email?: string;        // Opcional
  phone?: string;        // Opcional
  cpf?: string;          // Opcional
}
```

#### **Form (EmployeeForm.tsx)**
```typescript
// Validação removida
// if (!formData.fullName.trim()) {
//   newErrors.fullName = 'Nome completo é obrigatório';
// }

// Label sem asterisco vermelho
<label>Nome Completo</label>  // Sem <span className="text-red-500">*</span>

// Envia undefined se vazio
const dataToSend = {
  nickname: formData.nickname.trim(),
  fullName: formData.fullName.trim() || undefined,
  // ...
};
```

#### **Tratamento de Fallback**
```typescript
// EmployeeContracts.tsx e ContractForm.tsx
setEmployeeName(employee.fullName || employee.nickname);
```

---

## 🔧 Unique Constraints com NULL

### **Comportamento PostgreSQL**

**UNIQUE INDEX com WHERE NOT NULL:**
```sql
CREATE UNIQUE INDEX "uk_employee_cpf" 
ON "erp"."tb_employee"("employee_cpf") 
WHERE "employee_cpf" IS NOT NULL;
```

| Valor 1 | Valor 2 | Permitido? |
|---------|---------|------------|
| `NULL` | `NULL` | ✅ Sim (múltiplos NULL) |
| `"12345678900"` | `"12345678900"` | ❌ Não (duplicado) |
| `"12345678900"` | `NULL` | ✅ Sim (único não-NULL) |
| `"12345678900"` | `"98765432100"` | ✅ Sim (valores diferentes) |

**Por quê?**
- NULL não é considerado igual a NULL em UNIQUE constraints
- O `WHERE NOT NULL` garante que apenas valores preenchidos sejam validados
- Permite múltiplos empregados sem CPF/email/phone

---

## 🚀 Como Aplicar

### **1. Aplicar Migration no Banco**

```bash
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/004_make_employee_fields_optional.sql
```

### **2. Verificar**

```sql
-- Verificar colunas nullable
SELECT column_name, is_nullable, data_type
FROM information_schema.columns 
WHERE table_schema = 'erp' 
  AND table_name = 'tb_employee' 
  AND column_name IN ('employee_full_name', 'employee_email', 'employee_phone', 'employee_cpf');

-- Resultado esperado: is_nullable = YES para todos

-- Verificar índices UNIQUE
SELECT indexname, indexdef
FROM pg_indexes
WHERE tablename = 'tb_employee'
  AND indexname LIKE 'uk_employee%';
```

### **3. Reiniciar Backend e Frontend**

```bash
# Backend
cd backend
dotnet run

# Frontend
cd frontend
npm start
```

---

## 🧪 Cenários de Teste

### **Cenário 1: Empregado Apenas com Nickname**
```json
{
  "nickname": "João",
  "fullName": null,
  "email": null,
  "phone": null,
  "cpf": null
}
```
✅ **Deve funcionar** - Nickname único é o mínimo

### **Cenário 2: Múltiplos Empregados sem CPF**
```json
[
  { "nickname": "João", "cpf": null },
  { "nickname": "Maria", "cpf": null },
  { "nickname": "Pedro", "cpf": null }
]
```
✅ **Deve funcionar** - NULL não viola UNIQUE

### **Cenário 3: CPF Duplicado**
```json
[
  { "nickname": "João", "cpf": "12345678900" },
  { "nickname": "Maria", "cpf": "12345678900" }
]
```
❌ **Deve falhar** - CPF duplicado viola UNIQUE INDEX

### **Cenário 4: Nickname Duplicado na Mesma Empresa**
```json
[
  { "nickname": "João", "companyId": 1 },
  { "nickname": "João", "companyId": 1 }
]
```
❌ **Deve falhar** - Nickname único por empresa

### **Cenário 5: Mesmo Nickname em Empresas Diferentes**
```json
[
  { "nickname": "João", "companyId": 1 },
  { "nickname": "João", "companyId": 2 }
]
```
✅ **Deve funcionar** - Nickname único **por empresa**

---

## 📋 Checklist de Validação

- [x] Migration 004 criada
- [x] erp.sql atualizado
- [x] Entity Employee.cs com campos nullable
- [x] DTO EmployeeInputDTO.cs sem [Required] em fullName
- [x] Service employeeService.ts com interfaces atualizadas
- [x] Form EmployeeForm.tsx sem validação obrigatória
- [x] Tratamento de fallback em telas que usam fullName
- [x] UNIQUE INDEX com WHERE NOT NULL
- [x] Constraint de nickname único por empresa mantida

---

## 💡 Benefícios

1. **✅ Flexibilidade Total**
   - Empresas podem cadastrar empregados com informações mínimas
   - Nickname é identificador único e suficiente

2. **✅ Sem Dados Falsos**
   - Evita preenchimento de "N/A", "Não informado", etc.
   - NULL tem significado semântico claro

3. **✅ Validação Inteligente**
   - Se preenchido, deve ser único
   - Se não preenchido, permite múltiplos NULL

4. **✅ Compatibilidade**
   - Código existente continua funcionando
   - Fallback para nickname onde necessário

---

## 📄 Arquivos Modificados

### **Backend:**
1. `backend/-1-Domain/database/erp.sql`
2. `backend/-1-Domain/Entities/employee.cs`
3. `backend/-2-Application/DTOs/Employee/EmployeeInputDTO.cs`

### **Frontend:**
1. `frontend/src/services/employeeService.ts`
2. `frontend/src/pages/employees/EmployeeForm.tsx`
3. `frontend/src/pages/contracts/EmployeeContracts.tsx`
4. `frontend/src/pages/contracts/ContractForm.tsx`

### **Migration:**
1. `backend/-1-Domain/database/migrations/004_make_employee_fields_optional.sql`

---

## 🎯 Resultado Final

**Sistema adaptável para qualquer tipo de empresa:**
- ✅ Pequena empresa (apenas nicknames) → Funciona!
- ✅ Média empresa (dados parciais) → Funciona!
- ✅ Grande empresa (dados completos) → Funciona!

**Nickname é rei! 👑**

---

**Data:** 2025-11-14  
**Status:** ✅ 100% Implementado  
**Pronto para Produção!** 🎉
