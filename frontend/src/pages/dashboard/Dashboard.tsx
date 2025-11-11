import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Button } from '../../components/ui/Button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import {
  BarChart3,
  Building2,
  ChevronDown,
  DollarSign,
  Home,
  LogOut,
  Menu,
  Package,
  Settings,
  ShoppingCart,
  TrendingUp,
  Users,
  X
} from 'lucide-react';

export function Dashboard() {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const { user, selectedCompany, logout } = useAuth();

  const menuItems = [
    { icon: Home, label: 'Dashboard', path: '/dashboard', active: true },
    { icon: Users, label: 'Funcionários', path: '/employees' },
    { icon: ShoppingCart, label: 'Vendas', path: '/sales' },
    { icon: Package, label: 'Produtos', path: '/products' },
    { icon: DollarSign, label: 'Financeiro', path: '/financial' },
    { icon: BarChart3, label: 'Relatórios', path: '/reports' },
    { icon: Settings, label: 'Configurações', path: '/settings' },
  ];

  const stats = [
    {
      title: 'Empresas Ativas',
      value: '12',
      change: '+2 este mês',
      icon: Building2,
      color: 'text-blue-600',
      bgColor: 'bg-blue-100',
    },
    {
      title: 'Funcionários',
      value: '248',
      change: '+15 este mês',
      icon: Users,
      color: 'text-green-600',
      bgColor: 'bg-green-100',
    },
    {
      title: 'Tarefas Pendentes',
      value: '47',
      change: '12 atrasadas',
      icon: Package,
      color: 'text-yellow-600',
      bgColor: 'bg-yellow-100',
    },
    {
      title: 'Receita Mensal',
      value: 'R$ 125.420',
      change: '+18% vs mês anterior',
      icon: DollarSign,
      color: 'text-purple-600',
      bgColor: 'bg-purple-100',
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="fixed top-0 w-full bg-white border-b border-gray-200 z-40">
        <div className="flex items-center justify-between px-6 py-4">
          <div className="flex items-center space-x-4">
            <button
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="lg:hidden text-gray-600 hover:text-gray-900"
            >
              {sidebarOpen ? <X size={24} /> : <Menu size={24} />}
            </button>
            
            <img src="/logo-full.svg" alt="MeuGestor" className="h-10" />
            
            <div className="hidden md:flex items-center space-x-2">
              <span className="text-sm text-gray-600">Empresa:</span>
              <Button variant="ghost" size="sm" className="font-medium">
                {selectedCompany?.name}
                <ChevronDown className="ml-2 h-4 w-4" />
              </Button>
            </div>
          </div>

          <div className="flex items-center space-x-4">
            <span className="hidden sm:block text-sm text-gray-600">
              Olá, <span className="font-medium">{user?.name}</span>
            </span>
            <Button variant="ghost" size="sm" onClick={logout}>
              <LogOut className="h-4 w-4 mr-2" />
              Sair
            </Button>
          </div>
        </div>
      </header>

      <div className="flex pt-16">
        {/* Sidebar */}
        <aside
          className={`fixed left-0 top-16 h-[calc(100vh-4rem)] bg-white border-r border-gray-200 transition-transform z-30 ${
            sidebarOpen ? 'translate-x-0' : '-translate-x-full'
          } lg:translate-x-0 lg:w-64 w-64`}
        >
          <nav className="p-4 space-y-1">
            {menuItems.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                  item.active
                    ? 'bg-primary-50 text-primary-600'
                    : 'text-gray-700 hover:bg-gray-100'
                }`}
              >
                <item.icon className="h-5 w-5" />
                <span className="font-medium">{item.label}</span>
              </Link>
            ))}
          </nav>
        </aside>

        {/* Main Content */}
        <main className={`flex-1 ${sidebarOpen ? 'lg:ml-64' : ''} p-6`}>
          <div className="max-w-7xl mx-auto">
            {/* Welcome Section */}
            <div className="mb-8">
              <h1 className="text-3xl font-bold text-gray-900">
                Bem-vindo de volta, {user?.name?.split(' ')[0]}!
              </h1>
              <p className="text-gray-600 mt-2">
                Dashboard da empresa - Aqui você tem um resumo das atividades da empresa hoje
              </p>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              {stats.map((stat, index) => (
                <Card key={index}>
                  <CardContent className="p-6">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-600">{stat.title}</p>
                        <p className="text-2xl font-bold text-gray-900 mt-1">
                          {stat.value}
                        </p>
                        <p className="text-sm text-gray-500 mt-1">
                          {stat.change}
                        </p>
                      </div>
                      <div className={`${stat.bgColor} p-3 rounded-lg`}>
                        <stat.icon className={`h-6 w-6 ${stat.color}`} />
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Quick Actions */}
            <Card className="mb-8">
              <CardHeader>
                <CardTitle>Ações Rápidas</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                  <Button className="flex-col h-24">
                    <Building2 className="h-6 w-6 mb-2" />
                    <span>Nova Empresa</span>
                  </Button>
                  <Button variant="outline" className="flex-col h-24">
                    <Users className="h-6 w-6 mb-2" />
                    <span>Novo Funcionário</span>
                  </Button>
                  <Button variant="outline" className="flex-col h-24">
                    <Package className="h-6 w-6 mb-2" />
                    <span>Nova Tarefa</span>
                  </Button>
                  <Button variant="outline" className="flex-col h-24">
                    <BarChart3 className="h-6 w-6 mb-2" />
                    <span>Gerar Relatório</span>
                  </Button>
                </div>
              </CardContent>
            </Card>

            {/* Recent Activity & Next Tasks */}
            <div className="grid lg:grid-cols-2 gap-6">
              {/* Recent Activity */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center justify-between">
                    <span>Atividade Recente</span>
                    <TrendingUp className="h-5 w-5 text-gray-400" />
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {[
                      'Nova empresa cadastrada',
                      'Relatório mensal gerado',
                      'Tarefa #23 concluída',
                      'Pagamento recebido',
                      'Atualização de documentação',
                    ].map((activity, index) => (
                      <div key={index} className="flex items-center space-x-3">
                        <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                        <span className="text-sm text-gray-600">{activity}</span>
                        <span className="text-xs text-gray-400 ml-auto">
                          há {index + 1}h
                        </span>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>

              {/* Próximas Tarefas */}
              <Card>
                <CardHeader>
                  <CardTitle>Próximas Tarefas</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {[
                      { title: 'Revisar relatório mensal', priority: 'alta' },
                      { title: 'Reunião com fornecedores', priority: 'média' },
                      { title: 'Atualizar documentação', priority: 'baixa' },
                      { title: 'Fechar folha de pagamento', priority: 'alta' },
                      { title: 'Backup do sistema', priority: 'média' },
                    ].map((task, index) => (
                      <div key={index} className="flex items-center justify-between">
                        <span className="text-sm text-gray-700">{task.title}</span>
                        <span
                          className={`px-2 py-1 text-xs rounded-full ${
                            task.priority === 'alta'
                              ? 'bg-red-100 text-red-700'
                              : task.priority === 'média'
                              ? 'bg-yellow-100 text-yellow-700'
                              : 'bg-green-100 text-green-700'
                          }`}
                        >
                          {task.priority}
                        </span>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
