-- Migration: Corrigir empréstimos marcados incorretamente como quitados
-- Data: 2026-01-19

-- Corrigir isFullyPaid baseado APENAS no número de parcelas pagas
-- isFullyPaid = true SOMENTE quando installmentsPaid >= installments
UPDATE erp.tb_loan_advance
SET loan_advance_is_fully_paid = (loan_advance_installments_paid >= loan_advance_installments);

-- Verificar registros
SELECT 
    loan_advance_id,
    loan_advance_installments_paid,
    loan_advance_installments,
    loan_advance_remaining_amount,
    loan_advance_is_fully_paid,
    CASE WHEN loan_advance_installments_paid >= loan_advance_installments THEN 'Quitado' ELSE 'Em Aberto' END as status_correto
FROM erp.tb_loan_advance
ORDER BY loan_advance_is_fully_paid, loan_advance_installments_paid;
