---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Alter procedure [dbo].[CreditInterestInCustomerAccount]
@Date datetime
AS
Begin

	SET NOCOUNT ON
	DECLARE @CustomerProductId uniqueidentifier
	DECLARE @CustomerId uniqueidentifier
	DECLARE @Balance decimal(18,4)
	DECLARE @UpdatedBalance decimal(18,4)
	--DECLARE @TotalInterest decimal(18,4)
	DECLARE @Frequency int
	

	
	DECLARE creditinterest CURSOR

	STATIC FOR 

		select c.CustomerProductId, c.CustomerId, c.Balance, c.UpdatedBalance, p.Frequency from Customer m, CustomerProduct c, 
		Product p where m.CustomerId = c.CustomerId and p.ProductId = c.ProductId and m.IsDelete = 0 and c.IsDelete = 0 and c.IsActive = 1 
		and (c.Status != 5 or c.Status is null) and c.ProductType in (1,3,4,8,9) group by c.CustomerProductId, c.CustomerId, c.Balance, c.UpdatedBalance, p.Frequency 

	OPEN creditinterest

	IF @@CURSOR_ROWS > 0
	 BEGIN 
	 FETCH NEXT FROM creditinterest INTO  @CustomerProductId, @CustomerId, @Balance,@UpdatedBalance, @Frequency
	 WHILE @@Fetch_status = 0
	 BEGIN

	   BEGIN TRANSACTION TRANSCREDITINTEREST

	   BEGIN TRY

		DECLARE @StartDate as datetime
		DECLARE @EndDate as datetime

		IF @Frequency = 1
		BEGIN
			 SET @StartDate = DATEADD(day,1,@Date)
			 SET @EndDate = DATEADD(day,1,@Date)
		END
		ELSE IF @Frequency = 2
		BEGIN
			SET @StartDate = CONVERT(DATE,dateadd(dd,-(day(@Date)-1),@Date))
			SET @EndDate = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@Date)+1,0))
		END
		ELSE IF @Frequency = 3
		BEGIN
			SET @StartDate = DATEADD(q, DATEDIFF(q, 0, @Date), 0)
			SET @EndDate = DATEADD(d, -1, DATEADD(q, DATEDIFF(q, 0, @Date) + 1, 0)) 
		END
		ELSE IF @Frequency = 4
		BEGIN
			SET @StartDate = CAST(CAST(((((MONTH(@Date) - 1) / 6) * 6) + 1) AS VARCHAR) + '-1-' + CAST(YEAR(@Date) AS VARCHAR) AS DATETIME)
			SET @EndDate = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,DATEADD(day,1,CAST(CAST(((((MONTH(@Date) - 1) / 6) * 6) + 6) AS VARCHAR) + '-1-' + CAST(YEAR(@Date) AS VARCHAR) AS DATETIME)))+1,0))
		END
		ELSE IF @Frequency = 5
		BEGIN
			SET @StartDate =   DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) + 1, -1)
			SET @EndDate = DATEADD (dd, -1, DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) +1, 0))
		END

		if(@EndDate = CONVERT(char(10), @Date,126))
		BEGIN

	
		DECLARE @TotalInterest decimal(18,4) 

		SET @TotalInterest= (select ISNULL(SUM(d.TodaysInterest),0) as TotalInterest from CustomerProduct c, Product p, 
		DailyInterest d	where p.ProductId = c.ProductId and c.CustomerProductId = d.CustomerProductId and c.IsActive = 1 and 
		(c.Status != 5 or c.Status is null) and p.Frequency = @Frequency and c.CustomerProductId = @CustomerProductId and (d.CreatedDate >= @StartDate and d.CreatedDate <= @EndDate and d.IsPaid = 0))

		If(@TotalInterest > 0 and @TotalInterest is not null)
		BEGIN

		--Declare @Bal as decimal(18,4)
		--SET @Bal =  @UpdatedBalance+ @TotalInterest

		Declare @TransactionId as uniqueidentifier

		DECLARE @ProductTypeId int
		
		Select top 1 @ProductTypeId = ProductType from CustomerProduct with(nolock) where CustomerProductId = @CustomerProductId

		print @ProductTypeId
		
		If(@ProductTypeId = 9)
		BEGIN
			Exec SaveTransaction @CustomerProductId, @CustomerId, @TotalInterest, 0, 1, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 1, null,null,null, @TransactionId output
		END

		Exec SaveTransaction @CustomerProductId, @CustomerId, @TotalInterest, 0, 1, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 1, null,null,null, @TransactionId output

		--Update CustomerProduct set UpdatedBalance = UpdatedBalance + @TotalInterest, Balance = Balance + @TotalInterest where CustomerProductId = @CustomerProductId

		Update DailyInterest set IsPaid = 1 where CustomerProductId = @CustomerProductId and (CreatedDate >= @StartDate and CreatedDate <=	@EndDate)

		END
		END

		END TRY
	   BEGIN CATCH
		   ROLLBACK TRANSACTION TRANSCREDITINTEREST
	   END CATCH

	  COMMIT TRANSACTION TRANSCREDITINTEREST

	 FETCH NEXT FROM creditinterest INTO @CustomerProductId, @CustomerId, @Balance, @UpdatedBalance, @Frequency
	 END
	END
	CLOSE creditinterest
	DEALLOCATE creditinterest
	SET NOCOUNT OFF 



End



