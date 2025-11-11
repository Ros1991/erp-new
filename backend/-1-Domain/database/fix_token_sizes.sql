-- Script para aumentar tamanho dos campos de token
-- De 255/512 para 2048 caracteres para suportar JWT tokens maiores

-- Tabela: tb_user
ALTER TABLE "erp"."tb_user" 
ALTER COLUMN "user_reset_token" TYPE varchar(2048);

-- Tabela: tb_user_token
ALTER TABLE "erp"."tb_user_token" 
ALTER COLUMN "user_token_token" TYPE varchar(2048);

ALTER TABLE "erp"."tb_user_token" 
ALTER COLUMN "user_token_refresh_token" TYPE varchar(2048);

-- Verificar alterações
SELECT 
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'erp' 
AND table_name IN ('tb_user', 'tb_user_token')
AND column_name LIKE '%token%'
ORDER BY table_name, column_name;
