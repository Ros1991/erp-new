import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { Input } from '../../components/ui/Input';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { SwipeToDelete } from '../../components/ui/SwipeToDelete';
import { Protected } from '../../components/permissions/Protected';
import { usePermissions } from '../../contexts/PermissionContext';
import { useToast } from '../../contexts/ToastContext';
import companyUserService, { type CompanyUser, type CompanyUserFilters } from '../../services/companyUserService';
import { 
  Plus, 
  Search, 
  User,
  UserCheck,
  ShieldCheck,
  Edit,
  Trash2,
  ChevronDown,
  ChevronUp,
  ChevronsLeft,
  ChevronsRight,
  ChevronLeft,
  ChevronRight,
  Filter
} from 'lucide-react';

export function Users() {
  const navigate = useNavigate();
  const { showSuccess, handleBackendError } = useToast();
  const { hasPermission } = usePermissions();
  const [users, setUsers] = useState<CompanyUser[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  // Helper: Retorna email, telefone ou CPF (nessa ordem)
  const getUserIdentifier = (user: CompanyUser): string => {
    if (user.userEmail) return user.userEmail;
    if (user.userPhone) return user.userPhone;
    if (user.userCpf) return user.userCpf;
    return 'Sem identificação';
  };
  const [searchTerm, setSearchTerm] = useState('');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [showMobileFilters, setShowMobileFilters] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState<CompanyUser | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const pageSize = 10;

  const loadUsers = useCallback(async () => {
    setIsLoading(true);
    try {
      const filters: CompanyUserFilters = {
        searchTerm: searchTerm.trim() || undefined,
        page: currentPage,
        pageSize,
        sortBy: 'userEmail',
        sortDirection: sortDirection
      };

      const result = await companyUserService.getPaged(filters);
      setUsers(result.items);
      setTotalPages(result.totalPages);
      setTotalCount(result.total || result.totalCount || 0);
    } catch (err: any) {
      handleBackendError(err);
      setUsers([]);
    } finally {
      setIsLoading(false);
    }
  }, [searchTerm, sortDirection, currentPage, handleBackendError]);

  // Debounce no filtro de busca
  useEffect(() => {
    const timer = setTimeout(() => {
      loadUsers();
    }, 500);

    return () => clearTimeout(timer);
  }, [loadUsers]);

  const handleSort = () => {
    setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
  };

  const handleDeleteClick = (user: CompanyUser) => {
    setUserToDelete(user);
    setDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!userToDelete) return;

    setIsDeleting(true);
    try {
      await companyUserService.delete(userToDelete.companyUserId);
      showSuccess('Usuário removido da empresa com sucesso!');
      setDeleteDialogOpen(false);
      setUserToDelete(null);
      loadUsers(); // Recarrega a lista
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsDeleting(false);
    }
  };

  const handleCancelDelete = () => {
    setDeleteDialogOpen(false);
    setUserToDelete(null);
  };

  return (
    <MainLayout>
      {/* Header */}
      <div className="mb-6">
        {/* Desktop Header with Button */}
        <div className="hidden sm:flex sm:items-start sm:justify-between gap-3 mb-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Usuários</h1>
            <p className="text-base text-gray-600 mt-1">Gerencie os usuários da empresa</p>
          </div>
          <Protected requires="user.canCreate">
            <Button onClick={() => navigate('/users/new')}>
              <Plus className="h-4 w-4 mr-2" />
              Novo Usuário
            </Button>
          </Protected>
        </div>

        {/* Mobile Header with Filter Button */}
        <div className="sm:hidden mb-4">
          <div className="flex items-start justify-between gap-2">
            <div className="flex-1 min-w-0">
              <h1 className="text-2xl font-bold text-gray-900">Usuários</h1>
              <p className="text-sm text-gray-600 mt-1">Gerencie os usuários da empresa</p>
            </div>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setShowMobileFilters(!showMobileFilters)}
              className={`h-9 w-9 p-0 flex-shrink-0 ${showMobileFilters ? 'bg-primary-50 border-primary-300' : ''}`}
              title={showMobileFilters ? 'Ocultar filtros' : 'Mostrar filtros'}
            >
              <Filter className={`h-4 w-4 ${showMobileFilters ? 'text-primary-600' : ''}`} />
            </Button>
          </div>
        </div>

        {/* Desktop Filters (always visible) */}
        <div className="hidden sm:block relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            type="text"
            placeholder="Buscar por email, telefone, CPF ou cargo..."
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setCurrentPage(1);
            }}
            className="pl-10 h-9"
          />
        </div>

        {/* Mobile Filters (collapsible) */}
        {showMobileFilters && (
          <div className="sm:hidden relative mb-4 animate-in slide-in-from-top-2 duration-200">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
            <Input
              type="text"
              placeholder="Buscar por email, telefone, CPF ou cargo..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setCurrentPage(1);
              }}
              className="pl-10 h-9"
            />
          </div>
        )}
      </div>

      {/* Floating Action Button (Mobile only) */}
      <Protected requires="user.canCreate">
        <button
          onClick={() => navigate('/users/new')}
          className="sm:hidden fixed bottom-6 right-6 w-14 h-14 bg-primary-600 text-white rounded-full shadow-lg hover:bg-primary-700 active:scale-95 transition-all flex items-center justify-center z-50"
          aria-label="Novo Usuário"
        >
          <Plus className="h-6 w-6" />
        </button>
      </Protected>

      {/* Desktop Table */}
      <div className="hidden lg:block">
        <Card className="overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left">
                    <button
                      onClick={handleSort}
                      className="flex items-center space-x-1 text-xs font-medium text-gray-700 uppercase tracking-wider hover:text-gray-900"
                    >
                      <span>Email</span>
                      {sortDirection === 'asc' ? 
                        <ChevronUp className="h-4 w-4" /> : 
                        <ChevronDown className="h-4 w-4" />
                      }
                    </button>
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Cargo
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {isLoading ? (
                  <tr>
                    <td colSpan={3} className="px-6 py-12 text-center">
                      <div className="flex justify-center">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
                      </div>
                    </td>
                  </tr>
                ) : users.length === 0 ? (
                  <tr>
                    <td colSpan={3} className="px-6 py-12 text-center text-gray-500">
                      Nenhum usuário encontrado
                    </td>
                  </tr>
                ) : (
                  users.map((user) => (
                    <tr key={user.companyUserId} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4">
                        <div className="flex items-center space-x-3">
                          {user.roleIsSystem ? (
                            <ShieldCheck className="h-5 w-5 text-purple-600" />
                          ) : user.roleId ? (
                            <UserCheck className="h-5 w-5 text-green-600" />
                          ) : (
                            <User className="h-5 w-5 text-gray-600" />
                          )}
                          <div>
                            <div className="font-medium text-gray-900">{getUserIdentifier(user)}</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        {user.roleName ? (
                          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                            user.roleIsSystem 
                              ? 'bg-purple-100 text-purple-800' 
                              : 'bg-blue-100 text-blue-800'
                          }`}>
                            {user.roleName}
                          </span>
                        ) : (
                          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                            Sem cargo
                          </span>
                        )}
                      </td>
                      <td className="px-6 py-4 text-right">
                        <div className="flex items-center justify-end gap-2">
                          <Protected requires="user.canEdit">
                            <Button 
                              variant="ghost" 
                              size="sm" 
                              title="Editar usuário"
                              onClick={() => navigate(`/users/${user.companyUserId}/edit`)}
                            >
                              <Edit className="h-4 w-4" />
                            </Button>
                          </Protected>
                          <Protected requires="user.canDelete">
                            <Button 
                              variant="ghost" 
                              size="sm" 
                              className="text-red-600 hover:text-red-700 hover:bg-red-50"
                              onClick={() => handleDeleteClick(user)}
                              title="Excluir usuário"
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </Protected>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </Card>
      </div>

      {/* Mobile Cards */}
      <div className="lg:hidden space-y-4">
        {isLoading ? (
          <Card className="overflow-hidden">
            <CardContent className="p-12 text-center rounded-lg">
              <div className="flex justify-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
              </div>
            </CardContent>
          </Card>
        ) : users.length === 0 ? (
          <Card className="overflow-hidden">
            <CardContent className="p-12 text-center text-gray-500 rounded-lg">
              Nenhum usuário encontrado
            </CardContent>
          </Card>
        ) : (
          users.map((user) => {
            const canEdit = hasPermission('user.canEdit');
            const canDelete = hasPermission('user.canDelete');
            const isDisabled = !canEdit && !canDelete;
            
            return (
              <SwipeToDelete
                key={user.companyUserId}
                onDelete={canDelete ? () => handleDeleteClick(user) : () => {}}
                onTap={canEdit ? () => navigate(`/users/${user.companyUserId}/edit`) : undefined}
                disabled={isDisabled}
                showDeleteButton={canDelete}
              >
                <Card className={`transition-all overflow-hidden ${
                  isDisabled 
                    ? 'opacity-60 cursor-not-allowed' 
                    : 'hover:shadow-md active:bg-gray-50 cursor-pointer'
                }`}>
                  <CardContent className="p-4 rounded-lg">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-3 flex-1 min-w-0">
                        {user.roleIsSystem ? (
                          <ShieldCheck className="h-6 w-6 text-purple-600 flex-shrink-0" />
                        ) : user.roleId ? (
                          <UserCheck className="h-6 w-6 text-green-600 flex-shrink-0" />
                        ) : (
                          <User className="h-6 w-6 text-gray-600 flex-shrink-0" />
                        )}
                        <div className="flex-1 min-w-0">
                          <h3 className="font-semibold text-gray-900 truncate">{getUserIdentifier(user)}</h3>
                        </div>
                      </div>
                      {user.roleName ? (
                        <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium flex-shrink-0 ml-2 ${
                          user.roleIsSystem 
                            ? 'bg-purple-100 text-purple-800' 
                            : 'bg-blue-100 text-blue-800'
                        }`}>
                          {user.roleName}
                        </span>
                      ) : (
                        <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 flex-shrink-0 ml-2">
                          Sem cargo
                        </span>
                      )}
                    </div>
                  </CardContent>
                </Card>
              </SwipeToDelete>
            );
          })
        )}
      </div>

      {/* Pagination and Results Info */}
      {!isLoading && users.length > 0 && (
        <div className="mt-6 pb-24 sm:pb-6">
          <div className="flex flex-col gap-4">
            {/* Results Count */}
            <div className="text-xs sm:text-sm text-gray-600 text-center sm:text-left">
              Exibindo {users.length} de {totalCount} usuário(s)
              {totalPages > 1 && (
                <span className="hidden sm:inline"> • Página {currentPage} de {totalPages}</span>
              )}
            </div>

            {/* Pagination Controls */}
            {totalPages > 1 && (
              <div className="flex items-center gap-1 justify-center flex-wrap">
                {/* First Page */}
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setCurrentPage(1)}
                  disabled={currentPage === 1}
                  className="h-9 w-9 p-0"
                  title="Primeira página"
                >
                  <ChevronsLeft className="h-4 w-4" />
                </Button>

                {/* Previous Page */}
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
                  disabled={currentPage === 1}
                  className="h-9 w-9 p-0"
                  title="Página anterior"
                >
                  <ChevronLeft className="h-4 w-4" />
                </Button>

                {/* Page Numbers - Full display on mobile and desktop */}
                <div className="flex items-center gap-1">
                  {Array.from({ length: totalPages }, (_, i) => i + 1)
                    .filter((page) => {
                      // Show first page, last page, current page, and adjacent pages
                      if (page === 1 || page === totalPages) return true;
                      if (Math.abs(page - currentPage) <= 1) return true;
                      return false;
                    })
                    .map((page, index, array) => {
                      // Add ellipsis between non-consecutive pages
                      const prevPage = array[index - 1];
                      const showEllipsis = prevPage && page - prevPage > 1;

                      return (
                        <div key={page} className="flex items-center gap-1">
                          {showEllipsis && (
                            <span className="px-1 sm:px-2 text-gray-400 text-sm">...</span>
                          )}
                          <Button
                            variant={currentPage === page ? "default" : "outline"}
                            size="sm"
                            onClick={() => setCurrentPage(page)}
                            className="h-9 min-w-[32px] sm:min-w-9 px-2 sm:px-3 text-sm"
                          >
                            {page}
                          </Button>
                        </div>
                      );
                    })}
                </div>

                {/* Next Page */}
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setCurrentPage((prev) => Math.min(totalPages, prev + 1))}
                  disabled={currentPage === totalPages}
                  className="h-9 w-9 p-0"
                  title="Próxima página"
                >
                  <ChevronRight className="h-4 w-4" />
                </Button>

                {/* Last Page */}
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setCurrentPage(totalPages)}
                  disabled={currentPage === totalPages}
                  className="h-9 w-9 p-0"
                  title="Última página"
                >
                  <ChevronsRight className="h-4 w-4" />
                </Button>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Confirm Delete Dialog */}
      <ConfirmDialog
        isOpen={deleteDialogOpen}
        onClose={handleCancelDelete}
        onConfirm={handleConfirmDelete}
        title="Excluir Usuário"
        description={
          userToDelete ? (
            <>
              <p className="text-base mb-2">
                Tem certeza que deseja excluir o usuário{' '}
                <span className="font-semibold text-gray-900">{userToDelete.userEmail}</span>?
              </p>
            </>
          ) : ''
        }
        confirmText="Excluir"
        cancelText="Cancelar"
        variant="danger"
        isLoading={isDeleting}
      />
    </MainLayout>
  );
}
