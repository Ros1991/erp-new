-- =====================================================
-- MIGRATION 005: Melhorias no Sistema de Folha de Pagamento
-- Data: 2024-11-17
-- Descrição: Adiciona campos para controle de fechamento, 
--            empréstimos, 13º salário, férias e histórico
-- =====================================================

-- =====================================================
-- 1. tb_payroll - Controle de Fechamento e Auditoria
-- =====================================================
ALTER TABLE "erp"."tb_payroll" 
    ADD COLUMN "payroll_closed_at" timestamptz NULL,
    ADD COLUMN "payroll_closed_by" bigint NULL,
    ADD COLUMN "payroll_notes" text NULL,
    ADD COLUMN "payroll_snapshot" jsonb NULL;

COMMENT ON COLUMN "erp"."tb_payroll"."payroll_closed_at" IS 'Data/hora de fechamento da folha';
COMMENT ON COLUMN "erp"."tb_payroll"."payroll_closed_by" IS 'ID do usuário que fechou a folha';
COMMENT ON COLUMN "erp"."tb_payroll"."payroll_notes" IS 'Observações gerais da folha de pagamento';
COMMENT ON COLUMN "erp"."tb_payroll"."payroll_snapshot" IS 'Snapshot JSON completo ao fechar (empregados + itens para auditoria e rollback)';

-- Índices para performance na listagem
CREATE INDEX "idx_payroll_company_period" 
    ON "erp"."tb_payroll"("company_id", "payroll_period_end_date" DESC);

CREATE INDEX "idx_payroll_company_status" 
    ON "erp"."tb_payroll"("company_id", "payroll_is_closed");

-- Foreign key para usuário que fechou
ALTER TABLE "erp"."tb_payroll"
    ADD CONSTRAINT "fk_payroll_closed_by"
    FOREIGN KEY ("payroll_closed_by") REFERENCES "erp"."tb_user"("user_id");

-- =====================================================
-- 2. tb_payroll_employee - Contrato, 13º e Férias
-- =====================================================
ALTER TABLE "erp"."tb_payroll_employee"
    -- Rastreabilidade do contrato
    ADD COLUMN "contract_id" bigint NULL,
    ADD COLUMN "payroll_employee_base_salary" decimal(10,2) DEFAULT 0 NOT NULL,
    
    -- Décimo terceiro salário
    ADD COLUMN "payroll_employee_has_13th" boolean DEFAULT false NOT NULL,
    ADD COLUMN "payroll_employee_13th_type" varchar(10) NULL,
    ADD COLUMN "payroll_employee_13th_amount" decimal(10,2) NULL,
    ADD COLUMN "payroll_employee_13th_tax_option" varchar(10) NULL,
    
    -- Férias
    ADD COLUMN "payroll_employee_vacation_advance_paid" boolean DEFAULT false NOT NULL,
    ADD COLUMN "payroll_employee_vacation_start_date" date NULL,
    ADD COLUMN "payroll_employee_vacation_end_date" date NULL,
    ADD COLUMN "payroll_employee_vacation_notes" text NULL,
    
    -- Observações
    ADD COLUMN "payroll_employee_notes" text NULL;

COMMENT ON COLUMN "erp"."tb_payroll_employee"."contract_id" IS 'Contrato que gerou esta linha (snapshot para auditoria)';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_base_salary" IS 'Snapshot do salário base do contrato naquele momento';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_has_13th" IS 'Indica se há cálculo de 13º salário nesta folha';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_13th_type" IS 'Tipo de 13º: full (integral), partial (parcial)';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_13th_amount" IS 'Valor do 13º salário calculado';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_13th_tax_option" IS 'Tributação do 13º: full (total), none (não lançar), partial (parcial)';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_vacation_advance_paid" IS 'Flag se adiantamento de férias foi pago nesta folha (evita duplicação)';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_vacation_start_date" IS 'Data de início das férias';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_vacation_end_date" IS 'Data de término das férias';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_vacation_notes" IS 'Observações sobre as férias';
COMMENT ON COLUMN "erp"."tb_payroll_employee"."payroll_employee_notes" IS 'Observações sobre este empregado nesta folha';

-- Foreign key para contrato
ALTER TABLE "erp"."tb_payroll_employee"
    ADD CONSTRAINT "fk_payroll_employee_contract"
    FOREIGN KEY ("contract_id") REFERENCES "erp"."tb_contract"("contract_id");

-- Índices
CREATE INDEX "idx_payroll_employee_contract" 
    ON "erp"."tb_payroll_employee"("contract_id");

CREATE INDEX "idx_payroll_employee_payroll" 
    ON "erp"."tb_payroll_employee"("payroll_id");

-- =====================================================
-- 3. tb_payroll_item - Controle Manual e Empréstimos
-- =====================================================
ALTER TABLE "erp"."tb_payroll_item"
    ADD COLUMN "payroll_item_is_manual" boolean DEFAULT false NOT NULL,
    ADD COLUMN "payroll_item_is_active" boolean DEFAULT true NOT NULL,
    ADD COLUMN "payroll_item_source_type" varchar(20) NULL,
    ADD COLUMN "payroll_item_installment_number" int NULL,
    ADD COLUMN "payroll_item_installment_total" int NULL;

COMMENT ON COLUMN "erp"."tb_payroll_item"."payroll_item_is_manual" IS 'Item criado/editado manualmente (não recalcular automaticamente)';
COMMENT ON COLUMN "erp"."tb_payroll_item"."payroll_item_is_active" IS 'Soft delete (false = item removido/cancelado)';
COMMENT ON COLUMN "erp"."tb_payroll_item"."payroll_item_source_type" IS 'Origem: contract_benefit, contract_discount, loan, tax_inss, tax_irrf, tax_fgts, 13th, vacation, manual';
COMMENT ON COLUMN "erp"."tb_payroll_item"."payroll_item_installment_number" IS 'Para empréstimos: número da parcela (ex: 3)';
COMMENT ON COLUMN "erp"."tb_payroll_item"."payroll_item_installment_total" IS 'Para empréstimos: total de parcelas (ex: 12)';

-- Índices
CREATE INDEX "idx_payroll_item_employee" 
    ON "erp"."tb_payroll_item"("payroll_employee_id");

CREATE INDEX "idx_payroll_item_reference" 
    ON "erp"."tb_payroll_item"("payroll_item_reference_id") 
    WHERE "payroll_item_reference_id" IS NOT NULL;

CREATE INDEX "idx_payroll_item_active" 
    ON "erp"."tb_payroll_item"("payroll_employee_id", "payroll_item_is_active");

CREATE INDEX "idx_payroll_item_loan" 
    ON "erp"."tb_payroll_item"("payroll_item_reference_id", "payroll_item_source_type", "payroll_item_is_active") 
    WHERE "payroll_item_source_type" = 'loan';

-- =====================================================
-- 4. tb_loan_advance - Campos Calculados
-- =====================================================
ALTER TABLE "erp"."tb_loan_advance"
    ADD COLUMN "loan_advance_installments_paid" int DEFAULT 0 NOT NULL,
    ADD COLUMN "loan_advance_remaining_amount" decimal(10,2) DEFAULT 0 NOT NULL,
    ADD COLUMN "loan_advance_is_fully_paid" boolean DEFAULT false NOT NULL;

COMMENT ON COLUMN "erp"."tb_loan_advance"."loan_advance_installments_paid" IS 'Contador de parcelas pagas (atualizado ao fechar folha)';
COMMENT ON COLUMN "erp"."tb_loan_advance"."loan_advance_remaining_amount" IS 'Valor restante a pagar (denormalizado para performance)';
COMMENT ON COLUMN "erp"."tb_loan_advance"."loan_advance_is_fully_paid" IS 'Flag de quitação (true quando installments_paid >= installments)';

-- Índice para buscar empréstimos pendentes
CREATE INDEX "idx_loan_advance_employee_pending" 
    ON "erp"."tb_loan_advance"("employee_id", "loan_advance_is_approved", "loan_advance_is_fully_paid");

-- Inicializar campos calculados nos empréstimos existentes
UPDATE "erp"."tb_loan_advance"
SET 
    "loan_advance_remaining_amount" = "loan_advance_amount",
    "loan_advance_installments_paid" = 0,
    "loan_advance_is_fully_paid" = false
WHERE "loan_advance_remaining_amount" IS NULL;

-- =====================================================
-- 5. tb_contract - Índice para Performance
-- =====================================================
CREATE INDEX "idx_contract_payroll_active" 
    ON "erp"."tb_contract"("employee_id", "contract_is_payroll", "contract_is_active") 
    WHERE "contract_is_payroll" = true AND "contract_is_active" = true;

COMMENT ON INDEX "erp"."idx_contract_payroll_active" IS 'Índice parcial para busca rápida de contratos ativos para folha';

-- =====================================================
-- FIM DA MIGRATION
-- =====================================================
