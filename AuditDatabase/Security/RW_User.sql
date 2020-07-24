CREATE USER [OpsAudit_RW_User]
    WITH PASSWORD = N'$(RWUserPassword)';
GO
GRANT CONNECT TO [OpsAudit_RW_User]
GO


