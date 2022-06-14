import { ExtensionContext } from 'vscode';
import { Executable, ExecutableOptions, LanguageClient, LanguageClientOptions, ServerOptions, TransportKind } from 'vscode-languageclient/node';

const options: ExecutableOptions = { cwd: "/Users/nick/Dev/covalang/compiler-dup/src/Cli" };
const executable: Executable = { command: "dotnet", args: "run -c Release -- serve stdio".split(' '), options }
const serverOptions: ServerOptions = { run: executable, debug: executable };
const clientOptions: LanguageClientOptions = { documentSelector: [ 'plaintext' ] };
const client: LanguageClient = new LanguageClient('cova-language-server', 'Cova Language Server', serverOptions, clientOptions );
client.registerProposedFeatures();

export function activate(context: ExtensionContext): Promise<void> { return client.start(); }
export function deactivate(): Thenable<void> | undefined { return client.stop(); }