ALTER PROCEDURE dbo.GetComplexTypes
	@dt datetime
as
	select
		cast(Field1 as smallint) Field1,
		cast(Field2 as bigint) Field2,
		cast(Field3 as datetime) Field3,
		cast(Field4 as nvarchar(128)) Field4,
		45 AS FieldBoolean
	from (values
		(1, 2, getdate(), 'hello', 1)
	) v(Field1, Field2, Field3, Field4, FieldBoolean)
GO
