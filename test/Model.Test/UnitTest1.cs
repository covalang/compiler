using Cova.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Model.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var context = new Context(connection);
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw("pragma auto_vacuum = full;");
    }
}