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

Public: '+';
Private: '-';
Protected: '#';
Internal: '~';

True: 'true';
False: 'false';

Dot: '.';
EqualsSign: '=';

// Lexical rules

Identifier
	: [_A-Za-z][_A-Za-z0-9]*
	//: IdentifierStartCharacter IdentifierPartCharacter*
	;