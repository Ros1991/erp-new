import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Building2, Save, Trash2 } from 'lucide-react';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { Input } from '../../components/ui/Input';
import { Label } from '../../components/ui/Label';
import { useToast } from '../../contexts/ToastContext';
import { useAuth } from '../../contexts/AuthContext';
import companyService, { type Company } from '../../services/companyService';
import { parseBackendError } from '../../utils/errorHandler';

export function CompanySettings() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { loadCompanies } = useAuth();
  const { showError, showSuccess, showValidationErrors } = useToast();

  const [company, setCompany] = useState<Company | null>(null);
  const [name, setName] = useState('');
  const [cnpj, setCnpj] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

  useEffect(() => {
    loadCompanyData();
  }, [id]);

  const loadCompanyData = async () => {
    if (!id) return;

    setIsLoading(true);
    try {
      const data = await companyService.getCompanyById(Number(id));
      setCompany(data);
      setName(data.name);
      setCnpj(data.document ? companyService.formatCNPJ(data.document) : '');
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
      navigate('/companies');
    } finally {
      setIsLoading(false);
    }
  };

  const formatCNPJ = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 14) {
      return numbers
        .replace(/^(\d{2})(\d)/, '$1.$2')
        .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
        .replace(/\.(\d{3})(\d)/, '.$1/$2')
        .replace(/(\d{4})(\d)/, '$1-$2');
    }
    return cnpj;
  };

  const handleCNPJChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatCNPJ(e.target.value);
    setCnpj(formatted);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!id || !company) return;

    // Validações frontend
    if (!name.trim()) {
      showError('Nome da empresa é obrigatório');
      return;
    }

    if (name.length > 255) {
      showError('Nome da empresa deve ter no máximo 255 caracteres');
      return;
    }

    // Validar CNPJ apenas se foi informado
    let cnpjNumbers = '';
    if (cnpj.trim()) {
      cnpjNumbers = cnpj.replace(/\D/g, '');
      
      if (cnpjNumbers.length !== 14) {
        showError('CNPJ deve ter 14 dígitos');
        return;
      }

      if (!companyService.validateCNPJ(cnpjNumbers)) {
        showError('CNPJ inválido');
        return;
      }
    }

    setIsSaving(true);

    try {
      await companyService.updateCompany(Number(id), {
        name: name.trim(),
        document: cnpjNumbers || undefined
      });

      showSuccess('Empresa atualizada com sucesso!');
      
      // Recarregar lista de empresas
      await loadCompanies();
      
      // Voltar para lista
      navigate('/companies');
    } catch (err: any) {
      const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
      
      if (hasValidationErrors && validationErrors) {
        showValidationErrors(validationErrors);
      } else {
        showError(message);
      }
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!id || !company) return;

    setIsDeleting(true);

    try {
      await companyService.deleteCompany(Number(id));

      showSuccess('Empresa deletada com sucesso!');
      
      // Recarregar lista de empresas
      await loadCompanies();
      
      // Voltar para lista
      navigate('/companies');
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsDeleting(false);
      setShowDeleteConfirm(false);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Carregando...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      {/* Header */}
      <header className="bg-white border-b border-gray-200 px-6 py-4">
        <div className="container mx-auto">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => navigate('/companies')}
            className="mb-2"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <div className="flex items-center space-x-3">
            <Building2 className="h-8 w-8 text-primary-600" />
            <div>
              <h1 className="text-2xl font-bold text-gray-900">Configurações da Empresa</h1>
              <p className="text-sm text-gray-600">{company?.name}</p>
            </div>
          </div>
        </div>
      </header>

      {/* Content */}
      <div className="container mx-auto px-6 py-8">
        <div className="max-w-2xl mx-auto space-y-6">
          {/* Form de Edição */}
          <Card>
            <CardContent className="p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Informações da Empresa</h2>
              
              <form onSubmit={handleSave} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="name">
                    Nome da Empresa <span className="text-red-500">*</span>
                  </Label>
                  <Input
                    id="name"
                    type="text"
                    placeholder="Ex: Minha Empresa LTDA"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    disabled={isSaving}
                    required
                    maxLength={255}
                  />
                  <p className="text-xs text-gray-500">
                    Razão social ou nome fantasia da empresa
                  </p>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="cnpj">
                    CNPJ <span className="text-xs text-gray-500 font-normal">(opcional)</span>
                  </Label>
                  <Input
                    id="cnpj"
                    type="text"
                    placeholder="00.000.000/0000-00"
                    value={cnpj}
                    onChange={handleCNPJChange}
                    disabled={isSaving}
                    maxLength={18}
                  />
                  <p className="text-xs text-gray-500">
                    Cadastro Nacional de Pessoa Jurídica (14 dígitos)
                  </p>
                </div>

                <div className="pt-4">
                  <Button
                    type="submit"
                    className="w-full"
                    disabled={isSaving}
                  >
                    {isSaving ? (
                      <span className="flex items-center justify-center">
                        <svg className="animate-spin h-4 w-4 mr-2" viewBox="0 0 24 24">
                          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                        </svg>
                        Salvando...
                      </span>
                    ) : (
                      <>
                        <Save className="mr-2 h-4 w-4" />
                        Salvar Alterações
                      </>
                    )}
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>

          {/* Danger Zone */}
          <Card className="border-red-200">
            <CardContent className="p-6">
              <h2 className="text-lg font-semibold text-red-700 mb-2">Zona de Perigo</h2>
              <p className="text-sm text-gray-600 mb-4">
                A exclusão da empresa é permanente e não pode ser desfeita. Todos os dados associados serão mantidos para auditoria, mas a empresa não aparecerá mais na lista.
              </p>

              {!showDeleteConfirm ? (
                <Button
                  variant="outline"
                  className="border-red-300 text-red-700 hover:bg-red-50"
                  onClick={() => setShowDeleteConfirm(true)}
                  disabled={isDeleting}
                >
                  <Trash2 className="mr-2 h-4 w-4" />
                  Deletar Empresa
                </Button>
              ) : (
                <div className="space-y-3">
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <p className="text-sm font-medium text-red-800 mb-2">
                      ⚠️ Confirmar exclusão
                    </p>
                    <p className="text-sm text-red-700">
                      Tem certeza que deseja deletar a empresa <strong>{company?.name}</strong>? Esta ação não pode ser desfeita.
                    </p>
                  </div>
                  
                  <div className="flex gap-3">
                    <Button
                      variant="outline"
                      className="flex-1"
                      onClick={() => setShowDeleteConfirm(false)}
                      disabled={isDeleting}
                    >
                      Cancelar
                    </Button>
                    <Button
                      className="flex-1 bg-red-600 hover:bg-red-700"
                      onClick={handleDelete}
                      disabled={isDeleting}
                    >
                      {isDeleting ? (
                        <span className="flex items-center justify-center">
                          <svg className="animate-spin h-4 w-4 mr-2" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                          </svg>
                          Deletando...
                        </span>
                      ) : (
                        <>
                          <Trash2 className="mr-2 h-4 w-4" />
                          Confirmar Exclusão
                        </>
                      )}
                    </Button>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
