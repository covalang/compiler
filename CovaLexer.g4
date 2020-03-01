lexer grammar CovaLexer;

import IndentationLexer;

options {
	superClass = IndentationLexerBase;
}

// Keywords

Use: 'use';
Module: 'module';
Type: 'type';
Func: 'func';
True: 'true';
False: 'false';

Dot: '.';
EqualsSign: '=';

// Lexical rules

Identifier
	: [_A-Za-z][_A-Za-z0-9]+
	//: IdentifierStartCharacter IdentifierPartCharacter*
	;