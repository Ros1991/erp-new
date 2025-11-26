# Migration 007: Convert to Short Codes (Tags)

## 📋 **Problema**
```
PostgresException: 22001: valor é muito longo para tipo character varying(10)
```

Campos `loan_advance_discount_source` e `contract_benefit_discount_application` estavam armazenando textos longos como "Décimo Terceiro Salário" (23 chars) em colunas VARCHAR(10).

---

## 🎯 **Solução**

Converter textos longos para **códigos curtos** (tags) de até 10 caracteres:

| Texto Antigo | Código Novo | Tamanho |
|--------------|-------------|---------|
| Salário Mensal | `SALARIO` | 7 chars |
| Décimo Terceiro Salário | `13SAL` | 5 chars |
| Férias | `FERIAS` | 6 chars |
| Anual | `ANUAL` | 5 chars |
| Bônus | `BONUS` | 5 chars |
| Comissão | `COMISSAO` | 8 chars |
| Todos os Pagamentos | `TODOS` | 5 chars |

---

## 📁 **Arquivos Criados/Modificados**

### **Backend**

#### **1. Enums (C#)**
- `backend/-1-Domain/Enums/DiscountSourceType.cs`
  - Códigos para fonte de desconto de empréstimos
  - Helper: `GetDescription()` - retorna label amigável
  - Helper: `GetAll()` - retorna dicionário completo
  - Helper: `FromLegacyDescription()` - converte valores antigos

- `backend/-1-Domain/Enums/ApplicationType.cs`
  - Códigos para aplicação de benefícios/descontos
  - Mesmos helpers do DiscountSourceType

#### **2. Migration SQL**
- `backend/-1-Domain/database/migrations/007_convert_codes_to_short_tags.sql`
  - Converte registros existentes em `tb_loan_advance`
  - Converte registros existentes em `tb_contract_benefit_discount`
  - Scripts de verificação (comentados)

### **Frontend**

#### **3. Constants (TypeScript)**
- `frontend/src/constants/discountSource.ts`
  - `DiscountSourceCode` - enum com códigos
  - `DISCOUNT_SOURCE_OPTIONS` - array para combos
  - `getDiscountSourceLabel()` - retorna label amigável
  - `migrateDiscountSourceValue()` - converte valores antigos

- `frontend/src/constants/applicationType.ts`
  - `ApplicationTypeCode` - enum com códigos
  - `APPLICATION_TYPE_OPTIONS` - array para combos
  - `getApplicationTypeLabel()` - retorna label amigável
  - `migrateApplicationTypeValue()` - converte valores antigos

#### **4. Componentes Atualizados**

**Formulários (salvam códigos):**
- `frontend/src/pages/loan-advances/LoanAdvanceForm.tsx`
  - Combo usa `DISCOUNT_SOURCE_OPTIONS`
  - Salva códigos curtos (ex: `13SAL`)
  - Migra valores antigos ao carregar

- `frontend/src/components/ui/BenefitDiscountList.tsx`
  - Combo usa `APPLICATION_TYPE_OPTIONS`
  - Salva códigos curtos
  - Migra valores antigos ao editar

**Listagens (exibem labels):**
- `frontend/src/pages/loan-advances/LoanAdvances.tsx`
  - Usa `getDiscountSourceLabel()` para exibição
  - Mostra "Décimo Terceiro Salário" ao invés de "13SAL"

- `frontend/src/pages/contracts/EmployeeContracts.tsx`
  - Usa `getApplicationTypeLabel()` para exibição
  - Mostra labels amigáveis nos benefícios/descontos

---

## 🔄 **Como Aplicar**

### **1. Backend - Rodar Migration**

```bash
# PostgreSQL
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/007_convert_codes_to_short_tags.sql
```

A migration irá:
1. ✅ Converter "Décimo Terceiro" → `13SAL`
2. ✅ Converter "Salário Mensal" → `SALARIO`
3. ✅ Converter "Férias" → `FERIAS`
4. ✅ Truncar valores > 10 chars
5. ✅ Preservar códigos já válidos

### **2. Frontend - Recompilar**

```bash
cd frontend
npm install  # Se necessário
npm run dev  # ou npm run build
```

Os componentes irão:
1. ✅ Salvar códigos curtos no banco
2. ✅ Exibir labels amigáveis ao usuário
3. ✅ Migrar valores antigos automaticamente

---

## 🧪 **Testes de Verificação**

### **Banco de Dados**

```sql
-- 1. Verificar se há valores > 10 chars (deve retornar 0)
SELECT COUNT(*) FROM erp.tb_loan_advance 
WHERE LENGTH(loan_advance_discount_source) > 10;

SELECT COUNT(*) FROM erp.tb_contract_benefit_discount 
WHERE LENGTH(contract_benefit_discount_application) > 10;

-- 2. Ver distribuição de códigos
SELECT loan_advance_discount_source, COUNT(*) as total
FROM erp.tb_loan_advance
GROUP BY loan_advance_discount_source
ORDER BY total DESC;

SELECT contract_benefit_discount_application, COUNT(*) as total
FROM erp.tb_contract_benefit_discount
GROUP BY contract_benefit_discount_application
ORDER BY total DESC;
```

### **Frontend**

1. **Criar Empréstimo:**
   - ✅ Combo mostra "Décimo Terceiro Salário"
   - ✅ Banco salva `13SAL`

2. **Editar Empréstimo Antigo:**
   - ✅ Carrega e migra valor antigo
   - ✅ Combo funciona normalmente
   - ✅ Salva código correto

3. **Listar Empréstimos:**
   - ✅ Tabela exibe "Décimo Terceiro Salário"
   - ✅ Não exibe código `13SAL`

4. **Adicionar Benefício/Desconto:**
   - ✅ Combo mostra "Todos os Pagamentos"
   - ✅ Banco salva `TODOS`

---

## 📊 **Mapeamento Completo**

### **DiscountSource (Empréstimos)**

| Código | Label Frontend |
|--------|----------------|
| `SALARIO` | Salário Mensal |
| `13SAL` | Décimo Terceiro Salário |
| `FERIAS` | Férias |
| `ANUAL` | Anual |
| `BONUS` | Bônus |
| `COMISSAO` | Comissão |

### **ApplicationType (Benefícios/Descontos)**

| Código | Label Frontend |
|--------|----------------|
| `TODOS` | Todos os Pagamentos |
| `SALARIO` | Salário |
| `13SAL` | Décimo Terceiro |
| `FERIAS` | Férias |
| `BONUS` | Bônus |
| `COMISSAO` | Comissão |

---

## ✅ **Benefícios**

1. **✅ Sem erros de varchar(10)**
   - Todos os códigos cabem no campo

2. **✅ UX mantida**
   - Usuário continua vendo textos amigáveis
   - Formulários intuitivos

3. **✅ Compatibilidade**
   - Migration converte dados existentes
   - Frontend migra valores antigos automaticamente

4. **✅ Manutenibilidade**
   - Enum centralizado (fácil adicionar novos)
   - Helpers reutilizáveis

5. **✅ Performance**
   - Códigos curtos = menos bytes
   - Queries mais rápidas

---

## ⚠️ **Importante**

- ✅ **Rodar migration ANTES** de usar novo frontend
- ✅ **Backup do banco** antes da migration
- ✅ **Testar em dev** antes de prod
- ✅ Verificar queries após migration (ver scripts acima)

---

## 🔧 **Adicionar Novos Códigos**

### **Backend (C#)**

```csharp
// DiscountSourceType.cs ou ApplicationType.cs
public const string NewCode = "NEWCODE";  // Max 10 chars

// GetDescription()
NewCode => "Novo Tipo Descritivo",

// GetAll()
{ NewCode, "Novo Tipo Descritivo" },
```

### **Frontend (TypeScript)**

```typescript
// discountSource.ts ou applicationType.ts
export const DiscountSourceCode = {
  // ... existing
  NEW_CODE: 'NEWCODE',
} as const;

export const DISCOUNT_SOURCE_OPTIONS = [
  // ... existing
  { value: DiscountSourceCode.NEW_CODE, label: 'Novo Tipo Descritivo' },
];
```

---

**Criado:** 2024-11-19  
**Autor:** System  
**Issue:** PostgresException 22001 - valor muito longo para varchar(10)  
**Status:** ✅ Implementado
