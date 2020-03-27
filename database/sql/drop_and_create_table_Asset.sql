SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Asset has foreign key dependencies.
DROP TABLE IF EXISTS [dbo].[AssetHistory];
DROP TABLE IF EXISTS [dbo].[Asset];

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