import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

// Auth pages
import { Landing } from '../pages/auth/Landing';
import { Login } from '../pages/auth/Login';
import { Register } from '../pages/auth/Register';
import { ForgotPassword } from '../pages/auth/ForgotPassword';
import { ResetPassword } from '../pages/auth/ResetPassword';

// Company pages
import { CompanySelect } from '../pages/companies/CompanySelect';
import { CompanySettings } from '../pages/companies/CompanySettings';
import { CompanySettings as CompanySettingsPage } from '../pages/company-settings/CompanySettings';

// Dashboard pages
import { Dashboard } from '../pages/dashboard/Dashboard';

// Internal pages
import { Roles } from '../pages/roles/Roles';
import { RoleForm } from '../pages/roles/RoleForm';
import { Users } from '../pages/users/Users';
import { AddUser } from '../pages/users/AddUser';
import { EditUser } from '../pages/users/EditUser';
import { Employees } from '../pages/employees/Employees';
import { EmployeeForm } from '../pages/employees/EmployeeForm';
import { EmployeeContracts } from '../pages/contracts/EmployeeContracts';
import { ContractForm } from '../pages/contracts/ContractForm';
import { Accounts } from '../pages/accounts/Accounts';
import { AccountForm } from '../pages/accounts/AccountForm';
import { AccountPayableReceivables } from '../pages/account-payable-receivable/AccountPayableReceivables';
import { AccountPayableReceivableForm } from '../pages/account-payable-receivable/AccountPayableReceivableForm';
import { CostCenters } from '../pages/cost-centers/CostCenters';
import { CostCenterForm } from '../pages/cost-centers/CostCenterForm';
import { LoanAdvances } from '../pages/loan-advances/LoanAdvances';
import { LoanAdvanceForm } from '../pages/loan-advances/LoanAdvanceForm';
import { FinancialTransactions, FinancialTransactionDetails } from '../pages/financial-transactions';
import { Locations } from '../pages/locations/Locations';
import { LocationForm } from '../pages/locations/LocationForm';
import { PurchaseOrders } from '../pages/purchase-orders/PurchaseOrders';
import { PurchaseOrderForm } from '../pages/purchase-orders/PurchaseOrderForm';
import { SupplierCustomers } from '../pages/supplier-customers/SupplierCustomers';
import { SupplierCustomerForm } from '../pages/supplier-customers/SupplierCustomerForm';
import { Tasks } from '../pages/tasks/Tasks';
import { TaskForm } from '../pages/tasks/TaskForm';
import { Payrolls } from '../pages/payroll/Payrolls';
import { PayrollDetails } from '../pages/payroll/PayrollDetails';
import AccessDenied from '../pages/AccessDenied';

// Reports
import { 
  FinancialDashboard,
  CostCenterReport,
  AccountReport,
  SupplierCustomerReport,
  CashFlowReport,
  AccountsPayableReceivableReport,
  FinancialForecastReport,
  EmployeeAccountReport
} from '../pages/reports';
import { PermissionProtectedRoute } from '../components/permissions/PermissionProtectedRoute';

// Protected Route Component
function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();
  
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  return <>{children}</>;
}

// Company Protected Route
function CompanyProtectedRoute({ children }: { children: React.ReactNode }) {
  const { selectedCompany } = useAuth();
  
  if (!selectedCompany) {
    return <Navigate to="/companies" replace />;
  }
  
  return <>{children}</>;
}

export function AppRoutes() {
  return (
    <Routes>
      {/* Public Routes - Auth */}
      <Route path="/" element={<Landing />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />
      <Route path="/reset-password" element={<ResetPassword />} />
      
      {/* Protected Routes - Company Selection */}
      <Route
        path="/companies"
        element={
          <ProtectedRoute>
            <CompanySelect />
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Company Settings */}
      <Route
        path="/company/:id/settings"
        element={
          <ProtectedRoute>
            <CompanySettings />
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Dashboard */}
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <Dashboard />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Company Settings (new) */}
      <Route
        path="/company-settings"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="companySettings.canView">
                <CompanySettingsPage />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Roles */}
      <Route
        path="/roles"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="role.canView">
                <Roles />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/roles/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="role.canCreate">
                <RoleForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/roles/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="role.canEdit">
                <RoleForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Users */}
      <Route
        path="/users"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="user.canView">
                <Users />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/users/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="user.canCreate">
                <AddUser />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/users/:companyUserId/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="user.canEdit">
                <EditUser />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Employees */}
      <Route
        path="/employees"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="employee.canView">
                <Employees />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/employees/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="employee.canCreate">
                <EmployeeForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/employees/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="employee.canEdit">
                <EmployeeForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/employees/:employeeId/contracts"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="employee.canView">
                <EmployeeContracts />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/employees/:employeeId/contracts/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="employee.canEdit">
                <ContractForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/employees/:employeeId/contracts/:contractId"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="employee.canEdit">
                <ContractForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Accounts */}
      <Route
        path="/accounts"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="account.canView">
                <Accounts />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/accounts/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="account.canCreate">
                <AccountForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/accounts/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="account.canEdit">
                <AccountForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Account Payable Receivable */}
      <Route
        path="/account-payable-receivable"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="accountPayableReceivable.canView">
                <AccountPayableReceivables />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/account-payable-receivable/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="accountPayableReceivable.canCreate">
                <AccountPayableReceivableForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/account-payable-receivable/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="accountPayableReceivable.canEdit">
                <AccountPayableReceivableForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Cost Centers */}
      <Route
        path="/cost-centers"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="costCenter.canView">
                <CostCenters />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/cost-centers/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="costCenter.canCreate">
                <CostCenterForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/cost-centers/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="costCenter.canEdit">
                <CostCenterForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Loan Advances */}
      <Route
        path="/loan-advances"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="loanAdvance.canView">
                <LoanAdvances />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/loan-advances/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="loanAdvance.canCreate">
                <LoanAdvanceForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/loan-advances/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="loanAdvance.canEdit">
                <LoanAdvanceForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Financial Transactions */}
      <Route
        path="/financial-transactions"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="financialTransaction.canView">
                <FinancialTransactions />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/financial-transactions/:id"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="financialTransaction.canView">
                <FinancialTransactionDetails />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Locations */}
      <Route
        path="/locations"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="location.canView">
                <Locations />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/locations/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="location.canCreate">
                <LocationForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/locations/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="location.canEdit">
                <LocationForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Purchase Orders */}
      <Route
        path="/purchase-orders"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="purchaseOrder.canView">
                <PurchaseOrders />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/purchase-orders/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="purchaseOrder.canCreate">
                <PurchaseOrderForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/purchase-orders/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="purchaseOrder.canEdit">
                <PurchaseOrderForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Supplier Customers */}
      <Route
        path="/supplier-customers"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="supplierCustomer.canView">
                <SupplierCustomers />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/supplier-customers/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="supplierCustomer.canCreate">
                <SupplierCustomerForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/supplier-customers/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="supplierCustomer.canEdit">
                <SupplierCustomerForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Tasks */}
      <Route
        path="/tasks"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="task.canView">
                <Tasks />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/tasks/new"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="task.canCreate">
                <TaskForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/tasks/:id/edit"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="task.canEdit">
                <TaskForm />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Payroll */}
      <Route
        path="/payroll/:id"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="payroll.canView">
                <PayrollDetails />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/payroll"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <PermissionProtectedRoute requires="payroll.canView">
                <Payrolls />
              </PermissionProtectedRoute>
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Protected Routes - Reports */}
      <Route
        path="/reports/financial-dashboard"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <FinancialDashboard />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/cost-center"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <CostCenterReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/account"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <AccountReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/supplier-customer"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <SupplierCustomerReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/cash-flow"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <CashFlowReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/accounts-payable-receivable"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <AccountsPayableReceivableReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/financial-forecast"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <FinancialForecastReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports/employee-account"
        element={
          <ProtectedRoute>
            <CompanyProtectedRoute>
              <EmployeeAccountReport />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Access Denied Page */}
      <Route
        path="/access-denied"
        element={
          <ProtectedRoute>
            <AccessDenied />
          </ProtectedRoute>
        }
      />
      
      {/* Catch all - redirect to home */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
