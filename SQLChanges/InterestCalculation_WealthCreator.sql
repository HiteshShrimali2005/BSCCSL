Create procedure [dbo].[InterestCalculation_WealthCreator]
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
	DECLARE @LastTransactionTime datetime
	DECLARE @Transaction_WealthCreatorID bigint
	DECLARE @TranErrCnt int

	DECLARE interest CURSOR

	STATIC FOR 

		SELECT CP.CustomerProductId,CP.Balance,CP.InterestRate, CP.IsFreeze, CP.CustomerId, CP.ProductType, TR.DateofCreditAmount,TR.ID 
		FROM TRANSACTION_WEALTHCREATOR TR WITH(NOLOCK) 
		left join CustomerProduct CP on CP.CustomerProductId = tr.CustomerProductId 
		left join Customer CM on CM.CustomerId = CP.CustomerId where 
		CP.CustomerId = CM.CustomerId and CM.IsDelete = 0 and CM.IsDelete = 0 and  CP.OpeningDate <= @Date and CP.ProductType in (10) and 
		CP.IsActive = 1 and (CP.Status = 2 or CP.Status is null)

	SET @TRANERRCNT = 0

	OPEN interest


	IF @@CURSOR_ROWS > 0
		 BEGIN 
			 FETCH NEXT FROM interest INTO  @CustomerProductId, @Balance, @InterestRate, @IsFreeze, @CustomerId, @ProductType,@LastTransactionTime,@Transaction_WealthCreatorID
				WHILE @@Fetch_status = 0
				BEGIN

					BEGIN TRANSACTION TRANSINTERESTCALCULATION
						BEGIN TRY

						

							IF DATEDIFF(MONTH, @LastTransactionTime, GETDATE()) > 6  OR DATEDIFF(MONTH, @LastTransactionTime, GETDATE()) > 12
							BEGIN
								SET @InterestRate = 4
							END
							ELSE
							BEGIN
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

							 INSERT INTO DailyInterest_WealthCreator (CustomerProductId, TodaysInterest,InterestRate, IsPaid, CreatedDate,Transaction_WealthCreatorID) values
							 (@CustomerProductId, @Interest, @InterestRate, 0, @Date,@Transaction_WealthCreatorID)
						END
						
						
						
						END TRY
						BEGIN CATCH
							ROLLBACK TRANSACTION TRANSINTERESTCALCULATION
						END CATCH
					COMMIT TRANSACTION TRANSINTERESTCALCULATION

				FETCH NEXT FROM interest INTO  @CustomerProductId, @Balance, @InterestRate, @IsFreeze, @CustomerId, @ProductType,@LastTransactionTime,@Transaction_WealthCreatorID
				END
		END
	CLOSE interest
	DEALLOCATE interest
	
	  IF @TRANERRCNT = 0
	  BEGIN  
		UPDATE DAILYPROCESS_CONSOLE_LOG SET DAILYPROCESSDATE = @DATE WHERE DAILYPROCESSCODE = '010'
	  END

SET NOCOUNT OFF 	

END




