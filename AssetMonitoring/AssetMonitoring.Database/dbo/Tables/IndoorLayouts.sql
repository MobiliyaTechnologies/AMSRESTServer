CREATE TABLE [dbo].[IndoorLayouts]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
    [Name]                   VARCHAR(MAX) NOT NULL,
    [Description]            VARCHAR (MAX) NULL,
    [FileName]               VARCHAR(MAX) NOT NULL,
    [CreatedBy]              INT            NULL,
    [CreatedOn]              DATETIME       NULL,
    [ModifiedBy]             INT            NULL,
    [ModifiedOn]             DATETIME       NULL,
)
