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
import { Accounts } from '../pages/accounts/Accounts';
import { AccountForm } from '../pages/accounts/AccountForm';
import { AccountPayableReceivables } from '../pages/account-payable-receivable/AccountPayableReceivables';
import { CostCenters } from '../pages/cost-centers/CostCenters';
import { LoanAdvances } from '../pages/loan-advances/LoanAdvances';
import { Locations } from '../pages/locations/Locations';
import { PurchaseOrders } from '../pages/purchase-orders/PurchaseOrders';
import { SupplierCustomers } from '../pages/supplier-customers/SupplierCustomers';
import { Tasks } from '../pages/tasks/Tasks';
import AccessDenied from '../pages/AccessDenied';
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
