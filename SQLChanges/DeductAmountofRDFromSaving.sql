USE [BSCCSL]
GO
/****** Object:  StoredProcedure [dbo].[DeductAmountofRDFromSaving]    Script Date: 27-06-2022 09:33:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[DeductAmountofRDFromSaving] --'2022-07-26'
@Date datetime
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @CustomerProductId uniqueidentifier
  DECLARE @CustomerId uniqueidentifier
  DECLARE @Balance decimal(18, 4)
  DECLARE @UpdatedBalance decimal(18, 4)
  DECLARE @Amount decimal(18, 4)
  DECLARE @InterestRate decimal(18, 4)
  DECLARE @DueDate datetime
  DECLARE @ProductId uniqueidentifier
  DECLARE @RankId uniqueidentifier
  DECLARE @dd int
  DECLARE @OpeningDate datetime
  DECLARE @TimePeriod int
  DECLARE @NoOfMonthsORYears int
  DECLARE @PaymentType int
  DECLARE @LastInstallmentDate datetime	
  DECLARE @TotalInstallment int	
  DECLARE @TotalDays int
  DECLARE @InterestType int	
  DECLARE @NextInstallmentDate datetime
  DECLARE @OpeningBalance decimal(18,2)
  DECLARE @SkipFirstInstallment bit
  DECLARE @AgentId uniqueidentifier
    ----Add by Vishal
  DECLARE @ProductType int
  DECLARE @TranErrCnt int

  DECLARE creditdeposit CURSOR

  STATIC FOR

  SELECT c.CustomerProductId, c.CustomerId, c.Balance, c.UpdatedBalance, c.Amount, c.InterestRate, c.DueDate, c.ProductId as ComProductId, u.RankId as ComRankId, DATEPART(dd,c.DueDate) AS dd, 
  c.OpeningDate, c.TimePeriod, c.NoOfMonthsORYears, c.PaymentType, c.LastInstallmentDate, c.TotalInstallment, c.TotalDays, p.InterestType, c.NextInstallmentDate,ISNULL( c.OpeningBalance,0) as OpeningBalance, 
  c.SkipFirstInstallment, c.AgentId,c.ProductType  FROM Customer m, CustomerProduct c, Product p, [User] u WHERE m.CustomerId = c.CustomerId and c.ProductId = p.ProductId and c.AgentId = u.UserId and m.IsDelete = 0 and c.IsDelete = 0 
  and c.Status = 2 and c.ProductType in (4,9,10) AND c.IsActive = 1 and c.IsFreeze is null and CONVERT(char(10), c.NextInstallmentDate,126) = CONVERT(char(10), @Date,126) and ReferenceCustomerProductId is null
  --and c.CustomerId = '0a07af3e-3bd7-ec11-954c-40b076de2658'

  set @TranErrCnt = 0
  	OPEN creditdeposit

   IF @@CURSOR_ROWS > 0
  BEGIN
    FETCH NEXT FROM creditdeposit INTO @CustomerProductId, @CustomerId, @Balance, @UpdatedBalance, @Amount, @InterestRate, 
	@DueDate, @ProductId ,@RankId, @dd, @OpeningDate, @TimePeriod, @NoOfMonthsORYears, @PaymentType, @LastInstallmentDate, 
	@TotalInstallment, @TotalDays, @InterestType, @NextInstallmentDate, @OpeningBalance,@SkipFirstInstallment, @AgentId,@ProductType
    WHILE @@Fetch_status = 0
    BEGIN

		BEGIN TRANSACTION TRANSRD
		BEGIN TRY

			DECLARE @PaymentDate datetime

			IF @PaymentType = 1
			BEGIN
	 			SET @PaymentDate = DATEADD(day,1,@NextInstallmentDate)
			END
			ELSE IF @PaymentType = 2
			BEGIN
	 			SET @PaymentDate = DATEADD(month, 1,@NextInstallmentDate)

	 			DECLARE @Lastdd int 
	 
	 			SET @Lastdd = DATEPART(dd,DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0,@PaymentDate) + 1, 0)))
			 
	 			DECLARE @Paymentdd int 
	 
	 			SET @Paymentdd = DATEPART(dd,@PaymentDate)
	 
	 			IF(@Lastdd >= @dd and @Lastdd != @Paymentdd)
	 			BEGIN
	 				DECLARE @Diffdd int 
	 
	 				SET @Diffdd = @dd - @Paymentdd
	 	
	 				SET @PaymentDate = DATEADD(day,@Diffdd,@PaymentDate)
				END
	 
			  END
			ELSE IF @PaymentType = 3
				BEGIN
	 				SET @PaymentDate = DATEADD(month,3,@NextInstallmentDate)
				END
			ELSE IF @PaymentType = 4
				BEGIN
	 				SET @PaymentDate = DATEADD(month,6,@NextInstallmentDate)
				END
			ELSE IF @PaymentType = 5
				BEGIN
	 				SET @PaymentDate = DATEADD(year,1,@NextInstallmentDate)
				END

			--print 'paymentDate ' + convert(varchar,@NextInstallmentDate)

			DECLARE @InstallmentdeductedinRD int

			SET @InstallmentdeductedinRD = (Select COUNT(*) FROM RDPayment WHERE CustomerProductId = @CustomerProductId and RDPaymentType = 1)
	  
			IF (@TotalInstallment > @InstallmentdeductedinRD)																					BEGIN

				--get balance from saving account
				DECLARE @SavingBalance AS decimal(18, 2)
				DECLARE @SavingCustomerProductId AS uniqueidentifier
				DECLARE @SavingUpdatedBalance AS decimal(18, 2)

				SELECT TOP 1 @SavingBalance = Balance,@SavingCustomerProductId = CustomerProductId, @SavingUpdatedBalance = UpdatedBalance FROM CustomerProduct WHERE CustomerId = @CustomerId AND ProductType = 1 and IsDelete = 0 and IsActive = 1 order by CreatedDate
 

				-- get total Pending Installment
				DECLARE @TotalPendingInstallment AS int

				SET @TotalPendingInstallment = (SELECT COUNT(*) FROM RDPayment WHERE CustomerProductId = @CustomerProductId and IsPaid = 0 and RDPaymentType = 1)

				IF @TotalPendingInstallment > 0																												BEGIN
					-- checking balance in account then cut pending 1 installment
					DECLARE @RDPaymentId uniqueidentifier
					DECLARE @RDAmount decimal(18, 4)
					DECLARE @FirstInstallmentStartDate datetime
					DECLARE @FirstInstallmentEndDate datetime
					DECLARE @LatePaymentCharges decimal(18, 4)
					DECLARE @RDPaymentAmount decimal(18, 4)
				

				-- find startdate of pending first installment start date and end date
					SELECT TOP 1 @FirstInstallmentStartDate =  CONVERT(char(10), CreatedDate,126) , @FirstInstallmentEndDate = case when  LEAD( CONVERT(char(10), CreatedDate,126)) OVER (ORDER BY  CONVERT(char(10), CreatedDate,126)) is not null then  LEAD( CONVERT(char(10), CreatedDate,126)) OVER (ORDER BY  CONVERT(char(10), CreatedDate,126)) else  CONVERT(char(10), @Date,126) end FROM RDPayment WHERE CustomerProductId = @CustomerProductId and RDPaymentType = 1 and IsPaid = 0 ORDER BY CreatedDate asc

				-- calculate latepayment charges of daily using startdate and enddate
					SET @LatePaymentCharges = (SELECT COALESCE((Select SUM(ISNULL(Amount,0)) FROM RDPayment WHERE RDPaymentType = 2 and IsPaid = 0 and IsDelete = 0 and CustomerProductId = @CustomerProductId and CreatedDate >= @FirstInstallmentStartDate and CreatedDate < @FirstInstallmentEndDate group by CustomerProductId),0))
				
				-- get first installment of RD
					SELECT TOP 1 @RDPaymentId = RDPaymentId, @RDAmount = ISNULL(Amount,0) FROM RDPayment WHERE IsPaid = 0 and RDPaymentType = 1 and IsDelete = 0 and CustomerProductId = @CustomerProductId ORDER BY CreatedDate ASC

-- SUM of amount and latepayment charges

					SET @RDPaymentAmount = @RDAmount + ISNULL(@LatePaymentCharges,0)
-- if balance available then only installment will be deduct
					IF (@SavingBalance >= @RDPaymentAmount)
					BEGIN

						DECLARE @TotalInterestTillDate decimal(18, 4)

						-- credit interest
						IF @PaymentType != 1
						BEGIN

							DECLARE @CurrentMaturity decimal(18, 4)

							EXEC RDMaturityCalculation @Amount, @InterestRate, @OpeningDate, @FirstInstallmentEndDate, @DueDate, @PaymentType, @InterestType, @TotalInstallment, @OpeningBalance, @SkipFirstInstallment, @CurrentMaturity OUTPUT

							SET @TotalInterestTillDate = (SELECT COALESCE((Select ISNULL(SUM(TodaysInterest),0) from DailyInterest where IsPaid = 0 and CustomerProductId = @CustomerProductId and CreatedDate < @FirstInstallmentEndDate group by CustomerProductId),0))

							DECLARE @InterestToCredit decimal(18, 4)
							DECLARE @Total decimal(18, 4)

							SET @Total = @Balance + @TotalInterestTillDate + @RDAmount

							SET @InterestToCredit = @CurrentMaturity - @Total

						END

-- deduct amount of installment from Saving account and add transaction in saving account
						--Declare @diff decimal(18,4)
						--set  @diff=  @SavingUpdatedBalance - @RDAmount

						Declare @SavingTransactionId uniqueidentifier

						Exec SaveTransaction @SavingCustomerProductId, @CustomerId , @RDAmount,0, 2, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 2, @CustomerProductId,null,null, @SavingTransactionId OUTPUT

					--EXEC SaveTransactionSMS @SavingTransactionId

					-- add transaction of RD in Transaction
					
						--DECLARE @RDupdatedBalance decimal(18,4)
						--SET  @RDupdatedBalance=  @UpdatedBalance + @RDAmount

						Declare @RDTransactionId uniqueidentifier

						EXEC SaveTransaction @CustomerProductId, @CustomerId, @RDAmount, 0, 1, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 2, @SavingCustomerProductId,null,null, @RDTransactionId OUTPUT

						if(@ProductType = 10)
						BEGIN
							Declare @maturitydate as datetime
							set @maturitydate = CONVERT(char(10),DATEADD(MM,@NoOfMonthsORYears,@Date),126)
							EXEC SaveWealthCreatorTransaction @CustomerProductId,@PaymentType,@Date,@Amount,@maturitydate,@Date,null
						END
					--EXEC SaveTransactionSMS @RDTransactionId

					-- add transaction of Late Payment charges in Saving

						IF(@LatePaymentCharges is not null and @LatePaymentCharges > 0 and @PaymentType != 1)
						BEGIN

							--SET @diff = @diff - @LatePaymentCharges
					
							Exec SaveTransaction @SavingCustomerProductId, @CustomerId , @LatePaymentCharges, 0, 2, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 4, @CustomerProductId,null,null, @SavingTransactionId OUTPUT

						END

					--EXEC SaveTransactionSMS @SavingTransactionId

					-- Update balance of Saving account
					--UPDATE CustomerProduct SET UpdatedBalance = UpdatedBalance - @RDPaymentAmount, Balance = Balance - @RDPaymentAmount WHERE CustomerProductId = @SavingCustomerProductId

					-- Update balance of RD
					--UpdatedBalance = UpdatedBalance + @RDAmount, Balance = Balance + @RDAmount,
					UPDATE CustomerProduct SET  LastInstallmentDate = @Date, NextInstallmentDate = @PaymentDate WHERE CustomerProductId = @CustomerProductId

					-- Update flag of Pending RD installment
					Update RDPayment set IsPaid = 1, PaidDate = @Date WHERE RDPaymentId = @RDPaymentId

					-- Update flag of Pending Late payment charges
					Update RDPayment set IsPaid = 1, PaidDate = @Date WHERE RDPaymentType = 2 and IsDelete = 0 and CustomerProductId = @CustomerProductId and CreatedDate >= @FirstInstallmentStartDate and CreatedDate < @FirstInstallmentEndDate

					IF @PaymentType != 1 and @InterestToCredit > 0 and @InterestToCredit is not null
					BEGIN

						INSERT INTO DailyInterest (CustomerProductId, TodaysInterest,InterestRate, IsPaid, CreatedDate) values						(@CustomerProductId, @InterestToCredit, @InterestRate, 0, @Date)

					END

					DECLARE @RDInstallmentIntrest2 decimal(18, 2)

					EXEC RDAgentCommission @Amount, @OpeningDate, @Date, @ProductId, @RankId, @TimePeriod, @NoOfMonthsORYears, @RDInstallmentIntrest2 OUTPUT

					Update RDPayment set AgentCommission = @RDInstallmentIntrest2 WHERE RDPaymentId = @RDPaymentId

					EXEC ProductAgentCommission @Amount, @OpeningDate, @FirstInstallmentStartDate, @ProductId, @AgentId, @RankId, @TotalDays, @CustomerProductId, @RDPaymentId

					-- insert current pending installment in RDPayment
					INSERT INTO RDPayment (CustomerProductId, CustomerId, Amount, RDPaymentType, IsPaid, CreatedDate, AgentCommission, IsDelete) VALUES (@CustomerProductId, @CustomerId, @Amount, 1, 0, @Date, 0.00, 0)
				END
				ELSE
				BEGIN
				-- insert current pending installment in RDPayment
			
					INSERT INTO RDPayment (CustomerProductId, CustomerId, Amount, RDPaymentType, IsPaid, CreatedDate, AgentCommission, IsDelete)VALUES (@CustomerProductId, @CustomerId, @Amount, 1, 0, @Date, 0.00, 0)

					UPDATE CustomerProduct SET LastInstallmentDate = CONVERT(char(10), @Date,126), NextInstallmentDate = @PaymentDate WHERE CustomerProductId = @CustomerProductId
				END
        END
				ELSE 
				BEGIN
						-- if there is no pending installment then check balance in saving account if exist then deduct RD installment
						IF (@SavingBalance >= @Amount)
							BEGIN
				-- deduct amount of installment from Saving account and add transaction in saving account

							--Declare @diffcurrent decimal(18,4)
							--set  @diffcurrent=  @SavingUpdatedBalance - @Amount

							Declare @TransactionId uniqueidentifier
			
							Exec SaveTransaction @SavingCustomerProductId, @CustomerId, @Amount, 0, 2, null, @Date, null,null, 4,null,null, null, null, null, 0, @Date, null, null, 2, @CustomerProductId,null,null, @TransactionId OUTPUT

							--EXEC SaveTransactionSMS @TransactionId
			
							-- add transaction of RD in Transaction

							--DECLARE @RDupdatedBalanceCurrent decimal(18,4)
							--SET  @RDupdatedBalanceCurrent=  @UpdatedBalance + @Amount

		
							EXEC SaveTransaction @CustomerProductId, @CustomerId, @Amount, 0, 1, null, @Date, null, null, 4,null, null, null, null, null, 0, @Date, null, null, 2, @SavingCustomerProductId,null,null, @TransactionId OUTPUT

							if(@ProductType = 10)
						BEGIN
							Declare @maturitydate1 as datetime
							set @maturitydate1 = CONVERT(char(10),DATEADD(MM,@NoOfMonthsORYears,@Date),126)
							EXEC SaveWealthCreatorTransaction @CustomerProductId,@PaymentType,@Date,@Amount,@maturitydate1,@Date,null
						END
				--EXEC SaveTransactionSMS @TransactionId

				-- Update balance of Saving account
							--UPDATE CustomerProduct SET UpdatedBalance = UpdatedBalance - @Amount, Balance = Balance - @Amount WHERE 
							--CustomerProductId = @SavingCustomerProductId

							-- Update balance of RD
							--UpdatedBalance = UpdatedBalance + @Amount, Balance = Balance + @Amount, 
							UPDATE CustomerProduct SET LastInstallmentDate	= CONVERT(char(10), @Date,126), NextInstallmentDate = @PaymentDate WHERE CustomerProductId = @CustomerProductId

							DECLARE @RDInstallmentIntrest3 decimal(18, 2)

							EXEC RDAgentCommission @Amount, @OpeningDate, @Date, @ProductId, @RankId, @TimePeriod, @NoOfMonthsORYears, @RDInstallmentIntrest3 OUTPUT

							DECLARE @PaymentId uniqueidentifier

							SET @PaymentId = NEWID()
				
							INSERT INTO RDPayment (RDPaymentId, CustomerProductId, CustomerId, Amount, RDPaymentType, IsPaid, CreatedDate, AgentCommission, PaidDate, IsDelete) VALUES (@PaymentId, @CustomerProductId, @CustomerId, @Amount, 1, 1, @Date, @RDInstallmentIntrest3, @Date, 0)

							EXEC ProductAgentCommission @Amount, @OpeningDate, @Date, @ProductId, @AgentId, @RankId, @TotalDays, @CustomerProductId, @PaymentId
				
			END
			ELSE
			BEGIN
				-- insert current pending installment in RDPayment
			      INSERT INTO RDPayment (CustomerProductId, CustomerId, Amount, RDPaymentType, IsPaid, CreatedDate, AgentCommission, IsDelete) VALUES	(@CustomerProductId, @CustomerId, @Amount, 1, 0, @Date, 0.00, 0)

				  UPDATE CustomerProduct SET LastInstallmentDate = CONVERT(char(10), @Date,126), NextInstallmentDate = @PaymentDate WHERE CustomerProductId = @CustomerProductId
			END
		END

		--Deduct Mapped RD installment
		if exists (select * from CustomerProduct where ReferenceCustomerProductId = @CustomerProductId )
			BEGIN
				Exec DeductAmountofRDFromMappedProductByCPId @CustomerProductId,@Date
			END
		


	END

		
	  END TRY
	  BEGIN CATCH
            ROLLBACK TRANSACTION TRANSRD
			--Add by Vishal
			set @TranErrCnt = 1
	  END CATCH

	  COMMIT TRANSACTION TRANSRD

      FETCH NEXT FROM creditdeposit INTO @CustomerProductId, @CustomerId, @Balance, @UpdatedBalance, @Amount, @InterestRate, @DueDate ,@ProductId ,@RankId, @dd, @OpeningDate, @TimePeriod, @NoOfMonthsORYears, @PaymentType, @LastInstallmentDate, @TotalInstallment, @TotalDays, @InterestType, @NextInstallmentDate, @OpeningBalance, @SkipFirstInstallment, @AgentId,@ProductType

    END
  END
  CLOSE creditdeposit
  DEALLOCATE creditdeposit
  -- Add by Vishal
  if @TranErrCnt = 0
  begin
  --declare @Date datetime
  --set @Date = '2022-05-11 00:00:00.000'
	update DailyProcess_Console_Log set DailyProcessDate = @Date where DailyProcessCode = '002'
  end
  SET NOCOUNT OFF
END


--select * from DailyProcess_Console_Log where DailyProcessCode = '002'