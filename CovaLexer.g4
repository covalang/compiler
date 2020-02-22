lexer grammar CovaLexer;

options {
	superClass = IndentationLexerBase_;
}

@lexer::header {
	#include "IndentationLexerBase.hpp"
	typedef IndentationLexerBase<> IndentationLexerBase_;
}

@lexer::declarations {
	virtual std::unique_ptr<antlr4::Token> nextToken() override;
}

@lexer::definitions {
	std::unique_ptr<antlr4::Token> CovaLexer::nextToken() {
		return nextTokenWithIndentation<Indent, Dedent, NewLine>();
	}
}

tokens { Indent, Dedent }

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

NewLine
	: ( '\r'? '\n' | '\r' )
	{ foundNewLine(); }
	;

Whitespace: ' '+;

Indentations
	: '\t'+
	{ foundIndentations(); }
	;