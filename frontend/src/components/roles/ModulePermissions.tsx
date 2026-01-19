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
  extraPermissions?: Record<string, boolean>;
}

interface ModulePermissionsProps {
  module: Module;
  permissions: Permissions;
  onChange: (permission: string, value: boolean, isExtra?: boolean) => void;
  disabled?: boolean;
}

const ICON_MAP: Record<string, LucideIcon> = {
  shield: Shield,
  users: Users,
  wallet: Wallet
};


export function ModulePermissions({
  module,
  permissions,
  onChange,
  disabled = false
}: ModulePermissionsProps) {
  const [isExpanded, setIsExpanded] = useState(false);
  const checkboxRef = useRef<HTMLInputElement>(null);

  // Permissões base
  const BASE_PERMISSIONS = ['canView', 'canCreate', 'canEdit', 'canDelete'];
  
  // Usar apenas as permissões definidas no módulo
  const modulePermissionKeys = (module.permissionDetails || []).map(p => p.key);
  
  // Função para obter valor da permissão (base ou extra)
  const getPermissionValue = (key: string): boolean => {
    if (BASE_PERMISSIONS.includes(key)) {
      return permissions[key as keyof Omit<Permissions, 'extraPermissions'>] || false;
    }
    return permissions.extraPermissions?.[key] || false;
  };
  
  // Checkbox marcado apenas se TODAS as permissões do módulo estiverem marcadas
  const allPermissionsSelected = modulePermissionKeys.length > 0 && 
    modulePermissionKeys.every(key => getPermissionValue(key));
  const hasAnyPermission = modulePermissionKeys.some(key => getPermissionValue(key));
  const isIndeterminate = hasAnyPermission && !allPermissionsSelected;

  // Atualizar estado indeterminado do checkbox
  useEffect(() => {
    if (checkboxRef.current) {
      checkboxRef.current.indeterminate = isIndeterminate;
    }
  }, [isIndeterminate]);

  const handleSelectAll = (value: boolean) => {
    modulePermissionKeys.forEach(key => {
      const isExtra = !BASE_PERMISSIONS.includes(key);
      onChange(key, value, isExtra);
    });
  };

  // Obter ícone do módulo
  const ModuleIcon = module.icon && ICON_MAP[module.icon] ? ICON_MAP[module.icon] : Shield;

  // Contar permissões ativas do módulo
  const activePermissionsCount = modulePermissionKeys.filter(key => getPermissionValue(key)).length;

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
              {activePermissionsCount} permissões
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
          {(module.permissionDetails || []).map(detail => {
            const key = detail.key;
            const isExtra = !BASE_PERMISSIONS.includes(key);
            return (
              <label
                key={key}
                className="flex items-start gap-3 cursor-pointer hover:bg-gray-50 p-3 rounded-lg transition-colors"
              >
                <input
                  type="checkbox"
                  checked={getPermissionValue(key)}
                  onChange={(e) => onChange(key, e.target.checked, isExtra)}
                  disabled={disabled}
                  className="mt-0.5 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                />
                <div className="flex-1">
                  <div className="font-medium text-sm text-gray-900">
                    {detail.name}
                  </div>
                  {detail.description && (
                    <div className="text-xs text-gray-500 mt-0.5">
                      {detail.description}
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
