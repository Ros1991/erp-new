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
}

export function CostCenterDistribution({
  totalAmount,
  distributions,
  onChange,
  className = ''
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

  // Adicionar nova distribuição
  const handleAdd = () => {
    const newDistribution: CostCenterDistributionItem = {
      costCenterId: '',
      costCenterName: '',
      percentage: remaining > 0 ? Math.min(remaining, 100) : 0,
      amount: 0
    };

    const updated = [...distributions, newDistribution];
    updateAmounts(updated);
  };

  // Remover distribuição
  const handleRemove = (index: number) => {
    const updated = distributions.filter((_, i) => i !== index);
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

  // Atualizar porcentagem
  const handlePercentageChange = (index: number, percentage: number) => {
    const updated = [...distributions];
    updated[index] = {
      ...updated[index],
      percentage: Math.max(0, Math.min(100, Math.round(percentage / 5) * 5))
    };
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
            Divida o valor entre centros de custo (total deve ser 100%)
          </p>
        </div>
        <Button
          type="button"
          variant="outline"
          size="sm"
          onClick={handleAdd}
        >
          <Plus className="h-4 w-4 mr-1" />
          Adicionar
        </Button>
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
                      />
                      {distributions.length > 1 && (
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
