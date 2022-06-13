using Antlr4.Runtime.Atn;
using Cova.Compiler.Parser;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Cova.LanguageServer;

public sealed class TextDocumentSyncHandler : TextDocumentSyncHandlerBase
{
	public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
	{
		return new TextDocumentAttributes(uri, LanguageConstants.LanguageId);
	}

	protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
	{
		return new TextDocumentSyncRegistrationOptions(TextDocumentSyncKind.Incremental)
		{
			DocumentSelector = new DocumentSelector(
				DocumentFilter.ForLanguage(LanguageConstants.LanguageId),
				DocumentFilter.ForPattern("**/" + LanguageConstants.ProjectFileName)
			),
			Save = new SaveOptions { IncludeText = true }
		};
	}

	public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
	{
		var documentUri = request.TextDocument.Uri;

		if (Path.GetFileName(documentUri.Path) == LanguageConstants.ProjectFileName)
		{
			// handle project file
		}
		else
		{
			// handle source
			var parser = new CovaParserExtended(request.TextDocument.Text, request.TextDocument.Uri.GetFileSystemPath())
			{
				Interpreter = { PredictionMode = PredictionMode.SLL },
				BuildParseTree = false
			};
			var file = parser.compilationUnit();
		}

		return Unit.Task;
	}

	public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
	{
		foreach (var textDocumentContentChangeEvent in request.ContentChanges)
		{
			
		}

		return Unit.Task;
	}

	public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
	{
		return Unit.Task;
	}

	public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
	{
		return Unit.Task;
	}
}