USE [BSCCSL]
GO

/****** Object:  Table [dbo].[DailyInterest_WealthCreator]    Script Date: 25-06-2022 17:42:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DailyInterest_WealthCreator](
	[DailyInterestId] [uniqueidentifier] NOT NULL,
	[CustomerProductId] [uniqueidentifier] NOT NULL,
	[TodaysInterest] [decimal](18, 2) NOT NULL,
	[InterestRate] [decimal](18, 2) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[IsPaid] [bit] NOT NULL
) ON [PRIMARY]
GO


