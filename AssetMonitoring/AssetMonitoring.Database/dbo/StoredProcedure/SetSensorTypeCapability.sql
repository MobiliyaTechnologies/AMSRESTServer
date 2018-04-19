CREATE PROCEDURE SetSensorTypeCapability  
    @sensorType varchar(max),   
    @capability varchar(max)   
AS   
  Begin

  DECLARE @sensorTypeId INT;
  DECLARE @capabilityId INT;

  set @sensorTypeId = (select id from [dbo].[SensorTypes] where [Name] = @sensorType)
  set @capabilityId = (select id from [dbo].[Capabilities] where [Name] = @capability)

  IF NOT EXISTS (SELECT * FROM [dbo].[SensorTypeCapabilities] 
					WHERE [SensorType_Id] = @sensorTypeId and [Capability_Id] = @capabilityId)
	BEGIN
		INSERT INTO [dbo].[SensorTypeCapabilities]  ([SensorType_Id], [Capability_Id])
		VALUES (@sensorTypeId, @capabilityId)
	END

  End
GO  
