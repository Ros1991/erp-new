import { useState, useEffect } from 'react';
import { Button } from './Button';
import { Input } from './Input';
import { EntityPicker, type EntityPickerItem } from './EntityPicker';
import { Plus, Trash2, AlertCircle, CheckCircle2 } from 'lucide-react';
import costCenterService from '../../services/costCenterService';

export interface CostCenterDistributionItem {
  costCenterId: string;
  costCenterName: string;
  percentage: number;
  amount: number;
}

interface CostCenterDistributionProps {
  totalAmount: number; // Valor total em centavos
  distributions: CostCenterDistributionItem[];
  onChange: (distributions: CostCenterDistributionItem[]) => void;
  className?: string;
  readonly?: boolean; // Se true, desabilita todas as edições
}

export function CostCenterDistribution({
  totalAmount,
  distributions,
  onChange,
  className = '',
  readonly = false
}: CostCenterDistributionProps) {
  const [usedCostCenterIds, setUsedCostCenterIds] = useState<Set<string>>(new Set());

  useEffect(() => {
    const ids = new Set(distributions.map(d => d.costCenterId));
    setUsedCostCenterIds(ids);
  }, [distributions]);

  // Calcular soma das porcentagens
  const totalPercentage = distributions.reduce((sum, item) => sum + item.percentage, 0);
  const isValid = Math.abs(totalPercentage - 100) < 0.01;
  const remaining = 100 - totalPercentage;

  // Formatar moeda
  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value / 100);
  };

  // Arredondar para múltiplo de 5
  const roundToFive = (value: number): number => {
    return Math.max(0, Math.min(100, Math.round(value / 5) * 5));
  };

  // Adicionar nova distribuição e redistribuir porcentagens igualmente
  const handleAdd = () => {
    const currentCount = distributions.length;
    const newCount = currentCount + 1;
    
    // Calcular nova porcentagem para cada item (distribuição igual, múltiplo de 5)
    const basePercentage = roundToFive(100 / newCount);
    const totalBase = basePercentage * newCount;
    const adjustment = 100 - totalBase; // Ajuste para chegar a 100%
    
    // Atualizar itens existentes
    const updated = distributions.map((item, index) => ({
      ...item,
      percentage: index === 0 ? basePercentage + adjustment : basePercentage
    }));
    
    // Adicionar novo item
    const newDistribution: CostCenterDistributionItem = {
      costCenterId: '',
      costCenterName: '',
      percentage: basePercentage,
      amount: 0
    };

    updated.push(newDistribution);
    updateAmounts(updated);
  };

  // Remover distribuição e redistribuir porcentagens
  const handleRemove = (index: number) => {
    const updated = distributions.filter((_, i) => i !== index);
    
    // Se sobrou apenas 1 item, colocar 100%
    if (updated.length === 1) {
      updated[0] = { ...updated[0], percentage: 100 };
    } else if (updated.length > 1) {
      // Redistribuir igualmente entre os restantes
      const basePercentage = roundToFive(100 / updated.length);
      const totalBase = basePercentage * updated.length;
      const adjustment = 100 - totalBase;
      
      updated.forEach((item, i) => {
        updated[i] = { ...item, percentage: i === 0 ? basePercentage + adjustment : basePercentage };
      });
    }
    
    updateAmounts(updated);
  };

  // Atualizar centro de custo
  const handleCostCenterChange = (index: number, item: EntityPickerItem | null) => {
    const updated = [...distributions];
    updated[index] = {
      ...updated[index],
      costCenterId: item ? item.id.toString() : '',
      costCenterName: item ? item.displayText : ''
    };
    updateAmounts(updated);
  };

  // Atualizar porcentagem com balanceamento automático para manter 100%
  const handlePercentageChange = (index: number, percentage: number) => {
    const updated = [...distributions];
    const newPercentage = roundToFive(percentage);
    const oldPercentage = updated[index].percentage;
    
    // Se só tem 1 item, sempre deve ser 100%
    if (updated.length === 1) {
      updated[0] = { ...updated[0], percentage: 100 };
      updateAmounts(updated);
      return;
    }

    // Se tem 2 itens, ajustar o outro automaticamente
    if (updated.length === 2) {
      const otherIndex = index === 0 ? 1 : 0;
      const otherPercentage = 100 - newPercentage;
      
      // Verificar se o outro valor é válido (entre 0 e 100)
      if (otherPercentage >= 0 && otherPercentage <= 100) {
        updated[index] = { ...updated[index], percentage: newPercentage };
        updated[otherIndex] = { ...updated[otherIndex], percentage: otherPercentage };
      }
      updateAmounts(updated);
      return;
    }

    // Se tem 3+ itens, distribuir a diferença entre os outros proporcionalmente
    const difference = oldPercentage - newPercentage; // Diferença a distribuir (positivo = sobrou, negativo = precisa tirar)
    
    if (difference === 0) {
      updateAmounts(updated);
      return;
    }

    // Aplicar a nova porcentagem ao item alterado
    updated[index] = { ...updated[index], percentage: newPercentage };

    // Calcular a soma atual dos outros itens
    const otherItems = updated.filter((_, i) => i !== index);
    const otherTotal = otherItems.reduce((sum, item) => sum + item.percentage, 0);

    if (otherTotal === 0) {
      // Se todos os outros estão em 0, distribuir igualmente
      const share = roundToFive(difference / otherItems.length);
      let remaining = difference;
      
      updated.forEach((item, i) => {
        if (i !== index) {
          const newVal = roundToFive(Math.max(0, Math.min(100, item.percentage + share)));
          remaining -= (newVal - item.percentage);
          updated[i] = { ...item, percentage: newVal };
        }
      });
    } else {
      // Distribuir proporcionalmente, de 5 em 5
      let remainingDiff = difference;
      const otherIndices = updated.map((_, i) => i).filter(i => i !== index);
      
      // Ordenar outros itens por porcentagem (maior primeiro se precisamos tirar, menor primeiro se precisamos adicionar)
      otherIndices.sort((a, b) => {
        if (difference > 0) {
          // Precisamos adicionar, começar pelos menores
          return updated[a].percentage - updated[b].percentage;
        } else {
          // Precisamos tirar, começar pelos maiores
          return updated[b].percentage - updated[a].percentage;
        }
      });

      // Distribuir de 5 em 5
      while (Math.abs(remainingDiff) >= 5) {
        for (const i of otherIndices) {
          if (Math.abs(remainingDiff) < 5) break;
          
          const currentPercentage = updated[i].percentage;
          const adjustment = difference > 0 ? 5 : -5;
          const newVal = currentPercentage + adjustment;
          
          // Verificar limites
          if (newVal >= 0 && newVal <= 100) {
            updated[i] = { ...updated[i], percentage: newVal };
            remainingDiff -= adjustment;
          }
        }
        
        // Se não conseguiu ajustar nenhum item, sair do loop
        const currentTotal = updated.reduce((sum, item) => sum + item.percentage, 0);
        if (currentTotal === 100) break;
      }
    }

    // Garantir que a soma seja 100% fazendo ajuste final se necessário
    const finalTotal = updated.reduce((sum, item) => sum + item.percentage, 0);
    if (finalTotal !== 100) {
      const finalDiff = 100 - finalTotal;
      // Encontrar um item (que não seja o alterado) para ajustar
      for (let i = 0; i < updated.length; i++) {
        if (i !== index) {
          const newVal = updated[i].percentage + finalDiff;
          if (newVal >= 0 && newVal <= 100) {
            updated[i] = { ...updated[i], percentage: roundToFive(newVal) };
            break;
          }
        }
      }
    }

    updateAmounts(updated);
  };

  // Atualizar valores calculados
  const updateAmounts = (updated: CostCenterDistributionItem[]) => {
    const withAmounts = updated.map(item => ({
      ...item,
      amount: Math.round((totalAmount * item.percentage) / 100)
    }));
    onChange(withAmounts);
  };

  // Recalcular amounts quando totalAmount mudar
  useEffect(() => {
    if (distributions.length > 0) {
      updateAmounts(distributions);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [totalAmount]);

  // Buscar centros de custo
  const handleSearchCostCenter = async (searchTerm: string, page: number, currentCostCenterId?: string) => {
    try {
      const result = await costCenterService.getCostCenters({
        search: searchTerm,
        page: page,
        pageSize: 10,
      });

      // Filtrar centros já usados (exceto o atual)
      const filtered = result.items.filter(
        cc => cc.costCenterId.toString() === currentCostCenterId || !usedCostCenterIds.has(cc.costCenterId.toString())
      );

      return {
        items: filtered.map(item => ({
          id: item.costCenterId,
          displayText: item.name,
          secondaryText: item.isActive ? 'Ativo' : 'Inativo'
        })),
        totalPages: result.totalPages,
        totalCount: filtered.length
      };
    } catch (error) {
      console.error('Erro ao buscar centros de custo:', error);
      return {
        items: [],
        totalPages: 1,
        totalCount: 0
      };
    }
  };

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-sm font-medium text-gray-900">Distribuição por Centro de Custo</h3>
          <p className="text-xs text-gray-500 mt-1">
            {readonly 
              ? 'Centro de custo selecionado automaticamente (único disponível)'
              : 'Divida o valor entre centros de custo (total deve ser 100%)'}
          </p>
        </div>
        {!readonly && (
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={handleAdd}
          >
            <Plus className="h-4 w-4 mr-1" />
            Adicionar
          </Button>
        )}
      </div>

      {/* Progress Bar */}
      <div className="space-y-2">
        <div className="flex items-center justify-between text-xs">
          <span className="text-gray-600">Total: {totalPercentage.toFixed(2)}%</span>
          {isValid ? (
            <span className="flex items-center text-green-600 font-medium">
              <CheckCircle2 className="h-3 w-3 mr-1" />
              Completo
            </span>
          ) : (
            <span className="flex items-center text-amber-600 font-medium">
              <AlertCircle className="h-3 w-3 mr-1" />
              Faltam {remaining.toFixed(2)}%
            </span>
          )}
        </div>
        <div className="w-full h-3 bg-gray-200 rounded-full overflow-hidden">
          <div
            className={`h-full transition-all duration-300 ${
              isValid
                ? 'bg-green-500'
                : totalPercentage > 100
                ? 'bg-red-500'
                : 'bg-amber-500'
            }`}
            style={{ width: `${Math.min(totalPercentage, 100)}%` }}
          />
        </div>
      </div>

      {/* Lista de Distribuições */}
      {distributions.length === 0 ? (
        <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300">
          <p className="text-sm text-gray-500">
            Nenhum centro de custo adicionado
          </p>
          <p className="text-xs text-gray-400 mt-1">
            Clique em "Adicionar" para começar
          </p>
        </div>
      ) : (
        <div className="space-y-3">
          {distributions.map((item, index) => (
            <div
              key={index}
              className="p-4 bg-gray-50 rounded-lg border border-gray-200 space-y-3"
            >
                {/* Centro de Custo */}
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <div>
                    <label className="block text-xs font-medium text-gray-700 mb-1">
                      Centro de Custo
                    </label>
                    <EntityPicker
                      value={item.costCenterId ? Number(item.costCenterId) : null}
                      selectedLabel={item.costCenterName}
                      onChange={(selected) => handleCostCenterChange(index, selected)}
                      onSearch={(term, page) => handleSearchCostCenter(term, page, item.costCenterId)}
                      placeholder="Selecione um centro de custo"
                      label="Selecionar Centro de Custo"
                      disabled={readonly}
                    />
                  </div>

                  {/* Porcentagem */}
                  <div>
                    <label className="block text-xs font-medium text-gray-700 mb-1">
                      Porcentagem (%)
                    </label>
                    <div className="flex items-center gap-2">
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
                    </div>
                  </div>
                </div>

                {/* Slider e Valor */}
                <div className="space-y-2">
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
                  <div className="flex items-center justify-between text-xs">
                    <span className="text-gray-500">
                      {item.percentage.toFixed(2)}%
                    </span>
                    <span className="font-medium text-gray-900">
                      {formatCurrency(item.amount)}
                    </span>
                  </div>
                </div>
              </div>
            ))}
        </div>
      )}

      {/* Resumo */}
      {distributions.length > 0 && totalAmount > 0 && (
        <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium text-blue-900">
              Valor Total da Transação:
            </span>
            <span className="text-lg font-bold text-blue-900">
              {formatCurrency(totalAmount)}
            </span>
          </div>
        </div>
      )}

      {/* Avisos */}
      {!isValid && distributions.length > 0 && (
        <div className={`p-3 rounded-lg flex items-start gap-2 text-sm ${
          totalPercentage > 100
            ? 'bg-red-50 text-red-800 border border-red-200'
            : 'bg-amber-50 text-amber-800 border border-amber-200'
        }`}>
          <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
          <div>
            {totalPercentage > 100 ? (
              <p>A soma das porcentagens ({totalPercentage.toFixed(2)}%) ultrapassou 100%. Ajuste os valores.</p>
            ) : (
              <p>A soma das porcentagens está em {totalPercentage.toFixed(2)}%. Faltam {remaining.toFixed(2)}% para completar 100%.</p>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
