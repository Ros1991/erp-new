import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Eye, EyeOff, LogIn } from 'lucide-react';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Label } from '../../components/ui/Label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/Card';
import { useAuth } from '../../contexts/AuthContext';
import { useToast } from '../../contexts/ToastContext';
import { parseBackendError } from '../../utils/errorHandler';
import { 
  saveRememberMeData, 
  loadRememberMeData, 
  clearRememberMeData,
  isRememberMeEnabled 
} from '../../utils/rememberMe';

export function Login() {
  const [credential, setCredential] = useState(''); // Email, Phone ou CPF
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  
  const { login } = useAuth();
  const { showValidationErrors, showError } = useToast();

  // Carregar dados salvos ao montar o componente
  useEffect(() => {
    if (isRememberMeEnabled()) {
      const savedData = loadRememberMeData();
      
      if (savedData) {
        setCredential(savedData.credential);
        setPassword(savedData.password);
        setRememberMe(true);
      }
    }
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!credential) {
      showError('Por favor, insira seu e-mail, telefone ou CPF');
      return;
    }

    if (!password) {
      showError('Por favor, insira sua senha');
      return;
    }

    setIsLoading(true);

    try {
      // Se não for email (não tem @), remover formatação (telefone/CPF)
      const cleanCredential = credential.includes('@') 
        ? credential 
        : credential.replace(/\D/g, '');
      
      // Validar se credential limpo não está vazio (pode acontecer se digitar apenas letras sem @)
      if (!cleanCredential || cleanCredential.trim() === '') {
        showError('Por favor, insira um e-mail, telefone ou CPF válido');
        setIsLoading(false);
        return;
      }
      
      await login(cleanCredential, password);
      
      // Gerenciar "Lembrar de mim"
      if (rememberMe) {
        // Salvar credenciais no localStorage
        saveRememberMeData(credential, password);
      } else {
        // Limpar credenciais salvas
        clearRememberMeData();
      }
    } catch (err: any) {
      const { hasValidationErrors, validationErrors, message } = parseBackendError(err);
      
      if (hasValidationErrors && validationErrors) {
        showValidationErrors(validationErrors);
      } else {
        showError(message);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-accent-50 p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <img src="/logo-full.svg" alt="MeuGestor" className="h-32 mx-auto mb-6" />
          <p className="text-gray-600 mt-2">Faça login para acessar sua conta</p>
        </div>

        <Card className="shadow-xl">
          <CardHeader>
            <CardTitle>Login</CardTitle>
            <CardDescription>Entre com suas credenciais</CardDescription>
          </CardHeader>
          
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="credential">E-mail, Telefone ou CPF</Label>
                <Input
                  id="credential"
                  type="text"
                  placeholder="Digite seu e-mail, telefone ou CPF"
                  value={credential}
                  onChange={(e) => setCredential(e.target.value)}
                  disabled={isLoading}
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Senha</Label>
                <div className="relative">
                  <Input
                    id="password"
                    type={showPassword ? 'text' : 'password'}
                    placeholder="Digite sua senha"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    disabled={isLoading}
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
                    disabled={isLoading}
                  >
                    {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                  </button>
                </div>
              </div>

              <div className="flex items-center justify-between">
                <label className="flex items-center">
                  <input
                    type="checkbox"
                    checked={rememberMe}
                    onChange={(e) => setRememberMe(e.target.checked)}
                    className="rounded border-gray-300 text-primary-600 focus:ring-primary-500 mr-2"
                    disabled={isLoading}
                  />
                  <span className="text-sm text-gray-600">Lembrar de mim</span>
                </label>
                
                <Link 
                  to="/forgot-password" 
                  className="text-sm text-primary-600 hover:text-primary-700"
                >
                  Esqueceu a senha?
                </Link>
              </div>

              <Button type="submit" className="w-full" disabled={isLoading}>
                {isLoading ? (
                  <span className="flex items-center justify-center">
                    <svg className="animate-spin h-5 w-5 mr-3" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                    Entrando...
                  </span>
                ) : (
                  <>
                    <LogIn className="mr-2 h-5 w-5" />
                    Entrar
                  </>
                )}
              </Button>
            </form>

            <div className="mt-6 text-center">
              <p className="text-sm text-gray-600">
                Não tem uma conta?{' '}
                <Link to="/register" className="text-primary-600 hover:text-primary-700 font-medium">
                  Cadastre-se
                </Link>
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
