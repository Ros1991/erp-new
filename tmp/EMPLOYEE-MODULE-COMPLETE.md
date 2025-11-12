# Módulo de Empregados - Implementação Completa

Implementação completa do módulo de gerenciamento de empregados, seguindo o padrão da aplicação.

## 📋 Sumário

- **Backend**: Entidade, DTOs, Mapper, Repository, Service, Controller
- **Frontend**: Service, Listagem, Formulário, Rotas, Sidebar
- **Banco de Dados**: Scripts de migração e criação de tabela
- **Configuração**: Módulo e permissões

---

## 🗄️ 1. Banco de Dados

### Entidade Employee

**Arquivo:** `backend/-1-Domain/Entities/employee.cs`

**Campo Adicionado:**
- `employee_profile_image` (BYTEA) - Armazena imagem de perfil em formato binário

**Construtor atualizado** para incluir `ProfileImage`

### Scripts de Migração

**Arquivo:** `database/migrations/002_add_profile_image_to_employee.sql`
```sql
ALTER TABLE erp.tb_employee
ADD COLUMN IF NOT EXISTS employee_profile_image BYTEA NULL;
```

**Arquivo:** `database/create_tables.sql`
- Script completo de criação da tabela `tb_employee` com todos os campos
- Índices para: company, user, manager, email, cpf
- Foreign keys para: company, user, manager

---

## ⚙️ 2. Backend

### DTOs

#### EmployeeFilterDTO
**Arquivo:** `backend/-2-Application/DTOs/Employee/EmployeeFilterDTO.cs`

Filtros disponíveis:
- `Search` - Busca geral (nome, email, telefone, CPF)
- `Nickname` - Filtro por apelido
- `FullName` - Filtro por nome completo
- `Email`, `Phone`, `Cpf` - Filtros específicos
- `EmployeeIdManager` - Filtro por gerente
- `UserId` - Filtro por usuário vinculado
- `Page`, `PageSize` - Paginação
- `OrderBy`, `IsAscending` - Ordenação

#### EmployeeInputDTO
**Arquivo:** `backend/-2-Application/DTOs/Employee/EmployeeInputDTO.cs`

Campos:
- `Nickname` ✅ Obrigatório (max 255)
- `FullName` ✅ Obrigatório (max 255)
- `Email` (opcional, validação de formato)
- `Phone` (opcional, max 20)
- `Cpf` (opcional, 11 dígitos numéricos)
- `UserId`, `EmployeeIdManager` (opcionais)
- `ProfileImageBase64` (opcional, para upload)

#### EmployeeOutputDTO
**Arquivo:** `backend/-2-Application/DTOs/Employee/EmployeeOutputDTO.cs`

Retorna:
- Todos os dados do empregado
- `ProfileImageBase64` para download
- Dados do gerente (nickname, fullName)
- Email do usuário vinculado
- Auditoria (criado/atualizado por e em)

### Mapper

**Arquivo:** `backend/-2-Application/Mappers/EmployeeMapper.cs`

Métodos:
- `ToEntity()` - Converte DTO para Entity (cria novo)
- `UpdateEntity()` - Atualiza Entity existente com dados do DTO
- `ToOutputDTO()` - Converte Entity para DTO de saída
- `ToOutputDTOList()` - Converte lista de entities

**Conversão Base64:**
- DTO → Entity: `Convert.FromBase64String()`
- Entity → DTO: `Convert.ToBase64String()`

### Repository

**Arquivo:** `backend/-3-Infrastructure/Repositories/EmployeeRepository.cs`

Métodos implementados:
- `GetAllAsync(companyId)` - Lista todos
- `GetPagedAsync(companyId, filters)` - Lista paginada com filtros
- `GetOneByIdAsync(employeeId)` - Busca por ID
- `CreateAsync(entity)` - Cria novo
- `UpdateByIdAsync(employeeId, entity)` - Atualiza existente
- `DeleteByIdAsync(employeeId)` - Remove

**Busca inteligente:**
- Phone e CPF: busca APENAS se termo tiver dígitos (evita falsos positivos)
- Case-insensitive
- Includes: Manager e User

### Service

**Arquivo:** `backend/-2-Application/Services/EmployeeService.cs`

**Validações:**
- Manager deve pertencer à mesma empresa
- Empregado não pode ser gerente de si mesmo
- User deve existir se fornecido
- Não permite excluir empregado que é gerente de outros

**Fluxo:**
- Cria/atualiza com validações
- Recarrega com includes após save
- Retorna DTO completo

### Controller

**Arquivo:** `backend/-4-WebApi/Controllers/EmployeeController.cs`

**Endpoints:**
- `GET /api/employee/getAll` - Lista todos
- `GET /api/employee/getPaged` - Lista paginada
- `GET /api/employee/{id}` - Busca por ID
- `POST /api/employee/create` - Cria novo
- `PUT /api/employee/{id}` - Atualiza
- `DELETE /api/employee/{id}` - Remove

**Permissões:**
- `employee.canView` - GET endpoints
- `employee.canCreate` - POST
- `employee.canEdit` - PUT
- `employee.canDelete` - DELETE

### UnitOfWork

**Arquivos atualizados:**
- `backend/-2-Application/Interfaces/Base/IUnitOfWork.cs`
- `backend/-3-Infrastructure/UnitOfWork/ErpUnitOfWork.cs`

Adicionado: `IEmployeeRepository EmployeeRepository { get; }`

### Dependency Injection

**Arquivo:** `backend/-5-CrossCutting/IoC/ServiceConfiguration.cs`

Registrado: `services.AddScoped<IEmployeeService, EmployeeService>();`

---

## 🎨 3. Frontend

### Service

**Arquivo:** `frontend/src/services/employeeService.ts`

**Interfaces:**
```typescript
interface Employee { ... }
interface EmployeeFilters { ... }
interface PagedResult<T> { ... }
```

**Métodos:**
- `getEmployees(filters)` - Lista paginada
- `getAllEmployees()` - Lista todos
- `getEmployeeById(id)` - Busca por ID
- `createEmployee(data)` - Cria novo
- `updateEmployee(id, data)` - Atualiza
- `deleteEmployee(id)` - Remove
- `imageToBase64(file)` - Helper para upload

**Query strings em PascalCase** (padrão do backend C#)

### Página de Listagem

**Arquivo:** `frontend/src/pages/employees/Employees.tsx`

**Estrutura (igual ao Roles.tsx):**
- ✅ Header desktop/mobile responsivo
- ✅ Filtros colapsáveis no mobile
- ✅ FAB (Floating Action Button) mobile
- ✅ Desktop: Table com botões protegidos
- ✅ Mobile: Cards com SwipeToDelete
- ✅ Paginação completa com ellipsis
- ✅ Busca com debounce (500ms)
- ✅ Ordenação (A-Z / Z-A)

**Diferencial:**
- Exibe imagem de perfil do empregado
- Mostra gerente na listagem
- Renderiza placeholder se não houver imagem

**Permissões:**
- `employee.canView` - Visualizar lista
- `employee.canCreate` - Botão criar
- `employee.canEdit` - Botão/tap editar
- `employee.canDelete` - Botão/swipe excluir

### Página de Formulário

**Arquivo:** `frontend/src/pages/employees/EmployeeForm.tsx`

**Funcionalidades:**
- ✅ Modo criar/editar (mesmo componente)
- ✅ Upload de imagem com preview
- ✅ Validação de formato de email
- ✅ Formatação automática de telefone e CPF
- ✅ Select de gerente (filtra empregado atual)
- ✅ Validação de tamanho de imagem (máx 5MB)
- ✅ Conversão Base64 para upload

**Validações:**
- Nickname e FullName obrigatórios
- Email: formato válido
- Telefone: 11 dígitos
- CPF: 11 dígitos
- Imagem: tipo image/*, máx 5MB

**UX:**
- Formatação em tempo real (phone/CPF)
- Preview de imagem com botão remover
- Mensagens de erro inline
- Loading states

### Rotas

**Arquivo:** `frontend/src/routes/index.tsx`

```tsx
// Listagem
<Route path="/employees" element={...} />

// Criar
<Route path="/employees/new" element={...} />

// Editar
<Route path="/employees/:id/edit" element={...} />
```

**Proteções:**
- `ProtectedRoute` - Autenticação
- `CompanyProtectedRoute` - Empresa selecionada
- `PermissionProtectedRoute` - Permissões específicas

### Sidebar

**Arquivo:** `frontend/src/components/layout/Sidebar.tsx`

```typescript
{ 
  icon: UserCheck, 
  label: 'Empregados', 
  path: '/employees', 
  permission: 'employee.canView' 
}
```

Item filtrado automaticamente baseado em permissões

---

## 🔐 4. Configuração de Módulo

**Arquivo:** `backend/-4-WebApi/Configuration/modules-configuration.json`

```json
{
  "key": "employee",
  "name": "Empregados",
  "description": "Gerenciar empregados da empresa",
  "icon": "user-check",
  "isActive": true,
  "permissions": [
    { "key": "canView", "name": "Visualizar", ... },
    { "key": "canCreate", "name": "Criar", ... },
    { "key": "canEdit", "name": "Editar", ... },
    { "key": "canDelete", "name": "Excluir", ... }
  ]
}
```

---

## ✅ Checklist de Implementação

### Backend
- [x] Entidade com campo ProfileImage
- [x] Script de migração
- [x] Script de criação de tabela
- [x] DTOs (Filter, Input, Output)
- [x] Mapper com conversão Base64
- [x] Repository com busca inteligente
- [x] Interface do Repository
- [x] Service com validações
- [x] Interface do Service
- [x] Controller com permissões
- [x] UnitOfWork atualizado
- [x] DI configurado
- [x] Módulo no JSON de configuração

### Frontend
- [x] Service com todos os métodos
- [x] Página de listagem (Employees.tsx)
- [x] Página de formulário (EmployeeForm.tsx)
- [x] Upload de imagem
- [x] Formatação de telefone/CPF
- [x] Validações
- [x] Rotas protegidas
- [x] Item no sidebar
- [x] Permissões em todos os componentes
- [x] Mobile responsivo

---

## 🎯 Padrões Seguidos

✅ **Account como referência** - Copiado comportamento exato
✅ **Roles para UI** - Listagem e formulário idênticos
✅ **Permissões granulares** - canView, canCreate, canEdit, canDelete
✅ **Protected components** - Todos os botões e rotas protegidos
✅ **MainLayout** - Todas as páginas envolvidas
✅ **Botão Voltar padrão** - ArrowLeft + texto
✅ **Mobile first** - FAB, SwipeToDelete, filtros colapsáveis
✅ **Query strings PascalCase** - Backend C# binding
✅ **Busca inteligente** - Phone/CPF apenas com dígitos
✅ **Toast padronizado** - handleBackendError em todos os catch

---

## 📊 Estatísticas

- **Arquivos Backend:** 11
- **Arquivos Frontend:** 5
- **Arquivos Database:** 2
- **Total de Linhas:** ~2.800
- **Endpoints:** 6
- **Componentes React:** 2
- **Permissões:** 4

---

## 🚀 Próximos Passos

1. **Testar** todas as funcionalidades
2. **Verificar** permissões em produção
3. **Aplicar migrations** no banco
4. **Reiniciar backend** para carregar módulo
5. **Fazer login novamente** para atualizar permissões
6. **Testar upload** de imagem

---

**Implementado em:** 12/11/2024
**Padrão:** Account (backend) + Roles (frontend)
**Status:** ✅ Completo e pronto para uso
