# 🔍 DEBUG - Problema de Permissões

## 🎯 Passos para Debugar

### 1. **Abrir Console do Navegador**
- Pressione **F12**
- Vá na aba **Console**

### 2. **Fazer Logout**
- Clique em Sair/Logout

### 3. **Fazer Login Novamente**
- Use o usuário **DONO da empresa** (Owner)
- Faça login normalmente

### 4. **Selecionar Empresa**
- Selecione a empresa

### 5. **Verificar Logs no Console**

Procure pelos seguintes logs:

#### ✅ **Se tudo estiver OK, você verá:**
```
🔐 Permissões carregadas: {isAdmin: true, isSystemRole: false, modules: {...}}
✅ Acesso total (Admin/System) para: role.canView
```

#### ❌ **Se tiver problema, você verá:**
```
❌ Erro ao carregar permissões: [erro aqui]
🚫 Sem permissões carregadas para verificar: role.canView
```

OU

```
🔐 Permissões carregadas: {isAdmin: false, ...}
❌ Permissão role.canView: false
```

### 6. **Tirar Screenshot**
- Tire um print do console
- Me envie a mensagem completa

---

## 🔧 Verificações Rápidas

### **Verificar se o endpoint existe:**
1. Abra a aba **Network** (Rede) no F12
2. Selecione a empresa
3. Procure por uma chamada: **GET /api/auth/permissions**
4. Clique nela e veja:
   - **Status:** deve ser **200 OK**
   - **Response (Resposta):** deve ter algo como:
     ```json
     {
       "data": {
         "isAdmin": true,
         "isSystemRole": false,
         "modules": {
           "role": {
             "canView": true,
             "canCreate": true,
             "canEdit": true,
             "canDelete": true
           }
         }
       }
     }
     ```

### **Se o endpoint retornar 404:**
- O backend não tem o endpoint `/auth/permissions`
- Vamos precisar adicionar

### **Se o endpoint retornar 401/403:**
- Token JWT não está sendo enviado
- Problema de autenticação

### **Se o endpoint retornar 500:**
- Erro no backend
- Veja os logs do backend

---

## 🚨 Problemas Comuns

### **Problema 1: Permissões não carregam**
**Sintoma:** Vê o log `🚫 Sem permissões carregadas`
**Causa:** `loadPermissions()` não foi chamado
**Solução:** Verificar se `CompanySelect.tsx` está chamando `await loadPermissions()` ao selecionar empresa

### **Problema 2: isAdmin = false**
**Sintoma:** Vê `isAdmin: false` mas é owner
**Causa:** Cargo de Owner não tem `IsAdmin = true` no banco
**Solução:** Verificar o cargo no banco de dados

### **Problema 3: Módulos vazios**
**Sintoma:** `modules: {}`
**Causa:** Role sem permissões no campo `permissions`
**Solução:** Recriar o cargo de Owner

---

## 📝 SQL para Verificar no Banco

```sql
-- Verificar o cargo do usuário na empresa
SELECT 
    u.user_id,
    u.email,
    cu.role_id,
    r.name as role_name,
    r.is_system,
    r.permissions
FROM users u
JOIN company_users cu ON u.user_id = cu.user_id
JOIN roles r ON cu.role_id = r.role_id
WHERE u.user_id = [SEU_USER_ID]
  AND cu.company_id = [SUA_COMPANY_ID];

-- O campo 'permissions' deve conter um JSON parecido com:
-- {"isAdmin":true,"allowedEndpoints":["*"],"modules":{"role":{"canView":true,...}}}
```

---

## ✅ **ME ENVIE:**

1. **Screenshot do Console** com os logs
2. **Screenshot da aba Network** mostrando a chamada `/api/auth/permissions`
3. **Response do endpoint** (o JSON retornado)
4. **Resultado da query SQL** (se souber fazer)

Com essas informações eu consigo identificar exatamente onde está o problema! 🎯
