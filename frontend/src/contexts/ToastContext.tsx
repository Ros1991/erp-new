import React, { createContext, useContext, useState, useCallback } from 'react';
import { Toast, type ToastType } from '../components/ui/Toast';

interface ToastData {
  id: string;
  type: ToastType;
  title?: string;
  message: string;
  duration?: number;
}

interface ToastContextType {
  showToast: (type: ToastType, message: string, title?: string, duration?: number) => void;
  showSuccess: (message: string, title?: string) => void;
  showError: (message: string, title?: string) => void;
  showWarning: (message: string, title?: string) => void;
  showInfo: (message: string, title?: string) => void;
  showValidationErrors: (errors: Record<string, string[]>) => void;
}

const ToastContext = createContext<ToastContextType | undefined>(undefined);

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = useState<ToastData[]>([]);

  const removeToast = useCallback((id: string) => {
    setToasts((prev) => prev.filter((toast) => toast.id !== id));
  }, []);

  const showToast = useCallback(
    (type: ToastType, message: string, title?: string, duration = 5000) => {
      const id = Math.random().toString(36).substring(2, 9);
      const newToast: ToastData = { id, type, message, title, duration };
      
      setToasts((prev) => [...prev, newToast]);
    },
    []
  );

  const showSuccess = useCallback((message: string, title = 'Sucesso') => {
    showToast('success', message, title);
  }, [showToast]);

  const showError = useCallback((message: string, title = 'Erro') => {
    showToast('error', message, title);
  }, [showToast]);

  const showWarning = useCallback((message: string, title = 'Atenção') => {
    showToast('warning', message, title);
  }, [showToast]);

  const showInfo = useCallback((message: string, title = 'Informação') => {
    showToast('info', message, title);
  }, [showToast]);

  const showValidationErrors = useCallback((errors: Record<string, string[]>) => {
    // Exibir cada erro de validação como um toast separado
    Object.entries(errors).forEach(([, messages]) => {
      messages.forEach((message) => {
        showToast('error', message);
      });
    });
  }, [showToast]);

  return (
    <ToastContext.Provider
      value={{
        showToast,
        showSuccess,
        showError,
        showWarning,
        showInfo,
        showValidationErrors,
      }}
    >
      {children}
      
      {/* Toast Container */}
      <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2 max-w-md w-full pointer-events-none">
        <div className="pointer-events-auto space-y-2">
          {toasts.map((toast) => (
            <Toast
              key={toast.id}
              id={toast.id}
              type={toast.type}
              title={toast.title}
              message={toast.message}
              duration={toast.duration}
              onClose={removeToast}
            />
          ))}
        </div>
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const context = useContext(ToastContext);
  if (context === undefined) {
    throw new Error('useToast must be used within a ToastProvider');
  }
  return context;
}
