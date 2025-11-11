/*------------------------------------------------------------*/
/*   Script de Migração: Tornar CNPJ da Empresa Opcional     */
/*                                                            */
/*   Data: 2025-11-11                                         */
/*   Autor: Sistema ERP                                       */
/*                                                            */
/*   Descrição: Altera o campo company_document para         */
/*   permitir valores NULL e remove a constraint UNIQUE      */
/*------------------------------------------------------------*/

-- Verificar se o banco está configurado corretamente
SELECT current_database(), current_schema();

-- Backup recomendado antes de executar:
-- pg_dump -U postgres -d erp_database > backup_before_cnpj_optional.sql

/*------------------------------------------------------------*/
/* 1. Remover constraint UNIQUE do company_document          */
/*------------------------------------------------------------*/

-- Verificar se a constraint existe
SELECT 
    conname AS constraint_name,
    contype AS constraint_type
FROM pg_constraint
WHERE conrelid = 'erp.tb_company'::regclass
    AND conname = 'uk_company_document';

-- Remover constraint UNIQUE (se existir)
DO $$ 
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM pg_constraint 
        WHERE conrelid = 'erp.tb_company'::regclass 
            AND conname = 'uk_company_document'
    ) THEN
        ALTER TABLE "erp"."tb_company" DROP CONSTRAINT "uk_company_document";
        RAISE NOTICE 'Constraint uk_company_document removida com sucesso';
    ELSE
        RAISE NOTICE 'Constraint uk_company_document não existe, pulando...';
    END IF;
END $$;

/*------------------------------------------------------------*/
/* 2. Alterar campo company_document para permitir NULL      */
/*------------------------------------------------------------*/

-- Alterar coluna para aceitar NULL
ALTER TABLE "erp"."tb_company" 
    ALTER COLUMN "company_document" DROP NOT NULL;

-- Verificar a alteração
SELECT 
    column_name,
    data_type,
    is_nullable,
    character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'erp'
    AND table_name = 'tb_company'
    AND column_name = 'company_document';

/*------------------------------------------------------------*/
/* 3. (Opcional) Criar índice parcial para CNPJs únicos      */
/*    Permite múltiplos NULL mas mantém CNPJs únicos         */
/*------------------------------------------------------------*/

-- Criar índice único parcial (ignora valores NULL)
-- Isso permite múltiplas empresas sem CNPJ, mas garante
-- que CNPJs informados sejam únicos
CREATE UNIQUE INDEX IF NOT EXISTS "idx_company_document_unique_partial"
    ON "erp"."tb_company" ("company_document")
    WHERE "company_document" IS NOT NULL;

COMMENT ON INDEX "erp"."idx_company_document_unique_partial" IS 
    'Índice único parcial: permite múltiplos NULL mas garante que CNPJs informados sejam únicos';

/*------------------------------------------------------------*/
/* 4. Validar alterações                                     */
/*------------------------------------------------------------*/

-- Testar inserção de empresa sem CNPJ
DO $$ 
DECLARE
    v_user_id bigint;
    v_company_id bigint;
BEGIN
    -- Buscar um usuário válido para teste
    SELECT user_id INTO v_user_id FROM erp.tb_user LIMIT 1;
    
    IF v_user_id IS NOT NULL THEN
        -- Tentar inserir empresa sem CNPJ
        INSERT INTO erp.tb_company (
            company_name, 
            company_document, 
            user_id, 
            criado_por, 
            criado_em
        ) VALUES (
            'Empresa Teste - Sem CNPJ', 
            NULL,  -- CNPJ NULL
            v_user_id, 
            v_user_id, 
            NOW()
        ) RETURNING company_id INTO v_company_id;
        
        RAISE NOTICE 'Teste OK: Empresa sem CNPJ criada com ID %', v_company_id;
        
        -- Limpar teste
        DELETE FROM erp.tb_company WHERE company_id = v_company_id;
        RAISE NOTICE 'Teste removido com sucesso';
    ELSE
        RAISE NOTICE 'Nenhum usuário encontrado para teste. Pulando validação.';
    END IF;
END $$;

/*------------------------------------------------------------*/
/* 5. Resumo das alterações                                  */
/*------------------------------------------------------------*/

SELECT 
    'company_document' AS campo,
    (SELECT is_nullable FROM information_schema.columns 
     WHERE table_schema = 'erp' AND table_name = 'tb_company' 
     AND column_name = 'company_document') AS permite_null,
    (SELECT COUNT(*) FROM pg_constraint 
     WHERE conrelid = 'erp.tb_company'::regclass 
     AND conname = 'uk_company_document') AS constraint_unique_existe,
    (SELECT COUNT(*) FROM pg_indexes 
     WHERE schemaname = 'erp' AND tablename = 'tb_company' 
     AND indexname = 'idx_company_document_unique_partial') AS indice_parcial_existe;

RAISE NOTICE '========================================';
RAISE NOTICE 'Migração concluída com sucesso!';
RAISE NOTICE 'Campo company_document agora é OPCIONAL';
RAISE NOTICE 'CNPJs informados continuam únicos (índice parcial)';
RAISE NOTICE '========================================';
