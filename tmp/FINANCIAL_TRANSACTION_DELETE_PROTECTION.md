# Proteção de Deleção em Transações Financeiras

## 🎯 Objetivo

Impedir a deleção direta de transações financeiras que foram criadas automaticamente por outros módulos, garantindo integridade dos dados.

## 🔒 Regras Implementadas

### **Transações Protegidas (NÃO podem ser deletadas diretamente):**

1. **Empréstimos/Adiantamentos** (`loan_advance_id != null`)
   - Criadas automaticamente ao criar empréstimo
   - **Solução:** Delete o empréstimo, a transação será deletada automaticamente (CASCADE)

2. **Contas a Pagar/Receber** (`account_payable_receivable_id != null`)
   - Criadas automaticamente ao pagar/receber conta
   - **Solução:** Delete a conta, a transação será deletada automaticamente (CASCADE)

3. **Pedidos de Compra** (`purchase_order_id != null`)
   - Criadas automaticamente ao processar pedido
   - **Solução:** Delete o pedido, a transação será deletada automaticamente (CASCADE)

### **Transações Liberadas (PODEM ser deletadas):**

- ✅ Transações manuais (sem associação)
- ✅ Transações com apenas `supplier_customer_id` (fornecedor/cliente)
- ✅ Qualquer transação sem os IDs protegidos acima

---

## 💻 Implementação

### **Arquivo:** `FinancialTransactionService.cs`

```csharp
public async Task<bool> DeleteByIdAsync(long financialTransactionId)
{
    // 1. Buscar transação
    var transaction = await _unitOfWork.FinancialTransactionRepository
        .GetOneByIdAsync(financialTransactionId);
    
    if (transaction == null)
        throw new ValidationException("...", "Transação não encontrada");

    // 2. Validar relacionamentos
    if (transaction.LoanAdvanceId.HasValue)
        throw new ValidationException("...", "Associada a Empréstimo");

    if (transaction.AccountPayableReceivableId.HasValue)
        throw new ValidationException("...", "Associada a Conta a Pagar/Receber");

    if (transaction.PurchaseOrderId.HasValue)
        throw new ValidationException("...", "Associada a Pedido de Compra");

    // 3. Se não tem relacionamento, permite deletar
    return await _unitOfWork.FinancialTransactionRepository
        .DeleteByIdAsync(financialTransactionId);
}
```

---

## 🔄 Fluxo de Deleção

### **Cenário 1: Transação Manual**
```
User tenta deletar transação manual
    ↓
Validação: Sem relacionamentos ✅
    ↓
Deleta normalmente
```

### **Cenário 2: Transação de Empréstimo**
```
User tenta deletar transação de empréstimo
    ↓
Validação: loan_advance_id = 5 ❌
    ↓
Retorna erro: "Delete o empréstimo primeiro"
    ↓
User deleta empréstimo ID 5
    ↓
CASCADE deleta transação automaticamente
```

### **Cenário 3: Transação de Conta a Pagar**
```
User tenta deletar transação de conta
    ↓
Validação: account_payable_receivable_id = 10 ❌
    ↓
Retorna erro: "Delete a conta primeiro"
    ↓
User deleta conta ID 10
    ↓
CASCADE deleta transação automaticamente
```

---

## 📋 Mensagens de Erro

| Relacionamento | Mensagem |
|---------------|----------|
| Empréstimo | "Não é possível deletar esta transação pois ela está associada a um Empréstimo/Adiantamento. Delete o empréstimo primeiro." |
| Conta a Pagar/Receber | "Não é possível deletar esta transação pois ela está associada a uma Conta a Pagar/Receber. Delete a conta primeiro." |
| Pedido de Compra | "Não é possível deletar esta transação pois ela está associada a um Pedido de Compra. Delete o pedido primeiro." |
| Não encontrada | "Transação financeira não encontrada." |

---

## 🗄️ Relacionamentos no Banco

### **Constraints CASCADE:**

```sql
-- Empréstimo → Transação
ALTER TABLE "erp"."tb_financial_transaction" 
ADD CONSTRAINT "fk_financial_transaction_loan_advance"
FOREIGN KEY ("loan_advance_id") 
REFERENCES "erp"."tb_loan_advance"("loan_advance_id") 
ON DELETE CASCADE;

-- Conta → Transação (presumido)
ALTER TABLE "erp"."tb_financial_transaction" 
ADD CONSTRAINT "fk_financial_transaction_account_payable_receivable"
FOREIGN KEY ("account_payable_receivable_id") 
REFERENCES "erp"."tb_account_payable_receivable"("account_payable_receivable_id") 
ON DELETE CASCADE;

-- Pedido → Transação (presumido)
ALTER TABLE "erp"."tb_financial_transaction" 
ADD CONSTRAINT "fk_financial_transaction_purchase_order"
FOREIGN KEY ("purchase_order_id") 
REFERENCES "erp"."tb_purchase_order"("purchase_order_id") 
ON DELETE CASCADE;
```

**Comportamento:**
- Deletar registro pai → Deleta transação automaticamente
- Tentar deletar transação → Bloqueada pela validação do Service

---

## ✅ Benefícios

1. **Integridade de Dados**
   - Transações criadas automaticamente não ficam órfãs
   - Histórico sempre consistente

2. **UX Melhor**
   - Mensagem clara do que fazer
   - Usuário entende o fluxo correto

3. **Auditoria**
   - Rastreabilidade mantida
   - Não perde vínculo entre registros

4. **Limpeza Automática**
   - CASCADE cuida da exclusão
   - Sem código manual de limpeza

---

## 🧪 Testes

### **Teste 1: Tentar deletar transação de empréstimo**
```
DELETE /api/financial-transactions/123
    ↓
Status: 400 Bad Request
Body: {
  "message": "Não é possível deletar esta transação pois ela está associada a um Empréstimo/Adiantamento. Delete o empréstimo primeiro."
}
```

### **Teste 2: Deletar empréstimo (CASCADE)**
```
DELETE /api/loan-advances/5
    ↓
Status: 200 OK
    ↓
Transação associada deletada automaticamente ✅
```

### **Teste 3: Deletar transação manual**
```
DELETE /api/financial-transactions/456
(sem loan_advance_id, account_payable_receivable_id, purchase_order_id)
    ↓
Status: 200 OK
Transação deletada ✅
```

---

## 📝 Considerações

1. **Transações Manuais**
   - Podem ser deletadas normalmente
   - Útil para correções

2. **Supplier/Customer**
   - `supplier_customer_id` NÃO bloqueia deleção
   - É apenas informação, não cria dependência

3. **Ordem de Exclusão**
   - **Sempre** deletar registro pai (empréstimo, conta, pedido)
   - **Nunca** tentar deletar transação diretamente se associada

4. **Frontend**
   - Considerar esconder botão "Deletar" em transações associadas
   - Ou mostrar mensagem explicativa antes da tentativa

---

## 🔧 Manutenção

Se adicionar novos tipos de transações automáticas no futuro:
1. Adicionar nova coluna FK na tabela `tb_financial_transaction`
2. Adicionar nova validação no `DeleteByIdAsync`
3. Garantir `ON DELETE CASCADE` no banco
4. Atualizar esta documentação

---

**Data:** 2025-11-14  
**Status:** ✅ Implementado  
**Arquivo:** `FinancialTransactionService.cs` linha 151-184
