using System.Reflection;
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using NoAlloq;
// using static CmdLn;

var sc = new ServiceCollection();

//ConsoleApp.ServiceProvider = sc.BuildServiceProvider();
var cab = ConsoleApp.Create();
cab.ConfigureServices((cac, services) =>
{
	services.AddSingleton("World");
});
cab.Add<Root>();
cab.Add<Root.Package>("package");
await cab.RunAsync(args);

public sealed class Root(String s)
{
	/// <summary>Run a project.</summary>
	public async Task Run(CancellationToken ct)
	{
		Console.WriteLine($"Hello, {s}!");
	}

	/// <summary>Build a project.</summary>
	public void Build() {}

	/// <summary>Clean a project.</summary>
	public void Clean() {}

	/// <summary>Run tests.</summary>
	public void Test() {}

	/// <summary>Run language server.</summary>
	public void Serve() {}

	public sealed class Package
	{
		/// <summary>Install a package.</summary>
		public void Install() {}

		/// <summary>Uninstall a package.</summary>
		public void Uninstall() {}

		/// <summary>Update a package.</summary>
		public void Update() {}

		/// <summary>Restore packages.</summary>
		public void Restore() {}
	}
}

// CmdLn.Run(args, [
// 	Cmd("run", 'r', "Run a project", () => Console.WriteLine("Run")),
// 	Cmd("build", 'b', "Build a project", () => Console.WriteLine("Build")),
// 	Cmd("clean", 'c', "Clean a project", () => Console.WriteLine("Clean")),
// 	Cmd("test", 't', "Run tests", () => Console.WriteLine("Test")),
// 	Cmd("serve", 's', "Run language server", [
// 		SubCmd("stdio", 's', "Use stdin/stdout for LSP communication", () => Console.WriteLine("Serve")),
// 		SubCmd("pipe", 'p', "Use a named pipe for LSP communication", [
// 			Param<String>("name", 'n', "Name of the pipe", () => Console.WriteLine("Serve"))
// 		]),
// 		SubCmd("socket", 's', "Use a TCP port to connect for LSP communication", [
// 			Param<Int32>("port", 'p', "TCP port number", () => Console.WriteLine("Serve"))
// 		])
// 	]),
// 	new("restore", 'r', "Restore packages", () => Console.WriteLine("Restore"))
// ]);
//
// // Commands commands = new()
// // {
// // 	["run", 'r', "Run a project"] = () => {},
// // 	["build", 'b', "Build a project"] = () => {},
// // 	["clean", 'c', "Clean a project"] = () => {},
// // 	["test", 't', "Run tests"] = () => {},
// // 	["serve", 's', "Run language server"] = new()
// // 	{
// // 		["stdio", 's', "Use stdin/stdout for LSP communication"] = () => {},
// // 		["pipe", 'p', "Use a named pipe for LSP communication"] = new()
// // 		{
// // 			["name"] = () => {}
// // 		},
// // 		["socket", 's', "Use a TCP port to connect for LSP communication"] = new()
// // 		{
// // 			["port"] = () => {}
// // 		}
// // 	},
// // 	["restore", 'r', "Restore packages"] = () => {}
// // };
//
// // Dictionary<Foo, Bar> handlers = new()
// // {
// // 	["run"] = new(),
// // 	["build"] = new(),
// // 	["clean"] = new(),
// // 	["test"] = new(),
// // 	["serve"] = new(),
// // 	["restore"] = new(),
// // 	["serve stdio"] = new(),
// // 	["serve pipe"] = new(),
// // 	["serve socket"] = new(),
// // };
// //
// // 	() => {},
// // 	(String name, Int32 count) => {},
// // ];
//
// // Command[] commands = [
// // 	new('n', "new", "Create a new project"),
// // 	new('r', "run", "Run a project"),
// // 	new('b', "build", "Build a project"),
// // 	new('c', "clean", "Clean a project"),
// // 	new('t', "test", "Run tests"),
// // 	new('s', "serve", "Run language server", SubCommands: [
// // 		new('s', "stdio", "Use stdin/stdout for LSP communication"),
// // 		new('p', "pipe", "Use a named pipe for LSP communication", Arguments: [
// // 			new Argument<String>("name", "Name of the pipe")
// // 		]),
// // 		new('s', "socket", "Use a TCP port to connect for LSP communication", Arguments: [
// // 			new Argument<UInt16>("port", "TCP port number")
// // 		])
// // 	]),
// // 	new('r', "restore", "Restore packages")
// // ];
//
// // var command = commands.Where(x => x.ShortName == args[0]?[0] || x.LongName == args[0]);
//
// interface ICliParser
// {
// 	static abstract ReadOnlySpan<Command> Commands { get; }
// 	static abstract
// }
//
// readonly struct Commands
// {
// 	private readonly Dictionary<String, (String description, Delegate command)> longNameCommandMap = new();
// 	private readonly Dictionary<Char, (String description, Delegate command)> shortNameCommandMap = new();
//
// 	public Commands() {}
//
// 	public Delegate this[String longName, String description]
// 	{
// 		set => longNameCommandMap[longName] = (description, value);
// 	}
//
// 	public Delegate this[String longName, Char shortName, String description]
// 	{
// 		set
// 		{
// 			longNameCommandMap[longName] = (description, value);
// 			shortNameCommandMap[shortName] = (description, value);
// 		}
// 	}
// }
//
// struct Arguments
// {
// 	private Dictionary<String, Argument> longNameArgumentMap = new();
// 	private Dictionary<Char, Argument> shortNameArgumentMap = new();
//
// 	public Arguments() {}
//
// 	public Argument this[String longName] { set => longNameArgumentMap[longName] = value; }
//
// 	public Argument this[String longName, Char shortName]
// 	{
// 		set
// 		{
// 			longNameArgumentMap[longName] = value;
// 			shortNameArgumentMap[shortName] = value;
// 		}
// 	}
// }
//
// struct Foo
// {
// 	private Foo(String longName, Char? shortName) => (LongName, ShortName) = (longName, shortName);
// 	public Char? ShortName { get; set; }
// 	public String LongName { get; set; }
//
// 	public static implicit operator Foo(String longName) => new(longName, null);
// 	public static implicit operator Foo((String longName, Char shortName) names) => new(names.longName, names.shortName);
// }
//
// sealed record Command(
// 	String LongName,
// 	Char? ShortName,
// 	String Description,
// 	Commands? SubCommands = null,
// 	Arguments? Arguments = null
// ) : ICommand;
//
// sealed record Argument<T>(String Name, String Description) : Argument2(Name, Description);
// abstract record Argument2(String Name, String Description);
//
// interface ICommand
// {
// 	public String LongName { get; }
// 	public Char? ShortName { get; }
// 	public String Description { get; }
// 	public Commands? SubCommands { get; }
// 	public Arguments? Arguments { get; }
// }
//
// static class CmdLn
// {
// 	public static Int32 Run(String[] args, ReadOnlySpan<Cmd> commands)
// 	{
// 		if (args.Length == 0)
// 		{
// 			Console.WriteLine("No command specified");
// 			return 1;
// 		}
//
// 		var command = commands.FirstOrDefault(x => x.ShortName == args[0]?[0] || x.LongName == args[0]);
// 		if (command is null)
// 		{
// 			Console.WriteLine("Command not found");
// 			return 1;
// 		}
//
// 		return 0;
// 	}
//
// 	public static Cmd Cmd(String longName, Char? shortName, String description, Delegate command) =>
// 		new(longName, shortName, description, command);
//
// 	public static Cmd Cmd(String longName, Char? shortName, String description, ReadOnlySpan<Cmd> subCommands) =>
// 		new(longName, shortName, description, subCommands);
//
// 	public static Cmd SubCmd(String longName, Char? shortName, String description, Delegate command) =>
// 		new(longName, shortName, description, command);
//
// 	public static Cmd SubCmd(String longName, Char? shortName, String description, ReadOnlySpan<Cmd> subCommands) =>
// 		new(longName, shortName, description, subCommands);
//
// 	public static Cmd Param<T>(String name, Char? shortName, String description, Delegate command) =>
// 		new(name, shortName, description, command);
//
// 	public static Cmd Param<T>(String name, Char? shortName, String description) =>
// 		new(name, shortName, description);
// }
//
// readonly struct Cmd
// {
// 	private Cmd(String longName, Char? shortName, String description)
// 	{
// 		LongName = longName;
// 		ShortName = shortName;
// 		Description = description;
// 	}
//
// 	public Cmd(String longName, Char? shortName, String description, Delegate command)
// 		: this(longName, shortName, description) => Command = command;
//
// 	public Cmd(String longName, Char? shortName, String description, ReadOnlySpan<Cmd> subCommands)
// 		: this(longName, shortName, description) => SubCommands = subCommands;
//
// 	public String LongName { get; }
// 	public Char? ShortName { get; }
// 	public String Description { get; }
// 	public Delegate? Command { get; }
// 	public ReadOnlySpan<Cmd> SubCommands { get; }
// }
//