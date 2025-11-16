import { useEffect, useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/Dialog';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { PhoneInput } from '../ui/PhoneInput';
import { CpfInput } from '../ui/CpfInput';
import roleService from '../../services/roleService';
import { AlertCircle, Eye, EyeOff } from 'lucide-react';

interface CreateUserDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  employeeData: {
    nickname: string;
    email?: string;
    phone?: string;
    cpf?: string;
  };
  onConfirm: (data: {
    email?: string;
    phone?: string;
    cpf?: string;
    password: string;
    roleId: number;
  }) => Promise<void>;
}

interface Role {
  roleId: number;
  name: string;
  isSystem: boolean;
}

interface FormData {
  email: string;
  phone: string;
  cpf: string;
  password: string;
  confirmPassword: string;
  roleId: string;
}

interface FormErrors {
  email?: string;
  phone?: string;
  cpf?: string;
  password?: string;
  confirmPassword?: string;
  roleId?: string;
  general?: string;
}

export function CreateUserDialog({
  open,
  onOpenChange,
  employeeData,
  onConfirm
}: CreateUserDialogProps) {
  const [roles, setRoles] = useState<Role[]>([]);
  const [formData, setFormData] = useState<FormData>({
    email: '',
    phone: '',
    cpf: '',
    password: '',
    confirmPassword: '',
    roleId: ''
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  useEffect(() => {
    if (open) {
      loadRoles();
      // Preencher com dados do empregado
      setFormData({
        email: employeeData.email || '',
        phone: employeeData.phone || '',
        cpf: employeeData.cpf || '',
        password: '',
        confirmPassword: '',
        roleId: ''
      });
      setErrors({});
    }
  }, [open, employeeData]);

  const loadRoles = async () => {
    try {
      const data = await roleService.getAllRoles();
      setRoles(data);
    } catch (err) {
      console.error('Erro ao carregar cargos:', err);
    }
  };

  const handleChange = (field: keyof FormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Limpar erro do campo ao editar
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Validar que pelo menos um identificador foi preenchido
    if (!formData.email.trim() && !formData.phone.trim() && !formData.cpf.trim()) {
      newErrors.general = 'Preencha pelo menos um identificador (Email, Telefone ou CPF)';
    }

    // Validar email se preenchido
    if (formData.email.trim()) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(formData.email)) {
        newErrors.email = 'Email inválido';
      }
    }

    // Validar telefone se preenchido
    if (formData.phone.trim()) {
      const phoneNumbers = formData.phone.replace(/\D/g, '');
      if (phoneNumbers.length !== 11) {
        newErrors.phone = 'Telefone deve ter 11 dígitos (DDD + número)';
      }
    }

    // Validar CPF se preenchido
    if (formData.cpf.trim()) {
      const cpfNumbers = formData.cpf.replace(/\D/g, '');
      if (cpfNumbers.length !== 11) {
        newErrors.cpf = 'CPF deve ter 11 dígitos';
      }
    }

    // Validar senha
    if (!formData.password) {
      newErrors.password = 'Senha é obrigatória';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Senha deve ter no mínimo 6 caracteres';
    }

    // Validar confirmação de senha
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'As senhas não coincidem';
    }

    // Validar cargo
    if (!formData.roleId) {
      newErrors.roleId = 'Cargo é obrigatório';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleConfirm = async () => {
    if (!validateForm()) return;

    setIsLoading(true);

    try {
      await onConfirm({
        email: formData.email.trim() || undefined,
        phone: formData.phone || undefined,
        cpf: formData.cpf || undefined,
        password: formData.password,
        roleId: Number(formData.roleId)
      });
      onOpenChange(false);
    } catch (err: any) {
      setErrors({ general: err.response?.data?.message || 'Erro ao criar usuário' });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Criar Novo Usuário</DialogTitle>
        </DialogHeader>

        <div className="px-6 pb-6 space-y-4">
          {/* Alerta informativo */}
          <div className="bg-amber-50 border border-amber-200 rounded-lg p-3 flex items-start gap-2">
            <AlertCircle className="h-4 w-4 text-amber-600 mt-0.5 flex-shrink-0" />
            <div className="text-sm text-amber-900">
              <p className="font-medium">Nenhum usuário encontrado para este empregado.</p>
              <p className="mt-1">Preencha os dados abaixo para criar um novo usuário e associá-lo a <strong>{employeeData.nickname}</strong>.</p>
            </div>
          </div>

          {/* Erro geral */}
          {errors.general && (
            <div className="bg-red-50 border border-red-200 rounded-lg p-3 text-sm text-red-600">
              {errors.general}
            </div>
          )}

          {/* Email */}
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <Input
              id="email"
              type="email"
              value={formData.email}
              onChange={(e) => handleChange('email', e.target.value)}
              placeholder="usuario@exemplo.com"
              className={errors.email ? 'border-red-500' : ''}
            />
            {errors.email && <p className="text-sm text-red-600 mt-1">{errors.email}</p>}
          </div>

          {/* Telefone */}
          <div>
            <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
              Telefone
            </label>
            <PhoneInput
              id="phone"
              value={formData.phone}
              onChange={(value) => handleChange('phone', value)}
            />
            {errors.phone && <p className="text-sm text-red-600 mt-1">{errors.phone}</p>}
          </div>

          {/* CPF */}
          <div>
            <label htmlFor="cpf" className="block text-sm font-medium text-gray-700 mb-1">
              CPF
            </label>
            <CpfInput
              id="cpf"
              value={formData.cpf}
              onChange={(value) => handleChange('cpf', value)}
            />
            {errors.cpf && <p className="text-sm text-red-600 mt-1">{errors.cpf}</p>}
          </div>

          {/* Senha */}
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">
              Senha <span className="text-red-500">*</span>
            </label>
            <div className="relative">
              <Input
                id="password"
                type={showPassword ? 'text' : 'password'}
                value={formData.password}
                onChange={(e) => handleChange('password', e.target.value)}
                placeholder="Mínimo 6 caracteres"
                className={errors.password ? 'border-red-500 pr-10' : 'pr-10'}
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>
            {errors.password && <p className="text-sm text-red-600 mt-1">{errors.password}</p>}
          </div>

          {/* Confirmar Senha */}
          <div>
            <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700 mb-1">
              Confirmar Senha <span className="text-red-500">*</span>
            </label>
            <div className="relative">
              <Input
                id="confirmPassword"
                type={showConfirmPassword ? 'text' : 'password'}
                value={formData.confirmPassword}
                onChange={(e) => handleChange('confirmPassword', e.target.value)}
                className={errors.confirmPassword ? 'border-red-500 pr-10' : 'pr-10'}
              />
              <button
                type="button"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                {showConfirmPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>
            {errors.confirmPassword && <p className="text-sm text-red-600 mt-1">{errors.confirmPassword}</p>}
          </div>

          {/* Cargo */}
          <div>
            <label htmlFor="roleId" className="block text-sm font-medium text-gray-700 mb-1">
              Cargo <span className="text-red-500">*</span>
            </label>
            <select
              id="roleId"
              value={formData.roleId}
              onChange={(e) => handleChange('roleId', e.target.value)}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.roleId ? 'border-red-500' : 'border-gray-300'
              }`}
            >
              <option value="">Selecione um cargo</option>
              {roles.map((role) => (
                <option key={role.roleId} value={role.roleId}>
                  {role.name} {role.isSystem ? '(Sistema)' : ''}
                </option>
              ))}
            </select>
            {errors.roleId && <p className="text-sm text-red-600 mt-1">{errors.roleId}</p>}
          </div>

          {/* Botões */}
          <div className="flex gap-3 justify-end pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isLoading}
            >
              Cancelar
            </Button>
            <Button
              type="button"
              onClick={handleConfirm}
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Criando...
                </>
              ) : (
                'Criar e Associar'
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
