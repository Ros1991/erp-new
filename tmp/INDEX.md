# 📚 Documentação Técnica - Índice

Esta pasta contém toda a documentação técnica gerada durante o desenvolvimento do projeto ERP.

---

## 📁 Estrutura

### **Backend**
- **[PADRONIZACAO_ERROS.md](PADRONIZACAO_ERROS.md)** - Padronização de tratamento de erros e exceções

### **Frontend - UI/UX**
- **[TOAST_SYSTEM.md](TOAST_SYSTEM.md)** - Sistema de notificações toast
- **[FORMATACAO_CAMPOS.md](FORMATACAO_CAMPOS.md)** - Formatação de campos (CPF, telefone, etc)
- **[INTEGRATION_NOTES.md](INTEGRATION_NOTES.md)** - Notas de integração frontend-backend

### **Features - Empresas**
- **[COMPANY_DIALOG.md](COMPANY_DIALOG.md)** - Dialog de criação de empresas
- **[CNPJ_OPCIONAL.md](CNPJ_OPCIONAL.md)** - CNPJ tornado opcional no sistema
- **[OWNER_ROLE_AUTO_CREATE.md](OWNER_ROLE_AUTO_CREATE.md)** - Criação automática de role "Dono"

### **Segurança & Autenticação**
- **[USERID_FROM_TOKEN.md](USERID_FROM_TOKEN.md)** - UserID extraído do token JWT
- **[MIDDLEWARE_ISSYSTEM_BYPASS.md](MIDDLEWARE_ISSYSTEM_BYPASS.md)** - Bypass de permissões para roles do sistema

### **Infraestrutura**
- **[LIMPAR_GIT_CACHE.md](LIMPAR_GIT_CACHE.md)** - Limpeza de cache do Git

---

## 📂 READMEs das Pastas do Projeto

### **Frontend**
- **[frontend/README.md](frontend/README.md)** - Documentação principal do frontend

### **Frontend - Páginas**
- **[frontend/src/pages/auth/README.md](frontend/src/pages/auth/README.md)** - Páginas de autenticação
- **[frontend/src/pages/companies/README.md](frontend/src/pages/companies/README.md)** - Páginas de empresas
- **[frontend/src/pages/dashboard/README.md](frontend/src/pages/dashboard/README.md)** - Dashboard
- **[frontend/src/pages/financeiro/README.md](frontend/src/pages/financeiro/README.md)** - Módulo financeiro
- **[frontend/src/pages/ponto/README.md](frontend/src/pages/ponto/README.md)** - Controle de ponto
- **[frontend/src/pages/tarefas/README.md](frontend/src/pages/tarefas/README.md)** - Gestão de tarefas

### **Frontend - Core**
- **[frontend/src/routes/README.md](frontend/src/routes/README.md)** - Rotas da aplicação
- **[frontend/src/services/README.md](frontend/src/services/README.md)** - Services e integrações

---

## 📖 Como Usar

Cada arquivo contém:
- ✅ Objetivo da implementação
- ✅ Código modificado (antes/depois)
- ✅ Exemplos de uso
- ✅ Testes sugeridos
- ✅ Arquivos afetados

---

## 🔍 Busca Rápida

### Por Tecnologia:
- **Backend (C#):** PADRONIZACAO_ERROS, OWNER_ROLE_AUTO_CREATE, USERID_FROM_TOKEN, MIDDLEWARE_ISSYSTEM_BYPASS
- **Frontend (React/TS):** TOAST_SYSTEM, FORMATACAO_CAMPOS, COMPANY_DIALOG
- **Banco de Dados:** CNPJ_OPCIONAL, OWNER_ROLE_AUTO_CREATE

### Por Tipo:
- **Features:** COMPANY_DIALOG, OWNER_ROLE_AUTO_CREATE
- **Segurança:** USERID_FROM_TOKEN, MIDDLEWARE_ISSYSTEM_BYPASS
- **Validações:** CNPJ_OPCIONAL, FORMATACAO_CAMPOS
- **UI/UX:** TOAST_SYSTEM, COMPANY_DIALOG

---

---

## 🗂️ Estrutura de Pastas

```
tmp/
├── INDEX.md                                          (este arquivo)
│
├── Documentação Técnica (raiz)
│   ├── CNPJ_OPCIONAL.md
│   ├── COMPANY_DIALOG.md
│   ├── FORMATACAO_CAMPOS.md
│   ├── INTEGRATION_NOTES.md
│   ├── LIMPAR_GIT_CACHE.md
│   ├── MIDDLEWARE_ISSYSTEM_BYPASS.md
│   ├── OWNER_ROLE_AUTO_CREATE.md
│   ├── PADRONIZACAO_ERROS.md
│   ├── TOAST_SYSTEM.md
│   └── USERID_FROM_TOKEN.md
│
└── frontend/
    ├── README.md                                     (frontend principal)
    └── src/
        ├── pages/
        │   ├── auth/README.md
        │   ├── companies/README.md
        │   ├── dashboard/README.md
        │   ├── financeiro/README.md
        │   ├── ponto/README.md
        │   └── tarefas/README.md
        ├── routes/README.md
        └── services/README.md
```

---

**Total de arquivos:** 20 documentos  
**Última atualização:** 11/11/2025
