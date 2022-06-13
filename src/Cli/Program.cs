using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using Cova.Cli;
using Cova.LanguageServer;

var root = new RootCommand("Cova programming language");
var ctvs = new ValueSource<CancellationToken>();
var @new = new Command("new", "Create a new project");
var run = new Command("run", "Run a project");
var build = new Command("build", "Build a project");
var clean = new Command("clean", "Clean a project");
var test = new Command("test", "Run tests");
var serve = new Command("serve", "Run language server");
var restore = new Command("restore", "Restore packages");
foreach (var command in new[] { @new, run, build, clean, test, serve, restore })
	root.AddCommand(command);

var stdIo = new Command("stdio", "Use stdin/stdout for LSP communication");
var pipe = new Command("pipe", "Use a named pipe for LSP communication");
var socket = new Command("socket", "Use a TCP port to connect for LSP communication");
foreach (var command in new[] { stdIo, pipe, socket })
	serve.AddCommand(command);

stdIo.SetHandler(HandleStdIo, ctvs);

var pipeName = new Argument<String>("name", "Name of the pipe");
pipe.AddArgument(pipeName);
pipe.SetHandler(HandleNamedPipe, pipeName, ctvs);

var portNumber = new Argument<UInt16>("port", "TCP port number");
socket.AddArgument(portNumber);
socket.SetHandler(HandleSocket, portNumber, ctvs);

return await new CommandLineBuilder(root).UseDefaults().Build().InvokeAsync(args);


static async Task<Int32> HandleStdIo(CancellationToken cancellationToken)
{
	var input = Console.OpenStandardInput();
	var output = Console.OpenStandardOutput();
	return await new Server(input, output).RunAsync(cancellationToken);
}

static async Task<Int32> HandleNamedPipe(String pipeName, CancellationToken cancellationToken)
{
	const String pipePrefix = @"\\.\pipe\";
	if (pipeName.StartsWith(pipePrefix))
		pipeName = pipeName[pipePrefix.Length..]; // VSCode on Windows prefixes the pipe with \\.\pipe\
	var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
	pipe.Connect();
	return await new Server(pipe, pipe, pipe).RunAsync(cancellationToken);
}

static async Task<Int32> HandleSocket(UInt16 portNumber, CancellationToken cancellationToken)
{
	var tcp = new TcpClient();
	tcp.Connect(IPAddress.Loopback, portNumber);
	var stream = tcp.GetStream();
	return await new Server(stream, stream, tcp).RunAsync(cancellationToken);
}