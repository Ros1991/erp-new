-- Migration 007: Convert long text codes to short tags
-- Created: 2024-11-19
-- Description: Convert DiscountSource and Application fields from long descriptions to short codes
-- This fixes the varchar(10) length issue

-- ================================================
-- 1. LOAN ADVANCE - Convert discount_source
-- ================================================

-- Backup current values (commented out, uncomment if you want to preserve)
-- ALTER TABLE erp.tb_loan_advance ADD COLUMN loan_advance_discount_source_backup varchar(255);
-- UPDATE erp.tb_loan_advance SET loan_advance_discount_source_backup = loan_advance_discount_source;

-- Convert long descriptions to short codes
UPDATE erp.tb_loan_advance
SET loan_advance_discount_source = CASE
    -- Todos variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%todos%' OR 
         LOWER(loan_advance_discount_source) LIKE '%tudo%' THEN 'TODOS'
    
    -- Salário variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%salário%' OR 
         LOWER(loan_advance_discount_source) LIKE '%salario%' THEN 'SALARIO'
    
    -- Décimo Terceiro variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%décimo%' OR 
         LOWER(loan_advance_discount_source) LIKE '%decimo%' OR
         LOWER(loan_advance_discount_source) LIKE '%13%' THEN '13SAL'
    
    -- Férias variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%férias%' OR 
         LOWER(loan_advance_discount_source) LIKE '%ferias%' THEN 'FERIAS'
    
    -- Anual variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%anual%' THEN 'ANUAL'
    
    -- Bônus variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%bônus%' OR 
         LOWER(loan_advance_discount_source) LIKE '%bonus%' THEN 'BONUS'
    
    -- Comissão variants
    WHEN LOWER(loan_advance_discount_source) LIKE '%comissão%' OR 
         LOWER(loan_advance_discount_source) LIKE '%comissao%' THEN 'COMISSAO'
    
    -- If longer than 10 chars, truncate
    WHEN LENGTH(loan_advance_discount_source) > 10 THEN LEFT(loan_advance_discount_source, 10)
    
    -- Otherwise keep as is (if already short)
    ELSE loan_advance_discount_source
END
WHERE LENGTH(loan_advance_discount_source) > 10 
   OR loan_advance_discount_source NOT IN ('TODOS', 'SALARIO', '13SAL', 'FERIAS', 'ANUAL', 'BONUS', 'COMISSAO');

-- ================================================
-- 2. CONTRACT BENEFIT DISCOUNT - Convert application
-- ================================================

-- Backup current values (commented out, uncomment if you want to preserve)
-- ALTER TABLE erp.tb_contract_benefit_discount ADD COLUMN contract_benefit_discount_application_backup varchar(255);
-- UPDATE erp.tb_contract_benefit_discount SET contract_benefit_discount_application_backup = contract_benefit_discount_application;

-- Convert long descriptions to short codes
UPDATE erp.tb_contract_benefit_discount
SET contract_benefit_discount_application = CASE
    -- Anual variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%anual%' THEN 'ANUAL'
    
    -- Todos / Tudo variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%todos%' OR 
         LOWER(contract_benefit_discount_application) LIKE '%tudo%' THEN 'TODOS'
    
    -- Salário variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%salário%' OR 
         LOWER(contract_benefit_discount_application) LIKE '%salario%' THEN 'SALARIO'
    
    -- Décimo Terceiro variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%décimo%' OR 
         LOWER(contract_benefit_discount_application) LIKE '%decimo%' OR
         LOWER(contract_benefit_discount_application) LIKE '%13%' THEN '13SAL'
    
    -- Férias variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%férias%' OR 
         LOWER(contract_benefit_discount_application) LIKE '%ferias%' THEN 'FERIAS'
    
    -- Bônus variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%bônus%' OR 
         LOWER(contract_benefit_discount_application) LIKE '%bonus%' THEN 'BONUS'
    
    -- Comissão variants
    WHEN LOWER(contract_benefit_discount_application) LIKE '%comissão%' OR 
         LOWER(contract_benefit_discount_application) LIKE '%comissao%' THEN 'COMISSAO'
    
    -- If longer than 10 chars, truncate
    WHEN LENGTH(contract_benefit_discount_application) > 10 THEN LEFT(contract_benefit_discount_application, 10)
    
    -- Otherwise keep as is (if already short)
    ELSE contract_benefit_discount_application
END
WHERE LENGTH(contract_benefit_discount_application) > 10 
   OR contract_benefit_discount_application NOT IN ('SALARIO', '13SAL', 'FERIAS', 'ANUAL', 'TODOS', 'BONUS', 'COMISSAO');

-- ================================================
-- 3. Verification queries
-- ================================================

-- Check if any values are still too long (should return 0 rows)
-- SELECT loan_advance_id, loan_advance_discount_source, LENGTH(loan_advance_discount_source) as len
-- FROM erp.tb_loan_advance
-- WHERE LENGTH(loan_advance_discount_source) > 10;

-- SELECT contract_benefit_discount_id, contract_benefit_discount_application, LENGTH(contract_benefit_discount_application) as len
-- FROM erp.tb_contract_benefit_discount
-- WHERE LENGTH(contract_benefit_discount_application) > 10;

-- Show distribution of converted codes
-- SELECT loan_advance_discount_source, COUNT(*) as count
-- FROM erp.tb_loan_advance
-- GROUP BY loan_advance_discount_source
-- ORDER BY count DESC;

-- SELECT contract_benefit_discount_application, COUNT(*) as count
-- FROM erp.tb_contract_benefit_discount
-- GROUP BY contract_benefit_discount_application
-- ORDER BY count DESC;

-- ================================================
-- Summary of code mappings:
-- ================================================
-- TODOS     = Todos / Tudo
-- SALARIO   = Salário / Salário Mensal
-- 13SAL     = Décimo Terceiro / Décimo Terceiro Salário
-- FERIAS    = Férias
-- ANUAL     = Anual (não usado na UI)
-- BONUS     = Bônus (não usado na UI)
-- COMISSAO  = Comissão (não usado na UI)
-- ================================================
