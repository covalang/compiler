lexer grammar CovaLexer;

import IndentationLexer;

options {
	superClass = IndentationLexerBase;
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
Func: 'func';

// Visibility modifiers

PublicVisibility: Plus;
PrivateVisibility: Minus;
ProtectedVisibility: Octothorp;
InternalVisibility: Tilde;

// Storage types

StaticStorageType : '$';
InstanceStorageType : '|';

// Keywords/tokens

True: 'true';
False: 'false';

Arrow: '->';

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
LeftBrace: '{';
RightBrace: '}';
LeftBracket: '[';
RightBracket: ']';
SemiColon: ';';
Comma: ',';

Space: ' ';

// Lexical rules

Identifier
	: [_A-Za-z][_A-Za-z0-9]*
	//: IdentifierStartCharacter IdentifierPartCharacter*
	;