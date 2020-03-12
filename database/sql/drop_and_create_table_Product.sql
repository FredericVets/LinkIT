SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Asset has foreign key dependency
DROP TABLE IF EXISTS [dbo].[Asset]
DROP TABLE IF EXISTS [dbo].[Product]

CREATE TABLE [dbo].[Product](
	[Id] [bigint] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CreationDate] [DateTime2] NOT NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[ModificationDate] [DateTime2] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[Brand] [varchar](30) NOT NULL,
	[Type] [varchar](30) NOT NULL
)
GO