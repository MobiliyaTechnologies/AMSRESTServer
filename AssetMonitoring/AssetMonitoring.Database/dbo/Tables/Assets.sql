CREATE TABLE [dbo].[Assets] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [AssetBarcode] NVARCHAR (MAX) NULL,
    [CreatedBy]    INT            NULL,
    [CreatedOn]    DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedOn]   DATETIME       NULL,
    [SensorGroupId] INT NULL, 
    CONSTRAINT [PK_dbo.Assets] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Assets_ToSensorGroup] FOREIGN KEY ([SensorGroupId]) REFERENCES  [dbo].[SensorGroups] ([Id]) ON DELETE CASCADE
);


GO
