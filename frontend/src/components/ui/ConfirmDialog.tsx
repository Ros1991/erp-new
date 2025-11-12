import { type ReactNode } from 'react';
import { Button } from './Button';

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  description: string | ReactNode;
  confirmText?: string;
  cancelText?: string;
  variant?: 'danger' | 'warning' | 'info';
  isLoading?: boolean;
}

export function ConfirmDialog({
  isOpen,
  onClose,
  onConfirm,
  title,
  description,
  confirmText = 'Confirmar',
  cancelText = 'Cancelar',
  variant = 'danger',
  isLoading = false
}: ConfirmDialogProps) {
  if (!isOpen) return null;

  const variantStyles = {
    danger: 'bg-red-600 hover:bg-red-700 focus:ring-red-500',
    warning: 'bg-yellow-600 hover:bg-yellow-700 focus:ring-yellow-500',
    info: 'bg-primary-600 hover:bg-primary-700 focus:ring-primary-500'
  };

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black bg-opacity-50 z-50 animate-in fade-in duration-200"
        onClick={!isLoading ? onClose : undefined}
      />

      {/* Modal */}
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div
          className="bg-white rounded-2xl shadow-2xl w-full max-w-md animate-in zoom-in-95 duration-200"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Content */}
          <div className="p-6 text-center">
            {/* Title */}
            <h3 className="text-xl font-bold text-gray-900 mb-4">{title}</h3>
            
            {/* Description */}
            <div className="text-gray-600 mb-6">
              {typeof description === 'string' ? (
                <p>{description}</p>
              ) : (
                description
              )}
            </div>

            {/* Actions */}
            <div className="flex items-center justify-center gap-3">
              <Button
                variant="outline"
                onClick={onClose}
                disabled={isLoading}
                className="flex-1 max-w-[140px]"
              >
                {cancelText}
              </Button>
              <Button
                onClick={onConfirm}
                disabled={isLoading}
                className={`flex-1 max-w-[140px] ${variantStyles[variant]}`}
              >
                {isLoading ? 'Processando...' : confirmText}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
