USE [BSCCSL]
GO

/****** Object:  Table [dbo].[Transaction_WealthCreator]    Script Date: 25-06-2022 17:44:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transaction_WealthCreator](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerProductId] [uniqueidentifier] NULL,
	[PaymentType] [int] NULL,
	[DateofCreditAmount] [datetime] NULL,
	[Balance] [decimal](18, 2) NULL,
	[MaturityDate] [datetime] NULL,
	[CreatedDatetime] [datetime] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Transaction_WealthCreator] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Transaction_WealthCreator] ADD  CONSTRAINT [DF_Transaction_WealthCreator_CreatedDatetime]  DEFAULT (getdate()) FOR [CreatedDatetime]
GO


