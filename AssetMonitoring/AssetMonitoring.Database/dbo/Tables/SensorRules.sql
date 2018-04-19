CREATE TABLE [dbo].[SensorRules] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [MinThreshold]        VARCHAR(MAX) NOT NULL,
    [CapabilityFilterId] INT            NULL,
    [SensorGroupId]           INT            NULL,
    [CreatedBy]          INT            NULL,
    [CreatedOn]          DATETIME       NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedOn]         DATETIME       NULL,   
    [MaxThreshold] VARCHAR(MAX) NULL, 
    CONSTRAINT [PK_dbo.SensorRules] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.SensorRules_dbo.SensorCapabilityFilters_CapabilityFilterId] FOREIGN KEY ([CapabilityFilterId]) REFERENCES [dbo].[SensorCapabilityFilters] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.SensorRules_dbo.SensorGroups_SensorGroupId] FOREIGN KEY ([SensorGroupId]) REFERENCES [dbo].[SensorGroups] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CapabilityFilterId]
    ON [dbo].[SensorRules]([CapabilityFilterId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SensorGroupId]
    ON [dbo].[SensorRules]([SensorGroupId] ASC);

