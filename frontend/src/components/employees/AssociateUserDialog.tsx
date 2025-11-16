import { useEffect, useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/Dialog';
import { Button } from '../ui/Button';
import type { UserSearchResult } from '../../services/employeeService';
import roleService from '../../services/roleService';
import { AlertCircle, CheckCircle2, User } from 'lucide-react';

interface AssociateUserDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userSearchResult: UserSearchResult | null;
  employeeNickname: string;
  onConfirm: (roleId?: number) => Promise<void>;
}

interface Role {
  roleId: number;
  name: string;
  isSystem: boolean;
}

export function AssociateUserDialog({
  open,
  onOpenChange,
  userSearchResult,
  employeeNickname,
  onConfirm
}: AssociateUserDialogProps) {
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedRoleId, setSelectedRoleId] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (open && !userSearchResult?.hasCompanyAccess) {
      loadRoles();
    }
  }, [open, userSearchResult]);

  useEffect(() => {
    if (userSearchResult?.currentRoleId) {
      setSelectedRoleId(userSearchResult.currentRoleId.toString());
    } else {
      setSelectedRoleId('');
    }
  }, [userSearchResult]);

  const loadRoles = async () => {
    try {
      const data = await roleService.getAllRoles();
      setRoles(data);
    } catch (err) {
      console.error('Erro ao carregar cargos:', err);
    }
  };

  const handleConfirm = async () => {
    // Se não tem acesso à empresa, o cargo é obrigatório
    if (!userSearchResult?.hasCompanyAccess && !selectedRoleId) {
      setError('Selecione um cargo para dar acesso à empresa');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const roleId = selectedRoleId ? Number(selectedRoleId) : undefined;
      await onConfirm(roleId);
      onOpenChange(false);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao associar usuário');
    } finally {
      setIsLoading(false);
    }
  };

  const getUserIdentifier = () => {
    if (userSearchResult?.email) return userSearchResult.email;
    if (userSearchResult?.phone) return userSearchResult.phone;
    if (userSearchResult?.cpf) return userSearchResult.cpf;
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

  if (!userSearchResult) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Associar Usuário</DialogTitle>
        </DialogHeader>

        <div className="px-6 pb-6 space-y-4">
          {/* Informações do usuário encontrado */}
          <div className="bg-green-50 border border-green-200 rounded-lg p-4">
            <div className="flex items-start gap-3">
              <CheckCircle2 className="h-5 w-5 text-green-600 mt-0.5 flex-shrink-0" />
              <div className="flex-1">
                <p className="text-sm font-medium text-green-900">Usuário encontrado!</p>
                <div className="mt-2 space-y-1 text-sm text-green-800">
                  <p><strong>Identificador:</strong> {getUserIdentifier()}</p>
                  {userSearchResult.email && userSearchResult.email !== getUserIdentifier() && (
                    <p><strong>Email:</strong> {userSearchResult.email}</p>
                  )}
                  {userSearchResult.phone && (
                    <p><strong>Telefone:</strong> {formatPhone(userSearchResult.phone)}</p>
                  )}
                  {userSearchResult.cpf && (
                    <p><strong>CPF:</strong> {formatCpf(userSearchResult.cpf)}</p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Empregado */}
          <div className="flex items-center gap-2 text-sm text-gray-600">
            <User className="h-4 w-4" />
            <span>Empregado: <strong>{employeeNickname}</strong></span>
          </div>

          {/* Já tem acesso à empresa */}
          {userSearchResult.hasCompanyAccess && (
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <p className="text-sm text-blue-900">
                Este usuário já tem acesso à empresa com o cargo:{' '}
                <strong>{userSearchResult.currentRoleName || 'Sem cargo'}</strong>
              </p>
            </div>
          )}

          {/* Não tem acesso - precisa selecionar cargo */}
          {!userSearchResult.hasCompanyAccess && (
            <>
              <div className="bg-amber-50 border border-amber-200 rounded-lg p-3 flex items-start gap-2">
                <AlertCircle className="h-4 w-4 text-amber-600 mt-0.5 flex-shrink-0" />
                <p className="text-sm text-amber-900">
                  Este usuário não tem acesso a esta empresa. Selecione um cargo para dar acesso:
                </p>
              </div>

              <div>
                <label htmlFor="roleId" className="block text-sm font-medium text-gray-700 mb-2">
                  Cargo <span className="text-red-500">*</span>
                </label>
                <select
                  id="roleId"
                  value={selectedRoleId}
                  onChange={(e) => {
                    setSelectedRoleId(e.target.value);
                    setError('');
                  }}
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                    error ? 'border-red-500' : 'border-gray-300'
                  }`}
                >
                  <option value="">Selecione um cargo</option>
                  {roles.map((role) => (
                    <option key={role.roleId} value={role.roleId}>
                      {role.name} {role.isSystem ? '(Sistema)' : ''}
                    </option>
                  ))}
                </select>
                {error && <p className="text-sm text-red-600 mt-1">{error}</p>}
              </div>
            </>
          )}

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
                  Associando...
                </>
              ) : (
                'Confirmar'
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
