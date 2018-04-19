CREATE TABLE [dbo].[SensorTypes] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [CreatedBy]   INT            NULL,
    [CreatedOn]   DATETIME       NULL,
    [ModifiedBy]  INT            NULL,
    [ModifiedOn]  DATETIME       NULL,
    CONSTRAINT [PK_dbo.SensorTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

