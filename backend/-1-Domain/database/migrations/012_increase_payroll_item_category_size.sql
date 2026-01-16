-- Migration: Aumentar tamanho dos campos type e category em PayrollItem
-- Data: 2024-12-03
-- Descrição: Aumentar de varchar(10) para varchar(20) para comportar valores como "Adiantamento"

ALTER TABLE erp.tb_payroll_item 
ALTER COLUMN payroll_item_type TYPE varchar(20),
ALTER COLUMN payroll_item_category TYPE varchar(20);
