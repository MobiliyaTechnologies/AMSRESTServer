BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Roles] 
					WHERE Name = 'SuperAdmin')
	BEGIN
		INSERT INTO [dbo].[Roles]  (Name)
		VALUES ('SuperAdmin')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Roles] 
					WHERE Name = 'Admin')
	BEGIN
		INSERT INTO [dbo].[Roles]  (Name)
		VALUES ('Admin')
	END
END