-- Migration: Adicionar campos para fechamento de folha de pagamento
-- Data: 2026-01-19

-- Adicionar campos na tabela tb_payroll
ALTER TABLE erp.tb_payroll 
ADD COLUMN IF NOT EXISTS payroll_payment_date TIMESTAMP NULL,
ADD COLUMN IF NOT EXISTS payroll_account_id BIGINT NULL REFERENCES erp.tb_account(account_id),
ADD COLUMN IF NOT EXISTS payroll_inss_amount BIGINT NULL,
ADD COLUMN IF NOT EXISTS payroll_fgts_amount BIGINT NULL,
ADD COLUMN IF NOT EXISTS payroll_generated_transaction_ids TEXT NULL,
ADD COLUMN IF NOT EXISTS payroll_generated_loan_advance_ids TEXT NULL;

-- Modificar InstallmentsPaid para DECIMAL para suportar parcelas fracionadas (ex: 0.5 parcelas)
ALTER TABLE erp.tb_loan_advance 
ALTER COLUMN loan_advance_installments_paid TYPE DECIMAL(10,2) USING loan_advance_installments_paid::DECIMAL(10,2);

-- Comentários das colunas
COMMENT ON COLUMN erp.tb_payroll.payroll_payment_date IS 'Data do pagamento da folha';
COMMENT ON COLUMN erp.tb_payroll.payroll_account_id IS 'Conta corrente usada para pagamento';
COMMENT ON COLUMN erp.tb_payroll.payroll_inss_amount IS 'Valor do boleto INSS em centavos';
COMMENT ON COLUMN erp.tb_payroll.payroll_fgts_amount IS 'Valor do boleto FGTS em centavos';
COMMENT ON COLUMN erp.tb_payroll.payroll_generated_transaction_ids IS 'IDs das transações financeiras geradas ao fechar, separados por vírgula';
COMMENT ON COLUMN erp.tb_payroll.payroll_generated_loan_advance_ids IS 'IDs dos empréstimos gerados automaticamente (líquido negativo), separados por vírgula';
COMMENT ON COLUMN erp.tb_loan_advance.loan_advance_installments_paid IS 'Número de parcelas pagas (suporta frações, ex: 3.5 para meia parcela de 13º)';
