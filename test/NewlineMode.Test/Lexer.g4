lexer grammar Lexer;

options {
	superClass = NewlineModeLexerBase;
}

tokens { Indent, Dedent, SingleLineEnd }

@header {
	#pragma warning disable 3021
	#pragma warning disable 108
}

@lexer::members {
	protected override SpecialTokenTypes Tokens { get; } =
		new SpecialTokenTypes(Newline, (Tab, Indent, Dedent), (Arrow, SingleLineEnd), (LeftBrace, RightBrace));
}

Identifier: [A-Z];

Arrow: '->';

LeftBrace: '{';
RightBrace: '}';

Newline: '\r\n' | '\r' | '\n';
Tab: '\t';

Whitespace: [\p{Z}];