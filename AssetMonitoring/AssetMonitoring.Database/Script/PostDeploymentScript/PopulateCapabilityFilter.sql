
BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Vibration' and CapabilityId = 
                    (SELECT   Id FROM [dbo].[Capabilities]
		            WHERE Name = 'Accelerometer' ) )
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId)
		SELECT 'Vibration',  Id FROM [dbo].[Capabilities]
		WHERE Name = 'Accelerometer'  
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Invert' and CapabilityId = 
                    (SELECT   Id FROM [dbo].[Capabilities]
		            WHERE Name = 'Accelerometer' ) )
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId)
		SELECT 'Invert',  Id FROM [dbo].[Capabilities]
		WHERE Name = 'Accelerometer'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Vibration' and CapabilityId = 
                    (SELECT   Id FROM [dbo].[Capabilities]
		            WHERE Name = 'Magnetometer' ) )
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId)
		SELECT 'Vibration',  Id FROM [dbo].[Capabilities]
		WHERE Name = 'Magnetometer'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Vibration' and CapabilityId = 
                    (SELECT   Id FROM [dbo].[Capabilities]
		            WHERE Name = 'Gyroscope' ) )
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId)
		SELECT 'Vibration',  Id FROM [dbo].[Capabilities]
		WHERE Name = 'Gyroscope'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'AmbientTemperature')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'AmbientTemperature',  Id , 'range', '-10', '400' FROM [dbo].[Capabilities]
		WHERE Name = 'AmbientTemperature'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'ObjectTemperature')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'ObjectTemperature',  Id, 'range', '-10', '400' FROM [dbo].[Capabilities]
		WHERE Name = 'ObjectTemperature'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Humidity')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'Humidity',  Id, 'range', '0', '50' FROM [dbo].[Capabilities]
		WHERE Name = 'Humidity'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'BarometricPressure')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'BarometricPressure',  Id, 'range', '800', '1300' FROM [dbo].[Capabilities]
		WHERE Name = 'BarometricPressure'  
	END
END

BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Luxometer')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'Luxometer',  Id, 'range', '0', '25000' FROM [dbo].[Capabilities]
		WHERE Name = 'Luxometer'  
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'UVIndex')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'UVIndex',  Id, 'range', '0', '25' FROM [dbo].[Capabilities]
		WHERE Name = 'UVIndex'  
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'NoiseLevel')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId, Operator, MinValue, MaxValue)
		SELECT 'NoiseLevel',  Id, 'range', '0', '140' FROM [dbo].[Capabilities]
		WHERE Name = 'NoiseLevel'  
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'Gateway')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId)
		SELECT 'Gateway',  Id FROM [dbo].[Capabilities]
		WHERE Name = 'Gateway'  
	END
END


BEGIN
	IF NOT EXISTS (SELECT * FROM [dbo].[SensorCapabilityFilters] 
					WHERE Name = 'GatewayRange')
	BEGIN
		INSERT INTO [dbo].[SensorCapabilityFilters]  (Name,  CapabilityId)
		SELECT 'GatewayRange',  Id FROM [dbo].[Capabilities]
		WHERE Name = 'Gateway'  
	END
END