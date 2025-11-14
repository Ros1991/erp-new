# Status de Implementação: Contas e Centros de Custo Opcionais

## ✅ CONCLUÍDO

### **1. Banco de Dados**

#### **Migration 003 criada:**
- ✅ `backend/-1-Domain/database/migrations/003_make_account_id_optional.sql`
- Remove NOT NULL de `account_id`
- Remove DEFAULT de `account_id`

#### **erp.sql atualizado:**
- ✅ Linha 317: `"account_id" bigint NULL` (antes: `DEFAULT 0 NOT NULL`)

#### **Decisão de Design:**
- ✅ `tb_transaction_cost_center` e `tb_contract_cost_center` continuam com `cost_center_id NOT NULL`
- ✅ Se não há centros de custo: **não criar registros** nessas tabelas (abordagem mais limpa)

---

### **2. Backend - Entities**

#### **FinancialTransaction.cs:**
- ✅ Propriedade `AccountId`: `long` → `long?`
- ✅ Construtor: parâmetro `Param_AccountId`: `long` → `long?`

---

### **3. Backend - DTOs**

#### **LoanAdvanceInputDTO.cs:**
```csharp
// ANTES
[Required(ErrorMessage = "AccountId é obrigatório")]
public long AccountId { get; set; }

// DEPOIS
public long? AccountId { get; set; }
```

#### **FinancialTransactionInputDTO.cs:**
```csharp
// ANTES
[Required(ErrorMessage = "AccountId é obrigatório")]
public long AccountId { get; set; }

// DEPOIS
public long? AccountId { get; set; }
```

---

### **4. Backend - Services**

#### **LoanAdvanceService.cs:**
- ✅ Validação de centros de custo já é opcional (linha 56):
  ```csharp
  if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
  {
      // Valida soma = 100%
  }
  ```
- ✅ `dto.AccountId` pode ser `null` ao criar transação financeira

#### **FinancialTransactionService.cs:**
- ✅ Validação similar já implementada

---

### **5. Frontend - Hook Customizado**

#### **useAutoSelect.ts criado:**
- ✅ Hook reutilizável para auto-seleção
- ✅ Retorna: `shouldShow`, `autoSelected`, `message`
- ✅ Comportamento:
  - 0 itens → não mostra campo, mensagem: "Nenhuma conta cadastrada"
  - 1 item → auto-seleciona, mensagem: "Conta selecionada automaticamente: Banco Principal"
  - 2+ itens → mostra campo normalmente

**Localização:** `frontend/src/hooks/useAutoSelect.ts`

---

## ⏳ PENDENTE

### **6. Frontend - Formulários**

#### **A implementar:**

1. **LoanAdvanceForm.tsx:**
   - [ ] Importar `useAutoSelect`
   - [ ] Buscar contas ao montar (useState com lista de contas)
   - [ ] Buscar centros de custo ao montar (useState com lista)
   - [ ] Aplicar `useAutoSelect` para contas
   - [ ] Aplicar `useAutoSelect` para centros de custo
   - [ ] Mostrar mensagens informativas
   - [ ] Remover validação obrigatória de `accountId` (linha 88-90)
   - [ ] Ajustar submissão para enviar `null` se não houver conta

2. **FinancialTransactionForm.tsx:**
   - [ ] Mesmas mudanças do LoanAdvanceForm
   - [ ] Auto-select conta
   - [ ] Auto-select centros de custo

3. **ContractForm.tsx:**
   - [ ] Auto-select centros de custo
   - [ ] (Conta não é usada em contratos)

---

## 📝 Exemplo de Implementação

### **LoanAdvanceForm.tsx - Snippet:**

```typescript
import { useAutoSelect } from '../../hooks/useAutoSelect';
import costCenterService from '../../services/costCenterService';

// Estados adicionais
const [accounts, setAccounts] = useState<any[]>([]);
const [availableCostCenters, setAvailableCostCenters] = useState<any[]>([]);

// Buscar contas e centros de custo ao montar
useEffect(() => {
  loadAccountsAndCostCenters();
}, []);

const loadAccountsAndCostCenters = async () => {
  try {
    const [accountsData, costCentersData] = await Promise.all([
      accountService.getAccounts({ page: 1, pageSize: 100 }),
      costCenterService.getCostCenters({ page: 1, pageSize: 100 })
    ]);
    
    setAccounts(accountsData.items);
    setAvailableCostCenters(costCentersData.items);
    
    // Auto-selecionar se houver apenas 1 conta
    if (accountsData.items.length === 1 && !isEditing) {
      setFormData(prev => ({
        ...prev,
        accountId: accountsData.items[0].accountId.toString(),
        accountName: accountsData.items[0].accountName
      }));
    }
    
    // Auto-selecionar se houver apenas 1 centro de custo
    if (costCentersData.items.length === 1 && !isEditing) {
      setCostCenters([{
        costCenterId: costCentersData.items[0].costCenterId,
        costCenterName: costCentersData.items[0].costCenterName,
        percentage: 100,
        amount: Number(formData.amount) // Será ajustado quando amount mudar
      }]);
    }
  } catch (err) {
    handleBackendError(err);
  }
};

// Usar hook
const accountAutoSelect = useAutoSelect(
  accounts.length,
  'conta',
  formData.accountId 
    ? accounts.find(a => a.accountId.toString() === formData.accountId)
    : null
);

const costCenterAutoSelect = useAutoSelect(
  availableCostCenters.length,
  'centro de custo',
  null // ou selectedCostCenter se houver
);

// No JSX - Condicional para mostrar campo de conta
{accountAutoSelect.shouldShow && (
  <div>
    <label>Conta *</label>
    <EntityPicker
      value={Number(formData.accountId) || null}
      selectedLabel={formData.accountName}
      onChange={handleAccountChange}
      onSearch={handleSearchAccount}
      placeholder="Selecione uma conta"
      label="Selecionar Conta"
    />
  </div>
)}

{accountAutoSelect.message && (
  <div className="text-sm text-blue-600 bg-blue-50 p-3 rounded-md border border-blue-200">
    ℹ️ {accountAutoSelect.message}
  </div>
)}

// No JSX - Condicional para mostrar centros de custo
{costCenterAutoSelect.shouldShow && (
  <Card>
    <CardHeader>
      <CardTitle>Centros de Custo</CardTitle>
    </CardHeader>
    <CardContent>
      <CostCenterDistribution
        costCenters={costCenters}
        onChange={setCostCenters}
        totalAmount={Number(formData.amount)}
      />
    </CardContent>
  </Card>
)}

{costCenterAutoSelect.message && (
  <div className="text-sm text-blue-600 bg-blue-50 p-3 rounded-md">
    ℹ️ {costCenterAutoSelect.message}
  </div>
)}

// Validação - REMOVER obrigatoriedade de accountId
const validate = (): boolean => {
  const newErrors: Partial<Record<keyof LoanAdvanceFormData, string>> = {};

  if (!formData.employeeId) {
    newErrors.employeeId = 'Empregado é obrigatório';
  }

  // ❌ REMOVER ISSO:
  // if (!formData.accountId) {
  //   newErrors.accountId = 'Conta é obrigatória';
  // }

  // Resto da validação...
};

// Submissão - enviar accountId como number ou null
const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  
  // ...validações...
  
  const data = {
    employeeId: Number(formData.employeeId),
    accountId: formData.accountId ? Number(formData.accountId) : null, // ✅ null se vazio
    amount: Number(formData.amount),
    installments: formData.installments,
    discountSource: formData.discountSource,
    startDate: toUTCString(formData.startDate),
    isApproved: true,
    costCenterDistributions: costCenters.length > 0 ? costCenters.map(cc => ({
      costCenterId: cc.costCenterId,
      percentage: cc.percentage,
      amount: cc.amount
    })) : undefined // ✅ undefined se não houver centros
  };

  // Enviar...
};
```

---

## 🧪 Cenários de Teste

### **Teste 1: Empresa sem contas**
1. Não cadastrar nenhuma conta
2. Abrir formulário de empréstimo
3. ✅ Campo "Conta" não aparece
4. ✅ Mensagem: "Nenhuma conta cadastrada"
5. Salvar → `accountId = null`

### **Teste 2: Empresa com 1 conta**
1. Cadastrar apenas "Banco Principal"
2. Abrir formulário de empréstimo
3. ✅ Campo "Conta" não aparece (ou aparece readonly)
4. ✅ Mensagem: "Conta selecionada automaticamente: Banco Principal"
5. Salvar → `accountId = 1`

### **Teste 3: Empresa com 2+ contas**
1. Cadastrar "Banco Principal", "Caixa", "Sócio João"
2. Abrir formulário de empréstimo
3. ✅ Campo "Conta" aparece normalmente
4. ✅ Sem mensagem
5. Usuário seleciona conta
6. Salvar → `accountId = selected`

### **Teste 4: Empresa sem centros de custo**
1. Não cadastrar nenhum centro de custo
2. Abrir formulário de empréstimo
3. ✅ Card "Centros de Custo" não aparece
4. ✅ Mensagem: "Nenhum centro de custo cadastrado"
5. Salvar → `costCenterDistributions = undefined`

### **Teste 5: Empresa com 1 centro de custo**
1. Cadastrar apenas "Administrativo"
2. Abrir formulário de empréstimo
3. ✅ Card aparece com centro auto-selecionado (100%)
4. ✅ Mensagem: "Centro de custo selecionado automaticamente: Administrativo"
5. Salvar → `costCenterDistributions = [{ costCenterId: 1, percentage: 100 }]`

---

## 📦 Arquivos para Aplicar no Banco

### **Aplicar migrations em ordem:**

```bash
# 1. Migration 001 (se ainda não aplicou)
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/001_add_loan_advance_id_to_financial_transaction.sql

# 2. Migration 002 (se ainda não aplicou)
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/002_add_cascade_to_transaction_cost_center.sql

# 3. Migration 003 (NOVA - obrigatória)
psql -U postgres -d erp_database -f backend/-1-Domain/database/migrations/003_make_account_id_optional.sql
```

### **Ou banco novo:**
```bash
psql -U postgres -d erp_database -f backend/-1-Domain/database/erp.sql
```

---

## 🎯 Próximos Passos

1. **Aplicar Migration 003** no banco de dados
2. **Implementar mudanças no LoanAdvanceForm.tsx**
3. **Implementar mudanças no FinancialTransactionForm.tsx**
4. **Implementar mudanças no ContractForm.tsx**
5. **Testar todos os cenários**
6. **Documentar para o usuário final**

---

## 💡 Notas Importantes

1. **Validação de centros de custo:**
   - Se houver centros: soma = 100%
   - Se não houver: não validar, enviar `undefined`

2. **AccountId no backend:**
   - Aceita `null`
   - Transações sem conta específica são válidas

3. **UX clara:**
   - Mensagens informativas em azul
   - Ícone ℹ️ para indicar informação
   - Não confundir com erro (vermelho)

4. **Performance:**
   - Buscar contas e centros ao montar form
   - Cache se necessário (Context API?)

---

**Data:** 2025-11-14  
**Status Backend:** ✅ Completo  
**Status Frontend:** ⏳ Hook criado, formulários pendentes  
**Próxima ação:** Implementar auto-select nos formulários
