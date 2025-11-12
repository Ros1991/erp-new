import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Building2, Calendar, LogOut, Plus, Settings } from 'lucide-react';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { useAuth } from '../../contexts/AuthContext';
import { usePermissions } from '../../contexts/PermissionContext';
import { AddCompanyDialog } from '../../components/companies/AddCompanyDialog';

export function CompanySelect() {
  const { user, companies, selectCompany, loadCompanies, logout } = useAuth();
  const { loadPermissions } = usePermissions();
  const navigate = useNavigate();
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);

  useEffect(() => {
    loadCompanies();
  }, []);

  const handleSelectCompany = async (company: any) => {
    selectCompany(company);
    // Carregar permissões do usuário na empresa selecionada
    await loadPermissions();
    // Navegar para dashboard DEPOIS de carregar permissões
    navigate('/dashboard');
  };

  const handleCompanyCreated = () => {
    // Recarregar lista de empresas após criar uma nova
    loadCompanies();
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      {/* Header */}
      <header className="bg-white border-b border-gray-200 px-6 py-4">
        <div className="container mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <img src="/logo-full.svg" alt="MeuGestor" className="h-12" />
          </div>
          
          <div className="flex items-center space-x-4">
            <span className="text-sm text-gray-600">
              Olá, <span className="font-medium">{user?.email || user?.phone || user?.cpf || 'Usuário'}</span>
            </span>
            <Button variant="ghost" size="sm" onClick={logout}>
              <LogOut className="h-4 w-4 mr-2" />
              Sair
            </Button>
          </div>
        </div>
      </header>

      {/* Content */}
      <div className="container mx-auto px-6 py-12">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">Suas Empresas</h1>
          <p className="text-gray-600 mb-8">
            Gerencie suas empresas e acesse os dashboards
          </p>

          {/* Companies Grid */}
          <div className="grid md:grid-cols-2 gap-6 mb-8">
            {companies.map((company) => (
              <Card
                key={company.id}
                className="hover:shadow-lg transition-shadow cursor-pointer"
                onClick={() => handleSelectCompany(company)}
              >
                <CardContent className="p-6">
                  <div className="flex items-start justify-between mb-4">
                    <Building2 className="h-8 w-8 text-primary-600" />
                    {company.isActive ? (
                      <span className="px-2 py-1 bg-green-100 text-green-700 text-xs rounded-full font-medium">
                        Ativa
                      </span>
                    ) : (
                      <span className="px-2 py-1 bg-red-100 text-red-700 text-xs rounded-full font-medium">
                        Inativa
                      </span>
                    )}
                  </div>
                  
                  <h3 className="text-xl font-semibold text-gray-900 mb-2">
                    {company.name}
                  </h3>
                  
                  <div className="space-y-2 text-sm text-gray-600">
                    <div className="flex items-center">
                      <span className="font-medium mr-2">CNPJ:</span>
                      <span>{company.cnpj || <span className="italic text-gray-400">Não informado</span>}</span>
                    </div>
                    <div className="flex items-center">
                      <Calendar className="h-4 w-4 mr-2" />
                      <span>Criada em: {new Date(company.createdAt).toLocaleDateString('pt-BR')}</span>
                    </div>
                  </div>

                  <div className="mt-6 flex items-center justify-between">
                    <Button 
                      size="sm"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleSelectCompany(company);
                      }}
                    >
                      Acessar
                    </Button>
                    
                    {/* Botão de configurações só aparece se o usuário for o dono */}
                    {user?.userId === company.userId && (
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={(e) => {
                          e.stopPropagation();
                          navigate(`/company/${company.id}/settings`);
                        }}
                      >
                        <Settings className="h-4 w-4" />
                      </Button>
                    )}
                  </div>
                </CardContent>
              </Card>
            ))}

            {/* Add New Company Card */}
            <Card
              className="border-2 border-dashed border-gray-300 hover:border-primary-500 transition-colors cursor-pointer bg-gray-50/50"
              onClick={() => setIsAddDialogOpen(true)}
            >
              <CardContent className="p-6 flex flex-col items-center justify-center h-full min-h-[250px]">
                <div className="w-16 h-16 bg-primary-100 rounded-full flex items-center justify-center mb-4">
                  <Plus className="h-8 w-8 text-primary-600" />
                </div>
                <h3 className="text-lg font-semibold text-gray-700 mb-2">
                  Nova Empresa
                </h3>
                <p className="text-sm text-gray-500 text-center">
                  Adicione uma nova empresa para gerenciar
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>

      {/* Dialog para adicionar nova empresa */}
      <AddCompanyDialog
        open={isAddDialogOpen}
        onOpenChange={setIsAddDialogOpen}
        onSuccess={handleCompanyCreated}
      />
    </div>
  );
}
