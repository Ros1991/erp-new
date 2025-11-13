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
import accountPayableReceivableService, { 
  type AccountPayableReceivable, 
  type AccountPayableReceivableFilters 
} from '../../services/accountPayableReceivableService';
import { 
  Plus, 
  Search, 
  FileText,
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

export function AccountPayableReceivables() {
  const navigate = useNavigate();
  const { showSuccess, handleBackendError } = useToast();
  const { hasPermission } = usePermissions();
  const [items, setItems] = useState<AccountPayableReceivable[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [showMobileFilters, setShowMobileFilters] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [itemToDelete, setItemToDelete] = useState<AccountPayableReceivable | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const pageSize = 10;

  const loadItems = useCallback(async () => {
    setIsLoading(true);
    try {
      const filters: AccountPayableReceivableFilters = {
        search: searchTerm || undefined,
        page: currentPage,
        pageSize,
        orderBy: 'dueDate',
        isAscending: sortDirection === 'asc'
      };

      const result = await accountPayableReceivableService.getAccountPayableReceivables(filters);
      setItems(result.items);
      setTotalPages(result.totalPages);
      setTotalCount(result.totalCount);
    } catch (err: any) {
      handleBackendError(err);
      setItems([]);
    } finally {
      setIsLoading(false);
    }
  }, [searchTerm, sortDirection, currentPage, pageSize, handleBackendError]);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const handleSort = () => {
    setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
  };

  const handleDeleteClick = (item: AccountPayableReceivable) => {
    setItemToDelete(item);
    setDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!itemToDelete) return;

    setIsDeleting(true);
    try {
      await accountPayableReceivableService.deleteAccountPayableReceivable(itemToDelete.accountPayableReceivableId);
      showSuccess('Conta excluída com sucesso!');
      setDeleteDialogOpen(false);
      setItemToDelete(null);
      loadItems();
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsDeleting(false);
    }
  };

  const handleCancelDelete = () => {
    setDeleteDialogOpen(false);
    setItemToDelete(null);
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value / 100);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  return (
    <MainLayout>
      {/* Header */}
      <div className="mb-6">
        {/* Desktop Header with Button */}
        <div className="hidden sm:flex sm:items-start sm:justify-between gap-3 mb-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Contas a Pagar e Receber</h1>
            <p className="text-base text-gray-600 mt-1">Gerencie contas a pagar e a receber</p>
          </div>
          <Protected requires="accountPayableReceivable.canCreate">
            <Button onClick={() => navigate('/account-payable-receivable/new')}>
              <Plus className="h-4 w-4 mr-2" />
              Nova Conta
            </Button>
          </Protected>
        </div>

        {/* Mobile Header with Filter Button */}
        <div className="sm:hidden mb-4">
          <div className="flex items-start justify-between gap-2">
            <div className="flex-1 min-w-0">
              <h1 className="text-2xl font-bold text-gray-900">Contas</h1>
              <p className="text-sm text-gray-600 mt-1">A pagar e a receber</p>
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
            placeholder="Buscar por descrição..."
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
              placeholder="Buscar por descrição..."
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
      <Protected requires="accountPayableReceivable.canCreate">
        <button
          onClick={() => navigate('/account-payable-receivable/new')}
          className="sm:hidden fixed bottom-6 right-6 w-14 h-14 bg-primary-600 text-white rounded-full shadow-lg hover:bg-primary-700 active:scale-95 transition-all flex items-center justify-center z-50"
          aria-label="Nova Conta"
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
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Descrição
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Tipo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Valor
                  </th>
                  <th className="px-6 py-3 text-left">
                    <button
                      onClick={handleSort}
                      className="flex items-center space-x-1 text-xs font-medium text-gray-700 uppercase tracking-wider hover:text-gray-900"
                    >
                      <span>Vencimento</span>
                      {sortDirection === 'asc' ? 
                        <ChevronUp className="h-4 w-4" /> : 
                        <ChevronDown className="h-4 w-4" />
                      }
                    </button>
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-700 uppercase tracking-wider">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {isLoading ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center">
                      <div className="flex justify-center">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
                      </div>
                    </td>
                  </tr>
                ) : items.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-gray-500">
                      Nenhuma conta encontrada
                    </td>
                  </tr>
                ) : (
                  items.map((item) => (
                    <tr key={item.accountPayableReceivableId} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4">
                        <div className="font-medium text-gray-900">{item.description}</div>
                      </td>
                      <td className="px-6 py-4">
                        <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                          item.type === 'Payable' 
                            ? 'bg-red-100 text-red-800' 
                            : 'bg-green-100 text-green-800'
                        }`}>
                          {item.type === 'Payable' ? 'A Pagar' : 'A Receber'}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-900">
                        {formatCurrency(item.amount)}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-900">
                        {formatDate(item.dueDate)}
                      </td>
                      <td className="px-6 py-4">
                        <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                          item.isPaid 
                            ? 'bg-green-100 text-green-800' 
                            : 'bg-yellow-100 text-yellow-800'
                        }`}>
                          {item.isPaid ? 'Pago' : 'Pendente'}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-right">
                        <div className="flex items-center justify-end gap-2">
                          <Protected requires="accountPayableReceivable.canEdit">
                            <Button 
                              variant="ghost" 
                              size="sm" 
                              title="Editar conta"
                              onClick={() => navigate(`/account-payable-receivable/${item.accountPayableReceivableId}/edit`)}
                            >
                              <Edit className="h-4 w-4" />
                            </Button>
                          </Protected>
                          <Protected requires="accountPayableReceivable.canDelete">
                            <Button 
                              variant="ghost" 
                              size="sm" 
                              className="text-red-600 hover:text-red-700 hover:bg-red-50"
                              onClick={() => handleDeleteClick(item)}
                              title="Excluir conta"
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
          <Card>
            <CardContent className="p-12 text-center">
              <div className="flex justify-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
              </div>
            </CardContent>
          </Card>
        ) : items.length === 0 ? (
          <Card>
            <CardContent className="p-12 text-center text-gray-500">
              Nenhuma conta encontrada
            </CardContent>
          </Card>
        ) : (
          items.map((item) => {
            const canEdit = hasPermission('accountPayableReceivable.canEdit');
            const canDelete = hasPermission('accountPayableReceivable.canDelete');
            const isDisabled = !canEdit && !canDelete;
            
            return (
              <SwipeToDelete
                key={item.accountPayableReceivableId}
                onDelete={canDelete ? () => handleDeleteClick(item) : () => {}}
                onTap={canEdit ? () => navigate(`/account-payable-receivable/${item.accountPayableReceivableId}/edit`) : undefined}
                disabled={isDisabled}
                showDeleteButton={canDelete}
              >
                <Card className={`transition-all overflow-hidden ${
                  isDisabled 
                    ? 'opacity-60 cursor-not-allowed' 
                    : 'hover:shadow-md active:bg-gray-50 cursor-pointer'
                }`}>
                  <CardContent className="p-4 rounded-lg">
                    <div className="flex items-start gap-3">
                      <div className="flex-shrink-0">
                        <div className={`h-10 w-10 rounded-full flex items-center justify-center ${
                          item.type === 'Payable' ? 'bg-red-100' : 'bg-green-100'
                        }`}>
                          <FileText className={`h-5 w-5 ${
                            item.type === 'Payable' ? 'text-red-600' : 'text-green-600'
                          }`} />
                        </div>
                      </div>
                      <div className="flex-1 min-w-0">
                        <h3 className="font-semibold text-gray-900 truncate">{item.description}</h3>
                        <p className="text-sm text-gray-600 mt-1">
                          Vencimento: {formatDate(item.dueDate)}
                        </p>
                        <div className="flex gap-2 mt-2 flex-wrap">
                          <span className="text-sm font-medium text-gray-900">
                            {formatCurrency(item.amount)}
                          </span>
                          <span className={`text-xs px-2 py-1 rounded-full ${
                            item.type === 'Payable' 
                              ? 'bg-red-100 text-red-800' 
                              : 'bg-green-100 text-green-800'
                          }`}>
                            {item.type === 'Payable' ? 'A Pagar' : 'A Receber'}
                          </span>
                          <span className={`text-xs px-2 py-1 rounded-full ${
                            item.isPaid 
                              ? 'bg-green-100 text-green-800' 
                              : 'bg-yellow-100 text-yellow-800'
                          }`}>
                            {item.isPaid ? 'Pago' : 'Pendente'}
                          </span>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </SwipeToDelete>
            );
          })
        )}
      </div>

      {/* Pagination and Results Info */}
      {!isLoading && items.length > 0 && (
        <div className="mt-6 pb-24 sm:pb-6">
          <div className="flex flex-col gap-4">
            {/* Results Count */}
            <div className="text-xs sm:text-sm text-gray-600 text-center sm:text-left">
              Exibindo {items.length} de {totalCount} conta(s)
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

                {/* Page Numbers */}
                <div className="flex items-center gap-1">
                  {Array.from({ length: totalPages }, (_, i) => i + 1)
                    .filter((page) => {
                      if (page === 1 || page === totalPages) return true;
                      if (Math.abs(page - currentPage) <= 1) return true;
                      return false;
                    })
                    .map((page, index, array) => {
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
        title="Excluir Conta"
        description={
          itemToDelete ? (
            <>
              <p className="text-base mb-2">
                Tem certeza que deseja excluir a conta{' '}
                <span className="font-semibold text-gray-900">{itemToDelete.description}</span>?
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
