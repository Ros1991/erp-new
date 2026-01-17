-- Migration: Adicionar campos INSS e FGTS na tabela de folha de pagamento
-- Data: 2026-01-16

-- Adicionar coluna para total de INSS a recolher
ALTER TABLE erp.tb_payroll 
ADD COLUMN IF NOT EXISTS payroll_total_inss BIGINT NOT NULL DEFAULT 0;

-- Adicionar coluna para total de FGTS a recolher
ALTER TABLE erp.tb_payroll 
ADD COLUMN IF NOT EXISTS payroll_total_fgts BIGINT NOT NULL DEFAULT 0;

-- Comentários nas colunas
COMMENT ON COLUMN erp.tb_payroll.payroll_total_inss IS 'Total de INSS a recolher em centavos';
COMMENT ON COLUMN erp.tb_payroll.payroll_total_fgts IS 'Total de FGTS a recolher em centavos (8% do salário bruto dos funcionários com FGTS)';
