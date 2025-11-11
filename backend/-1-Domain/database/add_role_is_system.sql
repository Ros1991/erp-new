/*------------------------------------------------------------*/
/*   Script de Migração: Adicionar Campo role_is_system      */
/*                                                            */
/*   Data: 2025-11-11                                         */
/*   Autor: Sistema ERP                                       */
/*                                                            */
/*   Descrição: Adiciona campo para marcar roles do sistema  */
/*   (Owner/Admin) que não podem ser editadas ou deletadas   */
/*------------------------------------------------------------*/

-- Verificar banco atual
SELECT current_database(), current_schema();

-- Backup recomendado antes de executar:
-- pg_dump -U postgres -d erp_database > backup_before_role_is_system.sql

/*------------------------------------------------------------*/
/* 1. Adicionar coluna role_is_system                        */
/*------------------------------------------------------------*/

-- Verificar se a coluna já existe
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'erp' 
            AND table_name = 'tb_role' 
            AND column_name = 'role_is_system'
    ) THEN
        -- Adicionar coluna role_is_system
        ALTER TABLE "erp"."tb_role" 
            ADD COLUMN "role_is_system" boolean DEFAULT false NOT NULL;
        
        RAISE NOTICE 'Coluna role_is_system adicionada com sucesso';
    ELSE
        RAISE NOTICE 'Coluna role_is_system já existe, pulando...';
    END IF;
END $$;

/*------------------------------------------------------------*/
/* 2. Atualizar roles existentes chamadas "Dono" ou "Owner" */
/*------------------------------------------------------------*/

-- Marcar roles existentes com nome "Dono" ou "Owner" como system
UPDATE "erp"."tb_role"
SET "role_is_system" = true
WHERE LOWER("role_name") IN ('dono', 'owner', 'administrador', 'admin');

-- Verificar quantas roles foram marcadas
DO $$ 
DECLARE
    v_count integer;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM "erp"."tb_role"
    WHERE "role_is_system" = true;
    
    RAISE NOTICE 'Total de roles do sistema: %', v_count;
END $$;

/*------------------------------------------------------------*/
/* 3. Adicionar comentário na coluna                         */
/*------------------------------------------------------------*/

COMMENT ON COLUMN "erp"."tb_role"."role_is_system" IS 
    'Indica se a role é do sistema (Owner/Admin). Roles do sistema não podem ser editadas ou deletadas.';

/*------------------------------------------------------------*/
/* 4. Validar alteração                                      */
/*------------------------------------------------------------*/

-- Verificar estrutura da coluna
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'erp'
    AND table_name = 'tb_role'
    AND column_name = 'role_is_system';

-- Listar todas as roles do sistema
SELECT 
    role_id,
    company_id,
    role_name,
    role_is_system,
    criado_em
FROM "erp"."tb_role"
WHERE role_is_system = true
ORDER BY company_id, role_name;

/*------------------------------------------------------------*/
/* 5. Resumo                                                 */
/*------------------------------------------------------------*/

SELECT 
    'role_is_system' AS campo,
    (SELECT COUNT(*) FROM information_schema.columns 
     WHERE table_schema = 'erp' AND table_name = 'tb_role' 
     AND column_name = 'role_is_system') AS coluna_existe,
    (SELECT COUNT(*) FROM erp.tb_role WHERE role_is_system = true) AS roles_sistema;

RAISE NOTICE '========================================';
RAISE NOTICE 'Migração concluída com sucesso!';
RAISE NOTICE 'Campo role_is_system adicionado';
RAISE NOTICE 'Roles existentes "Dono/Owner" marcadas como sistema';
RAISE NOTICE '========================================';
