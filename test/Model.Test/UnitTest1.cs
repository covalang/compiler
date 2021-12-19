using System;
using System.IO;
using System.Linq;
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
        const String filename = "test.db";
        if (File.Exists(filename))
            File.Delete(filename);
        using var connection = new SqliteConnection($"Data Source={filename}");
        connection.Open();
        using var context = new Context(connection);
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw("pragma auto_vacuum = full;");

        {
            var ds = new DefinitionSource(new("path"), new("path"));
            var package = new Package(ds, "package");
            var module = new Module(ds, "module");
            package.Modules.Add(module);
            context.Packages.Add(package);
            context.Modules.Add(module);
            context.SaveChanges();
        }
        {
            //var symbols = context.Symbols.ToList();
        }

        var functions = context.Functions.ToQueryString();
    }

    [Fact]
    public void Duplicate_namespace_throws()
    {
        const String filename = "test.db";
        if (File.Exists(filename))
            File.Delete(filename);
        using var connection = new SqliteConnection($"Data Source={filename}");
        connection.Open();
        using var context = new Context(connection);
        context.Database.EnsureCreated();
        var ns1 = new Namespace(new (new("path"), new("path")), "namespace1");
        var ns2 = new Namespace(new (new("path"), new("path")), "namespace2");
        ns2.ParentNamespace = ns1;
        var ns3 = new Namespace(new (new("path"), new("path")), "namespace2");
        ns3.ParentNamespace = ns1;
        context.AddRange(ns1, ns2, ns3);
        Assert.Throws<DbUpdateException>(() => context.SaveChanges());
    }
}