import { useNavigate } from 'react-router-dom';
import { ShieldX, ArrowLeft } from 'lucide-react';
import { MainLayout } from '../components/layout/MainLayout';

export default function AccessDenied() {
  const navigate = useNavigate();

  return (
    <MainLayout>
      <div className="flex items-center justify-center min-h-[calc(100vh-200px)]">
        <div className="text-center max-w-md px-4">
          {/* Ícone */}
          <div className="flex justify-center mb-6">
            <div className="rounded-full bg-red-100 p-6">
              <ShieldX className="h-16 w-16 text-red-600" />
            </div>
          </div>

          {/* Título */}
          <h1 className="text-3xl font-bold text-gray-900 mb-4">
            Acesso Negado
          </h1>

          {/* Mensagem */}
          <p className="text-gray-600 mb-8">
            Você não tem permissão para acessar esta página. 
            Entre em contato com o administrador do sistema para solicitar acesso.
          </p>

          {/* Botões */}
          <div className="flex flex-col sm:flex-row gap-3 justify-center">
            <button
              onClick={() => navigate(-1)}
              className="inline-flex items-center justify-center gap-2 px-6 py-3 bg-white border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <ArrowLeft className="h-4 w-4" />
              Voltar
            </button>
            <button
              onClick={() => navigate('/dashboard')}
              className="inline-flex items-center justify-center gap-2 px-6 py-3 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors"
            >
              Ir para Dashboard
            </button>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
