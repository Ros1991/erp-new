/*------------------------------------------------------------*/
/*   Script para corrigir campos de data que estavam como VARCHAR   */
/*   Converte os campos para DATETIME                          */
/*------------------------------------------------------------*/

-- tb_task_employee: converter campos de data
ALTER TABLE "erp"."tb_task_employee" 
    ALTER COLUMN "task_employee_start_date" TYPE timestamp USING 
        CASE 
            WHEN "task_employee_start_date" IS NULL THEN NULL
            WHEN "task_employee_start_date" ~ '^\d{4}-\d{2}-\d{2}' THEN "task_employee_start_date"::timestamp
            ELSE NULL
        END;

ALTER TABLE "erp"."tb_task_employee" 
    ALTER COLUMN "task_employee_end_date" TYPE timestamp USING 
        CASE 
            WHEN "task_employee_end_date" IS NULL THEN NULL
            WHEN "task_employee_end_date" ~ '^\d{4}-\d{2}-\d{2}' THEN "task_employee_end_date"::timestamp
            ELSE NULL
        END;

-- tb_time_entry: converter timestamp
ALTER TABLE "erp"."tb_time_entry" 
    ALTER COLUMN "time_entry_timestamp" TYPE timestamp USING 
        CASE 
            WHEN "time_entry_timestamp" ~ '^\d{4}-\d{2}-\d{2}' THEN "time_entry_timestamp"::timestamp
            ELSE CURRENT_TIMESTAMP
        END;

-- tb_user: converter campo de expiração do token
ALTER TABLE "erp"."tb_user" 
    ALTER COLUMN "user_reset_token_expires_at" TYPE timestamp USING 
        CASE 
            WHEN "user_reset_token_expires_at" IS NULL THEN NULL
            WHEN "user_reset_token_expires_at" ~ '^\d{4}-\d{2}-\d{2}' THEN "user_reset_token_expires_at"::timestamp
            ELSE NULL
        END;

-- tb_user_token: converter campos de expiração
ALTER TABLE "erp"."tb_user_token" 
    ALTER COLUMN "user_token_expires_at" TYPE timestamp USING 
        CASE 
            WHEN "user_token_expires_at" ~ '^\d{4}-\d{2}-\d{2}' THEN "user_token_expires_at"::timestamp
            ELSE CURRENT_TIMESTAMP
        END;

ALTER TABLE "erp"."tb_user_token" 
    ALTER COLUMN "user_token_refresh_expires_at" TYPE timestamp USING 
        CASE 
            WHEN "user_token_refresh_expires_at" IS NULL THEN NULL
            WHEN "user_token_refresh_expires_at" ~ '^\d{4}-\d{2}-\d{2}' THEN "user_token_refresh_expires_at"::timestamp
            ELSE NULL
        END;
