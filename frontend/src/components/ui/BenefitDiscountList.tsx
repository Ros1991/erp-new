import { Plus, Trash2 } from 'lucide-react';
import { Button } from './Button';
import { Input } from './Input';
import { Label } from './Label';
import { Select } from './Select';
import { CurrencyInput } from './CurrencyInput';
import { APPLICATION_TYPE_OPTIONS, ApplicationTypeCode, migrateApplicationTypeValue } from '../../constants/applicationType';

export interface BenefitDiscountItem {
  description: string;
  type: string; // 'Benefício' ou 'Desconto'
  application: string; // Códigos curtos: SALARIO, 13SAL, FERIAS, TODOS, BONUS, COMISSAO
  amount: number; // Valor em centavos
}

interface BenefitDiscountListProps {
  items: BenefitDiscountItem[];
  onChange: (items: BenefitDiscountItem[]) => void;
}

const TYPES = [
  { value: 'Benefício', label: 'Benefício' },
  { value: 'Desconto', label: 'Desconto' },
];

export function BenefitDiscountList({ items, onChange }: BenefitDiscountListProps) {
  const handleAdd = () => {
    onChange([
      ...items,
      {
        description: '',
        type: 'Benefício',
        application: ApplicationTypeCode.SALARY,
        amount: 0,
      },
    ]);
  };

  const handleRemove = (index: number) => {
    const newItems = items.filter((_, i) => i !== index);
    onChange(newItems);
  };

  const handleChange = (index: number, field: keyof BenefitDiscountItem, value: string | number) => {
    const newItems = [...items];
    // Migrar valores antigos de application para códigos novos
    if (field === 'application' && typeof value === 'string') {
      value = migrateApplicationTypeValue(value);
    }
    newItems[index] = {
      ...newItems[index],
      [field]: value,
    };
    onChange(newItems);
  };

  const getTotalBenefits = () => {
    return items
      .filter((item) => item.type === 'Benefício')
      .reduce((sum, item) => sum + Number(item.amount || 0), 0) / 100; // Converter centavos para reais
  };

  const getTotalDiscounts = () => {
    return items
      .filter((item) => item.type === 'Desconto')
      .reduce((sum, item) => sum + Number(item.amount || 0), 0) / 100; // Converter centavos para reais
  };

  const getNetTotal = () => {
    return getTotalBenefits() - getTotalDiscounts();
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-medium">Benefícios e Descontos</h3>
          <p className="text-sm text-muted-foreground">
            Adicione benefícios e descontos do contrato
          </p>
        </div>
        <Button type="button" onClick={handleAdd} size="sm" variant="outline">
          <Plus className="h-4 w-4 mr-2" />
          Adicionar
        </Button>
      </div>

      {items.length === 0 ? (
        <div className="text-center py-8 text-muted-foreground">
          Nenhum benefício ou desconto adicionado
        </div>
      ) : (
        <div className="space-y-4">
          {items.map((item, index) => (
            <div
              key={index}
              className="grid grid-cols-1 md:grid-cols-12 gap-4 p-4 border rounded-lg bg-card"
            >
              {/* Descrição */}
              <div className="md:col-span-4">
                <Label htmlFor={`description-${index}`}>Descrição</Label>
                <Input
                  id={`description-${index}`}
                  value={item.description}
                  onChange={(e) => handleChange(index, 'description', e.target.value)}
                  placeholder="Ex: Vale Transporte"
                />
              </div>

              {/* Tipo */}
              <div className="md:col-span-2">
                <Label htmlFor={`type-${index}`}>Tipo</Label>
                <Select
                  id={`type-${index}`}
                  value={item.type}
                  onChange={(e) => handleChange(index, 'type', e.target.value)}
                >
                  {TYPES.map((type) => (
                    <option key={type.value} value={type.value}>
                      {type.label}
                    </option>
                  ))}
                </Select>
              </div>

              {/* Aplicação */}
              <div className="md:col-span-3">
                <Label htmlFor={`application-${index}`}>Aplicação</Label>
                <Select
                  id={`application-${index}`}
                  value={migrateApplicationTypeValue(item.application)}
                  onChange={(e) => handleChange(index, 'application', e.target.value)}
                >
                  {APPLICATION_TYPE_OPTIONS.map((app) => (
                    <option key={app.value} value={app.value}>
                      {app.label}
                    </option>
                  ))}
                </Select>
              </div>

              {/* Valor */}
              <div className="md:col-span-2">
                <Label htmlFor={`amount-${index}`}>Valor</Label>
                <CurrencyInput
                  id={`amount-${index}`}
                  value={item.amount.toString()}
                  onChange={(value) => handleChange(index, 'amount', Number(value))}
                />
              </div>

              {/* Botão Remover */}
              <div className="md:col-span-1 flex items-end">
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  onClick={() => handleRemove(index)}
                  className="text-destructive hover:text-destructive"
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            </div>
          ))}

          {/* Totalizadores */}
          <div className="border-t pt-4 space-y-2">
            <div className="flex justify-between text-sm">
              <span className="text-green-600 font-medium">Total Benefícios:</span>
              <span className="text-green-600 font-bold">
                R$ {getTotalBenefits().toFixed(2)}
              </span>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-red-600 font-medium">Total Descontos:</span>
              <span className="text-red-600 font-bold">
                R$ {getTotalDiscounts().toFixed(2)}
              </span>
            </div>
            <div className="flex justify-between text-base border-t pt-2">
              <span className="font-semibold">Valor Líquido:</span>
              <span className={`font-bold ${getNetTotal() >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                R$ {getNetTotal().toFixed(2)}
              </span>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
