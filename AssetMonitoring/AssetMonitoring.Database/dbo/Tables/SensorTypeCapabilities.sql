CREATE TABLE [dbo].[SensorTypeCapabilities] (
    [SensorType_Id] INT NOT NULL,
    [Capability_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.SensorTypeCapabilities] PRIMARY KEY CLUSTERED ([SensorType_Id] ASC, [Capability_Id] ASC),
    CONSTRAINT [FK_dbo.SensorTypeCapabilities_dbo.Capabilities_Capability_Id] FOREIGN KEY ([Capability_Id]) REFERENCES [dbo].[Capabilities] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.SensorTypeCapabilities_dbo.SensorTypes_SensorType_Id] FOREIGN KEY ([SensorType_Id]) REFERENCES [dbo].[SensorTypes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SensorType_Id]
    ON [dbo].[SensorTypeCapabilities]([SensorType_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Capability_Id]
    ON [dbo].[SensorTypeCapabilities]([Capability_Id] ASC);

