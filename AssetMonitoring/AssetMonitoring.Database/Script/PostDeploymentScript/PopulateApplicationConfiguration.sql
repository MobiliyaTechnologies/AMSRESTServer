BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] 
					WHERE ConfigurationType = 'PowerBI')
	BEGIN
		INSERT INTO [dbo].[ApplicationConfiguration]  (ConfigurationType)
		VALUES ('PowerBI')
	END
END
