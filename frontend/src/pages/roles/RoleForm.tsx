import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import roleService from '../../services/roleService';
import moduleConfigurationService, { type ModuleConfig } from '../../services/moduleConfigurationService';
import { parseBackendError } from '../../utils/errorHandler';
import { ArrowLeft, Save } from 'lucide-react';
import { ModulePermissions } from '../../components/roles/ModulePermissions';

interface ModulePermission {
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
  extraPermissions?: Record<string, boolean>;
}

interface RoleFormData {
  name: string;
  permissions: {
    isAdmin: boolean;
    modules: Record<string, ModulePermission>;
  };
}

export function RoleForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isSystemRole, setIsSystemRole] = useState(false);
  const [modules, setModules] = useState<ModuleConfig[]>([]);
  const [loadingModules, setLoadingModules] = useState(true);
  const [formData, setFormData] = useState<RoleFormData>({
    name: '',
    permissions: {
      isAdmin: false,
      modules: {}
    }
  });

  const isEditing = !!id;

  // Carregar configuração de módulos ao montar
  useEffect(() => {
    loadModulesConfiguration();
  }, []);

  // Carregar role se estiver editando
  useEffect(() => {
    if (isEditing) {
      loadRole();
    }
  }, [id]);

  const loadModulesConfiguration = async () => {
    setLoadingModules(true);
    try {
      const activeModules = await moduleConfigurationService.getActiveModules();
      setModules(activeModules);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setLoadingModules(false);
    }
  };

  const loadRole = async () => {
    setIsLoading(true);
    try {
      const role = await roleService.getRoleById(Number(id));
      setIsSystemRole(role.isSystem);
      
      // Se for cargo de sistema, força isAdmin como true
      const permissions = role.permissions || {
        isAdmin: false,
        modules: {}
      };
      
      if (role.isSystem) {
        permissions.isAdmin = true;
      }
      
      setFormData({
        name: role.name,
        permissions
      });
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
      navigate('/roles');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.name.trim()) {
      showError('Nome do cargo é obrigatório');
      return;
    }

    setIsSaving(true);
    try {
      // Para cargos de sistema, garante que permissions sempre seja isAdmin=true
      const dataToSave = {
        ...formData,
        permissions: isSystemRole 
          ? { isAdmin: true, modules: {} }
          : formData.permissions
      };
      
      if (isEditing) {
        await roleService.updateRole(Number(id), dataToSave);
        showSuccess('Cargo atualizado com sucesso!');
      } else {
        await roleService.createRole(dataToSave);
        showSuccess('Cargo criado com sucesso!');
      }
      navigate('/roles');
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsSaving(false);
    }
  };

  const handleModulePermissionChange = (
    moduleKey: string,
    permission: string,
    value: boolean,
    isExtra: boolean = false
  ) => {
    setFormData(prev => {
      const currentModule = prev.permissions.modules[moduleKey] || {
        canView: false,
        canCreate: false,
        canEdit: false,
        canDelete: false,
        extraPermissions: {}
      };

      if (isExtra) {
        // Permissão extra (canProcess, canClose, canReopen, etc.)
        return {
          ...prev,
          permissions: {
            ...prev.permissions,
            modules: {
              ...prev.permissions.modules,
              [moduleKey]: {
                ...currentModule,
                extraPermissions: {
                  ...(currentModule.extraPermissions || {}),
                  [permission]: value
                }
              }
            }
          }
        };
      } else {
        // Permissão base (canView, canCreate, canEdit, canDelete)
        return {
          ...prev,
          permissions: {
            ...prev.permissions,
            modules: {
              ...prev.permissions.modules,
              [moduleKey]: {
                ...currentModule,
                [permission]: value
              }
            }
          }
        };
      }
    });
  };

  const handleIsAdminChange = (value: boolean) => {
    // Não permite alterar se for cargo de sistema
    if (isSystemRole) return;
    
    setFormData(prev => ({
      ...prev,
      permissions: {
        ...prev.permissions,
        isAdmin: value
      }
    }));
  };

  if (isLoading || loadingModules) {
    return (
      <MainLayout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/roles')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <div className="flex items-center gap-3">
            <h1 className="text-3xl font-bold text-gray-900">
              {isEditing ? 'Editar Cargo' : 'Novo Cargo'}
            </h1>
            {isSystemRole && (
              <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800">
                Sistema
              </span>
            )}
          </div>
          <p className="text-gray-600 mt-1">
            {isSystemRole 
              ? 'Cargos do sistema têm acesso de administrador. Apenas o nome pode ser alterado.'
              : isEditing 
                ? 'Atualize as informações do cargo' 
                : 'Crie um novo cargo com permissões específicas'
            }
          </p>
        </div>

        <form onSubmit={handleSubmit}>
          {/* Basic Info */}
          <Card className="mb-6">
            <CardContent className="p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">
                Informações Básicas
              </h2>
              
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Nome do Cargo *
                  </label>
                  <Input
                    value={formData.name}
                    onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
                    placeholder="Ex: Gerente, Vendedor, Contador..."
                    disabled={isSaving}
                  />
                </div>

                <div className="flex items-center">
                  <input
                    type="checkbox"
                    id="isAdmin"
                    checked={formData.permissions.isAdmin}
                    onChange={(e) => handleIsAdminChange(e.target.checked)}
                    disabled={isSaving || isSystemRole}
                    className="rounded border-gray-300 text-primary-600 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed"
                  />
                  <label htmlFor="isAdmin" className="ml-2 block text-sm text-gray-700">
                    <span className="font-medium">Administrador</span>
                    <span className="text-gray-500 ml-2">
                      (Acesso total ao sistema)
                      {isSystemRole && <span className="ml-1 text-blue-600">- Cargo de Sistema</span>}
                    </span>
                  </label>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Permissions by Module */}
          {!formData.permissions.isAdmin && !isSystemRole && (
            <Card className="mb-6">
              <CardContent className="p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">
                  Permissões por Módulo
                </h2>
                <p className="text-sm text-gray-600 mb-6">
                  Defina quais ações este cargo pode realizar em cada módulo do sistema
                </p>

                <div className="space-y-4">
                  {modules.map(module => (
                    <ModulePermissions
                      key={module.key}
                      module={{
                        key: module.key,
                        label: module.name,
                        description: module.description,
                        icon: module.icon,
                        permissionDetails: module.permissions
                      }}
                      permissions={formData.permissions.modules[module.key] || {
                        canView: false,
                        canCreate: false,
                        canEdit: false,
                        canDelete: false,
                        extraPermissions: {}
                      }}
                      onChange={(permission, value, isExtra) =>
                        handleModulePermissionChange(module.key, permission, value, isExtra)
                      }
                      disabled={isSaving}
                    />
                  ))}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Actions */}
          <div className="flex items-center justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/roles')}
              disabled={isSaving}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={isSaving}>
              <Save className="h-4 w-4 mr-2" />
              {isSaving ? 'Salvando...' : 'Salvar Cargo'}
            </Button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
