if object_id('dbo.GetThisSingleInteger') is not null
	EXEC ('drop procedure dbo.GetThisSingleInteger')
GO
if object_id('dbo.GetComplexTypes') is not null
	EXEC ('drop procedure dbo.GetComplexTypes')
GO
if object_id('dbo.GetWithTableValuedParameter') is not null
	EXEC ('drop procedure dbo.GetWithTableValuedParameter')
GO
if object_id('dbo.GetMultipleResultSets') is not null
	EXEC ('drop procedure dbo.GetMultipleResultSets')
GO
if exists(select * from sys.types where name = 't_IntegerList')
	EXEC ('DROP TYPE [dbo].[t_IntegerList]')
GO
EXEC('CREATE TYPE [dbo].[t_IntegerList] AS TABLE (Id INT)')
GO


CREATE PROCEDURE dbo.GetThisSingleInteger
	@input1 int,
	@input2	nvarchar(50),
	@output int out
as
	set @output = (
		select value
		from (values (35)) v(value)
	)
	print 'hello!'
go


CREATE PROCEDURE dbo.GetComplexTypes
	@dt datetime
as
	select
		cast(Field1 as smallint) Field1,
		cast(Field2 as bigint) Field2,
		cast(Field3 as datetime) Field3,
		cast(Field4 as nvarchar(128)) Field4,
		cast(FieldBoolean as bit) FieldBoolean
	from (values
		(1, 2, getdate(), 'hello', 1)
	) v(Field1, Field2, Field3, Field4, FieldBoolean)
GO


CREATE PROCEDURE dbo.GetWithTableValuedParameter
	@filters [dbo].[t_IntegerList] READONLY
AS
	select
		cast(Field1 as smallint) Field1,
		cast(Field2 as bigint) Field2,
		cast(Field3 as datetime) Field3,
		cast(Field4 as nvarchar(128)) Field4,
		cast(FieldBoolean as bit) FieldBoolean
	from (values
		(1, 2, getdate(), 'hello', 1)
	) v(Field1, Field2, Field3, Field4, FieldBoolean)
GO


CREATE PROCEDURE dbo.GetMultipleResultSets
AS
	select 1 as [Result1]

	select 'hello' as [Result2]
GO
