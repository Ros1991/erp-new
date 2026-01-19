-- Migration: Adicionar campo loan_advance_date na tabela tb_loan_advance
-- Data: 2026-01-19
-- Descrição: Campo para armazenar a data real do empréstimo (usado no relatório de conta corrente)

-- Adicionar coluna loan_advance_date com valor default sendo a data atual
ALTER TABLE "erp"."tb_loan_advance" 
ADD COLUMN IF NOT EXISTS "loan_advance_date" date DEFAULT CURRENT_DATE NOT NULL;

-- Atualizar registros existentes: usar criado_em como data do empréstimo
UPDATE "erp"."tb_loan_advance" 
SET "loan_advance_date" = DATE("criado_em")
WHERE "loan_advance_date" = CURRENT_DATE;
