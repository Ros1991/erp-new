# 📊 Mudanças no Schema - Sistema de Folha de Pagamento

## 📋 Resumo

Implementação completa do sistema de gestão de folhas de pagamento com suporte a:
- ✅ Fechamento e reabertura de folhas
- ✅ Controle de empréstimos por parcelas
- ✅ 13º salário com opções de tributação
- ✅ Gestão de férias
- ✅ Itens manuais e automáticos
- ✅ Soft delete de itens
- ✅ Auditoria completa

---

## 🗃️ Arquivos Modificados

### **Backend - Entidades**
- ✅ `Payroll.cs` - Campos de fechamento e auditoria
- ✅ `PayrollEmployee.cs` - Campos de contrato, 13º e férias
- ✅ `PayrollItem.cs` - Campos de controle manual e parcelas
- ✅ `LoanAdvance.cs` - Campos calculados de pagamento

### **Backend - Database**
- ✅ `erp.sql` - Schema principal atualizado
- ✅ `migrations/005_payroll_improvements.sql` - Migration script

---

## 📊 Mudanças nas Tabelas

### **1. tb_payroll**

**Novos Campos:**
```sql
payroll_closed_at         timestamptz    -- Data/hora do fechamento
payroll_closed_by         bigint         -- Usuário que fechou
payroll_notes             text           -- Observações gerais
payroll_snapshot          jsonb          -- Backup para rollback
```

**Novos Índices:**
- `idx_payroll_company_period` - Busca rápida da última folha
- `idx_payroll_company_status` - Filtrar por status (aberta/fechada)

**Nova FK:**
- `fk_payroll_closed_by` → `tb_user(user_id)`

---

### **2. tb_payroll_employee**

**Novos Campos:**
```sql
-- Rastreabilidade
contract_id                           bigint
payroll_employee_base_salary          decimal(10,2)

-- Décimo Terceiro
payroll_employee_has_13th             boolean
payroll_employee_13th_type            varchar(10)      -- 'full', 'partial'
payroll_employee_13th_amount          decimal(10,2)
payroll_employee_13th_tax_option      varchar(10)      -- 'full', 'none', 'partial'

-- Férias
payroll_employee_vacation_advance_paid  boolean
payroll_employee_vacation_start_date    date
payroll_employee_vacation_end_date      date
payroll_employee_vacation_notes         text

-- Observações
payroll_employee_notes                text
```

**Novos Índices:**
- `idx_payroll_employee_contract` - Rastreabilidade de contratos
- `idx_payroll_employee_payroll` - Performance em joins

**Nova FK:**
- `fk_payroll_employee_contract` → `tb_contract(contract_id)`

---

### **3. tb_payroll_item** ⭐ CORE

**Novos Campos:**
```sql
payroll_item_is_manual            boolean       -- true = não recalcular
payroll_item_is_active            boolean       -- soft delete
payroll_item_source_type          varchar(20)   -- origem do item
payroll_item_installment_number   int           -- nº da parcela (empréstimos)
payroll_item_installment_total    int           -- total de parcelas
```

**source_type valores possíveis:**
- `'contract_benefit'` - Benefício do contrato
- `'contract_discount'` - Desconto do contrato
- `'loan'` - Parcela de empréstimo
- `'tax_inss'` - INSS
- `'tax_irrf'` - IRRF
- `'tax_fgts'` - FGTS
- `'13th'` - 13º salário
- `'vacation'` - Férias
- `'manual'` - Item criado manualmente

**Novos Índices:**
- `idx_payroll_item_employee` - Performance geral
- `idx_payroll_item_reference` - Buscar por referência (empréstimo, contrato)
- `idx_payroll_item_active` - Filtrar ativos/inativos
- `idx_payroll_item_loan` - Otimizado para empréstimos (índice parcial)

---

### **4. tb_loan_advance**

**Novos Campos:**
```sql
loan_advance_installments_paid    int            -- Contador de parcelas
loan_advance_remaining_amount     decimal(10,2)  -- Saldo restante
loan_advance_is_fully_paid        boolean        -- Flag de quitação
```

**Novo Índice:**
- `idx_loan_advance_employee_pending` - Buscar empréstimos pendentes

---

### **5. tb_contract**

**Novo Índice:**
- `idx_contract_payroll_active` - Índice parcial para contratos ativos de folha

---

## 🔄 Fluxos Principais

### **Criar Folha**
1. INSERT `tb_payroll` (status aberto)
2. Buscar contratos ativos via `idx_contract_payroll_active`
3. Para cada contrato → INSERT `tb_payroll_employee`
4. Criar itens automáticos:
   - Benefícios: `source_type = 'contract_benefit'`
   - Descontos: `source_type = 'contract_discount'`
   - Empréstimos: `source_type = 'loan'` + `installment_number`
   - Impostos: `source_type = 'tax_inss'`, `'tax_irrf'`, `'tax_fgts'`

### **Fechar Folha** ⚠️ IMPORTANTE
1. UPDATE `tb_payroll` SET `is_closed = true`, `closed_at = NOW()`, `snapshot = {...}`
2. Para cada item de empréstimo (`source_type = 'loan'` AND `is_active = true`):
   ```sql
   UPDATE tb_loan_advance
   SET installments_paid = installments_paid + 1,
       remaining_amount = remaining_amount - item_amount,
       is_fully_paid = (installments_paid + 1 >= installments)
   WHERE loan_advance_id = item.reference_id
   ```

### **Reabrir Folha (Rollback)** ⚠️ IMPORTANTE
1. Verificar se é a última folha fechada (validação!)
2. Para cada item de empréstimo (`source_type = 'loan'` AND `is_active = true`):
   ```sql
   UPDATE tb_loan_advance
   SET installments_paid = installments_paid - 1,
       remaining_amount = remaining_amount + item_amount,
       is_fully_paid = false
   WHERE loan_advance_id = item.reference_id
   ```
3. UPDATE `tb_payroll` SET `is_closed = false`, `closed_at = NULL`

### **Remover Parcela de Empréstimo**
```sql
-- Soft delete (preserva histórico)
UPDATE tb_payroll_item 
SET payroll_item_is_active = false 
WHERE payroll_item_id = @id;

-- Se a folha estiver aberta, nada mais a fazer
-- Se estiver fechada, precisa reabrir primeiro!
```

### **Buscar Empréstimos Pendentes**
```sql
SELECT * FROM tb_loan_advance
WHERE employee_id = @emp_id
  AND loan_advance_is_approved = true
  AND loan_advance_is_fully_paid = false
ORDER BY loan_advance_start_date;
```

### **Buscar Histórico de Parcelas**
```sql
SELECT 
    pi.payroll_item_id,
    pi.payroll_item_installment_number,
    pi.payroll_item_amount,
    pi.payroll_item_is_active,
    p.payroll_period_start_date,
    p.payroll_period_end_date,
    p.payroll_is_closed
FROM tb_payroll_item pi
JOIN tb_payroll_employee pe ON pi.payroll_employee_id = pe.payroll_employee_id
JOIN tb_payroll p ON pe.payroll_id = p.payroll_id
WHERE pi.payroll_item_reference_id = @loan_id
  AND pi.payroll_item_source_type = 'loan'
ORDER BY pi.payroll_item_installment_number;
```

### **Próxima Parcela a Cobrar**
```sql
SELECT COALESCE(MAX(payroll_item_installment_number), 0) + 1 AS next_installment
FROM tb_payroll_item
WHERE payroll_item_reference_id = @loan_id
  AND payroll_item_source_type = 'loan'
  AND payroll_item_is_active = true;
```

---

## 📝 Estruturas JSONB

### **payroll_snapshot (tb_payroll)**
```json
{
  "closed_at": "2024-03-31T23:59:59Z",
  "closed_by": 123,
  "employees": [
    {
      "payroll_employee_id": 456,
      "employee_id": 789,
      "contract_id": 101,
      "base_salary": 5000.00,
      "items": [
        {
          "payroll_item_id": 1001,
          "description": "Salário Base",
          "type": "Provento",
          "amount": 5000.00
        }
      ]
    }
  ],
  "totals": {
    "gross_pay": 50000.00,
    "deductions": 8500.00,
    "net_pay": 41500.00
  }
}
```

### **payroll_item_calculation_details (tb_payroll_item)**

**Para Impostos:**
```json
{
  "tax_type": "INSS",
  "calculation_basis": 5000.00,
  "rate": 0.11,
  "description": "INSS 11% sobre salário base",
  "brackets": [
    {"from": 0, "to": 1100.00, "rate": 0.075, "amount": 82.50},
    {"from": 1100.01, "to": 5000.00, "rate": 0.11, "amount": 429.00}
  ]
}
```

**Para Empréstimos:**
```json
{
  "loan_id": 456,
  "installment_number": 3,
  "installment_total": 12,
  "original_amount": 12000.00,
  "installment_amount": 1000.00,
  "remaining_before": 10000.00,
  "remaining_after": 9000.00
}
```

---

## ✅ Validações Importantes

### **Ao Fechar Folha:**
- ✅ Validar que não existe outra folha aberta para o mesmo período
- ✅ Validar que todos os cálculos estão corretos
- ✅ Criar snapshot completo antes de fechar
- ✅ Atualizar empréstimos APENAS após fechar

### **Ao Reabrir Folha:**
- ✅ Verificar se é a ÚLTIMA folha fechada
- ✅ Reverter todos os empréstimos marcados como pagos
- ✅ Não permitir reabertura de folhas antigas

### **Ao Recalcular:**
- ✅ Deletar APENAS itens com `is_manual = false`
- ✅ Preservar itens manuais
- ✅ Recriar itens automáticos

---

## 🎯 Benefícios da Arquitetura

✅ **Sem tabela extra** - Controle de parcelas direto em `tb_payroll_item`
✅ **Histórico completo** - Cada parcela tem registro individual
✅ **Soft delete** - Remover sem perder histórico
✅ **Rollback seguro** - Snapshot + reversão de empréstimos
✅ **Performance** - Índices otimizados para queries comuns
✅ **Auditoria** - Rastreabilidade completa de quem/quando/o quê
✅ **Flexibilidade** - Suporta itens manuais e automáticos
✅ **Escalável** - Padrão reutilizável para outros descontos

---

## 🚀 Próximos Passos

1. **Executar Migration:**
   ```bash
   psql -U postgres -d erp_db -f 005_payroll_improvements.sql
   ```

2. **Criar DTOs:**
   - `PayrollOutputDTO`
   - `PayrollInputDTO`
   - `PayrollEmployeeDTO`
   - `PayrollItemDTO`

3. **Criar Services:**
   - `PayrollService` - Lógica de criação/fechamento
   - `PayrollCalculationService` - Cálculos de impostos
   - `LoanAdvanceService` - Atualizar para novos campos

4. **Criar Endpoints:**
   - `POST /api/payroll` - Criar folha
   - `GET /api/payroll` - Listar com última destacada
   - `PUT /api/payroll/{id}/close` - Fechar folha
   - `PUT /api/payroll/{id}/reopen` - Reabrir folha
   - `POST /api/payroll/{id}/recalculate` - Recalcular

---

**Documentação criada em:** 2024-11-17
**Migration:** `005_payroll_improvements.sql`
**Versão:** 1.0
