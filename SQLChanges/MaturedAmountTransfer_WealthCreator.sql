create procedure [dbo].[MaturedAmountTransfer_WealthCreator]
@Date datetime
As
Begin

	SET NOCOUNT ON
	DECLARE @CustomerProductId uniqueidentifier
	DECLARE @CustomerId uniqueidentifier
	DECLARE @Balance decimal(18,4)
	DECLARE @UpdatedBalance decimal(18,4)
	DECLARE @TotalInterest decimal(18,4)

	
	DECLARE creditinterest CURSOR

	STATIC FOR 

		select d.CustomerProductId, c.CustomerId, c.Balance, c.UpdatedBalance, SUM(d.TodaysInterest) as TotalInterest from CustomerProduct c, 
		DailyInterest_WealthCreator d, Customer m where c.CustomerId = m.CustomerId and c.CustomerProductId = d.CustomerProductId and c.IsActive = 1 
		and c.Status = 2 and c.IsDelete = 0 and m.IsDelete = 0 and c.ProductType in(10) and MaturityDate = CONVERT(char(10), @Date,126) and d.IsPaid = 0 
		group by d.CustomerProductId,c.CustomerId,c.Balance, c.UpdatedBalance

	OPEN creditinterest

	IF @@CURSOR_ROWS > 0
	 BEGIN 
	 FETCH NEXT FROM creditinterest INTO  @CustomerProductId, @CustomerId, @Balance,@UpdatedBalance, @TotalInterest
	 WHILE @@Fetch_status = 0
	 BEGIN

		DECLARE @SavingBalance AS decimal(18, 2)
		DECLARE @SavingCustomerProductId AS uniqueidentifier
		DECLARE @SavingUpdatedBalance AS decimal(18, 2)

        SELECT @SavingBalance = Balance,@SavingCustomerProductId = CustomerProductId, @SavingUpdatedBalance = UpdatedBalance 
		FROM CustomerProduct WHERE CustomerId = @CustomerId AND ProductType = 10 and IsActive = 1


		Declare @RDUpdatedBalance decimal(18,4)
		SET @RDUpdatedBalance = @UpdatedBalance + @TotalInterest;

		DECLARE @TransactionId AS uniqueidentifier

		EXEC SaveTransaction @CustomerProductId, @CustomerId, @TotalInterest, @RDUpdatedBalance, 1, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 6, null,null,null, @TransactionId output

		--Update CustomerProduct set UpdatedBalance = UpdatedBalance + @TotalInterest, Balance = Balance + @TotalInterest where CustomerProductId = @CustomerProductId

		Declare @TotalBalance decimal(18,4)

		SET @TotalBalance = @Balance + @TotalInterest

		SET @RDUpdatedBalance = @RDUpdatedBalance - @TotalBalance;

		Exec SaveTransaction @CustomerProductId, @CustomerId , @TotalBalance, @RDUpdatedBalance, 2, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 2, @SavingCustomerProductId,null,null, @TransactionId output

		-- add transaction of RD in Transaction

		Declare @SBUpdatedBalance decimal(18,4)
		SET @SBUpdatedBalance = @SavingUpdatedBalance +  @TotalBalance;

		EXEC SaveTransaction @SavingCustomerProductId, @CustomerId, @TotalBalance,@SBUpdatedBalance, 1, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 2, @CustomerProductId,null,null, @TransactionId output

		-- Update balance of Saving account
		--UPDATE CustomerProduct SET UpdatedBalance = UpdatedBalance + @TotalBalance, Balance = Balance + @TotalBalance 
		--WHERE CustomerProductId = @SavingCustomerProductId

		-- Update balance of RD
		 --UpdatedBalance = UpdatedBalance - @TotalBalance, Balance = Balance - @TotalBalance,
		UPDATE CustomerProduct SET Status = 5 WHERE CustomerProductId = @CustomerProductId

		Update DailyInterest_WealthCreator set IsPaid = 0 where CustomerProductId = @CustomerProductId


		FETCH NEXT FROM creditinterest INTO @CustomerProductId, @CustomerId, @Balance,@UpdatedBalance, @TotalInterest
	 END
	END
	CLOSE creditinterest
	DEALLOCATE creditinterest
	SET NOCOUNT OFF 	
End



