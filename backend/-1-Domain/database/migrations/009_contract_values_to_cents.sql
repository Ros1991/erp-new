-- ================================================
-- Migration: Convert contract values from reais to centavos
-- Description: Standardizes monetary values to be stored as cents (bigint)
-- Author: System
-- Date: 2024-12-01
-- ================================================

-- 1. Convert contract value from reais to centavos
UPDATE erp.tb_contract 
SET contract_value = contract_value * 100 
WHERE contract_value IS NOT NULL;

-- 2. Convert benefit/discount amounts from reais to centavos
UPDATE erp.tb_contract_benefit_discount 
SET contract_benefit_discount_amount = contract_benefit_discount_amount * 100 
WHERE contract_benefit_discount_amount IS NOT NULL;

-- 3. Change column types from decimal to bigint

-- Contract value
ALTER TABLE erp.tb_contract 
ALTER COLUMN contract_value TYPE bigint USING contract_value::bigint;

-- Contract benefit/discount amount
ALTER TABLE erp.tb_contract_benefit_discount 
ALTER COLUMN contract_benefit_discount_amount TYPE bigint USING contract_benefit_discount_amount::bigint;

-- ================================================
-- Summary of Changes:
-- - tb_contract.contract_value: decimal -> bigint (stored in cents)
-- - tb_contract_benefit_discount.contract_benefit_discount_amount: decimal -> bigint (stored in cents)
-- ================================================
