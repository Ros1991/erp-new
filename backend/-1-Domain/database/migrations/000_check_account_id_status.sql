-- Script de Verificação: Account ID Status
-- Execute ANTES da migration 005 para diagnosticar o problema

-- 1. Verificar se a coluna aceita NULL
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default,
    CASE 
        WHEN is_nullable = 'YES' THEN '✅ Aceita NULL'
        ELSE '❌ NÃO aceita NULL - PROBLEMA!'
    END as status
FROM information_schema.columns 
WHERE table_schema = 'erp' 
  AND table_name = 'tb_financial_transaction' 
  AND column_name = 'account_id';

-- 2. Verificar constraint FK
SELECT 
    conname as constraint_name,
    contype as constraint_type,
    CASE confdeltype 
        WHEN 'a' THEN 'NO ACTION'
        WHEN 'r' THEN 'RESTRICT'
        WHEN 'c' THEN 'CASCADE'
        WHEN 'n' THEN 'SET NULL'
        WHEN 'd' THEN 'SET DEFAULT'
    END as on_delete,
    CASE confupdtype 
        WHEN 'a' THEN 'NO ACTION'
        WHEN 'r' THEN 'RESTRICT'
        WHEN 'c' THEN 'CASCADE'
        WHEN 'n' THEN 'SET NULL'
        WHEN 'd' THEN 'SET DEFAULT'
    END as on_update
FROM pg_constraint
WHERE conname = 'fk_financial_transaction_account';

-- 3. Verificar registros com account_id = 0 (valor inválido)
SELECT 
    COUNT(*) as total_com_zero,
    CASE 
        WHEN COUNT(*) > 0 THEN '⚠️ Existem registros com account_id = 0 - PRECISAM SER CORRIGIDOS!'
        ELSE '✅ Nenhum registro com account_id = 0'
    END as status
FROM "erp"."tb_financial_transaction"
WHERE "account_id" = 0;

-- 4. Verificar registros com account_id = NULL
SELECT 
    COUNT(*) as total_com_null,
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Já existem registros com account_id NULL'
        ELSE 'ℹ️ Nenhum registro com account_id NULL ainda'
    END as status
FROM "erp"."tb_financial_transaction"
WHERE "account_id" IS NULL;

-- 5. Verificar todas as contas existentes
SELECT 
    COUNT(*) as total_contas,
    CASE 
        WHEN COUNT(*) = 0 THEN '⚠️ NENHUMA CONTA CADASTRADA - Criar empréstimo vai falhar!'
        WHEN COUNT(*) = 1 THEN '✅ 1 conta cadastrada (será auto-selecionada)'
        ELSE CONCAT('✅ ', COUNT(*), ' contas cadastradas')
    END as status
FROM "erp"."tb_account";

-- 6. Resumo do diagnóstico
DO $$
DECLARE
    col_nullable text;
    fk_exists boolean;
    zero_count integer;
BEGIN
    -- Verificar nullable
    SELECT is_nullable INTO col_nullable
    FROM information_schema.columns 
    WHERE table_schema = 'erp' 
      AND table_name = 'tb_financial_transaction' 
      AND column_name = 'account_id';
    
    -- Verificar FK
    SELECT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'fk_financial_transaction_account'
    ) INTO fk_exists;
    
    -- Contar zeros
    SELECT COUNT(*) INTO zero_count
    FROM "erp"."tb_financial_transaction"
    WHERE "account_id" = 0;
    
    -- Relatório
    RAISE NOTICE '════════════════════════════════════════';
    RAISE NOTICE 'DIAGNÓSTICO - Account ID Status';
    RAISE NOTICE '════════════════════════════════════════';
    
    IF col_nullable = 'YES' THEN
        RAISE NOTICE '✅ Coluna aceita NULL';
    ELSE
        RAISE NOTICE '❌ Coluna NÃO aceita NULL - EXECUTAR MIGRATION 005!';
    END IF;
    
    IF fk_exists THEN
        RAISE NOTICE '✅ FK constraint existe';
    ELSE
        RAISE NOTICE '❌ FK constraint não existe - EXECUTAR MIGRATION 005!';
    END IF;
    
    IF zero_count > 0 THEN
        RAISE NOTICE '⚠️ % registros com account_id = 0 - EXECUTAR MIGRATION 005!', zero_count;
    ELSE
        RAISE NOTICE '✅ Nenhum registro com account_id inválido';
    END IF;
    
    RAISE NOTICE '════════════════════════════════════════';
    
    IF col_nullable = 'YES' AND fk_exists AND zero_count = 0 THEN
        RAISE NOTICE '🎉 TUDO OK! Não precisa executar migration 005.';
    ELSE
        RAISE NOTICE '🔧 EXECUTAR MIGRATION 005 para corrigir problemas!';
    END IF;
    
    RAISE NOTICE '════════════════════════════════════════';
END $$;
