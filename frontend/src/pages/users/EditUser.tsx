import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import roleService, { type Role } from '../../services/roleService';
import companyUserService, { type CompanyUser } from '../../services/companyUserService';
import { parseBackendError } from '../../utils/errorHandler';
import { ArrowLeft, Save } from 'lucide-react';

export function EditUser() {
  const navigate = useNavigate();
  const { companyUserId } = useParams<{ companyUserId: string }>();
  const { showError, showSuccess } = useToast();
  
  const [companyUser, setCompanyUser] = useState<CompanyUser | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedRole, setSelectedRole] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    loadData();
  }, [companyUserId]);

  const loadData = async () => {
    if (!companyUserId) return;

    setIsLoading(true);
    try {
      // Carregar dados do CompanyUser
      const user = await companyUserService.getById(parseInt(companyUserId));
      setCompanyUser(user);
      setSelectedRole(user.roleId || null);

      // Carregar roles
      const rolesResult = await roleService.getRoles({ pageSize: 100 });
      setRoles(rolesResult.items);
    } catch (error) {
      const { message } = parseBackendError(error);
      showError(message);
      navigate('/users');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSave = async () => {
    if (!companyUserId || !selectedRole) {
      showError('Selecione um cargo');
      return;
    }

    setIsSaving(true);
    try {
      await companyUserService.update(parseInt(companyUserId), {
        userId: companyUser!.userId,
        roleId: selectedRole
      });

      showSuccess('Cargo atualizado com sucesso!');
      navigate('/users');
    } catch (error) {
      const { message } = parseBackendError(error);
      showError(message);
    } finally {
      setIsSaving(false);
    }
  };

  // Helper para exibir identificador do usuário
  const getUserIdentifier = (): string => {
    if (!companyUser) return '';
    if (companyUser.userEmail) return companyUser.userEmail;
    if (companyUser.userPhone) return companyUser.userPhone;
    if (companyUser.userCpf) return companyUser.userCpf;
    return 'Sem identificação';
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
            onClick={() => navigate('/users')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">Editar Cargo do Usuário</h1>
          <p className="text-gray-600 mt-1">
            Usuário: <span className="font-medium">{getUserIdentifier()}</span>
          </p>
        </div>

      {/* Seleção de Cargo */}
      <Card>
        <CardContent className="p-6">
          <div className="space-y-4">
            <div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Selecione o Cargo</h3>
              <p className="text-sm text-gray-600">
                Cargo atual: <span className="font-medium">{companyUser?.roleName || 'Sem cargo'}</span>
              </p>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
              {roles.map((role) => (
                <div
                  key={role.roleId}
                  onClick={() => setSelectedRole(role.roleId)}
                  className={`p-4 border rounded-lg cursor-pointer transition-all ${
                    selectedRole === role.roleId
                      ? 'border-primary-500 bg-primary-50'
                      : 'border-gray-200 hover:border-primary-300 hover:bg-gray-50'
                  }`}
                >
                  <div className="font-medium text-gray-900">{role.name}</div>
                  {role.isSystem && (
                    <div className="text-xs text-purple-600 mt-1">Sistema</div>
                  )}
                </div>
              ))}
            </div>

            <div className="flex justify-end gap-2 pt-4 border-t">
              <Button
                variant="outline"
                onClick={() => navigate('/users')}
              >
                Cancelar
              </Button>
              <Button
                onClick={handleSave}
                disabled={!selectedRole || isSaving || selectedRole === companyUser?.roleId}
                className="flex items-center gap-2"
              >
                <Save className="h-4 w-4" />
                {isSaving ? 'Salvando...' : 'Salvar'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
      </div>
    </MainLayout>
  );
}
