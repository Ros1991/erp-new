# Dialog de Adicionar Empresa - Documentação

## ✅ Implementação Completa

Sistema de dialog modal para adicionar novas empresas com validação de CNPJ e integração completa com o backend.

---

## 📁 Arquivos Criados

### **1. Componente Dialog UI** 
**`src/components/ui/Dialog.tsx`**

Componente genérico de dialog modal reutilizável:
- ✅ `Dialog` - Container principal
- ✅ `DialogContent` - Conteúdo do dialog
- ✅ `DialogHeader` - Cabeçalho
- ✅ `DialogTitle` - Título
- ✅ `DialogDescription` - Descrição
- ✅ `DialogClose` - Botão fechar (X)

**Recursos:**
- Fecha com ESC
- Fecha clicando no backdrop
- Bloqueia scroll do body quando aberto
- Animações de entrada/saída

---

### **2. Company Service**
**`src/services/companyService.ts`**

Service completo para gerenciar empresas:

```typescript
// Métodos disponíveis:
- getMyCompanies()           // Lista empresas do usuário
- getCompanyById(id)         // Busca por ID
- createCompany(data)        // Cria nova empresa
- updateCompany(id, data)    // Atualiza empresa
- deleteCompany(id)          // Deleta empresa
- validateCNPJ(cnpj)         // Valida CNPJ (algoritmo completo)
- formatCNPJ(cnpj)          // Formata para exibição
```

**Interface CreateCompanyInput:**
```typescript
{
  name: string;        // Razão social (máx 255 caracteres)
  document: string;    // CNPJ sem formatação (14 dígitos)
  userId: number;      // ID do usuário (automático)
}
```

---

### **3. AddCompanyDialog Component**
**`src/components/companies/AddCompanyDialog.tsx`**

Dialog modal para adicionar empresas:

**Props:**
```typescript
{
  open: boolean;                    // Controla visibilidade
  onOpenChange: (open) => void;     // Callback de mudança
  onSuccess?: () => void;           // Callback após sucesso
}
```

**Funcionalidades:**
- ✅ Validação de CNPJ em tempo real
- ✅ Formatação automática (00.000.000/0000-00)
- ✅ Validação de campos obrigatórios
- ✅ Integração com toast para feedback
- ✅ Loading state durante criação
- ✅ Auto-focus no primeiro campo
- ✅ Limpa formulário após sucesso

**Validações:**
1. Nome obrigatório (máx 255 caracteres)
2. CNPJ obrigatório (exatamente 14 dígitos)
3. CNPJ válido (algoritmo de validação)
4. Usuário autenticado

---

### **4. CompanySelect Atualizado**
**`src/pages/companies/CompanySelect.tsx`**

**Mudanças:**
- ✅ Abre dialog ao invés de navegar para rota
- ✅ Recarrega lista após criar empresa
- ✅ Corrigido display do nome do usuário (email/phone/cpf)
- ✅ Removido imports não utilizados

---

## 🎨 UI/UX

### **Dialog Layout:**
```
┌─────────────────────────────────────┐
│  [Icon]  Nova Empresa           [X] │
│          Adicione uma nova...       │
├─────────────────────────────────────┤
│                                     │
│  Nome da Empresa *                  │
│  [_____________________________]    │
│  Razão social ou nome fantasia      │
│                                     │
│  CNPJ *                             │
│  [00.000.000/0000-00___________]    │
│  Cadastro Nacional... (14 dígitos)  │
│                                     │
│  [Cancelar]  [💼 Criar Empresa]    │
│                                     │
└─────────────────────────────────────┘
```

### **Estados:**

#### **1. Formulário Vazio**
- Campos em branco
- Botão "Criar Empresa" habilitado
- Auto-focus no campo Nome

#### **2. Durante Criação**
- Loading spinner
- Campos desabilitados
- Texto "Criando..."
- Não pode fechar

#### **3. Validação**
- Erros exibidos via Toast
- Campos mantêm valores
- Dialog permanece aberto

#### **4. Sucesso**
- Toast de sucesso
- Dialog fecha automaticamente
- Formulário limpo
- Lista de empresas atualizada

---

## 🔄 Fluxo de Uso

```
1. Usuário clica no card "Nova Empresa"
   ↓
2. Dialog abre com formulário vazio
   ↓
3. Usuário preenche Nome e CNPJ
   ↓
4. CNPJ é formatado automaticamente
   ↓
5. Usuário clica "Criar Empresa"
   ↓
6. Validações frontend
   ↓
7. POST para backend
   ↓
8. [SUCESSO] → Toast + Fecha Dialog + Recarrega lista
   [ERRO] → Toast com erro + Permanece aberto
```

---

## 🧪 Exemplos de Uso

### **1. Abrir Dialog**
```tsx
const [isOpen, setIsOpen] = useState(false);

<AddCompanyDialog
  open={isOpen}
  onOpenChange={setIsOpen}
  onSuccess={() => {
    console.log('Empresa criada!');
    loadCompanies();
  }}
/>
```

### **2. Validar CNPJ**
```typescript
import companyService from '@/services/companyService';

const cnpj = '11222333000181';
const isValid = companyService.validateCNPJ(cnpj);
// true ou false
```

### **3. Formatar CNPJ**
```typescript
const formatted = companyService.formatCNPJ('11222333000181');
// "11.222.333/0001-81"
```

---

## 🔐 Validação de CNPJ

Implementação completa do algoritmo de validação:

```typescript
validateCNPJ(cnpj: string): boolean {
  // 1. Remove formatação
  // 2. Verifica se tem 14 dígitos
  // 3. Verifica se não são todos iguais
  // 4. Valida primeiro dígito verificador
  // 5. Valida segundo dígito verificador
  // 6. Retorna true se válido
}
```

**CNPJs Válidos para Teste:**
- `11.222.333/0001-81`
- `00.000.000/0001-91`
- `11.444.777/0001-61`

**CNPJs Inválidos:**
- `11.111.111/1111-11` (todos iguais)
- `12.345.678/0001-00` (dígitos incorretos)
- `11.222.333/000` (incompleto)

---

## 📡 Endpoints do Backend

### **POST /companies**
```json
Request:
{
  "name": "Minha Empresa LTDA",
  "document": "11222333000181",
  "userId": 1
}

Response (201):
{
  "companyId": 10,
  "name": "Minha Empresa LTDA",
  "document": "11222333000181",
  "userId": 1,
  "criadoPor": 1,
  "atualizadoPor": null,
  "criadoEm": "2025-11-11T17:45:00Z",
  "atualizadoEm": null
}
```

### **GET /companies/my**
```json
Response (200):
[
  {
    "companyId": 1,
    "name": "Empresa 1",
    "document": "11222333000181",
    ...
  }
]
```

---

## ⚠️ Tratamento de Erros

### **Erros de Validação (Frontend)**
```
- Nome vazio
- Nome > 255 caracteres
- CNPJ vazio
- CNPJ != 14 dígitos
- CNPJ inválido
- Usuário não autenticado
```

### **Erros do Backend**
```
- CNPJ já cadastrado (409)
- Validação de campos (400)
- Não autorizado (401)
- Erro de servidor (500)
```

Todos exibidos via **Toast** com mensagens claras.

---

## 🎯 Checklist de Funcionalidades

- [x] Dialog UI genérico reutilizável
- [x] Service completo de empresas
- [x] Validação de CNPJ (algoritmo)
- [x] Formatação automática de CNPJ
- [x] Validações frontend
- [x] Integração com backend
- [x] Toast de feedback
- [x] Loading states
- [x] Recarregar lista após criação
- [x] Fechar com ESC
- [x] Fechar com backdrop
- [x] Auto-focus
- [x] Limpar formulário
- [x] Tratamento de erros
- [x] Responsivo

---

## 🚀 Próximos Passos

1. **Editar Empresa** - Dialog similar para edição
2. **Deletar Empresa** - Confirmação de exclusão
3. **Detalhes da Empresa** - Página de configurações
4. **Upload de Logo** - Adicionar imagem da empresa
5. **Validação de CNPJ duplicado** - Antes de enviar ao backend
6. **Histórico de empresas** - Empresas inativas/arquivadas

---

## 📚 Componentes Relacionados

- `Dialog.tsx` - Componente base
- `Button.tsx` - Botões
- `Input.tsx` - Inputs
- `Label.tsx` - Labels
- `Toast.tsx` - Notificações
- `Card.tsx` - Cards de empresa

**Tudo pronto e funcionando!** 🎉
