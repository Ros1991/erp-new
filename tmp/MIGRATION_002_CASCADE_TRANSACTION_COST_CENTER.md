# Migration 002: ON DELETE CASCADE para Transaction Cost Center

## 🎯 Objetivo

Corrigir erro de Foreign Key ao deletar transações financeiras que possuem centros de custo associados.

## ❌ Erro Resolvido

```
PostgresException: 23503: atualização ou exclusão em tabela "tb_financial_transaction" 
viola restrição de chave estrangeira "fk_transaction_cost_center_transaction" 
em "tb_transaction_cost_center"
```

## 🔧 O que foi feito

### **1. Atualizado erp.sql (linha 1036-1041)**

```sql
ALTER TABLE "erp"."tb_transaction_cost_center" 
ADD CONSTRAINT "fk_transaction_cost_center_transaction"
FOREIGN KEY ("financial_transaction_id") 
REFERENCES "erp"."tb_financial_transaction"("financial_transaction_id") 
ON DELETE CASCADE;  -- ✅ ADICIONADO
```

### **2. Criado script de migration**

**Arquivo:** `backend/-1-Domain/database/migrations/002_add_cascade_to_transaction_cost_center.sql`

```sql
-- 1. Remover constraint antiga
ALTER TABLE "erp"."tb_transaction_cost_center" 
DROP CONSTRAINT IF EXISTS "fk_transaction_cost_center_transaction";

-- 2. Recriar com CASCADE
ALTER TABLE "erp"."tb_transaction_cost_center" 
ADD CONSTRAINT "fk_transaction_cost_center_transaction"
FOREIGN KEY ("financial_transaction_id") 
REFERENCES "erp"."tb_financial_transaction"("financial_transaction_id") 
ON DELETE CASCADE;
```

---

## 🚀 Como Aplicar

### **Para bancos existentes:**

```bash
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/002_add_cascade_to_transaction_cost_center.sql
```

### **Para bancos novos:**

Apenas use o `erp.sql` que já está corrigido.

---

## 🔍 Como Verificar

Execute no PostgreSQL:

```sql
SELECT 
    tc.constraint_name,
    rc.delete_rule
FROM information_schema.table_constraints tc
JOIN information_schema.referential_constraints rc 
    ON tc.constraint_name = rc.constraint_name
WHERE tc.constraint_schema = 'erp'
  AND tc.constraint_name = 'fk_transaction_cost_center_transaction';
```

**Resultado esperado:**
- `delete_rule = 'CASCADE'` ✅

**Se retornar `NO ACTION`:**
- A migration ainda não foi aplicada ⚠️

---

## 🔄 Fluxo Correto Após Migration

### **Deletar Empréstimo:**

```
DELETE LoanAdvance ID 1
    ↓ (CASCADE)
DELETE FinancialTransaction (loan_advance_id = 1)
    ↓ (CASCADE)
DELETE TransactionCostCenter (financial_transaction_id = ...)
    ↓
✅ Sucesso! Tudo limpo
```

### **Deletar Transação Financeira:**

```
DELETE FinancialTransaction ID 5
    ↓ (CASCADE)
DELETE TransactionCostCenter (financial_transaction_id = 5)
    ↓
✅ Sucesso! Centros de custo deletados automaticamente
```

---

## 📊 Impacto

### **Antes (SEM CASCADE):**

```
1. User tenta deletar transação
2. PostgreSQL: ERRO! Tem centros de custo vinculados
3. User precisa deletar centros de custo manualmente primeiro
4. User deleta transação
```

### **Depois (COM CASCADE):**

```
1. User tenta deletar transação
2. PostgreSQL deleta centros de custo automaticamente
3. PostgreSQL deleta transação
4. ✅ Sucesso em 1 operação!
```

---

## ⚠️ IMPORTANTE

**Esta migration é OBRIGATÓRIA para o sistema funcionar corretamente!**

Sem ela:
- ❌ Não consegue deletar empréstimos
- ❌ Não consegue deletar transações financeiras com centros de custo
- ❌ Erro 23503 em produção

Com ela:
- ✅ Deleção funciona perfeitamente
- ✅ Limpeza automática de dados relacionados
- ✅ UX sem erros

---

## 🧪 Teste Rápido

Após aplicar a migration, teste:

```sql
-- 1. Criar transação com centro de custo
INSERT INTO tb_financial_transaction (...) VALUES (...);
INSERT INTO tb_transaction_cost_center (...) VALUES (...);

-- 2. Deletar transação
DELETE FROM tb_financial_transaction WHERE financial_transaction_id = ...;

-- 3. Verificar se centro de custo foi deletado automaticamente
SELECT * FROM tb_transaction_cost_center WHERE financial_transaction_id = ...;
-- Deve retornar 0 registros ✅
```

---

**Data:** 2025-11-14  
**Status:** ✅ Script criado e erp.sql atualizado  
**Próximo passo:** Aplicar migration no banco de dados
