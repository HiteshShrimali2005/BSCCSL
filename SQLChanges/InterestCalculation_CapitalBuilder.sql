Alter procedure [dbo].[InterestCalculation_CapitalBuilder]
@CustomerProductId uniqueidentifier,
@Balance decimal(18,4),
@InterestRate decimal(18,4),
@IsFreeze bit,
@CustomerId uniqueidentifier,
@ProductType int,
@Date datetime
As
Begin
SET NOCOUNT ON	
	
	SELECT @CustomerProductId,@Balance,@InterestRate,@IsFreeze,@CustomerId,@ProductType,@Date

	IF(@IsFreeze = 1)
	BEGIN
		SET @InterestRate = (Select TOP 1 InterestRate from CustomerProduct where CustomerId = @CustomerId and ProductType = @ProductType)
	END
	
	DECLARE @MonthlyTax decimal(18,4)
	DECLARE @DailyTax decimal(18,4)
	DECLARE @Interest decimal(18,4)

	SET @MonthlyTax = @InterestRate / 12

	SET @DailyTax = @MonthlyTax /  DAY(DATEADD(DD,-1,DATEADD(mm, DATEDIFF(mm, 0, @Date) + 1, 0)))

	SET @Interest = ((@Balance * @DailyTax)/100)  

	IF @Interest > 0 and @Interest is not null
	BEGIN
		 INSERT INTO DailyInterest (CustomerProductId, TodaysInterest,InterestRate, IsPaid, CreatedDate) values
		 (@CustomerProductId, @Interest, @InterestRate, 0, @Date)
	END

	SET NOCOUNT OFF 	

End


