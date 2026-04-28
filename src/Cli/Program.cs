using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using NoAlloq;

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