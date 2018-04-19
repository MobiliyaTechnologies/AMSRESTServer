CREATE TABLE [dbo].[ApplicationConfigurationEntry]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ConfigurationKey] [varchar](max) NOT NULL,
	[ConfigurationValue] [varchar](max) NOT NULL,
	[ApplicationConfigurationId] [int] NULL,
    [CreatedBy]    INT            NULL,
    [CreatedOn]    DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedOn]   DATETIME       NULL, 
    CONSTRAINT [FK_ApplicationConfigurationEntry_ToApplicationConfiguration] FOREIGN KEY ([ApplicationConfigurationId]) REFERENCES [ApplicationConfiguration]([Id]) ON DELETE CASCADE,
)
