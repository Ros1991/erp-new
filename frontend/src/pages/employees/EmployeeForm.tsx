import { useEffect, useState, useRef } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import employeeService, { type Employee } from '../../services/employeeService';
import { ArrowLeft, Save, Upload, X } from 'lucide-react';

interface EmployeeFormData {
  nickname: string;
  fullName: string;
  email: string;
  phone: string;
  cpf: string;
  employeeIdManager?: number;
  userId?: number;
  profileImageBase64?: string;
}

export function EmployeeForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [managers, setManagers] = useState<Employee[]>([]);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  
  const [formData, setFormData] = useState<EmployeeFormData>({
    nickname: '',
    fullName: '',
    email: '',
    phone: '',
    cpf: '',
  });

  const [errors, setErrors] = useState<Partial<Record<keyof EmployeeFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    loadManagers();
    if (isEditing) {
      loadEmployee();
    }
  }, [id]);

  const loadManagers = async () => {
    try {
      const allEmployees = await employeeService.getAllEmployees();
      // Filtra o empregado atual se estiver editando
      const availableManagers = id 
        ? allEmployees.filter(e => e.employeeId !== Number(id))
        : allEmployees;
      setManagers(availableManagers);
    } catch (err: any) {
      handleBackendError(err);
    }
  };

  const loadEmployee = async () => {
    setIsLoading(true);
    try {
      const employee = await employeeService.getEmployeeById(Number(id));
      setFormData({
        nickname: employee.nickname,
        fullName: employee.fullName,
        email: employee.email || '',
        phone: employee.phone || '',
        cpf: employee.cpf || '',
        employeeIdManager: employee.employeeIdManager,
        userId: employee.userId,
        profileImageBase64: employee.profileImageBase64,
      });
      
      // Definir preview da imagem se existir
      if (employee.profileImageBase64) {
        setImagePreview(`data:image/jpeg;base64,${employee.profileImageBase64}`);
      }
    } catch (err: any) {
      handleBackendError(err);
      navigate('/employees');
    } finally {
      setIsLoading(false);
    }
  };

  const formatPhone = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 11) {
      return numbers
        .replace(/(\d{2})(\d)/, '($1) $2')
        .replace(/(\d{5})(\d)/, '$1-$2');
    }
    return value;
  };

  const formatCpf = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 11) {
      return numbers
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    }
    return value;
  };

  const handleInputChange = (field: keyof EmployeeFormData, value: string | number | undefined) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Limpar erro do campo ao digitar
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handlePhoneChange = (value: string) => {
    const formatted = formatPhone(value);
    handleInputChange('phone', formatted);
  };

  const handleCpfChange = (value: string) => {
    const formatted = formatCpf(value);
    handleInputChange('cpf', formatted);
  };

  const handleImageChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validar tipo de arquivo
    if (!file.type.startsWith('image/')) {
      showError('Por favor, selecione um arquivo de imagem válido');
      return;
    }

    // Validar tamanho (máx 5MB)
    if (file.size > 5 * 1024 * 1024) {
      showError('A imagem deve ter no máximo 5MB');
      return;
    }

    try {
      // Converter para Base64
      const base64 = await employeeService.imageToBase64(file);
      setFormData(prev => ({ ...prev, profileImageBase64: base64 }));
      
      // Criar preview
      const reader = new FileReader();
      reader.onload = (e) => {
        setImagePreview(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    } catch (error) {
      showError('Erro ao processar imagem');
    }
  };

  const handleRemoveImage = () => {
    setFormData(prev => ({ ...prev, profileImageBase64: undefined }));
    setImagePreview(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Partial<Record<keyof EmployeeFormData, string>> = {};

    if (!formData.nickname.trim()) {
      newErrors.nickname = 'Apelido é obrigatório';
    }

    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Nome completo é obrigatório';
    }

    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email inválido';
    }

    if (formData.phone) {
      const cleanPhone = formData.phone.replace(/\D/g, '');
      if (cleanPhone.length !== 11) {
        newErrors.phone = 'Telefone deve ter 11 dígitos (DDD + número)';
      }
    }

    if (formData.cpf) {
      const cleanCpf = formData.cpf.replace(/\D/g, '');
      if (cleanCpf.length !== 11) {
        newErrors.cpf = 'CPF deve ter 11 dígitos';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsSaving(true);
    try {
      // Limpar formatação de phone e cpf antes de enviar
      const cleanPhone = formData.phone ? formData.phone.replace(/\D/g, '') : undefined;
      const cleanCpf = formData.cpf ? formData.cpf.replace(/\D/g, '') : undefined;

      const dataToSend = {
        nickname: formData.nickname.trim(),
        fullName: formData.fullName.trim(),
        email: formData.email.trim() || undefined,
        phone: cleanPhone,
        cpf: cleanCpf,
        employeeIdManager: formData.employeeIdManager,
        userId: formData.userId,
        profileImageBase64: formData.profileImageBase64,
      };

      if (isEditing) {
        await employeeService.updateEmployee(Number(id), dataToSend);
        showSuccess('Empregado atualizado com sucesso!');
      } else {
        await employeeService.createEmployee(dataToSend);
        showSuccess('Empregado criado com sucesso!');
      }
      
      navigate('/employees');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <MainLayout>
        <div className="flex items-center justify-center h-64">
          <div className="text-gray-500">Carregando...</div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/employees')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Empregado' : 'Novo Empregado'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing 
              ? 'Atualize as informações do empregado' 
              : 'Preencha os dados para criar um novo empregado'
            }
          </p>
        </div>

        <form onSubmit={handleSubmit}>
          <Card>
            <CardContent className="p-6 space-y-6">
              {/* Imagem de Perfil */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Foto de Perfil
                </label>
                <div className="flex items-center gap-4">
                  {imagePreview ? (
                    <div className="relative">
                      <img 
                        src={imagePreview} 
                        alt="Preview" 
                        className="h-24 w-24 rounded-full object-cover border-2 border-gray-300"
                      />
                      <button
                        type="button"
                        onClick={handleRemoveImage}
                        className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 hover:bg-red-600"
                      >
                        <X className="h-4 w-4" />
                      </button>
                    </div>
                  ) : (
                    <div className="h-24 w-24 rounded-full bg-gray-200 flex items-center justify-center">
                      <Upload className="h-8 w-8 text-gray-400" />
                    </div>
                  )}
                  <div>
                    <input
                      ref={fileInputRef}
                      type="file"
                      accept="image/*"
                      onChange={handleImageChange}
                      className="hidden"
                      id="image-upload"
                    />
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => fileInputRef.current?.click()}
                    >
                      <Upload className="h-4 w-4 mr-2" />
                      Selecionar Imagem
                    </Button>
                    <p className="text-xs text-gray-500 mt-1">
                      PNG, JPG ou JPEG (máx. 5MB)
                    </p>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Apelido */}
                <div>
                  <label htmlFor="nickname" className="block text-sm font-medium text-gray-700 mb-2">
                    Apelido <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="nickname"
                    value={formData.nickname}
                    onChange={(e) => handleInputChange('nickname', e.target.value)}
                    placeholder="Como é conhecido"
                    className={errors.nickname ? 'border-red-500' : ''}
                  />
                  {errors.nickname && (
                    <p className="text-sm text-red-500 mt-1">{errors.nickname}</p>
                  )}
                </div>

                {/* Nome Completo */}
                <div>
                  <label htmlFor="fullName" className="block text-sm font-medium text-gray-700 mb-2">
                    Nome Completo <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="fullName"
                    value={formData.fullName}
                    onChange={(e) => handleInputChange('fullName', e.target.value)}
                    placeholder="Nome completo do empregado"
                    className={errors.fullName ? 'border-red-500' : ''}
                  />
                  {errors.fullName && (
                    <p className="text-sm text-red-500 mt-1">{errors.fullName}</p>
                  )}
                </div>

                {/* Email */}
                <div>
                  <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                    Email
                  </label>
                  <Input
                    id="email"
                    type="email"
                    value={formData.email}
                    onChange={(e) => handleInputChange('email', e.target.value)}
                    placeholder="email@exemplo.com"
                    className={errors.email ? 'border-red-500' : ''}
                  />
                  {errors.email && (
                    <p className="text-sm text-red-500 mt-1">{errors.email}</p>
                  )}
                </div>

                {/* Telefone */}
                <div>
                  <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-2">
                    Telefone
                  </label>
                  <Input
                    id="phone"
                    value={formData.phone}
                    onChange={(e) => handlePhoneChange(e.target.value)}
                    placeholder="(11) 99999-9999"
                    className={errors.phone ? 'border-red-500' : ''}
                  />
                  {errors.phone && (
                    <p className="text-sm text-red-500 mt-1">{errors.phone}</p>
                  )}
                </div>

                {/* CPF */}
                <div>
                  <label htmlFor="cpf" className="block text-sm font-medium text-gray-700 mb-2">
                    CPF
                  </label>
                  <Input
                    id="cpf"
                    value={formData.cpf}
                    onChange={(e) => handleCpfChange(e.target.value)}
                    placeholder="000.000.000-00"
                    className={errors.cpf ? 'border-red-500' : ''}
                  />
                  {errors.cpf && (
                    <p className="text-sm text-red-500 mt-1">{errors.cpf}</p>
                  )}
                </div>

                {/* Gerente */}
                <div>
                  <label htmlFor="manager" className="block text-sm font-medium text-gray-700 mb-2">
                    Gerente
                  </label>
                  <select
                    id="manager"
                    value={formData.employeeIdManager || ''}
                    onChange={(e) => handleInputChange('employeeIdManager', e.target.value ? Number(e.target.value) : undefined)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
                  >
                    <option value="">Nenhum</option>
                    {managers.map(manager => (
                      <option key={manager.employeeId} value={manager.employeeId}>
                        {manager.nickname} - {manager.fullName}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Botões de Ação */}
          <div className="flex justify-end gap-3 mt-6">
            <Button
              type="submit"
              disabled={isSaving}
              className="flex items-center gap-2"
            >
              <Save className="h-4 w-4" />
              {isSaving ? 'Salvando...' : (isEditing ? 'Salvar Alterações' : 'Criar Empregado')}
            </Button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
