-- ================================================
-- Migration: Add description field to loan_advance
-- Description: Adds optional description field to tb_loan_advance
-- Author: System
-- Date: 2024-11-19
-- ================================================

-- Add description column to loan_advance table
ALTER TABLE erp.tb_loan_advance 
ADD COLUMN loan_advance_description VARCHAR(500) NULL;

-- Add comment to column
COMMENT ON COLUMN erp.tb_loan_advance.loan_advance_description IS 'Descrição adicional do empréstimo ou adiantamento (opcional)';

-- ================================================
-- Summary of Changes:
-- - Added loan_advance_description column (optional, max 500 chars)
-- ================================================
