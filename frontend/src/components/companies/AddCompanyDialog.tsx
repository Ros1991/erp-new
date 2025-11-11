import { useState } from 'react';
import { Building2 } from 'lucide-react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogClose } from '../ui/Dialog';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Label } from '../ui/Label';
import { useToast } from '../../contexts/ToastContext';
import { useAuth } from '../../contexts/AuthContext';
import companyService from '../../services/companyService';
import { parseBackendError } from '../../utils/errorHandler';

interface AddCompanyDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSuccess?: () => void;
}

export function AddCompanyDialog({ open, onOpenChange, onSuccess }: AddCompanyDialogProps) {
  const [name, setName] = useState('');
  const [cnpj, setCnpj] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const { user } = useAuth();
  const { showError, showSuccess, showValidationErrors } = useToast();

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

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

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

    if (!user?.userId) {
      showError('Usuário não autenticado');
      return;
    }

    setIsLoading(true);

    try {
      await companyService.createCompany({
        name: name.trim(),
        document: cnpjNumbers || undefined,
        userId: user.userId
      });

      showSuccess('Empresa criada com sucesso!');
      
      // Limpar formulário
      setName('');
      setCnpj('');
      
      // Fechar dialog
      onOpenChange(false);
      
      // Callback de sucesso
      if (onSuccess) {
        onSuccess();
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

  const handleClose = () => {
    if (!isLoading) {
      setName('');
      setCnpj('');
      onOpenChange(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogClose onClose={handleClose} />
        
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center">
              <Building2 className="h-6 w-6 text-primary-600" />
            </div>
            <div>
              <DialogTitle>Nova Empresa</DialogTitle>
              <DialogDescription>
                Adicione uma nova empresa para gerenciar
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="px-6 pb-6 space-y-4">
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
              disabled={isLoading}
              required
              maxLength={255}
              autoFocus
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
              disabled={isLoading}
              maxLength={18}
            />
            <p className="text-xs text-gray-500">
              Cadastro Nacional de Pessoa Jurídica (14 dígitos) - deixe em branco se não tiver
            </p>
          </div>

          <div className="flex gap-3 pt-4">
            <Button
              type="button"
              variant="outline"
              className="flex-1"
              onClick={handleClose}
              disabled={isLoading}
            >
              Cancelar
            </Button>
            <Button
              type="submit"
              className="flex-1"
              disabled={isLoading}
            >
              {isLoading ? (
                <span className="flex items-center justify-center">
                  <svg className="animate-spin h-4 w-4 mr-2" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                  </svg>
                  Criando...
                </span>
              ) : (
                <>
                  <Building2 className="mr-2 h-4 w-4" />
                  Criar Empresa
                </>
              )}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
