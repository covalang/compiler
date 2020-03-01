lexer grammar IndentationLexer;

@lexer::header {
	#include "IndentationLexerBase.hpp"
	typedef IndentationLexerBase_<> IndentationLexerBase;
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

NewLine
	: ( '\r'? '\n' | '\r' )
	{ foundNewLine(); }
	;

Indentations
	: '\t'+
	{ foundIndentations(); }
	;

Whitespace: ' '+;