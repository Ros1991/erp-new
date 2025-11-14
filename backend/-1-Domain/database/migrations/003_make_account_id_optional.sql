-- Migration: Make account_id optional in financial_transaction
-- Date: 2025-11-14
-- Description: Torna o campo account_id opcional para suportar pequenas empresas
--              sem contas cadastradas ou transações sem conta específica

-- 1. Remover NOT NULL constraint
ALTER TABLE "erp"."tb_financial_transaction" 
ALTER COLUMN "account_id" DROP NOT NULL;

-- 2. Remover DEFAULT (valores existentes com 0 continuam, mas novos podem ser NULL)
ALTER TABLE "erp"."tb_financial_transaction" 
ALTER COLUMN "account_id" DROP DEFAULT;

-- Comentário na coluna
COMMENT ON COLUMN "erp"."tb_financial_transaction"."account_id" 
IS 'ID da conta bancária (opcional - NULL para empresas sem contas ou transações sem conta específica)';
