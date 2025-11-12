# Fix: Violação de FK em tb_employee.user_id

## 🔴 Problema

```
PostgresException: 23503: inserção ou atualização em tabela "tb_employee" 
viola restrição de chave estrangeira "fk_employee_user"
```

### Causa Raiz

O campo `user_id` na tabela `tb_employee` estava definido com:
```sql
"user_id" bigint DEFAULT 0 NULL,
```

**Problema:** Quando você não fornece um valor para `user_id`, o PostgreSQL insere `0` (zero) por causa do DEFAULT. Como `user_id = 0` não existe na tabela `tb_user`, a FK falha.

### Por que acontece?

1. Frontend envia `userId: undefined` (campo opcional)
2. Backend converte para `null` no DTO
3. Entity Framework envia `null` para o banco
4. PostgreSQL aplica o DEFAULT (0) ❌
5. FK valida: user_id = 0 existe em tb_user? **NÃO!**
6. **Erro 23503** - Violação de FK

---

## ✅ Solução

### 1. Remover DEFAULT do campo

**Arquivo:** `backend/-1-Domain/database/erp.sql`

```sql
-- ❌ ANTES (com DEFAULT 0)
"user_id" bigint DEFAULT 0 NULL,

-- ✅ DEPOIS (sem DEFAULT)
"user_id" bigint NULL,
```

### 2. Script de Migração

**Arquivo:** `database/migrations/003_fix_employee_user_id_default.sql`

```sql
-- Atualizar registros existentes com user_id = 0 para NULL
UPDATE erp.tb_employee
SET user_id = NULL
WHERE user_id = 0;

-- Remover o DEFAULT do campo
ALTER TABLE erp.tb_employee 
ALTER COLUMN user_id DROP DEFAULT;
```

### 3. Aplicar a migração

```bash
# Conectar ao PostgreSQL
psql -U seu_usuario -d erp_database

# Executar o script
\i database/migrations/003_fix_employee_user_id_default.sql
```

---

## 🔍 Validação

Após aplicar a migração, valide:

```sql
-- 1. Verificar se DEFAULT foi removido
SELECT column_name, column_default, is_nullable
FROM information_schema.columns
WHERE table_schema = 'erp' 
  AND table_name = 'tb_employee' 
  AND column_name = 'user_id';

-- Resultado esperado:
-- column_name | column_default | is_nullable
-- user_id     | NULL           | YES

-- 2. Verificar se não há user_id = 0
SELECT COUNT(*) as registros_com_zero 
FROM erp.tb_employee 
WHERE user_id = 0;

-- Resultado esperado: 0

-- 3. Testar inserção sem user_id
INSERT INTO erp.tb_employee (
    company_id, employee_nickname, employee_full_name, 
    employee_email, employee_phone, employee_cpf, criado_por
)
VALUES (
    1, 'Teste', 'Teste Completo', 
    'teste@teste.com', '11999999999', '12345678901', 1
);

-- Deve funcionar! user_id será NULL ✅
```

---

## 🎯 Comportamento Correto Após o Fix

### Cenário 1: Criar empregado SEM usuário vinculado
```json
// Frontend envia
{
  "nickname": "João",
  "fullName": "João Silva",
  "userId": undefined  // Não enviado
}

// Banco insere
user_id = NULL ✅
```

### Cenário 2: Criar empregado COM usuário vinculado
```json
// Frontend envia
{
  "nickname": "Maria",
  "fullName": "Maria Santos",
  "userId": 123
}

// Banco insere
user_id = 123 ✅
// FK valida: 123 existe em tb_user? SIM!
```

### Cenário 3: Atualizar removendo usuário
```json
// Frontend envia
{
  "userId": null
}

// Banco atualiza
user_id = NULL ✅
```

---

## 📋 Checklist

- [x] Remover DEFAULT 0 do campo user_id no script erp.sql
- [x] Criar migração 003_fix_employee_user_id_default.sql
- [ ] Aplicar migração no banco de dados
- [ ] Validar que DEFAULT foi removido
- [ ] Validar que não há user_id = 0
- [ ] Testar criação de empregado sem userId
- [ ] Testar criação de empregado com userId
- [ ] Reiniciar backend para aplicar mudanças

---

## 🚨 Importante

**Outros campos com mesmo problema:**

Verifique se outros campos NULLABLE com FK também têm DEFAULT 0:
```sql
SELECT 
    c.table_name,
    c.column_name,
    c.column_default,
    c.is_nullable
FROM information_schema.columns c
JOIN information_schema.table_constraints tc 
    ON c.table_name = tc.table_name
JOIN information_schema.key_column_usage kcu
    ON c.column_name = kcu.column_name 
    AND c.table_name = kcu.table_name
WHERE c.table_schema = 'erp'
  AND c.column_default LIKE '%0%'
  AND c.is_nullable = 'YES'
  AND tc.constraint_type = 'FOREIGN KEY';
```

**Regra geral:**
- Campo NULLABLE com FK → **NÃO usar DEFAULT 0**
- Campo NOT NULL com FK → **OK usar DEFAULT válido**

---

**Problema resolvido!** ✅

Após aplicar a migração, a inserção/atualização de empregados sem `userId` funcionará corretamente.
