# CASCADE DELETE Configuration for Employee

## Overview
This migration adds CASCADE DELETE behavior to all foreign keys that reference `tb_employee`, allowing automatic deletion of related records when an employee is deleted.

## Files Modified

### 1. Migration File
**File:** `006_add_cascade_delete_employee.sql`
- Drops existing FK constraints
- Recreates with CASCADE DELETE or SET NULL

### 2. Main Schema
**File:** `erp.sql`
- Updated with CASCADE DELETE/SET NULL in all Employee FKs
- Added missing FK for `employee_id_general_manager`

## CASCADE DELETE Behavior

When an Employee is deleted, the following records are **automatically deleted**:

| Table | Column | Behavior |
|-------|--------|----------|
| `tb_contract` | `employee_id` | CASCADE DELETE |
| `tb_employee_allowed_location` | `employee_id` | CASCADE DELETE |
| `tb_task_employee` | `employee_id` | CASCADE DELETE |
| `tb_time_entry` | `employee_id` | CASCADE DELETE |
| `tb_loan_advance` | `employee_id` | CASCADE DELETE |
| `tb_payroll_employee` | `employee_id` | CASCADE DELETE |
| `tb_justification` | `employee_id` | CASCADE DELETE |

### Child Records (Cascaded from Parent)

When Contract is deleted → deletes:
- `tb_contract_benefit_discount` (CASCADE)
- `tb_contract_cost_center` (CASCADE)

When PayrollEmployee is deleted → deletes:
- `tb_payroll_item` (CASCADE)

When TaskEmployee is deleted → deletes:
- `tb_task_status_history` (CASCADE)

When LoanAdvance is deleted → deletes:
- `tb_financial_transaction` (CASCADE) *(already configured)*

## SET NULL Behavior

The following fields are **set to NULL** (not deleted):

| Table | Column | Behavior | Reason |
|-------|--------|----------|--------|
| `tb_company_setting` | `employee_id_general_manager` | SET NULL | Optional field |
| `tb_employee` | `employee_id_manager` | SET NULL | Self-reference, optional |

## Deletion Flow Example

```
DELETE Employee (ID=1)
    ├─ DELETE Contracts (employee_id=1)
    │   ├─ DELETE Contract Benefits/Discounts
    │   └─ DELETE Contract Cost Centers
    ├─ DELETE Employee Allowed Locations
    ├─ DELETE Task Employees
    │   └─ DELETE Task Status History
    ├─ DELETE Time Entries
    ├─ DELETE Loan Advances
    │   └─ DELETE Financial Transactions (already configured)
    ├─ DELETE Payroll Employees
    │   └─ DELETE Payroll Items
    ├─ DELETE Justifications
    └─ SET NULL: Company Setting (general_manager)
        SET NULL: Other Employees (manager reference)
```

## How to Apply

### Option 1: Fresh Database
Run the main schema:
```bash
psql -U postgres -d your_database -f erp.sql
```

### Option 2: Existing Database
Run the migration:
```bash
psql -U postgres -d your_database -f 006_add_cascade_delete_employee.sql
```

## Testing

To verify CASCADE DELETE is working:

```sql
-- 1. Create test employee
INSERT INTO erp.tb_employee (company_id, employee_nickname, criado_por, criado_em)
VALUES (1, 'Test Employee', 1, NOW())
RETURNING employee_id;

-- 2. Create related records (contract, time entry, etc.)
-- 3. Delete employee
DELETE FROM erp.tb_employee WHERE employee_id = [test_id];

-- 4. Verify related records were deleted
SELECT * FROM erp.tb_contract WHERE employee_id = [test_id]; -- Should be empty
SELECT * FROM erp.tb_time_entry WHERE employee_id = [test_id]; -- Should be empty
```

## Important Notes

⚠️ **WARNING**: CASCADE DELETE is irreversible!
- Always backup data before deleting employees in production
- Consider soft delete (deletado_em field) instead of hard delete
- Review all related records before deletion

✅ **Benefits**:
- Maintains referential integrity
- Prevents orphaned records
- Simplifies employee deletion logic
- No manual cleanup required

## Rollback

If needed to rollback (NOT RECOMMENDED):
```sql
-- Remove CASCADE/SET NULL from constraints
-- Recreate with default behavior (NO ACTION)
-- Use migration 006 as template but without ON DELETE clauses
```

---

**Created:** 2024-11-17  
**Author:** System  
**Impact:** High - Changes database referential integrity behavior
