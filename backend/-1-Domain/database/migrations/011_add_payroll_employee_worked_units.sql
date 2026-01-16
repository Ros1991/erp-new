-- Migration: Adicionar campo worked_units em PayrollEmployee
-- Data: 2024-12-03
-- Descrição: Campo para armazenar horas ou dias trabalhados de horistas/diaristas

-- Adicionar campo worked_units no PayrollEmployee
ALTER TABLE erp.tb_payroll_employee 
ADD COLUMN IF NOT EXISTS payroll_employee_worked_units decimal(10,2) NULL;

-- Comentário
COMMENT ON COLUMN erp.tb_payroll_employee.payroll_employee_worked_units IS 'Quantidade de horas ou dias trabalhados (para contratos de horistas e diaristas)';
