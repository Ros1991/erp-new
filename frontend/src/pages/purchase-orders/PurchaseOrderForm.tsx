import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import purchaseOrderService from '../../services/purchaseOrderService';
import { ArrowLeft, Save } from 'lucide-react';

interface PurchaseOrderFormData {
  userIdRequester: string;
  userIdApprover: string;
  description: string;
  totalAmount: string;
  status: string;
}

export function PurchaseOrderForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<PurchaseOrderFormData>({
    userIdRequester: '',
    userIdApprover: '',
    description: '',
    totalAmount: '0',
    status: 'Pendente',
  });

  const [errors, setErrors] = useState<Partial<Record<keyof PurchaseOrderFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadPurchaseOrder();
    }
  }, [id]);

  const loadPurchaseOrder = async () => {
    setIsLoading(true);
    try {
      const purchaseOrder = await purchaseOrderService.getPurchaseOrderById(Number(id));
      setFormData({
        userIdRequester: purchaseOrder.userIdRequester.toString(),
        userIdApprover: purchaseOrder.userIdApprover?.toString() || '',
        description: purchaseOrder.description,
        totalAmount: purchaseOrder.totalAmount.toString(),
        status: purchaseOrder.status,
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof PurchaseOrderFormData, string>> = {};

    if (!formData.userIdRequester.trim()) {
      newErrors.userIdRequester = 'ID do solicitante é obrigatório';
    } else if (isNaN(Number(formData.userIdRequester))) {
      newErrors.userIdRequester = 'ID do solicitante deve ser um número válido';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'Descrição é obrigatória';
    }

    if (!formData.totalAmount.trim()) {
      newErrors.totalAmount = 'Valor total é obrigatório';
    } else if (isNaN(Number(formData.totalAmount)) || Number(formData.totalAmount) <= 0) {
      newErrors.totalAmount = 'Valor total deve ser um número positivo';
    }

    if (!formData.status.trim()) {
      newErrors.status = 'Status é obrigatório';
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

    setIsSaving(true);
    try {
      const purchaseOrderData = {
        userIdRequester: Number(formData.userIdRequester),
        userIdApprover: formData.userIdApprover ? Number(formData.userIdApprover) : undefined,
        description: formData.description.trim(),
        totalAmount: Number(formData.totalAmount),
        status: formData.status.trim(),
      };

      if (isEditing) {
        await purchaseOrderService.updatePurchaseOrder(Number(id), purchaseOrderData);
        showSuccess('Ordem de compra atualizada com sucesso!');
      } else {
        await purchaseOrderService.createPurchaseOrder(purchaseOrderData);
        showSuccess('Ordem de compra criada com sucesso!');
      }

      navigate('/purchase-orders');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof PurchaseOrderFormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  if (isLoading) {
    return (
      <MainLayout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-2xl mx-auto">
        <div className="mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/purchase-orders')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Ordem de Compra' : 'Nova Ordem de Compra'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações da ordem de compra' : 'Preencha as informações para criar uma nova ordem de compra'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="userIdRequester" className="block text-sm font-medium text-gray-700 mb-1">
                    ID do Solicitante <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="userIdRequester"
                    type="text"
                    value={formData.userIdRequester}
                    onChange={(e) => handleChange('userIdRequester', e.target.value)}
                    placeholder="ID do usuário solicitante"
                    className={errors.userIdRequester ? 'border-red-500' : ''}
                  />
                  {errors.userIdRequester && <p className="text-sm text-red-600 mt-1">{errors.userIdRequester}</p>}
                </div>

                <div>
                  <label htmlFor="userIdApprover" className="block text-sm font-medium text-gray-700 mb-1">
                    ID do Aprovador (opcional)
                  </label>
                  <Input
                    id="userIdApprover"
                    type="text"
                    value={formData.userIdApprover}
                    onChange={(e) => handleChange('userIdApprover', e.target.value)}
                    placeholder="ID do usuário aprovador"
                  />
                </div>

                <div>
                  <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
                    Descrição <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    id="description"
                    value={formData.description}
                    onChange={(e) => handleChange('description', e.target.value)}
                    placeholder="Descrição da ordem de compra"
                    className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 ${errors.description ? 'border-red-500' : 'border-gray-300'}`}
                    rows={4}
                  />
                  {errors.description && <p className="text-sm text-red-600 mt-1">{errors.description}</p>}
                </div>

                <div>
                  <label htmlFor="totalAmount" className="block text-sm font-medium text-gray-700 mb-1">
                    Valor Total (R$) <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="totalAmount"
                    type="text"
                    value={formData.totalAmount}
                    onChange={(e) => handleChange('totalAmount', e.target.value)}
                    placeholder="0.00"
                    className={errors.totalAmount ? 'border-red-500' : ''}
                  />
                  {errors.totalAmount && <p className="text-sm text-red-600 mt-1">{errors.totalAmount}</p>}
                  <p className="text-sm text-gray-500 mt-1">Digite o valor em centavos (ex: 150000 = R$ 1.500,00)</p>
                </div>

                <div>
                  <label htmlFor="status" className="block text-sm font-medium text-gray-700 mb-1">
                    Status <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="status"
                    type="text"
                    value={formData.status}
                    onChange={(e) => handleChange('status', e.target.value)}
                    placeholder="Ex: Pendente, Aprovado, Rejeitado"
                    className={errors.status ? 'border-red-500' : ''}
                  />
                  {errors.status && <p className="text-sm text-red-600 mt-1">{errors.status}</p>}
                </div>
              </div>

            </CardContent>
          </Card>

          <div className="flex gap-3 justify-end">
                <Button type="submit" disabled={isSaving} className="flex-1 sm:flex-none">
                  {isSaving ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Salvando...
                    </>
                  ) : (
                    <>
                      <Save className="h-4 w-4 mr-2" />
                      {isEditing ? 'Atualizar' : 'Criar'} Ordem de Compra
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate('/purchase-orders')}
                  disabled={isSaving}
                  className="flex-1 sm:flex-none"
                >
                  Cancelar
                </Button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
