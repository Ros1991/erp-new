import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { PhoneInput } from '../../components/ui/PhoneInput';
import { DocumentInput, sanitizeDocument } from '../../components/ui/DocumentInput';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import supplierCustomerService from '../../services/supplierCustomerService';
import { ArrowLeft, Save } from 'lucide-react';

interface SupplierCustomerFormData {
  name: string;
  document: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
}

export function SupplierCustomerForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<SupplierCustomerFormData>({
    name: '',
    document: '',
    email: '',
    phone: '',
    address: '',
    isActive: true,
  });

  const [errors, setErrors] = useState<Partial<Record<keyof SupplierCustomerFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadSupplierCustomer();
    }
  }, [id]);

  const loadSupplierCustomer = async () => {
    setIsLoading(true);
    try {
      const supplierCustomer = await supplierCustomerService.getSupplierCustomerById(Number(id));
      setFormData({
        name: supplierCustomer.name,
        document: supplierCustomer.document || '',
        email: supplierCustomer.email || '',
        phone: supplierCustomer.phone || '',
        address: supplierCustomer.address || '',
        isActive: supplierCustomer.isActive,
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof SupplierCustomerFormData, string>> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Nome é obrigatório';
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
      const supplierCustomerData = {
        name: formData.name.trim(),
        document: formData.document ? sanitizeDocument(formData.document) : undefined,
        email: formData.email.trim() || undefined,
        phone: formData.phone || undefined,
        address: formData.address.trim() || undefined,
        isActive: formData.isActive,
      };

      if (isEditing) {
        await supplierCustomerService.updateSupplierCustomer(Number(id), supplierCustomerData);
        showSuccess('Fornecedor e Cliente atualizado com sucesso!');
      } else {
        await supplierCustomerService.createSupplierCustomer(supplierCustomerData);
        showSuccess('Fornecedor e Cliente criado com sucesso!');
      }

      navigate('/supplier-customers');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof SupplierCustomerFormData, value: string | boolean) => {
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
            onClick={() => navigate('/supplier-customers')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Fornecedor e Cliente' : 'Novo Fornecedor e Cliente'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações do fornecedor e cliente' : 'Preencha as informações para criar um novo fornecedor e cliente'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                    Nome <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="name"
                    type="text"
                    value={formData.name}
                    onChange={(e) => handleChange('name', e.target.value)}
                    placeholder="Ex: Empresa XYZ Ltda"
                    className={errors.name ? 'border-red-500' : ''}
                  />
                  {errors.name && <p className="text-sm text-red-600 mt-1">{errors.name}</p>}
                </div>

                <div>
                  <label htmlFor="document" className="block text-sm font-medium text-gray-700 mb-1">
                    Documento (CPF/CNPJ)
                  </label>
                  <DocumentInput
                    id="document"
                    value={formData.document}
                    onChange={(value) => handleChange('document', value)}
                  />
                  <p className="text-xs text-gray-500 mt-1">Digite com ou sem formatação (pontos e barras serão removidos)</p>
                </div>

                <div>
                  <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
                    E-mail
                  </label>
                  <Input
                    id="email"
                    type="email"
                    value={formData.email}
                    onChange={(e) => handleChange('email', e.target.value)}
                    placeholder="exemplo@email.com"
                  />
                </div>

                <div>
                  <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
                    Telefone
                  </label>
                  <PhoneInput
                    id="phone"
                    value={formData.phone}
                    onChange={(value) => handleChange('phone', value)}
                  />
                </div>

                <div>
                  <label htmlFor="address" className="block text-sm font-medium text-gray-700 mb-1">
                    Endereço
                  </label>
                  <textarea
                    id="address"
                    value={formData.address}
                    onChange={(e) => handleChange('address', e.target.value)}
                    placeholder="Rua, número, bairro, cidade, estado"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    rows={3}
                  />
                </div>

                <div className="flex items-center">
                  <input
                    id="isActive"
                    type="checkbox"
                    checked={formData.isActive}
                    onChange={(e) => handleChange('isActive', e.target.checked)}
                    className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                  />
                  <label htmlFor="isActive" className="ml-2 block text-sm text-gray-700">
                    Fornecedor e Cliente ativo
                  </label>
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
                      {isEditing ? 'Atualizar' : 'Criar'} Fornecedor e Cliente
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate('/supplier-customers')}
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
