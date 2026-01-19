import { useEffect, useState, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { CostCenterDistribution, type CostCenterDistributionItem } from '../../components/ui/CostCenterDistribution';
import { useToast } from '../../contexts/ToastContext';
import { parseBackendError } from '../../utils/errorHandler';
import companySettingService, { type CompanySettingInput } from '../../services/companySettingService';
import costCenterService from '../../services/costCenterService';
import { Save, Settings } from 'lucide-react';

const DUMMY_AMOUNT = 10000; // R$ 100,00 para cálculo visual

export function CompanySettings() {
  const { showError, showSuccess } = useToast();
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [distributions, setDistributions] = useState<CostCenterDistributionItem[]>([]);
  const [costCenterCount, setCostCenterCount] = useState(0);
  const [formData, setFormData] = useState<CompanySettingInput>({
    timeToleranceMinutes: 10,
    payrollDay: 5,
    payrollClosingDay: 25,
    vacationDaysPerYear: 30,
    minHoursForLunchBreak: 6,
    maxOvertimeHoursPerMonth: 44,
    allowWeekendWork: false,
    requireJustificationAfterHours: 2,
    weeklyHoursDefault: 44
  });

  const loadData = useCallback(async () => {
    setIsLoading(true);
    try {
      const [settings, costCentersData] = await Promise.all([
        companySettingService.getSettings(),
        costCenterService.getCostCenters({ page: 1, pageSize: 1 })
      ]);
      
      setCostCenterCount(costCentersData.totalCount);
      
      setFormData({
        employeeIdGeneralManager: settings.employeeIdGeneralManager,
        timeToleranceMinutes: settings.timeToleranceMinutes,
        payrollDay: settings.payrollDay,
        payrollClosingDay: settings.payrollClosingDay,
        vacationDaysPerYear: settings.vacationDaysPerYear,
        minHoursForLunchBreak: settings.minHoursForLunchBreak,
        maxOvertimeHoursPerMonth: settings.maxOvertimeHoursPerMonth,
        allowWeekendWork: settings.allowWeekendWork,
        requireJustificationAfterHours: settings.requireJustificationAfterHours,
        weeklyHoursDefault: settings.weeklyHoursDefault
      });

      // Converter para o formato do componente CostCenterDistribution
      setDistributions(settings.defaultCostCenterDistributions?.map((d: { costCenterId: number; costCenterName?: string; percentage: number }) => ({
        costCenterId: d.costCenterId.toString(),
        costCenterName: d.costCenterName || '',
        percentage: d.percentage,
        amount: Math.round((DUMMY_AMOUNT * d.percentage) / 100)
      })) || []);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsLoading(false);
    }
  }, [showError]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleChange = (field: keyof CompanySettingInput, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleDistributionsChange = (newDistributions: CostCenterDistributionItem[]) => {
    setDistributions(newDistributions);
  };

  const getTotalPercentage = () => {
    return distributions.reduce((sum, d) => sum + d.percentage, 0);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const total = getTotalPercentage();
    if (distributions.length > 0 && Math.abs(total - 100) > 0.01) {
      showError(`A soma das porcentagens deve ser 100%. Atual: ${total.toFixed(2)}%`);
      return;
    }

    setIsSaving(true);
    try {
      await companySettingService.saveSettings({
        ...formData,
        defaultCostCenterDistributions: distributions.map(d => ({
          costCenterId: Number(d.costCenterId),
          percentage: d.percentage
        }))
      });
      showSuccess('Configurações salvas com sucesso!');
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <MainLayout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-4xl mx-auto">
        <div className="mb-6">
          <div className="flex items-center gap-3">
            <Settings className="h-8 w-8 text-primary-600" />
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Configurações da Empresa</h1>
              <p className="text-gray-600 mt-1">Gerencie as configurações gerais e o rateio padrão</p>
            </div>
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Configurações de Folha de Pagamento */}
          <Card>
            <CardContent className="p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Configurações de Folha de Pagamento</h2>
              
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Dia do Pagamento
                  </label>
                  <Input
                    type="number"
                    min="1"
                    max="31"
                    value={formData.payrollDay}
                    onChange={(e) => handleChange('payrollDay', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Dia de Fechamento
                  </label>
                  <Input
                    type="number"
                    min="1"
                    max="31"
                    value={formData.payrollClosingDay}
                    onChange={(e) => handleChange('payrollClosingDay', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Horas Semanais Padrão
                  </label>
                  <Input
                    type="number"
                    min="1"
                    max="60"
                    value={formData.weeklyHoursDefault}
                    onChange={(e) => handleChange('weeklyHoursDefault', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Dias de Férias/Ano
                  </label>
                  <Input
                    type="number"
                    min="0"
                    max="60"
                    value={formData.vacationDaysPerYear}
                    onChange={(e) => handleChange('vacationDaysPerYear', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Máx. Horas Extras/Mês
                  </label>
                  <Input
                    type="number"
                    min="0"
                    max="100"
                    value={formData.maxOvertimeHoursPerMonth}
                    onChange={(e) => handleChange('maxOvertimeHoursPerMonth', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Tolerância (minutos)
                  </label>
                  <Input
                    type="number"
                    min="0"
                    max="60"
                    value={formData.timeToleranceMinutes}
                    onChange={(e) => handleChange('timeToleranceMinutes', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Horas Mín. p/ Almoço
                  </label>
                  <Input
                    type="number"
                    min="0"
                    max="12"
                    value={formData.minHoursForLunchBreak}
                    onChange={(e) => handleChange('minHoursForLunchBreak', Number(e.target.value))}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Horas p/ Justificativa
                  </label>
                  <Input
                    type="number"
                    min="0"
                    max="24"
                    value={formData.requireJustificationAfterHours}
                    onChange={(e) => handleChange('requireJustificationAfterHours', Number(e.target.value))}
                  />
                </div>

                <div className="flex items-center">
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={formData.allowWeekendWork}
                      onChange={(e) => handleChange('allowWeekendWork', e.target.checked)}
                      className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                    />
                    <span className="text-sm font-medium text-gray-700">Permite Trabalho Fim de Semana</span>
                  </label>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Rateio Padrão - Só mostra se houver mais de 1 centro de custo */}
          {costCenterCount > 1 && (
            <Card>
              <CardContent className="p-6">
                <div className="mb-4">
                  <div className="flex items-center gap-2">
                    <Settings className="h-5 w-5 text-primary-600" />
                    <h2 className="text-lg font-semibold text-gray-900">Rateio Padrão de Centros de Custo</h2>
                  </div>
                  <p className="text-sm text-gray-500 mt-2">
                    Configure a distribuição padrão para sugestão em transações financeiras e ordens de compra.
                  </p>
                </div>

                <CostCenterDistribution
                  totalAmount={DUMMY_AMOUNT}
                  distributions={distributions}
                  onChange={handleDistributionsChange}
                />
              </CardContent>
            </Card>
          )}

          {/* Actions */}
          <div className="flex justify-end">
            <Button type="submit" disabled={isSaving}>
              <Save className="h-4 w-4 mr-2" />
              {isSaving ? 'Salvando...' : 'Salvar Configurações'}
            </Button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
