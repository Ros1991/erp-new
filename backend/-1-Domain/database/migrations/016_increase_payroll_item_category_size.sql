-- Migration: Aumentar tamanho dos campos payroll_item_category e payroll_item_source_type
-- Data: 2026-01-18
-- Motivo: 
--   - payroll_item_category: valores como "Desconto Adiantamento" (21 chars)
--   - payroll_item_source_type: valores como "vacation_advance_salary" (23 chars)

ALTER TABLE erp.tb_payroll_item 
ALTER COLUMN payroll_item_category TYPE varchar(50);

ALTER TABLE erp.tb_payroll_item 
ALTER COLUMN payroll_item_source_type TYPE varchar(30);
