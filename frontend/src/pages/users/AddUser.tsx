import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { Input } from '../../components/ui/Input';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '../../components/ui/Dialog';
import { useToast } from '../../contexts/ToastContext';
import userService, { type User } from '../../services/userService';
import roleService, { type Role } from '../../services/roleService';
import companyUserService from '../../services/companyUserService';
import { Search, Plus, UserPlus, ArrowLeft, User as UserIcon, Mail, Phone, CreditCard } from 'lucide-react';

export function AddUser() {
  const navigate = useNavigate();
  const { showSuccess, showError, handleBackendError } = useToast();
  
  // Estados de busca de usuário
  const [searchTerm, setSearchTerm] = useState('');
  const [users, setUsers] = useState<User[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  
  // Estados de roles
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedRole, setSelectedRole] = useState<number | null>(null);
  
  // Estado de seleção
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  
  // Modal de novo usuário
  const [showNewUserModal, setShowNewUserModal] = useState(false);
  const [newUserData, setNewUserData] = useState({
    email: '',
    phone: '',
    cpf: '',
    password: '',
    confirmPassword: ''
  });
  const [isCreatingUser, setIsCreatingUser] = useState(false);

  // Carregar roles ao montar
  useEffect(() => {
    loadRoles();
  }, []);

  const loadRoles = async () => {
    try {
      const roles = await roleService.getAllRoles();
      setRoles(roles);
    } catch (error: any) {
      handleBackendError(error);
    }
  };

  // Buscar usuários
  const handleSearch = useCallback(async () => {
    const trimmedSearch = searchTerm.trim();
    
    // Só busca se tiver 3 ou mais caracteres
    if (trimmedSearch.length < 3) {
      setUsers([]);
      return;
    }

    setIsSearching(true);
    try {
      const result = await userService.getPaged({
        searchTerm: trimmedSearch,
        pageSize: 10
      });
      setUsers(result.items);
    } catch (error: any) {
      handleBackendError(error);
    } finally {
      setIsSearching(false);
    }
  }, [searchTerm, handleBackendError]);

  // Debounce na busca
  useEffect(() => {
    const timer = setTimeout(() => {
      handleSearch();
    }, 500);

    return () => clearTimeout(timer);
  }, [handleSearch]);

  // Selecionar usuário
  const handleSelectUser = (user: User) => {
    setSelectedUser(user);
    setSelectedRole(null); // Reset role selection
  };

  // Formatação de telefone
  const formatPhone = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 11) {
      return numbers
        .replace(/(\d{2})(\d)/, '($1) $2')
        .replace(/(\d{5})(\d)/, '$1-$2');
    }
    return value;
  };

  // Formatação de CPF
  const formatCpf = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 11) {
      return numbers
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    }
    return value;
  };

  // Criar novo usuário
  const handleCreateNewUser = async () => {
    // Validação: pelo menos um identificador obrigatório
    if (!newUserData.email && !newUserData.phone && !newUserData.cpf) {
      showError('Por favor, preencha pelo menos um: E-mail, Telefone ou CPF');
      return;
    }

    // Validação de e-mail
    if (newUserData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(newUserData.email)) {
      showError('E-mail inválido');
      return;
    }

    // Validação de telefone (11 dígitos)
    if (newUserData.phone && newUserData.phone.replace(/\D/g, '').length !== 11) {
      showError('Telefone deve ter 11 dígitos (DDD + número)');
      return;
    }

    // Validação de CPF (11 dígitos)
    if (newUserData.cpf && newUserData.cpf.replace(/\D/g, '').length !== 11) {
      showError('CPF deve ter 11 dígitos');
      return;
    }

    // Validação de senha
    if (!newUserData.password) {
      showError('Senha é obrigatória');
      return;
    }

    if (newUserData.password.length < 6) {
      showError('A senha deve ter no mínimo 6 caracteres');
      return;
    }

    if (newUserData.password !== newUserData.confirmPassword) {
      showError('As senhas não coincidem');
      return;
    }

    setIsCreatingUser(true);
    try {
      // Remover formatação de telefone e CPF antes de enviar
      const cleanPhone = newUserData.phone ? newUserData.phone.replace(/\D/g, '') : undefined;
      const cleanCpf = newUserData.cpf ? newUserData.cpf.replace(/\D/g, '') : undefined;

      const createdUser = await userService.create({
        email: newUserData.email || undefined,
        phone: cleanPhone,
        cpf: cleanCpf,
        password: newUserData.password
      });
      
      showSuccess('Usuário criado com sucesso!');
      setShowNewUserModal(false);
      
      // Definir o identificador para busca (prioridade: email > phone > cpf)
      const searchIdentifier = newUserData.email || newUserData.phone || newUserData.cpf || '';
      setSearchTerm(searchIdentifier);
      
      // Selecionar o usuário criado
      setSelectedUser(createdUser);
      
      // Atualizar lista de busca com o novo usuário no topo
      setUsers(prev => [createdUser, ...prev]);
      
      // Limpar formulário
      setNewUserData({ email: '', phone: '', cpf: '', password: '', confirmPassword: '' });
    } catch (error: any) {
      handleBackendError(error);
    } finally {
      setIsCreatingUser(false);
    }
  };

  // Salvar vínculo usuário-empresa
  const handleSave = async () => {
    if (!selectedUser) {
      showError('Selecione um usuário');
      return;
    }

    if (!selectedRole) {
      showError('Selecione um cargo');
      return;
    }

    setIsSaving(true);
    try {
      await companyUserService.create({
        userId: selectedUser.userId,
        roleId: selectedRole
      });

      showSuccess('Usuário adicionado à empresa com sucesso!');
      navigate('/users');
    } catch (error: any) {
      handleBackendError(error);
    } finally {
      setIsSaving(false);
    }
  };

  // Helper para exibir identificador do usuário
  const getUserIdentifier = (user: User): string => {
    if (user.email) return user.email;
    if (user.phone) return user.phone;
    if (user.cpf) return user.cpf;
    return 'Sem identificação';
  };

  return (
    <MainLayout>
      <div className="pb-20 sm:pb-0">
        {/* Desktop Header with Button */}
        <div className="hidden sm:block mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/users')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <div className="flex items-start justify-between gap-3">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Adicionar Usuário à Empresa</h1>
              <p className="text-gray-600 mt-1">
                Busque um usuário existente ou crie um novo
              </p>
            </div>
            <Button
              onClick={() => setShowNewUserModal(true)}
              className="flex items-center gap-2"
            >
              <Plus className="h-4 w-4" />
              Novo Usuário
            </Button>
          </div>
        </div>

        {/* Mobile Header */}
        <div className="sm:hidden mb-4">
          <Button
            variant="ghost"
            onClick={() => navigate('/users')}
            className="mb-3 -ml-2"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-2xl font-bold text-gray-900">Adicionar Usuário</h1>
          <p className="text-sm text-gray-600 mt-1">Busque um usuário existente</p>
        </div>

        {/* Search (always visible) */}
        <div className="relative mb-4">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            type="text"
            placeholder="Buscar usuário por email, telefone ou CPF (mínimo 3 caracteres)..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10 h-9"
          />
        </div>

      {/* Floating Action Button (Mobile only) */}
      <button
        onClick={() => setShowNewUserModal(true)}
        className="sm:hidden fixed bottom-6 right-6 w-14 h-14 bg-primary-600 text-white rounded-full shadow-lg hover:bg-primary-700 active:scale-95 transition-all flex items-center justify-center z-50"
        aria-label="Novo Usuário"
      >
        <Plus className="h-6 w-6" />
      </button>

      {/* Lista de Usuários Encontrados */}
      <Card className="mb-6">
        <CardContent className="p-6">
          <div className="space-y-4">

            {/* Lista de Usuários Encontrados */}
            {isSearching ? (
              <div className="text-center py-8 text-gray-500">Buscando...</div>
            ) : users.length > 0 ? (
              <div className="space-y-2">
                {users.map((user) => (
                  <div
                    key={user.userId}
                    onClick={() => {
                      // Se clicar no usuário já selecionado, desseleciona
                      if (selectedUser?.userId === user.userId) {
                        setSelectedUser(null);
                        setSelectedRole(null);
                      } else {
                        handleSelectUser(user);
                      }
                    }}
                    className={`p-4 border rounded-lg cursor-pointer transition-all ${
                      selectedUser?.userId === user.userId
                        ? 'border-primary-500 bg-primary-50'
                        : 'border-gray-200 hover:border-primary-300 hover:bg-gray-50'
                    }`}
                  >
                    <div className="flex items-center gap-3">
                      <div className={`p-2 rounded-full ${
                        selectedUser?.userId === user.userId
                          ? 'bg-primary-100'
                          : 'bg-gray-100'
                      }`}>
                        <UserIcon className={`h-5 w-5 ${
                          selectedUser?.userId === user.userId
                            ? 'text-primary-600'
                            : 'text-gray-600'
                        }`} />
                      </div>
                      <div className="flex-1">
                        <div className="font-medium text-gray-900">{getUserIdentifier(user)}</div>
                        <div className="flex gap-4 text-xs text-gray-500 mt-1">
                          {user.email && (
                            <span className="flex items-center gap-1">
                              <Mail className="h-3 w-3" />
                              {user.email}
                            </span>
                          )}
                          {user.phone && (
                            <span className="flex items-center gap-1">
                              <Phone className="h-3 w-3" />
                              {user.phone}
                            </span>
                          )}
                          {/* CPF só aparece embaixo se tiver email ou telefone em cima */}
                          {user.cpf && (user.email || user.phone) && (
                            <span className="flex items-center gap-1">
                              <CreditCard className="h-3 w-3" />
                              {user.cpf}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : searchTerm.trim() ? (
              <div className="text-center py-8 text-gray-500">
                Nenhum usuário encontrado
              </div>
            ) : null}
          </div>
        </CardContent>
      </Card>

      {/* Seleção de Cargo (aparece quando usuário é selecionado) */}
      {selectedUser && (
        <Card className="mb-6">
          <CardContent className="p-6">
            <div className="space-y-4">
              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2">Selecione o Cargo</h3>
                <p className="text-sm text-gray-600">
                  Usuário selecionado: <span className="font-medium">{getUserIdentifier(selectedUser)}</span>
                </p>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                {roles.map((role) => (
                  <div
                    key={role.roleId}
                    onClick={() => {
                      // Se clicar no cargo já selecionado, desseleciona
                      if (selectedRole === role.roleId) {
                        setSelectedRole(null);
                      } else {
                        setSelectedRole(role.roleId);
                      }
                    }}
                    className={`p-4 border rounded-lg cursor-pointer transition-all ${
                      selectedRole === role.roleId
                        ? 'border-primary-500 bg-primary-50'
                        : 'border-gray-200 hover:border-primary-300 hover:bg-gray-50'
                    }`}
                  >
                    <div className="font-medium text-gray-900">{role.name}</div>
                    {role.isSystem && (
                      <div className="text-xs text-purple-600 mt-1">Sistema</div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Actions (fora do card, igual ao RoleForm) */}
      {selectedUser && (
        <div className="flex items-center justify-end gap-3">
          <Button
            onClick={handleSave}
            disabled={!selectedRole || isSaving}
            className="flex items-center gap-2"
          >
            <UserPlus className="h-4 w-4" />
            {isSaving ? 'Salvando...' : 'Adicionar à Empresa'}
          </Button>
        </div>
      )}

      {/* Modal de Novo Usuário */}
      <Dialog open={showNewUserModal} onOpenChange={setShowNewUserModal}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Criar Novo Usuário</DialogTitle>
            <DialogDescription>
              Preencha os dados do novo usuário. Pelo menos um identificador (email, telefone ou CPF) é obrigatório.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4 px-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Email
              </label>
              <Input
                type="email"
                value={newUserData.email}
                onChange={(e) => setNewUserData(prev => ({ ...prev, email: e.target.value }))}
                placeholder="usuario@email.com"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Telefone
              </label>
              <Input
                type="text"
                value={newUserData.phone}
                onChange={(e) => setNewUserData(prev => ({ ...prev, phone: formatPhone(e.target.value) }))}
                placeholder="(11) 99999-9999"
                maxLength={15}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                CPF
              </label>
              <Input
                type="text"
                value={newUserData.cpf}
                onChange={(e) => setNewUserData(prev => ({ ...prev, cpf: formatCpf(e.target.value) }))}
                placeholder="123.456.789-00"
                maxLength={14}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Senha <span className="text-red-500">*</span>
              </label>
              <Input
                type="password"
                value={newUserData.password}
                onChange={(e) => setNewUserData(prev => ({ ...prev, password: e.target.value }))}
                placeholder="••••••••"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Confirmar Senha <span className="text-red-500">*</span>
              </label>
              <Input
                type="password"
                value={newUserData.confirmPassword}
                onChange={(e) => setNewUserData(prev => ({ ...prev, confirmPassword: e.target.value }))}
                placeholder="••••••••"
              />
            </div>
          </div>

          <div className="flex justify-end gap-2 px-6 pb-4">
            <Button
              variant="outline"
              onClick={() => {
                setShowNewUserModal(false);
                setNewUserData({ email: '', phone: '', cpf: '', password: '', confirmPassword: '' });
              }}
            >
              Cancelar
            </Button>
            <Button
              onClick={handleCreateNewUser}
              disabled={isCreatingUser}
            >
              {isCreatingUser ? 'Criando...' : 'Criar Usuário'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
      </div>
    </MainLayout>
  );
}
