USE [BSCCSL]
GO
/****** Object:  Table [dbo].[Setting]    Script Date: 22-06-2022 10:05:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Setting](
	[SeetingId] [uniqueidentifier] NOT NULL,
	[SettingName] [varchar](50) NULL,
	[Value] [varchar](50) NULL,
	[BranchId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.Setting] PRIMARY KEY CLUSTERED 
(
	[SeetingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'c9358856-0a67-e711-80e4-00505621610d', N'Monthly Income Scheme', N'MIS', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'ca358856-0a67-e711-80e4-00505621610d', N'MIS', N'02', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'cfe80362-0a67-e711-80e4-00505621610d', N'Monthly Income SchemeNo', N'000088', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'0d94e134-a6ef-ec11-9553-40b076de2658', N'Capital Builder', N'CB', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'3f9f003c-a6ef-ec11-9553-40b076de2658', N'CB', N'04', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'54502653-a9ef-ec11-9553-40b076de2658', N'Capital BuilderNo', N'000004', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'6a46490e-e3f1-ec11-9554-40b076de2658', N'Wealth Creator', N'WC', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'6b46490e-e3f1-ec11-9554-40b076de2658', N'WC', N'02', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'77e96920-e3f1-ec11-9554-40b076de2658', N'Wealth CreatorNo', N'000001', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'b0ebe48b-3820-e711-be37-941fd08a4b4d', N'ApplicationNo', N'9901', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'fdda4ee8-be0b-e711-a94b-a65d96eb6839', N'AccountNo', N'000001', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'feda4ee8-be0b-e711-a94b-a65d96eb6839', N'ClientId', N'009901', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'34f457ee-f119-e711-a94b-a65d96eb6839', N'UserCode', N'1449', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'5c265e8a-5f1f-e711-a94c-b5147654e6b8', N'SA', N'06', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'973e4d92-5f1f-e711-a94c-b5147654e6b8', N'CA', N'01', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'ffd51e9d-5f1f-e711-a94c-b5147654e6b8', N'FD', N'02', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'9c8e25a9-5f1f-e711-a94c-b5147654e6b8', N'RD', N'04', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'348348af-5f1f-e711-a94c-b5147654e6b8', N'LN', N'09', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'1d0da43e-651f-e711-a94c-b5147654e6b8', N'Saving Account', N'SA', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'1e0da43e-651f-e711-a94c-b5147654e6b8', N'Current Account', N'CA', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'a48e175a-651f-e711-a94c-b5147654e6b8', N'Fixed Deposit', N'FD', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'fc92d16e-651f-e711-a94c-b5147654e6b8', N'Recurring Deposit', N'RD', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'35f90075-651f-e711-a94c-b5147654e6b8', N'Loan', N'LN', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'c0512cd3-7d1f-e711-a94c-b5147654e6b8', N'ShareNumber', N'97859', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'436c558f-831f-e711-a94c-b5147654e6b8', N'BranchCode', N'25', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'87f6918a-0920-e711-a94c-b5147654e6b8', N'Saving AccountNo', N'009945', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'c2551d9b-0920-e711-a94c-b5147654e6b8', N'Current AccountNo', N'000001', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'270647aa-0920-e711-a94c-b5147654e6b8', N'Fixed DepositNo', N'001359', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'1eaebeb6-0920-e711-a94c-b5147654e6b8', N'Recurring DepositNo', N'009328', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'1ea1c769-0a20-e711-a94c-b5147654e6b8', N'LoanNo', N'001550', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'359cd4e9-b949-e711-be4b-c03fd5ad86b5', N'GroupLoanNo', N'000022', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'9fc69e5a-3421-e811-beb0-c03fd5ad86b5', N'Three Year Product', N'TYP', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'2f298757-3721-e811-beb0-c03fd5ad86b5', N'TYP', N'03', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'eeb0e7a0-c521-e811-beb0-c03fd5ad86b5', N'Three Year ProductNo', N'000224', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'e6c02bbc-d954-e711-be51-e35c1a118c8c', N'Regular Income Planner', N'RIP', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'0152bbc2-d954-e711-be51-e35c1a118c8c', N'RIP', N'04', NULL)
GO
INSERT [dbo].[Setting] ([SeetingId], [SettingName], [Value], [BranchId]) VALUES (N'a85a64e2-d954-e711-be51-e35c1a118c8c', N'Regular Income PlannerNo', N'000011', NULL)
GO
ALTER TABLE [dbo].[Setting] ADD  DEFAULT (newsequentialid()) FOR [SeetingId]
GO
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Setting_dbo.Branch_BranchId] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branch] ([BranchId])
GO
ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_dbo.Setting_dbo.Branch_BranchId]
GO
