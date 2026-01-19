-- Migration: Atualizar data das transações financeiras de empréstimos
-- Descrição: Corrige a data das transações financeiras vinculadas a empréstimos,
--            usando a data do empréstimo (loan_advance_date) em vez da data de criação.

-- Atualizar transaction_date das transações que têm loan_advance_id
-- para usar a data do empréstimo correspondente
UPDATE "erp"."tb_financial_transaction" ft
SET "financial_transaction_transaction_date" = la."loan_advance_date",
    "atualizado_em" = CURRENT_TIMESTAMP
FROM "erp"."tb_loan_advance" la
WHERE ft."loan_advance_id" = la."loan_advance_id"
  AND la."loan_advance_date" IS NOT NULL
  AND ft."financial_transaction_transaction_date" <> la."loan_advance_date";
