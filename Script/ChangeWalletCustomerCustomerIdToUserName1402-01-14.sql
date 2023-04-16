--IF NOT EXISTS (
--  SELECT
--    *
--  FROM
--    INFORMATION_SCHEMA.COLUMNS
--  WHERE
--    TABLE_NAME = 'WalletCustomer' AND COLUMN_NAME = 'Username')
--BEGIN
--  ALTER TABLE WalletCustomer
--    ADD Username nvarchar(1000) 
--END;
--Update WalletCustomer
--Set Username = b.Username

--From Customer as b Inner Join WalletCustomer as a

--    On a.CustomerId = b.Id

--EXEC sp_rename 'WalletCustomer.CustomerId', 'Username', 'COLUMN';
--ALTER TABLE WalletCustomer DROP COLUMN IsChange;


--ALTER TABLE WalletCustomerAmount 
--ALTER COLUMN Amount decimal(18,4);