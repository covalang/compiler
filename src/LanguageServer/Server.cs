using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;

namespace Cova.LanguageServer;

public sealed class Server : IDisposable
{
	static Server()
	{
		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
			.MinimumLevel.Verbose()
			.CreateLogger();
	}
	
	private readonly OmniSharp.Extensions.LanguageServer.Server.LanguageServer languageServer;

	public Server(Stream input, Stream output, IDisposable? disposeWhenDone = null)
	{
		languageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer.PreInit(options =>
		{
			options
				.WithInput(input)
				.WithOutput(output)
				.WithHandler<TextDocumentSyncHandler>()
				.ConfigureLogging(
					x => x
						.AddSerilog(Log.Logger)
						.AddLanguageProtocolLogging()
						.SetMinimumLevel(LogLevel.Debug)
				)
				;

			if (disposeWhenDone != null)
				options.RegisterForDisposal(disposeWhenDone);
		});
	}
	
	public async Task<Int32> RunAsync(CancellationToken cancellationToken)
	{
		Log.Information("Starting server...");
		await languageServer.Initialize(cancellationToken);
		Log.Information("Started server");
		await languageServer.WaitForExit;
		Dispose();
		return 0;
		// await languageServer.WaitForExit.WaitAsync(cancellationToken);
		// if (!languageServer.WaitForExit.IsCanceled)
		// 	return 0;
		// languageServer.ForcefulShutdown();
		// return 1;
	}

	private Int32 disposed;
	public void Dispose()
	{
		if (Interlocked.Exchange(ref disposed, 1) == 1)
			throw new ObjectDisposedException(typeof(Server).FullName);
		Log.Information("Stopping server...");
		languageServer.Dispose();
		Log.Information("Stopped server");
	}
}