import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/Dialog';
import { Button } from '../ui/Button';
import { AlertTriangle, User, Mail, Phone, FileText } from 'lucide-react';

interface DisassociateUserDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  employeeData: {
    nickname: string;
    fullName?: string;
  };
  userData: {
    email?: string;
    phone?: string;
    cpf?: string;
  };
  onConfirm: (removeCompanyAccess: boolean) => Promise<void>;
}

export function DisassociateUserDialog({
  open,
  onOpenChange,
  employeeData,
  userData,
  onConfirm
}: DisassociateUserDialogProps) {
  const [removeAccess, setRemoveAccess] = useState(true);
  const [isLoading, setIsLoading] = useState(false);

  const getUserIdentifier = () => {
    if (userData.email) return userData.email;
    if (userData.phone) return formatPhone(userData.phone);
    if (userData.cpf) return formatCpf(userData.cpf);
    return 'Sem identificação';
  };

  const formatPhone = (phone: string) => {
    const numbers = phone.replace(/\D/g, '');
    if (numbers.length === 11) {
      return numbers.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }
    return phone;
  };

  const formatCpf = (cpf: string) => {
    const numbers = cpf.replace(/\D/g, '');
    if (numbers.length === 11) {
      return numbers.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    }
    return cpf;
  };

  const handleConfirm = async () => {
    setIsLoading(true);
    try {
      await onConfirm(removeAccess);
      onOpenChange(false);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Desassociar Usuário</DialogTitle>
        </DialogHeader>

        <div className="px-6 pb-6 space-y-4">
          {/* Alerta de Aviso */}
          <div className="bg-amber-50 border border-amber-200 rounded-lg p-4 flex items-start gap-3">
            <AlertTriangle className="h-5 w-5 text-amber-600 mt-0.5 flex-shrink-0" />
            <div className="flex-1">
              <p className="text-sm font-medium text-amber-900">Desassociar usuário do empregado?</p>
              <p className="text-xs text-amber-700 mt-1">
                O vínculo entre o usuário e o empregado será removido.
              </p>
            </div>
          </div>

          {/* Informações */}
          <div className="space-y-3">
            <div className="flex items-center gap-2 text-sm">
              <User className="h-4 w-4 text-gray-400" />
              <span className="text-gray-600">
                Empregado: <strong className="text-gray-900">{employeeData.nickname}</strong>
                {employeeData.fullName && ` (${employeeData.fullName})`}
              </span>
            </div>

            <div className="flex items-center gap-2 text-sm">
              {userData.email ? (
                <Mail className="h-4 w-4 text-gray-400" />
              ) : userData.phone ? (
                <Phone className="h-4 w-4 text-gray-400" />
              ) : (
                <FileText className="h-4 w-4 text-gray-400" />
              )}
              <span className="text-gray-600">
                Usuário: <strong className="text-gray-900">{getUserIdentifier()}</strong>
              </span>
            </div>
          </div>

          {/* Opção de remover acesso */}
          <div className="border-t pt-4">
            <label className="flex items-start gap-3 cursor-pointer">
              <input
                type="checkbox"
                checked={removeAccess}
                onChange={(e) => setRemoveAccess(e.target.checked)}
                className="mt-1 h-4 w-4 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
              />
              <div className="flex-1">
                <span className="text-sm font-medium text-gray-900">
                  Remover acesso do usuário à empresa
                </span>
                <p className="text-xs text-gray-500 mt-1">
                  Se marcado, o usuário não terá mais acesso a esta empresa.
                  Se desmarcado, o usuário mantém seu acesso e cargo, mas não estará vinculado ao empregado.
                </p>
              </div>
            </label>
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
              variant="danger"
              onClick={handleConfirm}
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Desassociando...
                </>
              ) : removeAccess ? (
                'Desassociar e Remover Acesso'
              ) : (
                'Apenas Desassociar'
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
