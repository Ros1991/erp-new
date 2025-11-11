/*------------------------------------------------------------*/
/*   Migração de TIMESTAMP para TIMESTAMPTZ                   */
/*   Converte todos os campos de data para usar timezone      */
/*   IMPORTANTE: Execute este script DEPOIS de fix_datetime_fields.sql */
/*------------------------------------------------------------*/

-- Configurar banco para UTC
ALTER DATABASE current_database() SET timezone TO 'UTC';

-- tb_task_employee
ALTER TABLE "erp"."tb_task_employee" 
    ALTER COLUMN "task_employee_start_date" TYPE TIMESTAMPTZ 
    USING "task_employee_start_date" AT TIME ZONE 'UTC';

ALTER TABLE "erp"."tb_task_employee" 
    ALTER COLUMN "task_employee_end_date" TYPE TIMESTAMPTZ 
    USING "task_employee_end_date" AT TIME ZONE 'UTC';

ALTER TABLE "erp"."tb_task_employee" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ 
    USING "criado_em" AT TIME ZONE 'UTC';

ALTER TABLE "erp"."tb_task_employee" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ 
    USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_time_entry
ALTER TABLE "erp"."tb_time_entry" 
    ALTER COLUMN "time_entry_timestamp" TYPE TIMESTAMPTZ 
    USING "time_entry_timestamp" AT TIME ZONE 'UTC';

ALTER TABLE "erp"."tb_time_entry" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ 
    USING "criado_em" AT TIME ZONE 'UTC';

ALTER TABLE "erp"."tb_time_entry" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ 
    USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_user
ALTER TABLE "erp"."tb_user" 
    ALTER COLUMN "user_reset_token_expires_at" TYPE TIMESTAMPTZ 
    USING "user_reset_token_expires_at" AT TIME ZONE 'UTC';

-- tb_user_token
ALTER TABLE "erp"."tb_user_token" 
    ALTER COLUMN "user_token_expires_at" TYPE TIMESTAMPTZ 
    USING "user_token_expires_at" AT TIME ZONE 'UTC';

ALTER TABLE "erp"."tb_user_token" 
    ALTER COLUMN "user_token_refresh_expires_at" TYPE TIMESTAMPTZ 
    USING "user_token_refresh_expires_at" AT TIME ZONE 'UTC';

-- Campos de auditoria em todas as tabelas
-- tb_account
ALTER TABLE "erp"."tb_account" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_account" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_account_payable_receivable
ALTER TABLE "erp"."tb_account_payable_receivable" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_account_payable_receivable" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_company
ALTER TABLE "erp"."tb_company" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_company" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_company_setting
ALTER TABLE "erp"."tb_company_setting" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_company_setting" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_company_user
ALTER TABLE "erp"."tb_company_user" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_company_user" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_contract
ALTER TABLE "erp"."tb_contract" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_contract" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_contract_benefit_discount
ALTER TABLE "erp"."tb_contract_benefit_discount" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_contract_benefit_discount" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_contract_cost_center
ALTER TABLE "erp"."tb_contract_cost_center" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_contract_cost_center" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_cost_center
ALTER TABLE "erp"."tb_cost_center" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_cost_center" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_employee
ALTER TABLE "erp"."tb_employee" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_employee" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_employee_allowed_location
ALTER TABLE "erp"."tb_employee_allowed_location" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_employee_allowed_location" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_financial_transaction
ALTER TABLE "erp"."tb_financial_transaction" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_financial_transaction" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_justification
ALTER TABLE "erp"."tb_justification" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_justification" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_loan_advance
ALTER TABLE "erp"."tb_loan_advance" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_loan_advance" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_location
ALTER TABLE "erp"."tb_location" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_location" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_payroll
ALTER TABLE "erp"."tb_payroll" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_payroll" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_payroll_employee
ALTER TABLE "erp"."tb_payroll_employee" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_payroll_employee" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_payroll_item
ALTER TABLE "erp"."tb_payroll_item" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_payroll_item" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_purchase_order
ALTER TABLE "erp"."tb_purchase_order" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_purchase_order" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_role
ALTER TABLE "erp"."tb_role" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_role" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_task
ALTER TABLE "erp"."tb_task" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_task" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_task_comment
ALTER TABLE "erp"."tb_task_comment" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_task_comment" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_task_status_history
ALTER TABLE "erp"."tb_task_status_history" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_task_status_history" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- tb_transaction_cost_center
ALTER TABLE "erp"."tb_transaction_cost_center" 
    ALTER COLUMN "criado_em" TYPE TIMESTAMPTZ USING "criado_em" AT TIME ZONE 'UTC';
ALTER TABLE "erp"."tb_transaction_cost_center" 
    ALTER COLUMN "atualizado_em" TYPE TIMESTAMPTZ USING "atualizado_em" AT TIME ZONE 'UTC';

-- Verificar conversões
SELECT 
    table_name, 
    column_name, 
    data_type 
FROM information_schema.columns 
WHERE table_schema = 'erp' 
    AND (column_name LIKE '%em' OR column_name LIKE '%expires%' OR column_name LIKE '%date%' OR column_name LIKE '%timestamp%')
    AND data_type IN ('timestamp with time zone', 'timestamp without time zone')
ORDER BY table_name, column_name;
