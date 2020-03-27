SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[UserRole];

CREATE TABLE [dbo].[UserRole](
	[Id] bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserName] varchar(15) NOT NULL,
		CONSTRAINT UQ_UserRole_UserName UNIQUE (UserName),
	[Roles] varchar(256) NOT NULL
);

GO