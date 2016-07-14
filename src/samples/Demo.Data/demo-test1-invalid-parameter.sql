ALTER PROCEDURE dbo.GetThisSingleInteger
	@input1 int,
	@input2	nvarchar(50),
	@input3 int,
	@output int out
as
	set @output = (
		select value
		from (values (35)) v(value)
	)
	print 'hello!'
go

