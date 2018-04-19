BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'AmbientTemperature')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('AmbientTemperature', 'Celsius' , 'Celsius,Fahrenheit,Kelvin')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'ObjectTemperature')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('ObjectTemperature', 'Celsius' , 'Celsius,Fahrenheit,Kelvin')
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'Humidity')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('Humidity', '%' , '%')
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'BarometricPressure')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('BarometricPressure', 'mBar' , 'mBar')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'Accelerometer')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('Accelerometer', 'g' , 'g')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'Magnetometer')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('Magnetometer', 'mT' , 'mT')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'Gyroscope')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('Gyroscope', 'degree/s' , 'degree/s')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'Luxometer')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('Luxometer', 'lux' , 'lux')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'UVIndex')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('UVIndex', 'number' , 'number')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'NoiseLevel')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('NoiseLevel', 'dB' , 'dB')
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[Capabilities] 
					WHERE Name = 'Gateway')
	BEGIN
		INSERT INTO [dbo].[Capabilities]  (Name, Unit, SupportedUnits)
		VALUES ('Gateway', 'key' , 'key')
	END
END
