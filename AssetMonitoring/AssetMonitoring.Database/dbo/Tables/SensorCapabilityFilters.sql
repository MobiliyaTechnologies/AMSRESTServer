CREATE TABLE [dbo].[SensorCapabilityFilters] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (MAX) NOT NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [Operator]       VARCHAR (MAX)  NULL,
    [CapabilityId]   INT            NULL,
    [CreatedBy]      INT            NULL,
    [CreatedOn]      DATETIME       NULL,
    [ModifiedBy]     INT            NULL,
    [ModifiedOn]     DATETIME       NULL,
    [MinValue] VARCHAR(MAX) NULL, 
    [MaxValue] VARCHAR(MAX) NULL, 
    CONSTRAINT [PK_dbo.SensorCapabilityFilters] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.SensorCapabilityFilters_dbo.Capabilities_CapabilityId] FOREIGN KEY ([CapabilityId]) REFERENCES [dbo].[Capabilities] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CapabilityId]
    ON [dbo].[SensorCapabilityFilters]([CapabilityId] ASC);

