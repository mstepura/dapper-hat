# dapper-hat - a data access library with integration testing capabilities built on top of dapper-dot-net
The main idea behind this library is to test data access code against real MS SQL database objects (tables, functions, stored procedures), without dependency on data,
and check that query can be executed and it's interface - parameters and recordset structure - matches .NET mapping code.

## What dapper-hat is testing
* With dapper-hat tests run data layer code and execute (almost) any stored procedure, query or function with the help of `SET FMTONLY ON` and `sys.dm_exec_describe_first_result_set` function.
SQL Server validates any type of DML operation on a real SQL Server database with, no mocks, quickly and without changing database state. 
Existance of all participating objects and fields is verified, as well as that query or stored procedure can be compiled and plan is generated.
However, database state is not changed and the query executes almost instantaneously.
* Validate input and output parameters, their data types, as well as input parameter values.
* Validate output dataset structures and their mapping to classes. It will capture situations like:
 * field exists in data transfer object, but not returned by query,
 * SQL data type does not match mapped .NET property, e.g. int/boolean, long/int

## What dapper-hat is NOT testing
* Semantics of SQL queries and procedures. The test verifies structure of the result, not the result itself.
* What parameter values are used to call queries. This should be done by code unit tests.

## Why 
Often, when database state is present explicitly or implicitly in the test, we are in fact trying to fit several test types into one:
* code unit test (no external state)
* integration test (code-to-database)
* database unit test
* functional test
Each of these test types has its own scope, and when mixing them we are exploding the scope of our 'combined' test, making database refactoring or schema changes costly and painful.
dapper-hat is addressing integration test problem (code-to-database), thus keeping test isolation from external state and other side effects except code-to-database interaction.

## How

### Stored procedure
```sql
CREATE PROCEDURE dbo.GetThisSingleInteger
	@input1 int,
	@input2	int,
	@output bigint out
as
	set @output = @input1 + input2
go
```

### Data access layer code
```csharp
public class SampleDataService : ISampleDataService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    public SampleDataService(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
  
    public async Task<long> GetThisSingleInteger(int input1, int input2)
    {
        var prm = _connectionFactory.CreateParameters();
        prm.Add(name: "@input1", value: input1, dbType: DbType.Int32);
        prm.Add(name: "@input2", value: input2, dbType: DbType.Int32);
        prm.AddOutput(name: "@output", dbType: DbType.Int64);
  
        using (var connection = await _connectionFactory.Create())
        {
            await connection.Execute(sql: "dbo.GetThisSingleInteger", param: prm);
            return prm.Get<long?>("@output") ?? 0L;
        }
    }
}
```

### Test
```csharp
    [Test]
    public void GetThisSingleInteger()
    {
        IDatabaseConnectionFactory connectionFactory = new ValidatingDatabaseConnectionFactory(ConnectionString);
        var sut = new SampleDataService(connectionFactory);
        Assert.DoesNotThrowAsync(() => _sut.GetThisSingleInteger(1, 2));
    }
```

### Stored procedure change will make test fail
```sql
CREATE PROCEDURE dbo.GetThisSingleInteger
	@input1 int,
	@input2	int,
	@output int out -- changed from bigint to int
as
	set @output = @input1 + input2
go
```
