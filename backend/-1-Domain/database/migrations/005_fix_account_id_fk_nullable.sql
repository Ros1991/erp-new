-- Migration: Fix account_id FK to properly accept NULL values
-- Date: 2025-11-14
-- Description: Remove e recria a constraint FK para account_id garantindo que NULL seja aceito
--              Corrige o erro: 23503 inserção ou atualização em tabela tb_financial_transaction 
--              viola restrição de chave estrangeira fk_financial_transaction_account

-- 1. Verificar se existem registros com account_id = 0 e atualizar para NULL
UPDATE "erp"."tb_financial_transaction"
SET "account_id" = NULL
WHERE "account_id" = 0;

-- 2. Remover constraint antiga de FK
ALTER TABLE "erp"."tb_financial_transaction" 
DROP CONSTRAINT IF EXISTS "fk_financial_transaction_account";

-- 3. Garantir que a coluna é nullable (pode já estar, mas garante)
ALTER TABLE "erp"."tb_financial_transaction" 
ALTER COLUMN "account_id" DROP NOT NULL;

-- 4. Remover qualquer DEFAULT que possa existir
ALTER TABLE "erp"."tb_financial_transaction" 
ALTER COLUMN "account_id" DROP DEFAULT;

-- 5. Recriar constraint FK (FKs em PostgreSQL permitem NULL por padrão)
ALTER TABLE "erp"."tb_financial_transaction" 
ADD CONSTRAINT "fk_financial_transaction_account"
FOREIGN KEY ("account_id")
REFERENCES "erp"."tb_account" ("account_id")
ON DELETE RESTRICT
ON UPDATE CASCADE;

-- 6. Comentário explicativo
COMMENT ON COLUMN "erp"."tb_financial_transaction"."account_id" 
IS 'ID da conta bancária - OPCIONAL (NULL para empresas sem contas ou transações sem conta específica)';

-- 7. Verificação final
DO $$
BEGIN
    -- Verifica se a constraint foi criada corretamente
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'fk_financial_transaction_account'
    ) THEN
        RAISE EXCEPTION 'Constraint FK não foi criada corretamente!';
    END IF;
    
    -- Verifica se a coluna aceita NULL
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'erp' 
        AND table_name = 'tb_financial_transaction' 
        AND column_name = 'account_id' 
        AND is_nullable = 'NO'
    ) THEN
        RAISE EXCEPTION 'Coluna account_id ainda não aceita NULL!';
    END IF;
    
    RAISE NOTICE 'Migration 005 aplicada com sucesso! account_id agora aceita NULL corretamente.';
END $$;
