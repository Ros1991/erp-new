-- Migration: Adicionar novos campos em Contract e ContractBenefitDiscount
-- Data: 2024-12-02

-- 1. Adicionar campos no Contract
-- has_thirteenth_salary: Se o empregado recebe décimo terceiro
-- has_vacation_bonus: Se o empregado recebe adicional de férias
ALTER TABLE erp.tb_contract 
ADD COLUMN IF NOT EXISTS contract_has_thirteenth_salary boolean DEFAULT true,
ADD COLUMN IF NOT EXISTS contract_has_vacation_bonus boolean DEFAULT true;

-- 2. Adicionar campos no ContractBenefitDiscount
-- month: Mês do benefício (1-12) para benefícios anuais
-- has_taxes: Se o benefício tem impostos
-- is_proportional: Se o benefício pode ser proporcional
ALTER TABLE erp.tb_contract_benefit_discount 
ADD COLUMN IF NOT EXISTS contract_benefit_discount_month integer NULL,
ADD COLUMN IF NOT EXISTS contract_benefit_discount_has_taxes boolean DEFAULT false,
ADD COLUMN IF NOT EXISTS contract_benefit_discount_is_proportional boolean DEFAULT true;

-- Comentários
COMMENT ON COLUMN erp.tb_contract.contract_has_thirteenth_salary IS 'Indica se o empregado recebe décimo terceiro salário';
COMMENT ON COLUMN erp.tb_contract.contract_has_vacation_bonus IS 'Indica se o empregado recebe adicional de férias (1/3)';
COMMENT ON COLUMN erp.tb_contract_benefit_discount.contract_benefit_discount_month IS 'Mês do benefício (1-12) para benefícios anuais';
COMMENT ON COLUMN erp.tb_contract_benefit_discount.contract_benefit_discount_has_taxes IS 'Indica se o benefício tem incidência de impostos';
COMMENT ON COLUMN erp.tb_contract_benefit_discount.contract_benefit_discount_is_proportional IS 'Indica se o benefício pode ser calculado proporcionalmente';
