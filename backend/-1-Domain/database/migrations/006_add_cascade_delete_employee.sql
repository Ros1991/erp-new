-- Migration 006: Add CASCADE DELETE for Employee foreign keys
-- Created: 2024-11-17
-- Description: Configure CASCADE DELETE on all tables that reference tb_employee
-- This allows automatic deletion of related records when an employee is deleted

-- ================================================
-- 1. DROP existing constraints
-- ================================================

-- Contract
ALTER TABLE erp.tb_contract 
DROP CONSTRAINT IF EXISTS fk_contract_employee;

-- Employee Allowed Location
ALTER TABLE erp.tb_employee_allowed_location 
DROP CONSTRAINT IF EXISTS fk_employee_allowed_location_employee;

-- Task Employee
ALTER TABLE erp.tb_task_employee 
DROP CONSTRAINT IF EXISTS fk_task_employee_employee;

-- Time Entry
ALTER TABLE erp.tb_time_entry 
DROP CONSTRAINT IF EXISTS fk_time_entry_employee;

-- Loan Advance
ALTER TABLE erp.tb_loan_advance 
DROP CONSTRAINT IF EXISTS fk_loan_advance_employee;

-- Payroll Employee
ALTER TABLE erp.tb_payroll_employee 
DROP CONSTRAINT IF EXISTS fk_payroll_employee_employee;

-- Justification
ALTER TABLE erp.tb_justification 
DROP CONSTRAINT IF EXISTS fk_justification_employee;

-- Company Setting (General Manager)
ALTER TABLE erp.tb_company_setting 
DROP CONSTRAINT IF EXISTS fk_company_setting_general_manager;

-- Employee (Self-referencing Manager)
ALTER TABLE erp.tb_employee 
DROP CONSTRAINT IF EXISTS fk_employee_manager;

-- Contract Benefit Discount (child of Contract)
ALTER TABLE erp.tb_contract_benefit_discount 
DROP CONSTRAINT IF EXISTS fk_contract_benefit_discount_contract;

-- Contract Cost Center (child of Contract)
ALTER TABLE erp.tb_contract_cost_center 
DROP CONSTRAINT IF EXISTS fk_contract_cost_center_contract;

-- Payroll Item (child of PayrollEmployee)
ALTER TABLE erp.tb_payroll_item 
DROP CONSTRAINT IF EXISTS fk_payroll_item_payroll_employee;

-- Task Status History (child of TaskEmployee)
ALTER TABLE erp.tb_task_status_history 
DROP CONSTRAINT IF EXISTS fk_task_status_history_task_employee;

-- ================================================
-- 2. ADD constraints with CASCADE DELETE
-- ================================================

-- Contract -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_contract 
ADD CONSTRAINT fk_contract_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Employee Allowed Location -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_employee_allowed_location 
ADD CONSTRAINT fk_employee_allowed_location_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Task Employee -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_task_employee 
ADD CONSTRAINT fk_task_employee_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Time Entry -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_time_entry 
ADD CONSTRAINT fk_time_entry_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Loan Advance -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_loan_advance 
ADD CONSTRAINT fk_loan_advance_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Payroll Employee -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_payroll_employee 
ADD CONSTRAINT fk_payroll_employee_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Justification -> Employee (CASCADE DELETE)
ALTER TABLE erp.tb_justification 
ADD CONSTRAINT fk_justification_employee 
FOREIGN KEY (employee_id) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE CASCADE;

-- Company Setting -> Employee General Manager (SET NULL on delete)
-- Note: Using SET NULL because general manager is optional
ALTER TABLE erp.tb_company_setting 
ADD CONSTRAINT fk_company_setting_general_manager 
FOREIGN KEY (employee_id_general_manager) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE SET NULL;

-- Employee -> Employee Manager (SET NULL on delete)
-- Note: Using SET NULL because manager is optional
ALTER TABLE erp.tb_employee 
ADD CONSTRAINT fk_employee_manager 
FOREIGN KEY (employee_id_manager) 
REFERENCES erp.tb_employee(employee_id) 
ON DELETE SET NULL;

-- ================================================
-- 3. ADD CASCADE DELETE for child tables
-- ================================================

-- Contract Benefit Discount -> Contract (CASCADE DELETE)
ALTER TABLE erp.tb_contract_benefit_discount 
ADD CONSTRAINT fk_contract_benefit_discount_contract 
FOREIGN KEY (contract_id) 
REFERENCES erp.tb_contract(contract_id) 
ON DELETE CASCADE;

-- Contract Cost Center -> Contract (CASCADE DELETE)
ALTER TABLE erp.tb_contract_cost_center 
ADD CONSTRAINT fk_contract_cost_center_contract 
FOREIGN KEY (contract_id) 
REFERENCES erp.tb_contract(contract_id) 
ON DELETE CASCADE;

-- Payroll Item -> Payroll Employee (CASCADE DELETE)
ALTER TABLE erp.tb_payroll_item 
ADD CONSTRAINT fk_payroll_item_payroll_employee 
FOREIGN KEY (payroll_employee_id) 
REFERENCES erp.tb_payroll_employee(payroll_employee_id) 
ON DELETE CASCADE;

-- Task Status History -> Task Employee (CASCADE DELETE)
ALTER TABLE erp.tb_task_status_history 
ADD CONSTRAINT fk_task_status_history_task_employee 
FOREIGN KEY (task_employee_id) 
REFERENCES erp.tb_task_employee(task_employee_id) 
ON DELETE CASCADE;

-- ================================================
-- Summary of CASCADE behavior:
-- ================================================
-- When an Employee is deleted, the following will be CASCADE DELETED:
-- - All Contracts (tb_contract)
--   ├─ Contract Benefits/Discounts (tb_contract_benefit_discount)
--   └─ Contract Cost Centers (tb_contract_cost_center)
-- - All Employee Allowed Locations (tb_employee_allowed_location)
-- - All Task Employees (tb_task_employee)
--   └─ Task Status History (tb_task_status_history)
-- - All Time Entries (tb_time_entry)
-- - All Loan Advances (tb_loan_advance)
--   └─ Financial Transactions (tb_financial_transaction) - already configured
-- - All Payroll Employees (tb_payroll_employee)
--   └─ Payroll Items (tb_payroll_item)
-- - All Justifications (tb_justification)
--
-- The following will be SET TO NULL:
-- - Company Setting general_manager reference
-- - Employee manager reference (self-referencing)
-- ================================================
