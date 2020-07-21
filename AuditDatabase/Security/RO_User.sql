CREATE USER [OpsAudit_RO_User]
    WITH PASSWORD = N'$(ROUserPassword)';
GO
GRANT CONNECT TO [OpsAudit_RO_User]
GO


