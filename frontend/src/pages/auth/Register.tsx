import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Label } from '../../components/ui/Label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/Card';
import { Eye, EyeOff } from 'lucide-react';
import authService from '../../services/authService';
import { useToast } from '../../contexts/ToastContext';
import { parseBackendError } from '../../utils/errorHandler';

export function Register() {
  const navigate = useNavigate();
  const { showValidationErrors, showError, showSuccess } = useToast();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    email: '',
    phone: '',
    cpf: '',
    password: '',
    confirmPassword: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validação: pelo menos um dos 3 deve estar preenchido
    if (!formData.email && !formData.phone && !formData.cpf) {
      showError('Por favor, preencha pelo menos um: E-mail, Telefone ou CPF');
      return;
    }

    if (!formData.password) {
      showError('Por favor, insira uma senha');
      return;
    }

    if (formData.password !== formData.confirmPassword) {
      showError('As senhas não coincidem');
      return;
    }

    if (formData.password.length < 6) {
      showError('A senha deve ter no mínimo 6 caracteres');
      return;
    }

    // Validação de CPF (se preenchido)
    if (formData.cpf && formData.cpf.replace(/\D/g, '').length !== 11) {
      showError('CPF deve ter 11 dígitos');
      return;
    }

    setIsLoading(true);

    try {
      // Remover formatação de telefone e CPF antes de enviar
      const cleanPhone = formData.phone ? formData.phone.replace(/\D/g, '') : undefined;
      const cleanCpf = formData.cpf ? formData.cpf.replace(/\D/g, '') : undefined;
      
      // Registrar usuário usando authService
      await authService.register({
        email: formData.email || undefined,
        phone: cleanPhone,
        cpf: cleanCpf,
        password: formData.password,
        confirmPassword: formData.confirmPassword,
      });
      
      // Após registro bem-sucedido, redirecionar para seleção de empresas
      showSuccess('Conta criada com sucesso!');
      navigate('/companies');
    } catch (err: any) {
      const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
      
      if (hasValidationErrors && validationErrors) {
        showValidationErrors(validationErrors);
      } else {
        showError(message);
      }
      setIsLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
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

  const formatPhone = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 11) {
      return numbers
        .replace(/(\d{2})(\d)/, '($1) $2')
        .replace(/(\d{5})(\d)/, '$1-$2');
    }
    return value;
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-accent-50 p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <img src="/logo-full.svg" alt="MeuGestor" className="h-24 mx-auto mb-6" />
          <p className="text-gray-600">Crie sua conta gratuitamente</p>
        </div>

        <Card className="shadow-xl">
          <CardHeader>
            <CardTitle>Criar Conta</CardTitle>
            <CardDescription>Preencha pelo menos um dos campos abaixo</CardDescription>
          </CardHeader>
          
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">E-mail</Label>
                <Input
                  id="email"
                  name="email"
                  type="email"
                  placeholder="seu@email.com"
                  value={formData.email}
                  onChange={handleChange}
                  maxLength={255}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="phone">Telefone</Label>
                <Input
                  id="phone"
                  name="phone"
                  type="tel"
                  placeholder="(00) 00000-0000"
                  value={formData.phone}
                  onChange={(e) => {
                    const formatted = formatPhone(e.target.value);
                    setFormData({ ...formData, phone: formatted });
                  }}
                  maxLength={15}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="cpf">CPF</Label>
                <Input
                  id="cpf"
                  name="cpf"
                  type="text"
                  placeholder="000.000.000-00"
                  value={formData.cpf}
                  onChange={(e) => {
                    const formatted = formatCpf(e.target.value);
                    setFormData({ ...formData, cpf: formatted });
                  }}
                  maxLength={14}
                />
              </div>

              <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg text-blue-700 text-xs">
                <strong>Atenção:</strong> Preencha pelo menos um: E-mail, Telefone ou CPF
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Senha *</Label>
                <div className="relative">
                  <Input
                    id="password"
                    name="password"
                    type={showPassword ? 'text' : 'password'}
                    placeholder="Mínimo 6 caracteres"
                    value={formData.password}
                    onChange={handleChange}
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
                  >
                    {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                  </button>
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="confirmPassword">Confirmar Senha *</Label>
                <div className="relative">
                  <Input
                    id="confirmPassword"
                    name="confirmPassword"
                    type={showConfirmPassword ? 'text' : 'password'}
                    placeholder="Digite a senha novamente"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
                  >
                    {showConfirmPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                  </button>
                </div>
              </div>

              <Button type="submit" className="w-full" disabled={isLoading}>
                {isLoading ? 'Criando conta...' : 'Criar Conta'}
              </Button>

              <div className="text-center text-sm text-gray-600">
                Já tem uma conta?{' '}
                <Link to="/login" className="text-primary-600 hover:text-primary-700 font-medium">
                  Fazer login
                </Link>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
