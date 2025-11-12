-- Migration: Corrigir DEFAULT do campo user_id na tabela tb_employee
-- Problema: O campo tinha DEFAULT 0, mas 0 não existe em tb_user, causando violação de FK
-- Solução: Remover o DEFAULT e alterar valores 0 existentes para NULL

-- Passo 1: Atualizar registros existentes com user_id = 0 para NULL
UPDATE erp.tb_employee
SET user_id = NULL
WHERE user_id = 0;

-- Passo 2: Remover o DEFAULT do campo
ALTER TABLE erp.tb_employee 
ALTER COLUMN user_id DROP DEFAULT;

-- Validação: Verificar se ainda existem user_id = 0
-- SELECT COUNT(*) FROM erp.tb_employee WHERE user_id = 0;
-- Resultado esperado: 0
