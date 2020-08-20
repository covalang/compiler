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
	protected override SpecialTokenTypes GetTokenTypes() => (Newline, (Tab, Indent, Dedent), (Arrow, SingleLineEnd), (LeftBrace, RightBrace));
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
Delegate: 'delegate';

Field: 'field';
Prop: 'prop';
Func: 'func';
Local: 'local';

// Storage types

StaticStorageType : '$';
//InstanceStorageType : '|';

// Keywords/tokens

//Boolean

True: 'true';
False: 'false';
BooleanLiteral: True | False;

And: 'and' | '∧' | '/\\';
Or: 'or' | '∨' | '\\/';
Not: 'not' | '¬';

Equal: '==';
NotEqual: '!=';
Within: '><';
Without: '<>';
LessThanOrEqual: '<=' | '≤';
LessThan: '<';
GreaterThanOrEqual: '>=' | '≥';
GreaterThan: '>';

Is: 'is';
Isnt: 'isnt';

// Bitwise

Ampersand: '&';
VerticalBar: '|';
Caret: '^';

TripleLeftChevron: '<<<';
DoubleLeftChevron: '<<';
// LeftChevron: '<';

TripleRightChevron: '>>>';
DoubleRightChevron: '>>';
// RightChevron: '>';

PlusMinus: '±' | '+-';
Plus: '+';
Minus: '-';
Multiply: '*';
Divide: '/';
Modulo: '%';
Power: '**';
Root: '//' | '√';
Octothorp: '#';
Tilde: '~';


TripleDot: '...';
DoubleDot: '..';
Dot: '.';

EqualsSign: '=';

LeftParenthesis: '(';
RightParenthesis: ')';
LeftBracket: '[';
RightBracket: ']';
DoubleColon: '::';
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
	: [_A-Za-z][_A-Za-z0-9]*
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