import { BrowserRouter as Router } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ToastProvider } from './contexts/ToastContext';
import { PermissionProvider } from './contexts/PermissionContext';
import { AppRoutes } from './routes';

function App() {
  return (
    <Router>
      <ToastProvider>
        <PermissionProvider>
          <AuthProvider>
            <AppRoutes />
          </AuthProvider>
        </PermissionProvider>
      </ToastProvider>
    </Router>
  );
}

export default App;
