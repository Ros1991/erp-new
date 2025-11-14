# Integração de Empréstimos com Transações Financeiras

## 📋 Resumo

Implementação completa da funcionalidade de criação automática de transações financeiras ao criar empréstimos/adiantamentos, com rastreabilidade e exclusão em cascata.

## 🎯 Objetivos Alcançados

1. ✅ Adicionar campo `loan_advance_id` na tabela `tb_financial_transaction`
2. ✅ Criar script de migração para bancos existentes
3. ✅ Atualizar script principal (`erp.sql`)
4. ✅ Implementar criação automática de transação ao criar empréstimo
5. ✅ Garantir exclusão em cascata (deletar empréstimo = deletar transação)
6. ✅ Adicionar campos de conta e centros de custo ao formulário de empréstimo
7. ✅ Busca automática de centros de custo do contrato ativo do funcionário

## 📂 Arquivos Modificados/Criados

### Backend - Banco de Dados

**Criados:**
- `backend/-1-Domain/database/migrations/001_add_loan_advance_id_to_financial_transaction.sql`
- `backend/-1-Domain/database/migrations/README.md`

**Modificados:**
- `backend/-1-Domain/database/erp.sql` - Adicionado coluna e FK

### Backend - Domínio

**Modificados:**
- `backend/-1-Domain/Entities/financialTransaction.cs`
  - Adicionada propriedade `LoanAdvanceId`
  - Adicionada relação `LoanAdvance`
  - Atualizado construtor

### Backend - Application Layer

**Modificados:**
- `backend/-2-Application/DTOs/LoanAdvance/LoanAdvanceInputDTO.cs`
  - Adicionado `AccountId` (obrigatório)
  - Adicionado `CostCenterDistributions` (opcional)

- `backend/-2-Application/DTOs/FinancialTransaction/FinancialTransactionOutputDTO.cs`
  - Adicionado `LoanAdvanceId`

- `backend/-2-Application/Mappers/FinancialTransactionMapper.cs`
  - Atualizado `ToFinancialTransactionOutputDTO` para incluir `LoanAdvanceId`
  - Atualizado `ToEntity` para aceitar `LoanAdvanceId` (null por padrão)

- `backend/-2-Application/Services/LoanAdvanceService.cs`
  - Implementada criação automática de `FinancialTransaction`
  - Vinculação da transação ao empréstimo via `LoanAdvanceId`
  - Criação automática de `TransactionCostCenter`
  - Validação de centros de custo (soma = 100%)

### Frontend

**Modificados:**
- `frontend/src/services/loanAdvanceService.ts`
  - Atualizada interface para incluir `accountId` e `costCenterDistributions`

- `frontend/src/pages/loan-advances/LoanAdvanceForm.tsx`
  - Adicionado campo de seleção de conta (EntityPicker)
  - Adicionado card de centros de custo (CostCenterDistribution)
  - Implementada busca automática de centros de custo do contrato ativo
  - Validações de conta e centros de custo
  - Conversões centavos ↔ reais

- `frontend/src/pages/loan-advances/LoanAdvances.tsx`
  - Removida coluna "Aprovado" (não utilizada)

## 🗄️ Estrutura do Banco de Dados

### Tabela: tb_financial_transaction

```sql
CREATE TABLE "erp"."tb_financial_transaction"(
    -- ... campos existentes ...
    "loan_advance_id" bigint NULL,
    -- ... demais campos ...
);

-- Foreign Key com CASCADE
ALTER TABLE "erp"."tb_financial_transaction" 
ADD CONSTRAINT "fk_financial_transaction_loan_advance"
FOREIGN KEY ("loan_advance_id") 
REFERENCES "erp"."tb_loan_advance"("loan_advance_id") 
ON DELETE CASCADE;
```

### Comportamento ON DELETE CASCADE

Quando um registro de `tb_loan_advance` é deletado:
- O PostgreSQL **automaticamente deleta** a transação financeira relacionada
- Não é necessário código adicional no Service
- Garante integridade referencial

## 🔄 Fluxo de Criação de Empréstimo

```
1. Usuário preenche formulário
   ├─ Seleciona Funcionário
   │  └─ Sistema busca contrato ativo
   │     └─ Carrega centros de custo automaticamente
   ├─ Seleciona Conta (obrigatório)
   ├─ Preenche Valor e Parcelas
   ├─ Revisa Centros de Custo (editável)
   └─ Submete

2. Backend (LoanAdvanceService.CreateAsync)
   ├─ Valida dados do empréstimo
   ├─ Valida centros de custo (soma = 100%)
   ├─ Cria registro em tb_loan_advance
   ├─ Busca nome do funcionário
   ├─ Cria FinancialTransaction
   │  ├─ Tipo: "Saída"
   │  ├─ Descrição: "Empréstimo/Adiantamento - [Nome]"
   │  ├─ LoanAdvanceId: [ID do empréstimo criado]
   │  └─ AccountId, Amount, TransactionDate
   └─ Cria TransactionCostCenter (se houver)
      └─ Vincula centros de custo à transação

3. Resultado
   ├─ Empréstimo criado
   ├─ Transação financeira criada e vinculada
   └─ Centros de custo distribuídos
```

## 🚀 Como Aplicar a Migração

### Para Bancos Existentes

Execute o script de migração:

```bash
psql -U seu_usuario -d erp_database -f backend/-1-Domain/database/migrations/001_add_loan_advance_id_to_financial_transaction.sql
```

### Para Novos Bancos

Use o script principal que já contém a atualização:

```bash
psql -U seu_usuario -d erp_database -f backend/-1-Domain/database/erp.sql
```

## 🧪 Testes Recomendados

1. **Criar empréstimo com funcionário que tem contrato ativo**
   - Verificar se centros de custo carregam automaticamente
   - Verificar criação da transação financeira
   - Verificar distribuição dos centros de custo

2. **Criar empréstimo com funcionário sem contrato**
   - Verificar possibilidade de adicionar centros manualmente
   - Verificar criação da transação

3. **Deletar empréstimo**
   - Verificar se transação financeira é deletada automaticamente
   - Verificar se centros de custo são deletados

4. **Validações**
   - Tentar criar sem conta: deve falhar
   - Tentar criar com centros de custo != 100%: deve falhar
   - Tentar criar com centro de custo vazio: deve falhar

## 📊 Exemplos de Dados

### Empréstimo Criado

```json
{
  "employeeId": 2,
  "accountId": 1,
  "amount": 50000, // R$ 500,00 em centavos
  "installments": 5,
  "discountSource": "Mensal",
  "startDate": "2025-12-01T00:00:00Z",
  "costCenterDistributions": [
    { "costCenterId": 1, "percentage": 60, "amount": 30000 },
    { "costCenterId": 2, "percentage": 40, "amount": 20000 }
  ]
}
```

### Transação Financeira Gerada

```json
{
  "companyId": 1,
  "accountId": 1,
  "loanAdvanceId": 1, // ← VINCULADO!
  "description": "Empréstimo/Adiantamento - Rodrigo Oliveira",
  "type": "Saída",
  "amount": 50000,
  "transactionDate": "2025-12-01T00:00:00Z",
  "costCenterDistributions": [
    { "costCenterId": 1, "percentage": 60, "amount": 30000 },
    { "costCenterId": 2, "percentage": 40, "amount": 20000 }
  ]
}
```

## 🎨 UX Highlights

1. **Busca Inteligente de Centros de Custo**
   - Ao selecionar funcionário, busca contrato ativo
   - Carrega centros de custo automaticamente
   - Feedback visual: "✓ Centros de custo carregados..."

2. **Seleção de Conta**
   - EntityPicker com busca e paginação
   - Mostra tipo da conta como informação secundária

3. **Distribuição de Centros de Custo**
   - Componente reutilizado (CostCenterDistribution)
   - Cálculo automático de valores por porcentagem
   - Validação em tempo real

4. **Conversão Automática**
   - Frontend trabalha em centavos
   - Backend trabalha em reais
   - Conversões automáticas e transparentes

## 🔐 Segurança

- ✅ Validação de permissões (loanAdvance.canCreate)
- ✅ Validação de dados obrigatórios
- ✅ Validação de soma de percentuais
- ✅ Uso de DateTimeHelper.ToUtc() para datas
- ✅ Foreign Key com CASCADE para integridade

## 📝 Notas Importantes

1. **ON DELETE CASCADE**: A exclusão é automática via FK, não é necessário código adicional
2. **LoanAdvanceId nullable**: Nem toda transação está relacionada a um empréstimo
3. **Centros de Custo opcionais**: Sistema permite criar empréstimo sem centros de custo
4. **Busca automática**: Facilita UX mas não obriga uso dos centros do contrato

## 🐛 Troubleshooting

**Problema**: Transação não é deletada ao deletar empréstimo
- Verificar se a FK foi criada com `ON DELETE CASCADE`
- Verificar logs do PostgreSQL

**Problema**: Centros de custo não carregam automaticamente
- Verificar se funcionário tem contrato ativo
- Verificar se contrato tem centros de custo configurados
- Verificar console do navegador para erros

**Problema**: Erro ao criar empréstimo
- Verificar se conta existe
- Verificar soma de percentuais dos centros de custo
- Verificar formato das datas (UTC)

## ✅ Checklist de Implementação

- [x] Script de migração criado
- [x] Script principal atualizado
- [x] Entidade FinancialTransaction atualizada
- [x] DTOs atualizados
- [x] Mappers atualizados
- [x] LoanAdvanceService implementado
- [x] Frontend - interface atualizada
- [x] Frontend - validações implementadas
- [x] Frontend - busca automática de centros de custo
- [x] Documentação criada
- [x] ON DELETE CASCADE configurado

---

**Data de Implementação:** 2025-11-14  
**Versão:** 1.0  
**Status:** ✅ Completo
