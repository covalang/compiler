lexer grammar IndentationLexer;

options {
	superClass = IndentationLexerBase;
}

@lexer::members {
	public override IToken NextToken() => NextTokenWithIndentation(NewLine, Indent, Dedent);
}

@header {
	#pragma warning disable 3021
}

tokens { Indent, Dedent }

NewLine
	: ( '\r'? '\n' | '\r' )
	{ FoundNewLine(Hidden); }
	;

Indentations
	: '\t'+
	{ FoundIndentations(Hidden); }
	;