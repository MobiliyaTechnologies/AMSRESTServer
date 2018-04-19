CREATE TABLE [dbo].[SensorGroups]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [Description] VARCHAR(MAX) NULL,
    [CreatedBy]          INT            NULL,
    [CreatedOn]          DATETIME       NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedOn]         DATETIME       NULL, 
    [RuleCreationInProgress] BIT NOT NULL DEFAULT 0
)
