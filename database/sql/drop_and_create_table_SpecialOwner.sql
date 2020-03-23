SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[SpecialOwner]

CREATE TABLE [dbo].[SpecialOwner](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CreationDate] DateTime2 NOT NULL,
	[CreatedBy] varchar(30) NOT NULL,
	[ModificationDate] DateTime2 NOT NULL,
	[ModifiedBy] varchar(30) NOT NULL,
	[Name] varchar(30) NOT NULL,
	[Remark] varchar(30)
)
GO