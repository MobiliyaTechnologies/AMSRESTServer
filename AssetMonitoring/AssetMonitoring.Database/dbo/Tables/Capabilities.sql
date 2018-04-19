CREATE TABLE [dbo].[Capabilities] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [CreatedBy]   INT            NULL,
    [CreatedOn]   DATETIME       NULL,
    [ModifiedBy]  INT            NULL,
    [ModifiedOn]  DATETIME       NULL,
    [Unit] NVARCHAR(MAX) NOT NULL, 
    [SupportedUnits] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_dbo.Capabilities] PRIMARY KEY CLUSTERED ([Id] ASC)
);

