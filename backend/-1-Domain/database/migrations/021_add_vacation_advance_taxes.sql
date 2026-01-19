-- Migration: Adicionar campos de impostos adiantados nas férias
-- Data: 2026-01-19
-- Descrição: Armazena INSS e IRRF adiantados nas férias para descontar na folha seguinte

-- Adicionar coluna para INSS adiantado nas férias
ALTER TABLE "erp"."tb_payroll_employee" 
ADD COLUMN IF NOT EXISTS "payroll_employee_vacation_advance_inss" bigint DEFAULT 0;

-- Adicionar coluna para IRRF adiantado nas férias
ALTER TABLE "erp"."tb_payroll_employee" 
ADD COLUMN IF NOT EXISTS "payroll_employee_vacation_advance_irrf" bigint DEFAULT 0;

-- Adicionar coluna para indicar que benefícios/descontos foram adiantados
ALTER TABLE "erp"."tb_payroll_employee" 
ADD COLUMN IF NOT EXISTS "payroll_employee_vacation_advance_benefits" boolean DEFAULT false;
