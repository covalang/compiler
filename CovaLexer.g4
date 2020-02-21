lexer grammar CovaLexer;

tokens { INDENT, DEDENT }

@lexer::header {
#include <string>
#include <stack>
#include <queue>
}

@lexer::members {
private:
	// Initializing `pendingDent` to true means any whitespace at the beginning
	// of the file will trigger an INDENT, which will probably be a syntax error,
	// as it is in Python.
	bool pendingDent = true;
	int indentCount = 0;
	std::queue<std::unique_ptr<antlr4::Token>> tokenQueue;
	std::stack<int> indentStack;
	std::unique_ptr<antlr4::Token> initialIndentToken = nullptr;
	int getSavedIndent() { return indentStack.size() == 0 ? 0 : indentStack.top(); }

	std::unique_ptr<antlr4::CommonToken> createToken(int type, std::string text, std::unique_ptr<antlr4::Token> const & next)
	{
		auto token = std::make_unique<antlr4::CommonToken>(type, text);
		if (nullptr != initialIndentToken) {
			token->setStartIndex(initialIndentToken->getStartIndex());
			token->setLine(initialIndentToken->getLine());
			token->setCharPositionInLine(initialIndentToken->getCharPositionInLine());
			token->setStopIndex(next->getStartIndex() - 1);
		}
		return token;
	}

public:
	virtual std::unique_ptr<antlr4::Token> nextToken() override
	{
		// Return tokens from the queue if it is not empty.
		if (tokenQueue.size() != 0) {
			auto result = std::move(tokenQueue.front());
			tokenQueue.pop();
			return result;
		}

		// Grab the next token and if nothing special is needed, simply return it.
		// Initialize `initialIndentToken` if needed.
		auto next = antlr4::Lexer::nextToken();

		//NOTE: This could be an appropriate spot to count whitespace or deal with
		//NEWLINES, but it is already handled with custom actions down in the
		//lexer rules.
		if (pendingDent && nullptr == initialIndentToken && NEWLINE != next->getType())
			initialIndentToken = std::make_unique<antlr4::CommonToken>(next.get());
			
		if (nullptr == next || HIDDEN == next->getChannel() || NEWLINE == next->getType())
			return next;

		// Handle EOF. In particular, handle an abrupt EOF that comes without an
		// immediately preceding NEWLINE.
		if (next->getType() == EOF)
		{
			indentCount = 0;

			// EOF outside of `pendingDent` state means input did not have a final
			// NEWLINE before end of file.
			if (!pendingDent)
			{
				auto commonToken = createToken(NEWLINE, "NEWLINE", next);
				initialIndentToken = std::make_unique<antlr4::CommonToken>(next.get());
				tokenQueue.push(std::move(commonToken));
			}
		}

		// Before exiting `pendingDent` state queue up proper INDENTS and DEDENTS.
		while (indentCount != getSavedIndent())
		{
			if (indentCount > getSavedIndent())
			{
				indentStack.push(indentCount);
				tokenQueue.push(createToken(INDENT, "INDENT" + std::to_string(indentCount - 1), next));
			}
			else
			{
				indentStack.pop();
				tokenQueue.push(createToken(DEDENT, "DEDENT" + std::to_string(getSavedIndent()), next));
			}
		}
		pendingDent = false;
  		tokenQueue.push(std::make_unique<antlr4::CommonToken>(next.get()));

		auto result = std::move(tokenQueue.front());
		tokenQueue.pop();
		return result;
	}
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

NEWLINE : ( '\r'? '\n' | '\r' ) {
	if (pendingDent)
		setChannel(HIDDEN);
	pendingDent = true;
	indentCount = 0;
	initialIndentToken = nullptr;
};

Whitespace: ' '+;

Indents: '\t'+ {
	setChannel(HIDDEN);
	if (pendingDent)
		indentCount += getText().length();
};