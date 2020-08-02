lexer grammar CovaLexer;

options {
	superClass = NewlineModeLexerBase;
}

tokens { Indent, Dedent, SingleLineEnd }

@header {
	#pragma warning disable 3021
	#pragma warning disable 108

	//using System.Collections.Generic;
}

@lexer::members {
	protected override SpecialTokenTypes Tokens { get; } =
		new SpecialTokenTypes(Newline, (Tab, Indent, Dedent), (Arrow, SingleLineEnd), (LeftBrace, RightBrace));
}

// Keywords

Use: 'use';
Namespace: 'namespace';

Type: 'type';
Enum: 'enum';
// Class: 'class';
Struct: 'struct';
Interface: 'interface';
Trait: 'trait';
Delegate: 'delegate';

Field: 'field';
Prop: 'prop';
Func: 'func';
Local: 'local';

// Storage types

StaticStorageType : '$';
InstanceStorageType : '|';

// Keywords/tokens

True: 'true';
False: 'false';

LessThanOrEqual: '<=';
GreaterThanOrEqual: '>=';
LessThan: '<';
GreaterThan: '>';
NotEqual: '!=';
Equal: '==';
And: 'and';
Or: 'or';

Power: '**';
Minus: '-';
Not: 'not';
Multiply: '*';
Divide: '/';
Modulo: '%';
Plus: '+';
Octothorp: '#';
Tilde: '~';

Dot: '.';
EqualsSign: '=';

LeftParenthesis: '(';
RightParenthesis: ')';
LeftBracket: '[';
RightBracket: ']';
SemiColon: ';';
Comma: ',';

// Lexical rules

Identifier
	: [_A-Za-z][_A-Za-z0-9]*
	//: IdentifierStartCharacter IdentifierPartCharacter*
	;

IntegerLiteral: [0-9][0-9_]*;

StringLiteral: '"' ('\\"' | ~'"')* '"';

Arrow: '->';

LeftBrace: '{';
RightBrace: '}';

// Whitespace

//Space: ' ';

Newline: '\r\n' | '\r' | '\n';
Tab: '\t';
Whitespace: [\p{Z}];