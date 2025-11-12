import { ChevronDown, ChevronUp, Shield, Users, Wallet } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';
import { useState, useRef, useEffect } from 'react';

interface PermissionDetail {
  key: string;
  name: string;
  description: string;
  order: number;
  color?: string;
}

interface Module {
  key: string;
  label: string;
  description: string;
  icon?: string;
  permissionDetails?: PermissionDetail[];
}

interface Permissions {
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}

interface ModulePermissionsProps {
  module: Module;
  permissions: Permissions;
  onChange: (permission: keyof Permissions, value: boolean) => void;
  disabled?: boolean;
}

const ICON_MAP: Record<string, LucideIcon> = {
  shield: Shield,
  users: Users,
  wallet: Wallet
};

const PERMISSION_LABELS = {
  canView: 'Visualizar',
  canCreate: 'Criar',
  canEdit: 'Editar',
  canDelete: 'Excluir'
};

export function ModulePermissions({
  module,
  permissions,
  onChange,
  disabled = false
}: ModulePermissionsProps) {
  const [isExpanded, setIsExpanded] = useState(false);
  const checkboxRef = useRef<HTMLInputElement>(null);

  // Checkbox marcado apenas se TODAS as permissões estiverem marcadas
  const allPermissionsSelected = Object.values(permissions).every(p => p);
  const hasAnyPermission = Object.values(permissions).some(p => p);
  const isIndeterminate = hasAnyPermission && !allPermissionsSelected;

  // Atualizar estado indeterminado do checkbox
  useEffect(() => {
    if (checkboxRef.current) {
      checkboxRef.current.indeterminate = isIndeterminate;
    }
  }, [isIndeterminate]);

  const handleSelectAll = (value: boolean) => {
    (Object.keys(permissions) as Array<keyof Permissions>).forEach(key => {
      onChange(key, value);
    });
  };

  // Obter ícone do módulo
  const ModuleIcon = module.icon && ICON_MAP[module.icon] ? ICON_MAP[module.icon] : Shield;

  // Obter detalhes de uma permissão específica
  const getPermissionDetails = (key: keyof Permissions): PermissionDetail | undefined => {
    const details = module.permissionDetails?.find(p => p.key === key);
    console.log(`Detalhes da permissão ${key}:`, details);
    return details;
  };

  return (
    <div className="border border-gray-200 rounded-lg overflow-hidden">
      {/* Module Header */}
      <button
        type="button"
        onClick={() => setIsExpanded(!isExpanded)}
        className="w-full px-4 py-3 flex items-center justify-between bg-gray-50 hover:bg-gray-100 transition-colors"
        disabled={disabled}
      >
        <div className="flex items-center gap-3">
          <div className="flex items-center">
            <input
              ref={checkboxRef}
              type="checkbox"
              checked={allPermissionsSelected}
              onChange={(e) => {
                e.stopPropagation();
                // Se está indeterminado ou desmarcado, marcar todos
                // Se está totalmente marcado, desmarcar todos
                const shouldSelectAll = isIndeterminate || !allPermissionsSelected;
                handleSelectAll(shouldSelectAll);
              }}
              onClick={(e) => e.stopPropagation()}
              disabled={disabled}
              className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
            />
          </div>
          <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary-100 text-primary-600">
            <ModuleIcon className="h-5 w-5" />
          </div>
          <div className="text-left">
            <div className="font-medium text-gray-900">{module.label}</div>
            <div className="text-xs text-gray-500">{module.description}</div>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {hasAnyPermission && !isExpanded && (
            <span className="text-xs text-primary-600 font-medium">
              {Object.values(permissions).filter(p => p).length} permissões
            </span>
          )}
          {isExpanded ? (
            <ChevronUp className="h-4 w-4 text-gray-400" />
          ) : (
            <ChevronDown className="h-4 w-4 text-gray-400" />
          )}
        </div>
      </button>

      {/* Permissions Details */}
      {isExpanded && (
        <div className="p-4 bg-white space-y-2">
          {(Object.keys(PERMISSION_LABELS) as Array<keyof Permissions>).map(key => {
            const details = getPermissionDetails(key);
            return (
              <label
                key={key}
                className="flex items-start gap-3 cursor-pointer hover:bg-gray-50 p-3 rounded-lg transition-colors"
              >
                <input
                  type="checkbox"
                  checked={permissions[key]}
                  onChange={(e) => onChange(key, e.target.checked)}
                  disabled={disabled}
                  className="mt-0.5 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                />
                <div className="flex-1">
                  <div className="font-medium text-sm text-gray-900">
                    {details?.name || PERMISSION_LABELS[key]}
                  </div>
                  {details?.description && (
                    <div className="text-xs text-gray-500 mt-0.5">
                      {details.description}
                    </div>
                  )}
                </div>
              </label>
            );
          })}
        </div>
      )}
    </div>
  );
}
