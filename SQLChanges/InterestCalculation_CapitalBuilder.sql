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

    -- it's for debug purpose , can uncomment below statement and test the values
    --SELECT @CustomerProductId,@Balance,@InterestRate,@IsFreeze,@CustomerId,@ProductType,@Date 

    DECLARE @LastTransactionTime datetime
    DECLARE @PaymentType int    

    SELECT @LastTransactionTime = max(TransactionTime) from Transactions TR with(nolock)  
    where TR.CustomerProductId = @CustomerProductId and TR.CustomerId = @CustomerId and TR.[Type] in (1,3,5) 

    Select TOP 1 @PaymentType = PaymentType from CustomerProduct with(nolock) 
    where CustomerId = @CustomerId and ProductType = @ProductType

    IF ((DATEDIFF(MONTH, @LastTransactionTime, GETDATE())) > 6 and @PaymentType in (2,3)) OR 
            ((DATEDIFF(MONTH, @LastTransactionTime, GETDATE())) > 12 and @PaymentType in (4,5))
    BEGIN
        SET @InterestRate = 4
    END
    ELSE
        SET @InterestRate = (Select TOP 1 InterestRate from CustomerProduct with(nolock) where CustomerId = @CustomerId and ProductType = @ProductType)
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