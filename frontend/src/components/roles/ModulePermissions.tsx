import { ChevronDown, ChevronUp } from 'lucide-react';
import { useState } from 'react';

interface Module {
  key: string;
  label: string;
  description: string;
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

  const hasAnyPermission = Object.values(permissions).some(p => p);

  const handleSelectAll = (value: boolean) => {
    (Object.keys(permissions) as Array<keyof Permissions>).forEach(key => {
      onChange(key, value);
    });
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
              type="checkbox"
              checked={hasAnyPermission}
              onChange={(e) => {
                e.stopPropagation();
                handleSelectAll(e.target.checked);
              }}
              onClick={(e) => e.stopPropagation()}
              disabled={disabled}
              className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
            />
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
        <div className="p-4 bg-white space-y-3">
          {(Object.keys(PERMISSION_LABELS) as Array<keyof Permissions>).map(key => (
            <label
              key={key}
              className="flex items-center gap-3 cursor-pointer hover:bg-gray-50 p-2 rounded transition-colors"
            >
              <input
                type="checkbox"
                checked={permissions[key]}
                onChange={(e) => onChange(key, e.target.checked)}
                disabled={disabled}
                className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
              />
              <span className="text-sm text-gray-700">
                {PERMISSION_LABELS[key]}
              </span>
            </label>
          ))}
        </div>
      )}
    </div>
  );
}
