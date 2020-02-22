lexer grammar CovaLexer;


@lexer::header {
	#include "IndentationLexerBase.hpp"
	typedef IndentationLexerBase<> IndentationLexerBase_;
}

@lexer::members {
	virtual std::unique_ptr<antlr4::Token> nextToken() override {
		return nextTokenWithIndentation<Indent, Dedent, NewLine>();
	}
}

options {
	superClass = IndentationLexerBase_;
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