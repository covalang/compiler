lexer grammar CovaLexer;

tokens { Indent, Dedent, SingleLineEnd }

@header {
	#pragma warning disable 3021
	#pragma warning disable 108

	//using System.Collections.Generic;
}

@lexer::members {
	private readonly DentHelper dentHelper = new DentHelper(Newline, Tab, Indent, Dedent);
	public override IToken NextToken() => dentHelper.NextTokenWithIndentation(base.NextToken);
}

// Keywords

Use: 'use';
Namespace: 'namespace';

Type: 'type';
Enum: 'enum';
Class: 'class';
Struct: 'struct';
Interface: 'interface';
Trait: 'trait';
//Delegate: 'delegate'; NOTE: use 'type func' to define a Function Type https://en.wikipedia.org/wiki/Function_type

Field: 'field';
Prop: 'prop';
Func: 'func';
Local: 'local';

// Storage types

StaticStorageType : '$';
//InstanceStorageType : '|';

//Boolean

True: 'true';
False: 'false';
BooleanLiteral: True | False;

And: 'and' | '∧';
Or: 'or' | '∨';
Not: 'not' | '¬';

// Equality

Equal: '==';
NotEqual: '!=';

// Relational

LessThanOrEqual: '≤';
GreaterThanOrEqual: '≥';

Is: 'is';
Isnt: 'isnt';

// Bitwise

Ampersand: '&';
VerticalBar: '|';
Exclamation: '!';
Caret: '^';

LeftChevron: '<';
RightChevron: '>';

PlusMinus: '±';
Plus: '+';
Minus: '-';
Asterisk: '*';
Slash: '/';
Backslash: '\\';
Percent: '%';
Root: '√';
Octothorp: '#';
Tilde: '~';


Dot: '.';

Equals: '=';

LeftParenthesis: '(';
RightParenthesis: ')';
LeftBracket: '[';
RightBracket: ']';
Colon: ':';
SemiColon: ';';
Comma: ',';

// Sets

// Membership (formal)
EmptySet: '∅';
Membership: '∈';
NonMembership: '∉';

// Subset (formal)
Subset: '⊆';
Superset: '⊇';
ProperSubset: '⊂' | '⊊';
ProperSuperset: '⊃' | '⊋';

// Operations (formal)
Union: '∪';
Intersection: '∩';

// Lexical rules

Identifier
	: [_A-Za-z][_A-Za-z0-9-]*
	//: IdentifierStartCharacter IdentifierPartCharacter*
	;


IntegerLiteral: DecimalPrefix? IntegerDigit (DigitSeparator | IntegerDigit)*;
RealLiteral: DecimalPrefix? IntegerLiteral+ Dot IntegerLiteral+;

fragment DigitSeparator: '_';
fragment IntegerDigit: [0-9];
fragment HexadecimalDigit: [0-9a-fA-F];
fragment BinaryDigit: [01];

fragment DecimalPrefix: '0d';
fragment HexadecimalPrefix: '0x';
fragment BinaryPrefix: '0b';
fragment UnicodePrefix: '0u';

StringLiteral: '"' (EscapedCharacter | ~'"') '"';
CharacterLiteral: '\'' (EscapedCharacter | ~'\'') '\'';

fragment EscapedCharacter
	: '\\' [0\\tnr"']
	| '\\x' HexadecimalDigit HexadecimalDigit
	| '\\u' '{' HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit '}'
	| '\\u' '{' HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit '}'
	;

Arrow: '->';
FatArrow: '=>';

LeftBrace: '{';
RightBrace: '}';

// Whitespace

//Space: ' ';

Newline: '\r\n' | '\r' | '\n';
Tab: '\t';
Whitespace: [\p{Z}];