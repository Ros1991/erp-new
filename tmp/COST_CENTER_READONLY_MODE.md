# 🔒 Modo Readonly para Centro de Custo Auto-Selecionado

## 🎯 Objetivo

Quando houver **apenas 1 centro de custo disponível** e ele for **auto-selecionado**, bloquear todas as edições para evitar que o usuário:
- Exclua o centro de custo
- Altere a porcentagem (deve permanecer 100%)
- Adicione mais centros de custo

---

## ✅ Implementação

### **1. Componente CostCenterDistribution.tsx**

#### **Nova Prop:**
```typescript
interface CostCenterDistributionProps {
  totalAmount: number;
  distributions: CostCenterDistributionItem[];
  onChange: (distributions: CostCenterDistributionItem[]) => void;
  className?: string;
  readonly?: boolean; // ✨ NOVO: Desabilita todas as edições
}
```

#### **Comportamento quando `readonly={true}`:**

| Elemento | Estado Normal | Estado Readonly |
|----------|---------------|-----------------|
| **Botão "Adicionar"** | ✅ Visível | ❌ Oculto |
| **EntityPicker** | ✅ Editável | 🔒 Disabled |
| **Input Porcentagem** | ✅ Editável | 🔒 Disabled |
| **Slider** | ✅ Editável | 🔒 Disabled |
| **Botão "Remover"** | ✅ Visível (se > 1) | ❌ Oculto |
| **Mensagem do Header** | "Divida o valor..." | "Centro de custo selecionado automaticamente (único disponível)" |

#### **Código Aplicado:**

```typescript
// Header com mensagem condicional
<p className="text-xs text-gray-500 mt-1">
  {readonly 
    ? 'Centro de custo selecionado automaticamente (único disponível)'
    : 'Divida o valor entre centros de custo (total deve ser 100%)'}
</p>

// Botão Adicionar - só mostra se não readonly
{!readonly && (
  <Button type="button" variant="outline" size="sm" onClick={handleAdd}>
    <Plus className="h-4 w-4 mr-1" />
    Adicionar
  </Button>
)}

// EntityPicker - disabled quando readonly
<EntityPicker
  value={item.costCenterId ? Number(item.costCenterId) : null}
  selectedLabel={item.costCenterName}
  onChange={(selected) => handleCostCenterChange(index, selected)}
  onSearch={(term, page) => handleSearchCostCenter(term, page, item.costCenterId)}
  placeholder="Selecione um centro de custo"
  label="Selecionar Centro de Custo"
  disabled={readonly}
/>

// Input Porcentagem - disabled quando readonly
<Input
  type="number"
  min="0"
  max="100"
  step="5"
  value={item.percentage}
  onChange={(e) => handlePercentageChange(index, parseFloat(e.target.value) || 0)}
  className="flex-1"
  disabled={readonly}
/>

// Botão Remover - só mostra se > 1 item E não readonly
{distributions.length > 1 && !readonly && (
  <Button
    type="button"
    variant="ghost"
    size="sm"
    onClick={() => handleRemove(index)}
    className="text-red-600 hover:text-red-700 hover:bg-red-50"
  >
    <Trash2 className="h-4 w-4" />
  </Button>
)}

// Slider - disabled quando readonly
<input
  type="range"
  min="0"
  max="100"
  step="5"
  value={item.percentage}
  onChange={(e) => handlePercentageChange(index, parseFloat(e.target.value))}
  className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-primary-600"
  disabled={readonly}
/>
```

---

### **2. Formulários Atualizados**

Todos os formulários agora passam `readonly={availableCostCenters.length === 1}`:

#### **LoanAdvanceForm.tsx**
```typescript
<CostCenterDistribution
  totalAmount={Number(formData.amount) / 100}
  distributions={costCenters}
  onChange={setCostCenters}
  readonly={availableCostCenters.length === 1}
/>
```

#### **FinancialTransactionForm.tsx**
```typescript
<CostCenterDistribution
  totalAmount={Number(formData.amount)}
  distributions={costCenterDistributions}
  onChange={setCostCenterDistributions}
  readonly={availableCostCenters.length === 1}
/>
```

#### **ContractForm.tsx**
```typescript
<CostCenterDistribution
  totalAmount={Number(formData.value) / 100}
  distributions={costCenters}
  onChange={setCostCenters}
  readonly={availableCostCenters.length === 1}
/>
```

---

## 🎨 Resultado Visual

### **Modo Normal (2+ centros de custo)**

```
┌─────────────────────────────────────────────────┐
│ Distribuição por Centro de Custo    [+ Adicionar] │
│ Divida o valor entre centros de custo...       │
├─────────────────────────────────────────────────┤
│ ✅ Total: 100.00% - Completo                    │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
├─────────────────────────────────────────────────┤
│ Centro de Custo      | Porcentagem (%)   [🗑️] │
│ [🔍 Vendas        ▼] | [60      ] ━━━━━━━     │
│ R$ 600,00                                       │
├─────────────────────────────────────────────────┤
│ Centro de Custo      | Porcentagem (%)   [🗑️] │
│ [🔍 Administrativo▼] | [40      ] ━━━━━━━     │
│ R$ 400,00                                       │
└─────────────────────────────────────────────────┘
```

### **Modo Readonly (1 centro de custo)**

```
┌─────────────────────────────────────────────────┐
│ Distribuição por Centro de Custo               │
│ Centro de custo selecionado automaticamente     │
│ (único disponível)                              │
├─────────────────────────────────────────────────┤
│ ✅ Total: 100.00% - Completo                    │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
├─────────────────────────────────────────────────┤
│ Centro de Custo      | Porcentagem (%)         │
│ [🔒 Vendas       ▼] | [100     ] 🔒━━━━━━━   │
│ R$ 1.000,00                                     │
└─────────────────────────────────────────────────┘
```

**Observações:**
- ❌ Sem botão "Adicionar"
- 🔒 EntityPicker desabilitado (cinza)
- 🔒 Input de porcentagem desabilitado (cinza)
- 🔒 Slider desabilitado (cinza)
- ❌ Sem botão de remover

---

## 📋 Lógica de Ativação

```typescript
readonly={availableCostCenters.length === 1}
```

| Quantidade de Centros | Readonly? | Motivo |
|----------------------|-----------|--------|
| **0 centros** | ❌ Não | Card não é exibido |
| **1 centro** | ✅ Sim | Auto-selecionado, não deve ser alterado |
| **2+ centros** | ❌ Não | Usuário deve escolher e distribuir |

---

## 🎯 Casos de Uso

### **Cenário 1: Empresa com 1 Centro de Custo**
1. ✅ Sistema carrega e detecta 1 centro disponível
2. ✅ Auto-seleciona com 100%
3. ✅ Exibe mensagem informativa azul: "Centro de custo selecionado automaticamente: Vendas"
4. ✅ Card de distribuição em **modo readonly**
5. 🔒 Usuário **não pode** alterar/remover

### **Cenário 2: Empresa com 0 Centros de Custo**
1. ✅ Card de distribuição **não é exibido**
2. ✅ Transação criada sem centros de custo

### **Cenário 3: Empresa com 2+ Centros de Custo**
1. ✅ Card exibido em **modo normal**
2. ✅ Usuário pode adicionar/remover/editar
3. ✅ Sistema valida que total = 100%

### **Cenário 4: Empregado com Contrato Ativo (múltiplos centros)**
1. ✅ Sistema carrega centros do contrato
2. ✅ Mesmo tendo 1 centro disponível em todo sistema, se contrato tem múltiplos, permite edição
3. ✅ **Readonly só ativa se `availableCostCenters.length === 1`**

---

## 🔧 Benefícios

1. **✅ Segurança:** Evita que usuário exclua/altere centro de custo único
2. **✅ UX Claro:** Mensagem explica porque está desabilitado
3. **✅ Consistência:** Mesmo comportamento em todos os formulários
4. **✅ Validação Automática:** Sempre 100% quando há 1 centro
5. **✅ Menos Erros:** Usuário não pode fazer alterações inválidas

---

## 📊 Arquivos Modificados

| Arquivo | Mudança |
|---------|---------|
| `components/ui/CostCenterDistribution.tsx` | ✅ Adicionado prop `readonly` + lógica condicional |
| `pages/loan-advances/LoanAdvanceForm.tsx` | ✅ Passa `readonly={availableCostCenters.length === 1}` |
| `pages/financial-transactions/FinancialTransactionForm.tsx` | ✅ Passa `readonly={availableCostCenters.length === 1}` |
| `pages/contracts/ContractForm.tsx` | ✅ Passa `readonly={availableCostCenters.length === 1}` |

---

## 🧪 Como Testar

### **Teste 1: Readonly Ativo**
1. Crie/edite empresa para ter **apenas 1 centro de custo** cadastrado
2. Crie novo empréstimo/transação/contrato
3. ✅ Verificar: Card aparece com centro auto-selecionado em 100%
4. ✅ Verificar: Mensagem "Centro de custo selecionado automaticamente (único disponível)"
5. ✅ Verificar: Botão "Adicionar" não aparece
6. ✅ Verificar: EntityPicker está desabilitado (cinza, não clicável)
7. ✅ Verificar: Input de porcentagem está desabilitado (cinza)
8. ✅ Verificar: Slider está desabilitado (cinza)
9. ✅ Verificar: Botão de remover não aparece

### **Teste 2: Modo Normal**
1. Adicione mais 1 centro de custo na empresa (total 2+)
2. Crie novo empréstimo/transação/contrato
3. ✅ Verificar: Card aparece vazio
4. ✅ Verificar: Mensagem "Divida o valor entre centros de custo..."
5. ✅ Verificar: Botão "Adicionar" está visível
6. ✅ Verificar: EntityPicker está habilitado
7. ✅ Verificar: Input de porcentagem está habilitado
8. ✅ Verificar: Slider está habilitado
9. ✅ Verificar: Botão de remover aparece quando > 1 distribuição

### **Teste 3: Persistência ao Salvar**
1. Com 1 centro readonly, salve empréstimo/transação
2. ✅ Verificar: Backend recebe distribuição com 100%
3. Recarregue a página de edição
4. ✅ Verificar: Centro continua auto-selecionado e readonly

---

## 💡 Decisões de Design

### **Por que não esconder o card quando readonly?**
❌ **Ruim:** Usuário fica confuso sobre onde foi o centro de custo
✅ **Bom:** Mostra claramente que o centro foi selecionado automaticamente

### **Por que desabilitar ao invés de esconder os campos?**
❌ **Ruim:** Esconder dá impressão que algo está errado
✅ **Bom:** Disabled mostra que o valor existe mas não pode ser alterado

### **Por que mensagem diferente no header?**
✅ **Contexto:** Usuário entende imediatamente por que não pode editar
✅ **Transparência:** Sistema comunica o comportamento automático

---

**Data:** 2025-11-14  
**Status:** ✅ 100% Implementado  
**Pronto para Produção!** 🎉
