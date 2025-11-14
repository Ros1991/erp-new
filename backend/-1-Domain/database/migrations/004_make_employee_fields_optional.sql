-- Migration: Make employee fields optional (only nickname required)
-- Date: 2025-11-14
-- Description: Torna apenas o nickname obrigatório na tabela de empregados.
--              Permite NULL em full_name, email, phone e cpf.
--              Cria UNIQUE INDEX com WHERE NOT NULL para permitir múltiplos NULL.

-- 1. Remover constraints UNIQUE antigas
ALTER TABLE "erp"."tb_employee" DROP CONSTRAINT IF EXISTS "uk_employee_cpf";
ALTER TABLE "erp"."tb_employee" DROP CONSTRAINT IF EXISTS "uk_employee_email";
ALTER TABLE "erp"."tb_employee" DROP CONSTRAINT IF EXISTS "uk_employee_phone";
DROP INDEX IF EXISTS "erp"."uk_employee_cpf";
DROP INDEX IF EXISTS "erp"."uk_employee_email";
DROP INDEX IF EXISTS "erp"."uk_employee_phone";

-- 2. Tornar colunas nullable (apenas nickname continua obrigatório)
ALTER TABLE "erp"."tb_employee" 
ALTER COLUMN "employee_full_name" DROP NOT NULL;

ALTER TABLE "erp"."tb_employee" 
ALTER COLUMN "employee_email" DROP NOT NULL;

ALTER TABLE "erp"."tb_employee" 
ALTER COLUMN "employee_phone" DROP NOT NULL;

ALTER TABLE "erp"."tb_employee" 
ALTER COLUMN "employee_cpf" DROP NOT NULL;

-- 3. Criar UNIQUE INDEX com suporte a NULL (múltiplos NULL permitidos, valores não-NULL únicos)
CREATE UNIQUE INDEX "uk_employee_cpf" 
ON "erp"."tb_employee"("employee_cpf") 
WHERE "employee_cpf" IS NOT NULL;

CREATE UNIQUE INDEX "uk_employee_email" 
ON "erp"."tb_employee"("employee_email") 
WHERE "employee_email" IS NOT NULL;

CREATE UNIQUE INDEX "uk_employee_phone" 
ON "erp"."tb_employee"("employee_phone") 
WHERE "employee_phone" IS NOT NULL;

-- 4. Comentários explicativos
COMMENT ON COLUMN "erp"."tb_employee"."employee_nickname" 
IS 'Apelido do empregado - OBRIGATÓRIO e único por empresa';

COMMENT ON COLUMN "erp"."tb_employee"."employee_full_name" 
IS 'Nome completo do empregado - OPCIONAL';

COMMENT ON COLUMN "erp"."tb_employee"."employee_email" 
IS 'Email do empregado - OPCIONAL (se preenchido, deve ser único)';

COMMENT ON COLUMN "erp"."tb_employee"."employee_phone" 
IS 'Telefone do empregado - OPCIONAL (se preenchido, deve ser único)';

COMMENT ON COLUMN "erp"."tb_employee"."employee_cpf" 
IS 'CPF do empregado - OPCIONAL (se preenchido, deve ser único)';
