SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[SpecialOwner];
DROP TABLE IF EXISTS [dbo].[AssetHistory];
DROP TABLE IF EXISTS [dbo].[Asset];
DROP TABLE IF EXISTS [dbo].[Product];
DROP TABLE IF EXISTS [dbo].[UserRole];

GO

CREATE TABLE [dbo].[SpecialOwner](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CreationDate] DateTime2 NOT NULL,
	[CreatedBy] varchar(30) NOT NULL,
	[ModificationDate] DateTime2 NOT NULL,
	[ModifiedBy] varchar(30) NOT NULL,
	[Name] varchar(30) NOT NULL,
	[Remark] varchar(30)
);

GO

CREATE TABLE [dbo].[Product](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CreationDate] DateTime2 NOT NULL,
	[CreatedBy] varchar(30) NOT NULL,
	[ModificationDate] DateTime2 NOT NULL,
	[ModifiedBy] varchar(30) NOT NULL,
	[Brand] varchar(30) NOT NULL,
	[Type] varchar(30) NOT NULL,
	[Group] varchar(30) NOT NULL
);

GO

-- Add the unique constraint.
ALTER TABLE [dbo].[Product] ADD CONSTRAINT UQ_Product_Brand_Type_Group UNIQUE([Brand], [Type], [Group]);

GO

CREATE TABLE [dbo].[Asset](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CreationDate] DateTime2 NOT NULL,
	[CreatedBy] varchar(30) NOT NULL,
	[ModificationDate] DateTime2 NOT NULL,
	[ModifiedBy] varchar(30) NOT NULL,
	[IctsReference] varchar(30),
	[Tag] varchar(15) NOT NULL,
		CONSTRAINT UQ_Asset_Tag UNIQUE (Tag),
	[Serial] varchar(30),
	[ProductId] bigint NOT NULL,
		CONSTRAINT FK_Asset_Product_Id FOREIGN KEY (ProductId)
		REFERENCES [dbo].[Product](Id),
	[Description] varchar(500),
	[InvoiceDate] DateTime2,
	[InvoiceNumber] varchar(30),
	[Price] decimal(18,2),
	[PaidBy] varchar(30) NOT NULL,
	[Owner] varchar(30) NOT NULL,
	[InstallDate] DateTime2,
	[InstalledBy] varchar(30),
	[Remark] varchar(500),
	[TeamAsset] bit NOT NULL,
	[Deleted] bit NOT NULL
);

GO

CREATE TABLE [dbo].[AssetHistory](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[AssetId] bigint NOT NULL,
		 CONSTRAINT FK_AssetHistory_Asset_Id FOREIGN KEY (AssetId)
		 REFERENCES [dbo].[Asset](Id),
	[CreationDate] DateTime2 NOT NULL,
	[CreatedBy] varchar(30) NOT NULL,
	[ModificationDate] DateTime2 NOT NULL,
	[ModifiedBy] varchar(30) NOT NULL,
	[IctsReference] varchar(30),
	[Tag] varchar(15) NOT NULL,
	[Serial] varchar(30),
	[ProductId] bigint NOT NULL,
		CONSTRAINT FK_AssetHistory_Product_Id FOREIGN KEY (ProductId)
		REFERENCES [dbo].[Product](Id),
	[Description] varchar(500),
	[InvoiceDate] DateTime2,
	[InvoiceNumber] varchar(30),
	[Price] decimal(18,2),
	[PaidBy] varchar(30) NOT NULL,
	[Owner] varchar(30) NOT NULL,
	[InstallDate] DateTime2,
	[InstalledBy] varchar(30),
	[Remark] varchar(500),
	[TeamAsset] bit NOT NULL,
	[Deleted] bit NOT NULL
);

GO

DROP INDEX IF EXISTS IX_AssetHistory_AssetId ON [dbo].[AssetHistory];
CREATE NONCLUSTERED INDEX IX_AssetHistory_AssetId ON [dbo].[AssetHistory](AssetId);

DROP INDEX IF EXISTS IX_AssetHistory_Tag ON [dbo].[AssetHistory];
CREATE NONCLUSTERED INDEX IX_AssetHistory_Tag ON [dbo].[AssetHistory](Tag);

GO

-- Create the AfterUpdate trigger on the Asset table.
-- Whenever an Asset is updated, the previous values are inserted into the AssetHistory table. So the is a trace of the changes.
DROP TRIGGER IF EXISTS [dbo].[AfterAssetUpdateTrigger];

GO

CREATE TRIGGER [dbo].[AfterAssetUpdateTrigger] on [dbo].[Asset]
AFTER UPDATE
AS
	INSERT INTO [dbo].[AssetHistory] ([AssetId], [CreationDate], [CreatedBy], [ModificationDate], [ModifiedBy], [IctsReference], [Tag], [Serial], [ProductId], [Description], 
		[InvoiceDate], [InvoiceNumber], [Price], [PaidBy], [Owner], [InstallDate], [InstalledBy], [Remark], [TeamAsset], [Deleted])
	SELECT del.Id, del.CreationDate, del.CreatedBy, del.ModificationDate, del.ModifiedBy, del.IctsReference, del.Tag, del.Serial, del.ProductId, del.Description, 
		del.InvoiceDate, del.InvoiceNumber, del.Price, del.PaidBy, del.Owner, del.InstallDate, del.InstalledBy, del.Remark, del.TeamAsset, del.Deleted 
	FROM DELETED del
;

GO

CREATE TABLE [dbo].[UserRole](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserName] varchar(15) NOT NULL,
		CONSTRAINT UQ_UserRole_UserName UNIQUE (UserName),
	[Roles] varchar(256) NOT NULL
);

GO