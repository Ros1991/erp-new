# ⚠️ IMPORTANTE: Aplicar Migrations do Banco de Dados

## 🚨 Problema Atual

A exclusão de empréstimos está falhando com erro de Foreign Key porque a migration **001** ainda **NÃO foi aplicada** no banco.

## 🔧 Solução Temporária (Aplicada)

Foi adicionada lógica manual no `LoanAdvanceService.DeleteByIdAsync` para:
1. Deletar `TransactionCostCenter` relacionados
2. Deletar `FinancialTransaction` relacionada
3. Deletar o `LoanAdvance`

**Isso funciona, mas é redundante!**

## ✅ Solução Definitiva

### Execute a migration 001:

```bash
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/001_add_loan_advance_id_to_financial_transaction.sql
```

### O que a migration faz:

1. Adiciona coluna `loan_advance_id` em `tb_financial_transaction`
2. Cria Foreign Key com **ON DELETE CASCADE**
3. Cria índice para performance

### Após aplicar a migration:

- ✅ A lógica manual no Service continuará funcionando (redundante mas segura)
- ✅ O banco automaticamente deletará as transações em cascata
- ✅ O código ficará mais simples
- ✅ Você pode opcionalmente remover a lógica manual do Service depois

## 📋 Script da Migration 001

```sql
-- Adicionar a coluna loan_advance_id
ALTER TABLE "erp"."tb_financial_transaction" 
ADD COLUMN "loan_advance_id" bigint NULL;

-- Adicionar foreign key constraint com CASCADE
ALTER TABLE "erp"."tb_financial_transaction" 
ADD CONSTRAINT "fk_financial_transaction_loan_advance" 
FOREIGN KEY ("loan_advance_id") 
REFERENCES "erp"."tb_loan_advance"("loan_advance_id") 
ON DELETE CASCADE;

-- Criar índice
CREATE INDEX "idx_financial_transaction_loan_advance" 
ON "erp"."tb_financial_transaction"("loan_advance_id");
```

## 🔍 Como Verificar se a Migration foi Aplicada

Execute no PostgreSQL:

```sql
-- Verificar se a coluna existe
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'erp' 
  AND table_name = 'tb_financial_transaction' 
  AND column_name = 'loan_advance_id';

-- Verificar se a FK existe
SELECT constraint_name, delete_rule
FROM information_schema.referential_constraints
WHERE constraint_schema = 'erp'
  AND constraint_name = 'fk_financial_transaction_loan_advance';
```

**Resultado esperado:**
- Coluna `loan_advance_id` existe: ✅
- Delete rule = `CASCADE`: ✅

## 🎯 Fluxo Após Migration

```
DELETE LoanAdvance ID 1
    ↓
PostgreSQL automaticamente deleta:
    ├─ tb_financial_transaction (loan_advance_id = 1)
    │   └─ CASCADE para tb_transaction_cost_center
    └─ Sucesso!
```

## ⏱️ Quando Aplicar

**RECOMENDAÇÃO:** Aplique a migration **o quanto antes** para:
- Simplificar o código
- Melhorar performance (menos queries)
- Usar recursos nativos do banco
- Garantir integridade referencial automática

## 📝 Checklist

- [ ] Aplicar migration 001
- [ ] Verificar se coluna `loan_advance_id` existe
- [ ] Verificar se FK com CASCADE existe
- [ ] Testar exclusão de empréstimo
- [ ] (Opcional) Simplificar código do Service removendo deleções manuais

---

**Status Atual:** Código funciona, mas aguarda migration para otimização! 🚀
