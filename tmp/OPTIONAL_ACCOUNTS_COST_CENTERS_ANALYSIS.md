# Análise: Contas e Centros de Custo Opcionais

## 🎯 Objetivo

Tornar o sistema flexível para pequenas empresas que possuem:
- **0 contas** ou **1 conta** apenas
- **0 centros de custo** ou **1 centro de custo** apenas

## 📊 Comportamento Desejado

### **Regras de UX:**

| Quantidade | Comportamento |
|------------|---------------|
| **0 itens** | Não mostrar campo/card, salvar NULL |
| **1 item** | Selecionar automaticamente, não mostrar campo (ou mostrar desabilitado) |
| **2+ itens** | Mostrar campo normalmente para seleção |

---

## 🗄️ Banco de Dados - Mudanças Necessárias

### **Tabelas que precisam aceitar NULL:**

#### **1. tb_financial_transaction**
```sql
-- ANTES
"account_id" bigint DEFAULT 0 NOT NULL

-- DEPOIS
"account_id" bigint NULL  -- Remove DEFAULT 0 e NOT NULL
```

**Impacto:**
- ✅ Transações sem conta específica
- ✅ Pequenas empresas sem contas cadastradas

#### **2. tb_contract_cost_center**
```sql
-- ANTES
"cost_center_id" bigint DEFAULT 0 NOT NULL

-- DEPOIS
"cost_center_id" bigint NULL
```

**Impacto:**
- ✅ Contratos sem centro de custo específico
- ⚠️ Verificar UNIQUE constraint (contract_id, cost_center_id)
- ⚠️ Se cost_center_id for NULL, permitir múltiplos registros?
- 💡 **DECISÃO:** Talvez essa tabela deva continuar obrigatória
  - Se não tem centro de custo, simplesmente não cria registro nesta tabela

#### **3. tb_transaction_cost_center**
```sql
-- ANTES
"cost_center_id" bigint DEFAULT 0 NOT NULL

-- DEPOIS
"cost_center_id" bigint NULL
```

**Impacto:**
- ✅ Transações sem centro de custo
- ⚠️ Verificar UNIQUE constraint (financial_transaction_id, cost_center_id)
- 💡 **DECISÃO:** Se não tem centro de custo, não cria registro nesta tabela

---

## 🔧 Recomendações de Implementação

### **Abordagem 1: NULL nos campos FK (mais simples)**

**tb_financial_transaction:**
- ✅ account_id pode ser NULL
- ✅ Se NULL, transação sem conta específica

**tb_transaction_cost_center:**
- ❌ NÃO deixar cost_center_id NULL
- ✅ Se não tem centro de custo, **não cria registro**
- ✅ Transação pode ter 0, 1 ou N centros de custo

**tb_contract_cost_center:**
- ❌ NÃO deixar cost_center_id NULL
- ✅ Se não tem centro de custo, **não cria registro**
- ✅ Contrato pode ter 0, 1 ou N centros de custo

### **Abordagem 2: Tabelas de relação sempre opcionais**

As tabelas de junção (`tb_transaction_cost_center`, `tb_contract_cost_center`) são **relacionamentos N-N**.

Se não há centro de custo:
- Simplesmente **não criar registros** nessas tabelas
- Deixar vazio (0 registros) = sem centro de custo

**CONCLUSÃO:** Abordagem 2 é mais limpa! ✅

---

## 📋 Migration Necessária

### **Migration 003: Tornar account_id opcional**

```sql
-- tb_financial_transaction
ALTER TABLE "erp"."tb_financial_transaction" 
ALTER COLUMN "account_id" DROP NOT NULL;

ALTER TABLE "erp"."tb_financial_transaction" 
ALTER COLUMN "account_id" DROP DEFAULT;
```

**Observação:** 
- tb_contract_cost_center e tb_transaction_cost_center **NÃO precisam de mudança**
- Basta não criar registros quando não houver centros de custo

---

## 🎨 Frontend - Telas para Ajustar

### **1. LoanAdvanceForm.tsx**

**Campos afetados:**
- ✅ Conta (accountId) - mostrar/ocultar
- ✅ Centros de Custo (costCenters) - mostrar/ocultar

**Lógica:**
```typescript
// Ao montar componente:
const accounts = await accountService.getAccounts();
const costCenters = await costCenterService.getCostCenters();

// Auto-seleção
if (accounts.length === 0) {
  // Não mostrar campo, accountId = null
  setShowAccountField(false);
  setFormData(prev => ({ ...prev, accountId: '' }));
}
else if (accounts.length === 1) {
  // Auto-selecionar, campo readonly ou oculto
  setFormData(prev => ({ 
    ...prev, 
    accountId: accounts[0].id,
    accountName: accounts[0].name 
  }));
  setShowAccountField(false); // ou mostrar desabilitado
}
else {
  // Mostrar campo normalmente
  setShowAccountField(true);
}

// Mesma lógica para costCenters
```

### **2. FinancialTransactionForm.tsx**

**Campos afetados:**
- ✅ Conta (accountId)
- ✅ Centros de Custo (costCenterDistributions)

**Lógica:** Idêntica ao LoanAdvanceForm

### **3. ContractForm.tsx**

**Campos afetados:**
- ✅ Centros de Custo (costCenterDistributions)

**Lógica:** Igual aos anteriores

---

## 🔨 Backend - Ajustes Necessários

### **DTOs - Remover [Required]**

#### **LoanAdvanceInputDTO.cs**
```csharp
// ANTES
[Required(ErrorMessage = "AccountId é obrigatório")]
public long AccountId { get; set; }

// DEPOIS
public long? AccountId { get; set; }  // Opcional
```

#### **FinancialTransactionInputDTO.cs**
```csharp
// ANTES
[Required(ErrorMessage = "Conta é obrigatória")]
public long AccountId { get; set; }

// DEPOIS
public long? AccountId { get; set; }  // Opcional
```

#### **CostCenterDistributionDTO.cs**

Este DTO está correto, mas validar:
```csharp
public List<CostCenterDistributionDTO>? CostCenterDistributions { get; set; }
```

### **Services - Validações**

#### **LoanAdvanceService.CreateAsync**

```csharp
// ANTES
// Validação de centros de custo: soma = 100%

// DEPOIS
// Se não tem centros de custo, pular validação
if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
{
    // Validar soma = 100%
    var totalPercentage = dto.CostCenterDistributions.Sum(cc => cc.Percentage);
    if (Math.Abs(totalPercentage - 100) > 0.01m)
    {
        throw new ValidationException(...);
    }
}

// AccountId pode ser null
var financialTransaction = new FinancialTransaction(
    companyId,
    dto.AccountId,  // Pode ser null agora
    // ...
);
```

#### **FinancialTransactionService.CreateAsync**

Mesma lógica do LoanAdvanceService.

---

## 🛠️ Implementação - Hook Customizado

### **useAutoSelect.ts** (novo hook)

```typescript
interface UseAutoSelectOptions<T> {
  items: T[];
  idField: keyof T;
  labelField: keyof T;
  onSelect?: (item: T | null) => void;
}

interface UseAutoSelectResult<T> {
  shouldShow: boolean;
  selectedItem: T | null;
  isAutoSelected: boolean;
}

export function useAutoSelect<T>(
  options: UseAutoSelectOptions<T>
): UseAutoSelectResult<T> {
  const { items, idField, labelField, onSelect } = options;
  
  const [selectedItem, setSelectedItem] = useState<T | null>(null);
  const [isAutoSelected, setIsAutoSelected] = useState(false);
  
  useEffect(() => {
    if (items.length === 0) {
      setSelectedItem(null);
      setIsAutoSelected(false);
      onSelect?.(null);
    }
    else if (items.length === 1) {
      setSelectedItem(items[0]);
      setIsAutoSelected(true);
      onSelect?.(items[0]);
    }
    else {
      setIsAutoSelected(false);
    }
  }, [items]);
  
  const shouldShow = items.length > 1;
  
  return {
    shouldShow,
    selectedItem,
    isAutoSelected
  };
}
```

**Uso:**
```typescript
const { shouldShow: showAccountField, selectedItem: autoAccount } = useAutoSelect({
  items: accounts,
  idField: 'accountId',
  labelField: 'accountName',
  onSelect: (account) => {
    if (account) {
      setFormData(prev => ({
        ...prev,
        accountId: account.accountId.toString(),
        accountName: account.accountName
      }));
    }
  }
});

// No JSX
{showAccountField && (
  <EntityPicker ... />
)}

{!showAccountField && autoAccount && (
  <div className="text-sm text-gray-600">
    Conta: {autoAccount.accountName} (selecionada automaticamente)
  </div>
)}
```

---

## 📊 Checklist de Implementação

### **Banco de Dados:**
- [ ] Migration 003 criada
- [ ] Migration 003 aplicada
- [ ] Testes de NULL em account_id

### **Backend:**
- [ ] LoanAdvanceInputDTO.AccountId → nullable
- [ ] FinancialTransactionInputDTO.AccountId → nullable
- [ ] LoanAdvanceService: validação opcional de centros de custo
- [ ] FinancialTransactionService: validação opcional de centros de custo
- [ ] FinancialTransaction entity: AccountId nullable
- [ ] Testes unitários

### **Frontend:**
- [ ] Hook useAutoSelect criado
- [ ] LoanAdvanceForm: auto-select conta
- [ ] LoanAdvanceForm: auto-select centros de custo
- [ ] FinancialTransactionForm: auto-select conta
- [ ] FinancialTransactionForm: auto-select centros de custo
- [ ] ContractForm: auto-select centros de custo
- [ ] Testes E2E

### **Documentação:**
- [ ] README atualizado
- [ ] Migration documentada
- [ ] Exemplos de uso

---

## 🧪 Cenários de Teste

### **Cenário 1: Empresa sem contas**
1. Criar empréstimo
2. Campo "Conta" não deve aparecer
3. Salvar com accountId = null
4. ✅ Deve funcionar

### **Cenário 2: Empresa com 1 conta**
1. Criar empréstimo
2. Campo "Conta" auto-selecionado (readonly ou oculto)
3. Salvar com accountId = [única conta]
4. ✅ Deve funcionar

### **Cenário 3: Empresa com 2+ contas**
1. Criar empréstimo
2. Campo "Conta" aparece normalmente
3. Usuário seleciona
4. ✅ Deve funcionar

### **Cenário 4: Empresa sem centros de custo**
1. Criar empréstimo
2. Card "Centros de Custo" não aparece
3. Salvar sem costCenterDistributions
4. ✅ Deve funcionar

### **Cenário 5: Empresa com 1 centro de custo**
1. Criar empréstimo
2. Card mostra centro auto-selecionado (100%)
3. Salvar com 1 centro de custo
4. ✅ Deve funcionar

---

## 💡 Observações Importantes

1. **Validação de soma de percentuais:**
   - Se tem centros de custo: soma = 100%
   - Se não tem: pular validação

2. **Tabelas de relação N-N:**
   - Não criar registros se não houver centros de custo
   - 0 registros = sem centro de custo

3. **UX:**
   - Mostrar mensagem quando auto-selecionado
   - "Conta selecionada automaticamente: Banco Principal"
   - "Sem centros de custo cadastrados"

4. **Performance:**
   - Buscar contas e centros de custo ao montar form
   - Cache se possível

---

**Status:** 📝 Análise completa  
**Próximo passo:** Implementar migration e começar ajustes no backend
