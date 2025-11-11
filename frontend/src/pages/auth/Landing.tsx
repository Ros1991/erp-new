import { Link } from 'react-router-dom';
import { Button } from '../../components/ui/Button';
import { TrendingUp, Shield, Users, BarChart, CheckCircle, ArrowRight } from 'lucide-react';

export function Landing() {
  return (
    <div className="min-h-screen">
      {/* Header */}
      <header className="fixed top-0 w-full bg-white/80 backdrop-blur-md z-50 border-b border-gray-100">
        <div className="container mx-auto px-6 py-4">
          <nav className="flex items-center justify-between">
            <div className="flex items-center space-x-2">
              <img src="/logo-full.svg" alt="MeuGestor" className="h-12" />
            </div>
            
            <div className="hidden md:flex items-center space-x-8">
              <a href="#features" className="text-gray-600 hover:text-primary-600 transition">
                Recursos
              </a>
              <a href="#pricing" className="text-gray-600 hover:text-primary-600 transition">
                Preços
              </a>
              <a href="#contact" className="text-gray-600 hover:text-primary-600 transition">
                Contato
              </a>
            </div>
            
            <div className="flex items-center space-x-4">
              <Link to="/login">
                <Button variant="ghost">Entrar</Button>
              </Link>
              <Link to="/register">
                <Button>Começar Grátis</Button>
              </Link>
            </div>
          </nav>
        </div>
      </header>

      {/* Hero Section */}
      <section className="pt-32 pb-20 px-6">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center">
            <h1 className="text-5xl md:text-6xl font-bold text-gray-900 mb-6">
              Gerencie sua empresa com
              <span className="text-primary-600"> inteligência</span>
            </h1>
            <p className="text-xl text-gray-600 mb-8 max-w-3xl mx-auto">
              Sistema completo de gestão empresarial para pequenas e médias empresas.
              Simplifique processos, aumente a produtividade e tome decisões baseadas em dados.
            </p>
            <div className="flex items-center justify-center space-x-4">
              <Link to="/register">
                <Button size="lg" className="px-8">
                  Começar Grátis
                  <ArrowRight className="ml-2 h-5 w-5" />
                </Button>
              </Link>
              <Button size="lg" variant="outline">
                Assistir Demo
              </Button>
            </div>
          </div>

          {/* Dashboard Preview */}
          <div className="mt-16 rounded-2xl shadow-2xl overflow-hidden border border-gray-200">
            <img 
              src="/dashboard-preview.png" 
              alt="Dashboard Preview" 
              className="w-full"
              onError={(e) => {
                e.currentTarget.src = 'https://via.placeholder.com/1200x600/f3f4f6/6b7280?text=Dashboard+Preview';
              }}
            />
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-20 bg-gray-50">
        <div className="container mx-auto px-6">
          <div className="text-center mb-16">
            <h2 className="text-4xl font-bold text-gray-900 mb-4">
              Recursos que impulsionam seu negócio
            </h2>
            <p className="text-xl text-gray-600">
              Tudo que você precisa em um só lugar
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                icon: <BarChart className="h-8 w-8" />,
                title: "Dashboard Inteligente",
                description: "Visualize métricas importantes e tome decisões baseadas em dados reais."
              },
              {
                icon: <Users className="h-8 w-8" />,
                title: "Gestão de Equipes",
                description: "Controle funcionários, tarefas e departamentos de forma integrada."
              },
              {
                icon: <Shield className="h-8 w-8" />,
                title: "Segurança Avançada",
                description: "Seus dados protegidos com criptografia de ponta e backups automáticos."
              },
              {
                icon: <TrendingUp className="h-8 w-8" />,
                title: "Relatórios Completos",
                description: "Relatórios detalhados e personalizáveis para acompanhar o crescimento."
              },
              {
                icon: <CheckCircle className="h-8 w-8" />,
                title: "Multi-empresas",
                description: "Gerencie múltiplas empresas em uma única conta."
              },
              {
                icon: <ArrowRight className="h-8 w-8" />,
                title: "API Integrada",
                description: "Integre com outras ferramentas que você já usa no dia a dia."
              }
            ].map((feature, index) => (
              <div key={index} className="bg-white rounded-xl p-8 shadow-sm hover:shadow-md transition">
                <div className="text-primary-600 mb-4">{feature.icon}</div>
                <h3 className="text-xl font-semibold mb-2">{feature.title}</h3>
                <p className="text-gray-600">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 bg-primary-600">
        <div className="container mx-auto px-6 text-center">
          <h2 className="text-4xl font-bold text-white mb-4">
            Pronto para transformar sua gestão?
          </h2>
          <p className="text-xl text-white/90 mb-8">
            Comece gratuitamente e escale conforme cresce
          </p>
          <Link to="/register">
            <Button size="lg" variant="secondary" className="px-8">
              Criar Conta Grátis
              <ArrowRight className="ml-2 h-5 w-5" />
            </Button>
          </Link>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-gray-900 text-gray-400 py-12">
        <div className="container mx-auto px-6">
          <div className="text-center">
            <p>&copy; 2024 MeuGestor. Todos os direitos reservados.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}
