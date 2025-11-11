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
import { Users } from '../pages/users/Users';
import { Accounts } from '../pages/accounts/Accounts';

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
              <Roles />
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
              <Users />
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
              <Accounts />
            </CompanyProtectedRoute>
          </ProtectedRoute>
        }
      />
      
      {/* Catch all - redirect to home */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
