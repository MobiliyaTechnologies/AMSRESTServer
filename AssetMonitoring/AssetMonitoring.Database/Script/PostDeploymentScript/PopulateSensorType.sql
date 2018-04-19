BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorTypes] 
					WHERE [Name] = 'SensorTag1350')
	BEGIN
		INSERT INTO [dbo].[SensorTypes]  ([Name])
		VALUES ('SensorTag1350')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorTypes] 
					WHERE [Name] = 'SensorTag2650')
	BEGIN
		INSERT INTO [dbo].[SensorTypes]  ([Name])
		VALUES ('SensorTag2650')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorTypes] 
					WHERE [Name] = 'ThunderBoard-Sense')
	BEGIN
		INSERT INTO [dbo].[SensorTypes]  ([Name])
		VALUES ('ThunderBoard-Sense')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorTypes] 
					WHERE [Name] = 'ThunderBoard-React')
	BEGIN
		INSERT INTO [dbo].[SensorTypes]  ([Name])
		VALUES ('ThunderBoard-React')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorTypes] 
					WHERE [Name] = 'Bosch-XDK')
	BEGIN
		INSERT INTO [dbo].[SensorTypes]  ([Name])
		VALUES ('Bosch-XDK')
	END
END