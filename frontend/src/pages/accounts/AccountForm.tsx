import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import accountService from '../../services/accountService';
import { ArrowLeft, Save } from 'lucide-react';

interface AccountFormData {
  name: string;
  type: string;
  initialBalance: string;
}

export function AccountForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<AccountFormData>({
    name: '',
    type: '',
    initialBalance: '0',
  });

  const [errors, setErrors] = useState<Partial<Record<keyof AccountFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadAccount();
    }
  }, [id]);

  const loadAccount = async () => {
    setIsLoading(true);
    try {
      const account = await accountService.getAccountById(Number(id));
      setFormData({
        name: account.name,
        type: account.type,
        initialBalance: account.initialBalance.toString(),
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof AccountFormData, string>> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Nome é obrigatório';
    }

    if (!formData.type.trim()) {
      newErrors.type = 'Tipo é obrigatório';
    }

    if (!formData.initialBalance.trim()) {
      newErrors.initialBalance = 'Saldo inicial é obrigatório';
    } else if (isNaN(Number(formData.initialBalance))) {
      newErrors.initialBalance = 'Saldo inicial deve ser um número válido';
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
      const accountData = {
        name: formData.name.trim(),
        type: formData.type.trim(),
        initialBalance: Number(formData.initialBalance),
      };

      if (isEditing) {
        await accountService.updateAccount(Number(id), accountData);
        showSuccess('Conta atualizada com sucesso!');
      } else {
        await accountService.createAccount(accountData);
        showSuccess('Conta criada com sucesso!');
      }

      navigate('/accounts');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof AccountFormData, value: string) => {
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
            onClick={() => navigate('/accounts')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Conta' : 'Nova Conta'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações da conta' : 'Preencha as informações para criar uma nova conta'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                    Nome da Conta <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="name"
                    type="text"
                    value={formData.name}
                    onChange={(e) => handleChange('name', e.target.value)}
                    placeholder="Ex: Banco do Brasil - Conta Corrente"
                    className={errors.name ? 'border-red-500' : ''}
                  />
                  {errors.name && <p className="text-sm text-red-600 mt-1">{errors.name}</p>}
                </div>

                <div>
                  <label htmlFor="type" className="block text-sm font-medium text-gray-700 mb-1">
                    Tipo <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="type"
                    type="text"
                    value={formData.type}
                    onChange={(e) => handleChange('type', e.target.value)}
                    placeholder="Ex: Conta Corrente, Poupança, Caixa"
                    className={errors.type ? 'border-red-500' : ''}
                  />
                  {errors.type && <p className="text-sm text-red-600 mt-1">{errors.type}</p>}
                </div>

                <div>
                  <label htmlFor="initialBalance" className="block text-sm font-medium text-gray-700 mb-1">
                    Saldo Inicial (R$) <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="initialBalance"
                    type="text"
                    value={formData.initialBalance}
                    onChange={(e) => handleChange('initialBalance', e.target.value)}
                    placeholder="0.00"
                    className={errors.initialBalance ? 'border-red-500' : ''}
                  />
                  {errors.initialBalance && <p className="text-sm text-red-600 mt-1">{errors.initialBalance}</p>}
                  <p className="text-sm text-gray-500 mt-1">Digite o valor em centavos (ex: 1000 = R$ 10,00)</p>
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
                      {isEditing ? 'Atualizar' : 'Criar'} Conta
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate('/accounts')}
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
