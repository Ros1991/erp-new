# CNPJ Opcional - Documentação de Alterações

## 📋 Objetivo

Tornar o campo CNPJ (company_document) **opcional** para empresas em todo o sistema, permitindo cadastro de empresas sem documento fiscal.

---

## ✅ Alterações Realizadas

### **1. Backend - Banco de Dados**

#### **A. Script Principal (`erp.sql`)**

**Alterações:**
```sql
-- ANTES
"company_document" varchar(14) NOT NULL,
ALTER TABLE "erp"."tb_company" ADD CONSTRAINT "uk_company_document" UNIQUE("company_document");

-- DEPOIS
"company_document" varchar(14) NULL,
-- Constraint UNIQUE removida
```

**Localização:** `backend/-1-Domain/database/erp.sql` linhas 103

---

#### **B. Script de Migração** ✨ NOVO

**Arquivo:** `backend/-1-Domain/database/make_company_document_optional.sql`

**O que faz:**
1. ✅ Remove constraint `uk_company_document` (UNIQUE)
2. ✅ Altera coluna para aceitar `NULL`
3. ✅ Cria índice único parcial (permite múltiplos NULL)
4. ✅ Testa inserção de empresa sem CNPJ
5. ✅ Valida alterações

**Como executar:**
```bash
psql -U postgres -d erp_database < backend/-1-Domain/database/make_company_document_optional.sql
```

**Índice parcial criado:**
```sql
CREATE UNIQUE INDEX idx_company_document_unique_partial
    ON erp.tb_company (company_document)
    WHERE company_document IS NOT NULL;
```
- ✅ Permite **múltiplas empresas sem CNPJ** (NULL)
- ✅ Garante que **CNPJs informados sejam únicos**

---

### **2. Backend - Entities**

#### **Company.cs**

**Arquivo:** `backend/-1-Domain/Entities/company.cs`

**Alterações:**
```csharp
// ANTES
[Column("company_document")]
public string Document { get; set; }

public Company(string Param_Name, string Param_Document, ...)

// DEPOIS
[Column("company_document")]
public string? Document { get; set; }

public Company(string Param_Name, string? Param_Document, ...)
```

---

### **3. Backend - DTOs**

#### **A. CompanyInputDTO.cs**

**Arquivo:** `backend/-2-Application/DTOs/CompanyInputDTO.cs`

**Alterações:**
```csharp
// ANTES
[Required(ErrorMessage = "Document é obrigatório")]
[StringLength(14, ErrorMessage = "Document deve ter no máximo 14 caracteres")]
public string Document { get; set; }

// DEPOIS
[StringLength(14, ErrorMessage = "Document deve ter no máximo 14 caracteres")]
public string? Document { get; set; }
```

- ❌ Removido `[Required]`
- ✅ Tornado `nullable`
- ✅ Mantido `[StringLength]` para validar quando informado

---

#### **B. CompanyOutputDTO.cs**

**Arquivo:** `backend/-2-Application/DTOs/CompanyOutputDTO.cs`

**Alterações:**
```csharp
// ANTES
public string Document { get; set; }

// DEPOIS
public string? Document { get; set; }
```

---

### **4. Frontend - Services**

#### **companyService.ts**

**Arquivo:** `frontend/src/services/companyService.ts`

**Alterações:**
```typescript
// Interface Company
export interface Company {
  companyId: number;
  name: string;
  document?: string;  // ← Tornado opcional
  userId: number;
  // ...
}

// Interface CreateCompanyInput
export interface CreateCompanyInput {
  name: string;
  document?: string;  // ← Tornado opcional
  userId: number;
}
```

---

### **5. Frontend - Components**

#### **A. AddCompanyDialog.tsx**

**Arquivo:** `frontend/src/components/companies/AddCompanyDialog.tsx`

**Alterações na Validação:**
```typescript
// ANTES - CNPJ obrigatório
if (!cnpj.trim()) {
  showError('CNPJ é obrigatório');
  return;
}

// DEPOIS - CNPJ opcional, valida apenas se informado
let cnpjNumbers = '';
if (cnpj.trim()) {
  cnpjNumbers = cnpj.replace(/\D/g, '');
  
  if (cnpjNumbers.length !== 14) {
    showError('CNPJ deve ter 14 dígitos');
    return;
  }

  if (!companyService.validateCNPJ(cnpjNumbers)) {
    showError('CNPJ inválido');
    return;
  }
}
```

**Alterações no Envio:**
```typescript
// ANTES
document: cnpjNumbers,

// DEPOIS
document: cnpjNumbers || undefined,
```

**Alterações na UI:**
```tsx
<!-- ANTES -->
<Label htmlFor="cnpj">
  CNPJ <span className="text-red-500">*</span>
</Label>
<Input
  id="cnpj"
  required
  // ...
/>
<p className="text-xs text-gray-500">
  Cadastro Nacional de Pessoa Jurídica (14 dígitos)
</p>

<!-- DEPOIS -->
<Label htmlFor="cnpj">
  CNPJ <span className="text-xs text-gray-500 font-normal">(opcional)</span>
</Label>
<Input
  id="cnpj"
  // required REMOVIDO
  // ...
/>
<p className="text-xs text-gray-500">
  Cadastro Nacional de Pessoa Jurídica (14 dígitos) - deixe em branco se não tiver
</p>
```

---

#### **B. CompanySelect.tsx**

**Arquivo:** `frontend/src/pages/companies/CompanySelect.tsx`

**Alterações:**
```tsx
<!-- ANTES -->
<span className="font-medium mr-2">CNPJ:</span>
<span>{company.cnpj}</span>

<!-- DEPOIS -->
<span className="font-medium mr-2">CNPJ:</span>
<span>
  {company.cnpj || <span className="italic text-gray-400">Não informado</span>}
</span>
```

---

### **6. Frontend - Contexts**

#### **AuthContext.tsx**

**Arquivo:** `frontend/src/contexts/AuthContext.tsx`

**Alterações:**
```typescript
// ANTES
interface Company {
  id: number;
  name: string;
  cnpj: string;
  isActive: boolean;
  createdAt: string;
}

// DEPOIS
interface Company {
  id: number;
  name: string;
  cnpj?: string;  // ← Tornado opcional
  isActive: boolean;
  createdAt: string;
}
```

---

## 🔄 Comportamento do Sistema

### **Empresas SEM CNPJ:**
- ✅ Podem ser cadastradas normalmente
- ✅ `company_document` = `NULL` no banco
- ✅ Exibe "Não informado" na listagem
- ✅ Sem validação de CNPJ no formulário

### **Empresas COM CNPJ:**
- ✅ CNPJ deve ter 14 dígitos
- ✅ CNPJ deve ser válido (validação de dígitos)
- ✅ CNPJ deve ser único (índice parcial)
- ✅ Exibe CNPJ formatado na listagem

---

## 📊 Comparação: Antes x Depois

| Aspecto | ANTES | DEPOIS |
|---------|-------|--------|
| **Campo no Banco** | `NOT NULL` | `NULL` |
| **Constraint UNIQUE** | Sim (todas empresas) | Índice parcial (só CNPJs informados) |
| **DTO Input** | `[Required]` | Opcional |
| **Entity** | `string` | `string?` |
| **Frontend Validation** | Obrigatório | Apenas se informado |
| **UI** | Asterisco vermelho (*) | "(opcional)" |
| **Listagem** | Sempre mostra CNPJ | "Não informado" se NULL |
| **Múltiplas empresas sem CNPJ** | ❌ Não permitido | ✅ Permitido |

---

## 🧪 Testes

### **1. Criar empresa SEM CNPJ**
```
✅ Nome: "Meu Negócio"
✅ CNPJ: [deixar em branco]
✅ Deve criar com sucesso
✅ Banco: company_document = NULL
✅ Listagem: "Não informado"
```

### **2. Criar empresa COM CNPJ válido**
```
✅ Nome: "Empresa LTDA"
✅ CNPJ: "11.222.333/0001-81"
✅ Deve criar com sucesso
✅ Banco: company_document = "11222333000181"
✅ Listagem: "11.222.333/0001-81"
```

### **3. Criar empresa COM CNPJ inválido**
```
❌ Nome: "Teste"
❌ CNPJ: "11.111.111/1111-11"
❌ Erro: "CNPJ inválido"
❌ Não deve criar
```

### **4. Criar múltiplas empresas sem CNPJ**
```
✅ Empresa 1: sem CNPJ
✅ Empresa 2: sem CNPJ
✅ Empresa 3: sem CNPJ
✅ Todas devem ser criadas
✅ Banco: todas com company_document = NULL
```

### **5. CNPJ duplicado**
```
❌ Empresa 1: CNPJ "11.222.333/0001-81"
❌ Empresa 2: CNPJ "11.222.333/0001-81"
❌ Erro: constraint violation (índice parcial)
```

---

## 📁 Arquivos Modificados

### **Backend:**
```
backend/
├── -1-Domain/
│   ├── database/
│   │   ├── erp.sql                                  ← Modificado
│   │   └── make_company_document_optional.sql       ← NOVO
│   └── Entities/
│       └── company.cs                               ← Modificado
└── -2-Application/
    └── DTOs/
        ├── CompanyInputDTO.cs                       ← Modificado
        └── CompanyOutputDTO.cs                      ← Modificado
```

### **Frontend:**
```
frontend/
└── src/
    ├── components/
    │   └── companies/
    │       └── AddCompanyDialog.tsx                 ← Modificado
    ├── contexts/
    │   └── AuthContext.tsx                          ← Modificado
    ├── pages/
    │   └── companies/
    │       └── CompanySelect.tsx                    ← Modificado
    └── services/
        └── companyService.ts                        ← Modificado
```

### **Documentação:**
```
CNPJ_OPCIONAL.md                                     ← NOVO
```

---

## 🚀 Deploy / Migração

### **Para Banco de Dados NOVO:**
```bash
# Usar o erp.sql já atualizado
psql -U postgres -d erp_database < backend/-1-Domain/database/erp.sql
```

### **Para Banco de Dados EXISTENTE:**
```bash
# 1. Fazer backup
pg_dump -U postgres -d erp_database > backup_before_cnpj_optional.sql

# 2. Executar migração
psql -U postgres -d erp_database < backend/-1-Domain/database/make_company_document_optional.sql

# 3. Validar
# O script de migração já inclui validação automática
```

---

## ⚠️ Observações Importantes

### **1. Índice Único Parcial**
- Criado automaticamente pelo script de migração
- **Permite:** Múltiplas empresas com `company_document = NULL`
- **Garante:** CNPJs informados sejam únicos
- **Sintaxe PostgreSQL:** `WHERE company_document IS NOT NULL`

### **2. Validação Frontend**
- CNPJ **não é obrigatório**
- Se informado, **deve ser válido**
- Formatação automática mantida
- Validação de 14 dígitos mantida

### **3. Validação Backend**
- `[Required]` removido do DTO
- `[StringLength(14)]` mantido
- Entity aceita `null`
- Banco aceita `NULL`

### **4. Compatibilidade**
- ✅ API continua aceitando CNPJ
- ✅ API agora aceita `null` ou `undefined`
- ✅ Empresas antigas com CNPJ continuam funcionando
- ✅ Novas empresas podem ser sem CNPJ

---

## ✅ Checklist de Implementação

- [x] Atualizar `erp.sql` (company_document NULL)
- [x] Criar script de migração
- [x] Atualizar entity `Company`
- [x] Atualizar `CompanyInputDTO`
- [x] Atualizar `CompanyOutputDTO`
- [x] Atualizar interfaces TypeScript
- [x] Atualizar validações do `AddCompanyDialog`
- [x] Atualizar UI do formulário
- [x] Atualizar exibição na listagem
- [x] Atualizar `AuthContext`
- [x] Criar documentação

---

## 🎉 Resultado Final

**CNPJ agora é completamente opcional em todo o sistema!**

- ✅ Empresas podem ser criadas sem CNPJ
- ✅ CNPJ continua sendo validado quando informado
- ✅ CNPJs únicos garantidos (índice parcial)
- ✅ UI clara sobre campo opcional
- ✅ Compatível com empresas existentes
- ✅ Scripts de migração prontos

**Sistema pronto para produção!** 🚀
