CREATE TABLE [dbo].[ApplicationConfiguration]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1) ,
    [ConfigurationType] [varchar](max) NOT NULL,
    [CreatedBy]    INT            NULL,
    [CreatedOn]    DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedOn]   DATETIME       NULL,
)
