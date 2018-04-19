CREATE TABLE [dbo].[Gateways] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Name]                   NVARCHAR (MAX) NOT NULL,
    [Description]            NVARCHAR (MAX) NULL,
    [IotHubAccessTocken]     NVARCHAR (MAX) NULL,
    [CreatedBy]              INT            NULL,
    [CreatedOn]              DATETIME       NULL,
    [ModifiedBy]             INT            NULL,
    [ModifiedOn]             DATETIME       NULL,
    [GatewayKey] NVARCHAR(500) NOT NULL, 
    [LayoutX] FLOAT NULL, 
    [LayoutY] FLOAT NULL, 
    [IndoorLayoutId] INT NULL, 
    CONSTRAINT [PK_dbo.Gateways] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Gateways_ToIndoorLayouts] FOREIGN KEY ([IndoorLayoutId]) REFERENCES [IndoorLayouts]([Id]) ON DELETE SET NULL
);

