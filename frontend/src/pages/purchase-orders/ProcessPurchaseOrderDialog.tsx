import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../../components/ui/Dialog';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { useToast } from '../../contexts/ToastContext';
import { parseBackendError } from '../../utils/errorHandler';
import purchaseOrderService, { type PurchaseOrder, type ProcessPurchaseOrderInput, type CostCenterDistribution } from '../../services/purchaseOrderService';
import accountService, { type Account } from '../../services/accountService';
import costCenterService, { type CostCenter } from '../../services/costCenterService';
import companySettingService from '../../services/companySettingService';
import { Check, X, Plus, Trash2 } from 'lucide-react';

interface ProcessPurchaseOrderDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  purchaseOrder: PurchaseOrder | null;
  onSuccess: () => void;
}

export function ProcessPurchaseOrderDialog({
  open,
  onOpenChange,
  purchaseOrder,
  onSuccess
}: ProcessPurchaseOrderDialogProps) {
  const { showSuccess, showError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [costCenters, setCostCenters] = useState<CostCenter[]>([]);
  const [distributions, setDistributions] = useState<CostCenterDistribution[]>([]);
  
  const [formData, setFormData] = useState({
    status: 'Aprovado' as 'Aprovado' | 'Rejeitado',
    processedMessage: '',
    processedAt: new Date().toISOString().slice(0, 16),
    transactionDescription: '',
    accountId: undefined as number | undefined
  });

  useEffect(() => {
    if (open && purchaseOrder) {
      loadData();
      setFormData(prev => ({
        ...prev,
        transactionDescription: purchaseOrder.description,
        processedAt: new Date().toISOString().slice(0, 16)
      }));
    }
  }, [open, purchaseOrder]);

  const loadData = async () => {
    try {
      const [accountsResult, centersData, defaultDist] = await Promise.all([
        accountService.getAccounts({ pageSize: 100 }),
        costCenterService.getAllCostCenters(),
        companySettingService.getDefaultDistributions()
      ]);
      
      setAccounts(accountsResult.items);
      setCostCenters(centersData);
      
      // Carregar rateio padrão
      if (defaultDist && defaultDist.length > 0) {
        setDistributions(defaultDist.map((d: { costCenterId: number; percentage: number }) => ({
          costCenterId: d.costCenterId,
          percentage: d.percentage
        })));
      } else {
        setDistributions([]);
      }
    } catch (err) {
      console.error('Erro ao carregar dados:', err);
    }
  };

  const handleAddDistribution = () => {
    const availableCostCenters = costCenters.filter(
      cc => !distributions.some(d => d.costCenterId === cc.costCenterId)
    );
    
    if (availableCostCenters.length === 0) {
      showError('Todos os centros de custo já foram adicionados');
      return;
    }

    setDistributions(prev => [...prev, {
      costCenterId: availableCostCenters[0].costCenterId,
      percentage: 0
    }]);
  };

  const handleRemoveDistribution = (index: number) => {
    setDistributions(prev => prev.filter((_, i) => i !== index));
  };

  const handleDistributionChange = (index: number, field: 'costCenterId' | 'percentage', value: any) => {
    setDistributions(prev => {
      const updated = [...prev];
      updated[index] = { ...updated[index], [field]: Number(value) };
      return updated;
    });
  };

  const getTotalPercentage = () => {
    return distributions.reduce((sum, d) => sum + d.percentage, 0);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!purchaseOrder) return;

    // Validação para aprovação
    if (formData.status === 'Aprovado') {
      if (!formData.accountId) {
        showError('Selecione uma conta corrente para aprovar');
        return;
      }
      
      const total = getTotalPercentage();
      if (distributions.length > 0 && Math.abs(total - 100) > 0.01) {
        showError(`A soma das porcentagens deve ser 100%. Atual: ${total.toFixed(2)}%`);
        return;
      }
    }

    setIsLoading(true);
    try {
      const input: ProcessPurchaseOrderInput = {
        status: formData.status,
        processedMessage: formData.processedMessage || undefined,
        processedAt: new Date(formData.processedAt).toISOString(),
        transactionDescription: formData.status === 'Aprovado' ? formData.transactionDescription : undefined,
        accountId: formData.status === 'Aprovado' ? formData.accountId : undefined,
        costCenterDistributions: formData.status === 'Aprovado' && distributions.length > 0 ? distributions : undefined
      };

      await purchaseOrderService.processPurchaseOrder(purchaseOrder.purchaseOrderId, input);
      showSuccess(`Ordem de compra ${formData.status === 'Aprovado' ? 'aprovada' : 'rejeitada'} com sucesso!`);
      onOpenChange(false);
      onSuccess();
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsLoading(false);
    }
  };

  if (!purchaseOrder) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Processar Ordem de Compra #{purchaseOrder.purchaseOrderId}</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="px-6 pb-6 space-y-4">
          {/* Status */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Decisão</label>
            <div className="flex gap-3">
              <Button
                type="button"
                variant={formData.status === 'Aprovado' ? 'default' : 'outline'}
                onClick={() => setFormData(prev => ({ ...prev, status: 'Aprovado' }))}
                className={formData.status === 'Aprovado' ? 'bg-green-600 hover:bg-green-700' : ''}
              >
                <Check className="h-4 w-4 mr-2" />
                Aprovar
              </Button>
              <Button
                type="button"
                variant={formData.status === 'Rejeitado' ? 'default' : 'outline'}
                onClick={() => setFormData(prev => ({ ...prev, status: 'Rejeitado' }))}
                className={formData.status === 'Rejeitado' ? 'bg-red-600 hover:bg-red-700' : ''}
              >
                <X className="h-4 w-4 mr-2" />
                Rejeitar
              </Button>
            </div>
          </div>

          {/* Data/Hora do Processamento */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Data/Hora do Processamento *
            </label>
            <Input
              type="datetime-local"
              value={formData.processedAt}
              onChange={(e) => setFormData(prev => ({ ...prev, processedAt: e.target.value }))}
              required
            />
          </div>

          {/* Mensagem */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Mensagem / Justificativa
            </label>
            <textarea
              value={formData.processedMessage}
              onChange={(e) => setFormData(prev => ({ ...prev, processedMessage: e.target.value }))}
              className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
              rows={3}
              placeholder={formData.status === 'Rejeitado' ? 'Motivo da rejeição...' : 'Observações...'}
            />
          </div>

          {/* Campos específicos para aprovação */}
          {formData.status === 'Aprovado' && (
            <>
              {/* Descrição da Transação */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Descrição da Transação
                </label>
                <Input
                  value={formData.transactionDescription}
                  onChange={(e) => setFormData(prev => ({ ...prev, transactionDescription: e.target.value }))}
                  placeholder="Descrição para a transação financeira"
                />
                <p className="text-xs text-gray-500 mt-1">
                  Se alterar, a descrição da ordem também será atualizada
                </p>
              </div>

              {/* Conta Corrente */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Conta Corrente *
                </label>
                <select
                  value={formData.accountId || ''}
                  onChange={(e) => setFormData(prev => ({ ...prev, accountId: Number(e.target.value) || undefined }))}
                  className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
                  required
                >
                  <option value="">Selecione uma conta...</option>
                  {accounts.map(acc => (
                    <option key={acc.accountId} value={acc.accountId}>
                      {acc.name} ({acc.type})
                    </option>
                  ))}
                </select>
              </div>

              {/* Rateio de Centros de Custo */}
              <div>
                <div className="flex items-center justify-between mb-2">
                  <label className="block text-sm font-medium text-gray-700">
                    Rateio de Centros de Custo
                  </label>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={handleAddDistribution}
                  >
                    <Plus className="h-4 w-4 mr-1" />
                    Adicionar
                  </Button>
                </div>

                {distributions.length === 0 ? (
                  <p className="text-sm text-gray-500 text-center py-4">
                    Nenhum rateio configurado
                  </p>
                ) : (
                  <div className="space-y-2">
                    {distributions.map((dist, index) => (
                      <div key={index} className="flex items-center gap-2">
                        <select
                          value={dist.costCenterId}
                          onChange={(e) => handleDistributionChange(index, 'costCenterId', e.target.value)}
                          className="flex-1 rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
                        >
                          {costCenters.map(cc => (
                            <option 
                              key={cc.costCenterId} 
                              value={cc.costCenterId}
                              disabled={distributions.some((d, i) => i !== index && d.costCenterId === cc.costCenterId)}
                            >
                              {cc.name}
                            </option>
                          ))}
                        </select>
                        <div className="w-24">
                          <Input
                            type="number"
                            min="0"
                            max="100"
                            step="0.01"
                            value={dist.percentage}
                            onChange={(e) => handleDistributionChange(index, 'percentage', e.target.value)}
                            className="text-right"
                          />
                        </div>
                        <span className="text-gray-500 text-sm">%</span>
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          onClick={() => handleRemoveDistribution(index)}
                          className="text-red-500 hover:text-red-700 hover:bg-red-50"
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    ))}

                    <div className="flex justify-end items-center gap-2 pt-2 border-t">
                      <span className="text-sm font-medium text-gray-700">Total:</span>
                      <span className={`font-bold ${Math.abs(getTotalPercentage() - 100) < 0.01 ? 'text-green-600' : 'text-red-600'}`}>
                        {getTotalPercentage().toFixed(2)}%
                      </span>
                    </div>
                  </div>
                )}
              </div>
            </>
          )}

          {/* Actions */}
          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isLoading}
            >
              Cancelar
            </Button>
            <Button
              type="submit"
              disabled={isLoading}
              className={formData.status === 'Aprovado' ? 'bg-green-600 hover:bg-green-700' : 'bg-red-600 hover:bg-red-700'}
            >
              {isLoading ? 'Processando...' : formData.status === 'Aprovado' ? 'Aprovar' : 'Rejeitar'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
