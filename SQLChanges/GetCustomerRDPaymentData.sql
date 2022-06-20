USE [BSCCSL]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[GetCustomerRDPaymentData]
@CustomerProductId nvarchar(Max)
as
Begin
	
		Select sum(Amount) as Amount from RDPayment with(nolock)
		WHERE CustomerProductId = @CustomerProductId
		and IsPaid  = 1 and RDPaymentType = 1
		GROUP BY Amount 		
End