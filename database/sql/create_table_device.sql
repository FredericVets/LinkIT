/****** Object:  Table [dbo].[Device]    Script Date: 22/02/2020 18:47:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Device](
	[Id] [bigint] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Brand] [nvarchar](20) NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
	[Owner] [nvarchar](20) NOT NULL,
	[Tag] [nvarchar](20) NOT NULL,
)
GO

-- See https://stackoverflow.com/questions/4413178/executescalar-with-scope-identity-generating-system-invalidcastexception on how to use SCOPE_IDENTITY
-- SELECT CONVERT(int, SCOPE_IDENTITY())