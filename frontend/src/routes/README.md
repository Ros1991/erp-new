# Sistema de Rotas

Este arquivo centraliza todas as rotas do sistema para facilitar manutenção e organização.

## Estrutura

### Rotas Públicas (Auth)
- `/` - Landing page institucional
- `/login` - Login com email, telefone ou CPF
- `/register` - Cadastro de novos usuários
- `/forgot-password` - Recuperação de senha

### Rotas Protegidas (Requer Autenticação)
- `/companies` - Seleção de empresa (multi-tenant)

### Rotas Protegidas (Requer Autenticação + Empresa Selecionada)
- `/dashboard` - Dashboard principal

## Componentes de Proteção

### ProtectedRoute
Verifica se o usuário está autenticado. Se não, redireciona para `/login`.

### CompanyProtectedRoute
Verifica se uma empresa foi selecionada. Se não, redireciona para `/companies`.

## Como Adicionar Novas Rotas

1. Crie a página na pasta apropriada dentro de `src/pages/`
2. Importe a página em `src/routes/index.tsx`
3. Adicione a rota no componente `<Routes>` com a proteção adequada

### Exemplo de Rota Pública
```tsx
<Route path="/sua-rota" element={<SuaPagina />} />
```

### Exemplo de Rota Protegida
```tsx
<Route
  path="/sua-rota"
  element={
    <ProtectedRoute>
      <CompanyProtectedRoute>
        <SuaPagina />
      </CompanyProtectedRoute>
    </ProtectedRoute>
  }
/>
```
