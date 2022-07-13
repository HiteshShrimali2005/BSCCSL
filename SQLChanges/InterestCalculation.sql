---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Alter procedure [dbo].[InterestCalculation]
@Date datetime
As
Begin
SET NOCOUNT ON
	
	DECLARE @CustomerProductId uniqueidentifier
	DECLARE @Balance decimal(18,4)
	DECLARE @InterestRate decimal(18,4)
	DECLARE @IsFreeze bit
	DECLARE @CustomerId uniqueidentifier
	DECLARE @ProductType int	

	DECLARE interest CURSOR

	STATIC FOR 

		select c.CustomerProductId,c.Balance,c.InterestRate, c.IsFreeze, c.CustomerId, c.ProductType from CustomerProduct c, Customer m where 
		c.CustomerId = m.CustomerId and c.IsDelete = 0 and m.IsDelete = 0 and c.OpeningDate <= @Date and c.ProductType in (1,3,4,7,8,9,10) and 
		c.IsActive = 1 and (c.Status = 2 or c.Status is null)

	OPEN interest

	IF @@CURSOR_ROWS > 0
		 BEGIN 
			 FETCH NEXT FROM interest INTO  @CustomerProductId, @Balance, @InterestRate, @IsFreeze, @CustomerId, @ProductType
				WHILE @@Fetch_status = 0
				BEGIN

					BEGIN TRANSACTION TRANSINTERESTCALCULATION

					BEGIN TRY

					IF  @ProductType = 9
					BEGIN
						EXEC DBO.InterestCalculation_CapitalBuilder @CustomerProductId, @Balance, @InterestRate, @IsFreeze, @CustomerId, @ProductType, @Date
					END
					ELSE IF @ProductType = 10
					BEGIN
						EXEC DBO.InterestCalculation_WealthCreator @CustomerProductId, @Balance, @InterestRate, @IsFreeze, @CustomerId, @ProductType, @Date
					END
					ELSE
					BEGIN
						IF(@IsFreeze = 1 and @ProductType = 4)
						BEGIN
							SET @InterestRate = (Select TOP 1 InterestRate from CustomerProduct where CustomerId = @CustomerId and ProductType = 1)
						END
					
						DECLARE @MonthlyTax decimal(18,4)
						DECLARE @DailyTax decimal(18,4)
						DECLARE @Interest decimal(18,4)

						SET @MonthlyTax = @InterestRate / 12

						SET @DailyTax = @MonthlyTax /  DAY(DATEADD(DD,-1,DATEADD(mm, DATEDIFF(mm, 0, @Date) + 1, 0)))

						SET @Interest = ((@Balance * @DailyTax)/100)  

						IF @Interest > 0 and @Interest is not null
						BEGIN
							 INSERT INTO DailyInterest (CustomerProductId, TodaysInterest,InterestRate, IsPaid, CreatedDate) values											(@CustomerProductId, @Interest, @InterestRate, 0, @Date)
						END
					END
						
					 	END TRY
						BEGIN CATCH
							ROLLBACK TRANSACTION TRANSINTERESTCALCULATION
						END CATCH
						COMMIT TRANSACTION TRANSINTERESTCALCULATION

					FETCH NEXT FROM interest INTO  @CustomerProductId, @Balance, @InterestRate, @IsFreeze, @CustomerId, @ProductType
				END
		END
	CLOSE interest
	DEALLOCATE interest

	SET NOCOUNT OFF 	

End


