ALTER PROCEDURE dbo.GetThisSingleInteger
	@input1 int,
	@input2	nvarchar(50),
	@output int out
as
	set @output = (
		select value
		from dbo.[Some Non Existing Table]
	)
	print 'hello!'
go

