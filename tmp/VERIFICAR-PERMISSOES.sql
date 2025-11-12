-- VERIFICAR PERMISSÕES DO USUÁRIO

-- 1. Ver dados do usuário atual
SELECT 
    u.user_id,
    u.email,
    u.phone,
    u.cpf,
    u.is_active
FROM users u
ORDER BY u.created_at DESC
LIMIT 5;

-- 2. Ver vínculo do usuário com a empresa
SELECT 
    cu.company_user_id,
    cu.user_id,
    cu.company_id,
    cu.role_id,
    c.name as company_name
FROM company_users cu
JOIN companies c ON c.company_id = cu.company_id
ORDER BY cu.created_at DESC
LIMIT 5;

-- 3. Ver o cargo (role) vinculado
SELECT 
    r.role_id,
    r.name as role_name,
    r.is_system,
    r.company_id,
    r.permissions
FROM roles r
ORDER BY r.created_at DESC
LIMIT 5;

-- 4. QUERY COMPLETA - Verificar permissões do seu usuário
-- SUBSTITUA os valores <SEU_USER_ID> e <SUA_COMPANY_ID>
SELECT 
    u.user_id,
    u.email,
    c.company_id,
    c.name as company_name,
    r.role_id,
    r.name as role_name,
    r.is_system,
    r.permissions
FROM users u
JOIN company_users cu ON u.user_id = cu.user_id
JOIN companies c ON c.company_id = cu.company_id
LEFT JOIN roles r ON r.role_id = cu.role_id
WHERE u.user_id = <SEU_USER_ID>
  AND c.company_id = <SUA_COMPANY_ID>;

-- Se o campo 'permissions' estiver NULL ou vazio, esse é o problema!
-- O cargo não tem permissões configuradas.

-- 5. SOLUÇÃO: Atualizar permissões do cargo Owner
-- Primeiro, identifique o role_id do Owner na query acima
-- Depois execute (SUBSTITUA <ROLE_ID_DO_OWNER>):

UPDATE roles
SET permissions = '{
  "isAdmin": true,
  "allowedEndpoints": ["*"],
  "modules": {
    "role": {
      "canView": true,
      "canCreate": true,
      "canEdit": true,
      "canDelete": true
    },
    "user": {
      "canView": true,
      "canCreate": true,
      "canEdit": true,
      "canDelete": true
    },
    "account": {
      "canView": true,
      "canCreate": true,
      "canEdit": true,
      "canDelete": true
    }
  }
}'
WHERE role_id = <ROLE_ID_DO_OWNER>;

-- ATENÇÃO: Use isso apenas para o cargo de OWNER (dono da empresa)
-- Outros cargos devem ter permissões configuradas via interface
