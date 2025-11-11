import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Label } from '../../components/ui/Label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/Card';
import { CheckCircle, ArrowLeft } from 'lucide-react';
import authService from '../../services/authService';
import { useToast } from '../../contexts/ToastContext';
import { parseBackendError } from '../../utils/errorHandler';

export function ForgotPassword() {
  const navigate = useNavigate();
  const { showValidationErrors, showError } = useToast();
  const [credential, setCredential] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSuccess(false);

    if (!credential) {
      showError('Por favor, insira seu e-mail, telefone ou CPF');
      return;
    }

    setIsLoading(true);

    try {
      // Se não for email (não tem @), remover formatação (telefone/CPF)
      const cleanCredential = credential.includes('@') 
        ? credential 
        : credential.replace(/\D/g, '');
      
      // Enviar código de recuperação usando authService
      await authService.forgotPassword({ credential: cleanCredential });
      
      setSuccess(true);
      setIsLoading(false);
      
      // Redirecionar para tela de reset após 2 segundos
      setTimeout(() => {
        navigate('/reset-password');
      }, 2000);
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

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-accent-50 p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <img src="/logo-full.svg" alt="MeuGestor" className="h-24 mx-auto mb-6" />
          <p className="text-gray-600">Recupere o acesso à sua conta</p>
        </div>

        <Card className="shadow-xl">
          <CardHeader>
            <CardTitle>Esqueci minha senha</CardTitle>
            <CardDescription>
              {success 
                ? 'Código enviado com sucesso!'
                : 'Digite seu e-mail para receber o código'
              }
            </CardDescription>
          </CardHeader>
          
          <CardContent>
            {success ? (
              <div className="space-y-4">
                <div className="flex items-start gap-3 p-4 bg-green-50 border border-green-200 rounded-lg text-green-700">
                  <CheckCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
                  <div className="text-sm">
                    <p className="font-medium mb-1">Código enviado!</p>
                    <p className="text-green-600">
                      Enviamos um código de verificação para <strong>{credential}</strong>.
                      Verifique sua caixa de entrada e spam. Você será redirecionado...
                    </p>
                  </div>
                </div>

                <div className="space-y-3">
                  <Button
                    type="button"
                    variant="outline"
                    className="w-full"
                    onClick={() => {
                      setSuccess(false);
                      setCredential('');
                    }}
                  >
                    Enviar novamente
                  </Button>

                  <Link to="/login" className="block">
                    <Button type="button" variant="ghost" className="w-full">
                      <ArrowLeft className="mr-2 h-4 w-4" />
                      Voltar para login
                    </Button>
                  </Link>
                </div>
              </div>
            ) : (
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="credential">E-mail, Telefone ou CPF</Label>
                  <Input
                    id="credential"
                    type="text"
                    placeholder="seu@email.com, (11) 98765-4321 ou 123.456.789-00"
                    value={credential}
                    onChange={(e) => setCredential(e.target.value)}
                    required
                  />
                  <p className="text-xs text-gray-500">
                    Enviaremos um código de verificação
                  </p>
                </div>

                <Button type="submit" className="w-full" disabled={isLoading}>
                  {isLoading ? 'Enviando...' : 'Enviar código de verificação'}
                </Button>

                <Link to="/login" className="block">
                  <Button type="button" variant="ghost" className="w-full">
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Voltar para login
                  </Button>
                </Link>
              </form>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
