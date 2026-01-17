-- Migration: Adicionar campos de 13º salário na tabela de folha de pagamento
-- Data: 2026-01-17

-- Adicionar coluna para porcentagem do 13º aplicado
ALTER TABLE erp.tb_payroll 
ADD COLUMN IF NOT EXISTS payroll_thirteenth_percentage INTEGER NULL;

-- Adicionar coluna para opção de impostos do 13º
ALTER TABLE erp.tb_payroll 
ADD COLUMN IF NOT EXISTS payroll_thirteenth_tax_option VARCHAR(20) NULL;

-- Comentários nas colunas
COMMENT ON COLUMN erp.tb_payroll.payroll_thirteenth_percentage IS 'Porcentagem do 13º salário aplicado (0-100)';
COMMENT ON COLUMN erp.tb_payroll.payroll_thirteenth_tax_option IS 'Opção de impostos do 13º: none, proportional, full';
