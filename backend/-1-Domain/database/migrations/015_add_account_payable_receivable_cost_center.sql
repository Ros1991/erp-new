-- Migration: Criar tabela de distribuição de centro de custo para contas a pagar/receber
-- Data: 2026-01-18

-- Criar tabela de distribuição de centro de custo própria para AccountPayableReceivable
CREATE TABLE IF NOT EXISTS erp.tb_account_payable_receivable_cost_center (
    account_payable_receivable_cost_center_id BIGSERIAL PRIMARY KEY,
    account_payable_receivable_id BIGINT NOT NULL,
    cost_center_id BIGINT NOT NULL,
    account_payable_receivable_cost_center_amount BIGINT NOT NULL DEFAULT 0,
    account_payable_receivable_cost_center_percentage NUMERIC(5,2) NOT NULL DEFAULT 0,
    criado_por BIGINT NOT NULL,
    atualizado_por BIGINT,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP,
    
    CONSTRAINT fk_apr_cost_center_apr FOREIGN KEY (account_payable_receivable_id) 
        REFERENCES erp.tb_account_payable_receivable(account_payable_receivable_id) ON DELETE CASCADE,
    CONSTRAINT fk_apr_cost_center_cost_center FOREIGN KEY (cost_center_id) 
        REFERENCES erp.tb_cost_center(cost_center_id) ON DELETE RESTRICT,
    CONSTRAINT fk_apr_cost_center_criado_por FOREIGN KEY (criado_por) 
        REFERENCES erp.tb_user(user_id) ON DELETE RESTRICT,
    CONSTRAINT fk_apr_cost_center_atualizado_por FOREIGN KEY (atualizado_por) 
        REFERENCES erp.tb_user(user_id) ON DELETE RESTRICT
);

-- Índice para busca por conta a pagar/receber
CREATE INDEX IF NOT EXISTS idx_apr_cost_center_apr_id 
    ON erp.tb_account_payable_receivable_cost_center(account_payable_receivable_id);

-- Índice para busca por centro de custo
CREATE INDEX IF NOT EXISTS idx_apr_cost_center_cost_center_id 
    ON erp.tb_account_payable_receivable_cost_center(cost_center_id);

-- Comentários
COMMENT ON TABLE erp.tb_account_payable_receivable_cost_center IS 'Distribuição de centros de custo para contas a pagar/receber';
COMMENT ON COLUMN erp.tb_account_payable_receivable_cost_center.account_payable_receivable_cost_center_id IS 'ID único da distribuição';
COMMENT ON COLUMN erp.tb_account_payable_receivable_cost_center.account_payable_receivable_id IS 'Referência à conta a pagar/receber';
COMMENT ON COLUMN erp.tb_account_payable_receivable_cost_center.cost_center_id IS 'Referência ao centro de custo';
COMMENT ON COLUMN erp.tb_account_payable_receivable_cost_center.account_payable_receivable_cost_center_amount IS 'Valor em centavos';
COMMENT ON COLUMN erp.tb_account_payable_receivable_cost_center.account_payable_receivable_cost_center_percentage IS 'Porcentagem da distribuição';
