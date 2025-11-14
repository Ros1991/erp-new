import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Label } from '../../components/ui/Label';
import { Select } from '../../components/ui/Select';
import { CurrencyInput } from '../../components/ui/CurrencyInput';
import { useToast } from '../../contexts/ToastContext';
import contractService, { type ContractInput } from '../../services/contractService';
import employeeService from '../../services/employeeService';
import { BenefitDiscountList, type BenefitDiscountItem } from '../../components/ui/BenefitDiscountList';
import { CostCenterDistribution, type CostCenterDistributionItem } from '../../components/ui/CostCenterDistribution';
import { toUTCString } from '../../utils/dateUtils';

interface ContractFormData {
  type: string;
  value: string;
  isPayroll: boolean;
  hasInss: boolean;
  hasIrrf: boolean;
  hasFgts: boolean;
  startDate: string;
  endDate: string;
  weeklyHours: string;
  isActive: boolean;
}

export function ContractForm() {
  const { employeeId, contractId } = useParams<{ employeeId: string; contractId: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [employeeName, setEmployeeName] = useState('');
  
  const [formData, setFormData] = useState<ContractFormData>({
    type: 'CLT',
    value: '0',
    isPayroll: true,
    hasInss: true,
    hasIrrf: true,
    hasFgts: true,
    startDate: new Date().toISOString().split('T')[0],
    endDate: '',
    weeklyHours: '40',
    isActive: true,
  });

  const [benefitsDiscounts, setBenefitsDiscounts] = useState<BenefitDiscountItem[]>([]);
  const [costCenters, setCostCenters] = useState<CostCenterDistributionItem[]>([]);
  const [errors, setErrors] = useState<Partial<Record<keyof ContractFormData, string>>>({});

  const isEditing = !!contractId;

  useEffect(() => {
    if (employeeId) {
      loadEmployee();
    }
    if (isEditing && contractId) {
      loadContract();
    }
  }, [employeeId, contractId]);

  const loadEmployee = async () => {
    try {
      const employee = await employeeService.getEmployeeById(Number(employeeId));
      setEmployeeName(employee.fullName);
    } catch (err: any) {
      handleBackendError(err);
    }
  };

  const loadContract = async () => {
    setIsLoading(true);
    try {
      const contract = await contractService.getContractById(Number(contractId));
      setFormData({
        type: contract.type,
        value: (contract.value * 100).toString(), // Converter de reais para centavos
        isPayroll: contract.isPayroll,
        hasInss: contract.hasInss,
        hasIrrf: contract.hasIrrf,
        hasFgts: contract.hasFgts,
        startDate: contract.startDate.split('T')[0],
        endDate: contract.endDate ? contract.endDate.split('T')[0] : '',
        weeklyHours: contract.weeklyHours?.toString() || '',
        isActive: contract.isActive,
      });

      if (contract.benefitsDiscounts && contract.benefitsDiscounts.length > 0) {
        setBenefitsDiscounts(
          contract.benefitsDiscounts.map((b) => ({
            description: b.description,
            type: b.type,
            application: b.application,
            amount: b.amount * 100, // Converter de reais para centavos
          }))
        );
      }

      if (contract.costCenters && contract.costCenters.length > 0) {
        setCostCenters(
          contract.costCenters.map((c) => ({
            costCenterId: c.costCenterId.toString(),
            costCenterName: c.costCenterName || '',
            percentage: c.percentage,
            amount: 0,
          }))
        );
      }
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleInputChange = (field: keyof ContractFormData, value: string | boolean) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors((prev) => ({ ...prev, [field]: undefined }));
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof ContractFormData, string>> = {};

    if (!formData.type.trim()) {
      newErrors.type = 'Tipo é obrigatório';
    }

    if (!formData.value.trim()) {
      newErrors.value = 'Valor é obrigatório';
    } else if (isNaN(Number(formData.value)) || Number(formData.value) <= 0) {
      newErrors.value = 'Valor deve ser maior que zero';
    }

    if (!formData.startDate) {
      newErrors.startDate = 'Data de início é obrigatória';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      showError('Por favor, corrija os erros no formulário');
      return;
    }

    // Validar centros de custo se houver
    if (costCenters.length > 0) {
      const totalPercentage = costCenters.reduce((sum, c) => sum + c.percentage, 0);
      
      if (Math.abs(totalPercentage - 100) > 0.01) {
        showError('A soma das porcentagens dos centros de custo deve ser 100%');
        return;
      }

      const hasEmptyCostCenter = costCenters.some((c) => !c.costCenterId);
      if (hasEmptyCostCenter) {
        showError('Selecione um centro de custo para todas as distribuições');
        return;
      }
    }

    setIsSaving(true);
    try {
      const contractData: ContractInput = {
        employeeId: Number(employeeId),
        type: formData.type,
        value: Number(formData.value) / 100, // Converter de centavos para reais
        isPayroll: formData.isPayroll,
        hasInss: formData.hasInss,
        hasIrrf: formData.hasIrrf,
        hasFgts: formData.hasFgts,
        startDate: toUTCString(new Date(formData.startDate))!,
        endDate: formData.endDate ? toUTCString(new Date(formData.endDate))! : undefined,
        weeklyHours: formData.weeklyHours ? Number(formData.weeklyHours) : undefined,
        isActive: formData.isActive,
        benefitsDiscounts:
          benefitsDiscounts.length > 0
            ? benefitsDiscounts.map((b) => ({
                description: b.description,
                type: b.type,
                application: b.application,
                amount: b.amount / 100, // Converter de centavos para reais
              }))
            : undefined,
        costCenters:
          costCenters.length > 0
            ? costCenters.map((c) => ({
                costCenterId: Number(c.costCenterId),
                percentage: c.percentage,
              }))
            : undefined,
      };

      if (isEditing) {
        await contractService.updateContract(Number(contractId), contractData);
        showSuccess('Contrato atualizado com sucesso');
      } else {
        await contractService.createContract(contractData);
        showSuccess('Contrato criado com sucesso');
      }

      navigate(`/employees/${employeeId}/contracts`);
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-5xl">
      <Button
        variant="outline"
        onClick={() => navigate(`/employees/${employeeId}/contracts`)}
        className="mb-4"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar
      </Button>
      <div className="mb-6">
        <h1 className="text-3xl font-bold">{isEditing ? 'Editar Contrato' : 'Novo Contrato'}</h1>
        <p className="text-muted-foreground">{employeeName}</p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Dados Principais */}
        <Card>
          <CardHeader>
            <CardTitle>Dados do Contrato</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Tipo */}
              <div>
                <Label htmlFor="type">Tipo *</Label>
                <Select
                  id="type"
                  value={formData.type}
                  onChange={(e) => handleInputChange('type', e.target.value)}
                >
                  <option value="">Selecione o tipo</option>
                  <option value="CLT">CLT</option>
                  <option value="PJ">PJ</option>
                  <option value="Diarista">Diarista</option>
                  <option value="Horista">Horista</option>
                </Select>
                {errors.type && <p className="text-sm text-destructive mt-1">{errors.type}</p>}
              </div>

              {/* Valor */}
              <div>
                <Label htmlFor="value">Valor *</Label>
                <CurrencyInput
                  id="value"
                  value={formData.value}
                  onChange={(value) => handleInputChange('value', value)}
                  className={errors.value ? 'border-red-500' : ''}
                />
                {errors.value && <p className="text-sm text-destructive mt-1">{errors.value}</p>}
              </div>

              {/* Data Início */}
              <div>
                <Label htmlFor="startDate">Data de Início *</Label>
                <Input
                  id="startDate"
                  type="date"
                  value={formData.startDate}
                  onChange={(e) => handleInputChange('startDate', e.target.value)}
                />
                {errors.startDate && <p className="text-sm text-destructive mt-1">{errors.startDate}</p>}
              </div>

              {/* Data Fim */}
              <div>
                <Label htmlFor="endDate">Data de Término</Label>
                <Input
                  id="endDate"
                  type="date"
                  value={formData.endDate}
                  onChange={(e) => handleInputChange('endDate', e.target.value)}
                />
              </div>

              {/* Horas Semanais */}
              <div>
                <Label htmlFor="weeklyHours">Horas Semanais</Label>
                <Input
                  id="weeklyHours"
                  type="number"
                  min="0"
                  value={formData.weeklyHours}
                  onChange={(e) => handleInputChange('weeklyHours', e.target.value)}
                />
              </div>
            </div>

            {/* Checkboxes */}
            <div className="space-y-3 pt-4 border-t">
              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="isActive"
                  checked={formData.isActive}
                  onChange={(e) => handleInputChange('isActive', e.target.checked)}
                  className="w-4 h-4"
                />
                <Label htmlFor="isActive" className="cursor-pointer">Contrato Ativo</Label>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="isPayroll"
                  checked={formData.isPayroll}
                  onChange={(e) => handleInputChange('isPayroll', e.target.checked)}
                  className="w-4 h-4"
                />
                <Label htmlFor="isPayroll" className="cursor-pointer">É Folha de Pagamento</Label>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="hasInss"
                  checked={formData.hasInss}
                  onChange={(e) => handleInputChange('hasInss', e.target.checked)}
                  className="w-4 h-4"
                />
                <Label htmlFor="hasInss" className="cursor-pointer">Tem INSS</Label>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="hasIrrf"
                  checked={formData.hasIrrf}
                  onChange={(e) => handleInputChange('hasIrrf', e.target.checked)}
                  className="w-4 h-4"
                />
                <Label htmlFor="hasIrrf" className="cursor-pointer">Tem IRRF</Label>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="hasFgts"
                  checked={formData.hasFgts}
                  onChange={(e) => handleInputChange('hasFgts', e.target.checked)}
                  className="w-4 h-4"
                />
                <Label htmlFor="hasFgts" className="cursor-pointer">Tem FGTS</Label>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Benefícios e Descontos */}
        <Card>
          <CardContent className="p-6">
            <BenefitDiscountList items={benefitsDiscounts} onChange={setBenefitsDiscounts} />
          </CardContent>
        </Card>

        {/* Centros de Custo */}
        <Card>
          <CardContent className="p-6">
            <CostCenterDistribution
              totalAmount={Number(formData.value) / 100} // Converter de centavos para reais
              distributions={costCenters}
              onChange={setCostCenters}
            />
          </CardContent>
        </Card>

        {/* Botões */}
        <div className="flex gap-3 justify-end">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate(`/employees/${employeeId}/contracts`)}
            disabled={isSaving}
          >
            Cancelar
          </Button>
          <Button type="submit" disabled={isSaving}>
            {isSaving ? (
              <>
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Salvando...
              </>
            ) : (
              <>
                <Save className="h-4 w-4 mr-2" />
                Salvar
              </>
            )}
          </Button>
        </div>
      </form>
      </div>
    </MainLayout>
  );
}
