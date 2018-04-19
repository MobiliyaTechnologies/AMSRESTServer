CREATE TABLE [dbo].[Sensors] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (MAX) NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [SensorKey] NVARCHAR (500) NOT NULL,
    [SensorTypeId]     INT            NULL,
    [CreatedBy]        INT            NULL,
    [CreatedOn]        DATETIME       NULL,
    [ModifiedBy]       INT            NULL,
    [ModifiedOn]       DATETIME       NULL,
    [SensorGroupId] INT NULL, 
    [AssetId] INT NULL, 
    CONSTRAINT [PK_dbo.Sensors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Sensors_dbo.SensorTypes_SensorTypeId] FOREIGN KEY ([SensorTypeId]) REFERENCES [dbo].[SensorTypes] ([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_Sensors_ToSensorGroup] FOREIGN KEY ([SensorGroupId]) REFERENCES  [dbo].[SensorGroups] ([Id]) ON DELETE NO ACTION, 
    CONSTRAINT [FK_Sensors_ToAsset] FOREIGN KEY ([AssetId]) REFERENCES [dbo].[Assets]([Id]) ON DELETE SET NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_SensorTypeId]
    ON [dbo].[Sensors]([SensorTypeId] ASC);

  