#include <iostream>

#include "antlr4-runtime.h"
#include "CovaLexer.h"
#include "CovaParser.h"
#include "CovaParserBaseListener.h"

using namespace antlr4;

class Listener : public CovaParserBaseListener {
public:
  void enterFile(CovaParser::FileContext *ctx) override {
	// Do something when entering the key rule.
	std::cout << ctx->getText() << std::endl;
  }
};


int main(int argc, const char* argv[]) {
	std::cout << argv[1] << std::endl;
	std::ifstream stream;
	stream.open(argv[1]);
	ANTLRInputStream input(stream);
	CovaLexer lexer(&input);

	for (auto const & token : lexer.getAllTokens())
		std::cout << token->getText() << ", ";
	lexer.reset();

	CommonTokenStream tokens(&lexer);
	CovaParser parser(&tokens);

	tree::ParseTree *tree = parser.file();
	Listener listener;
	tree::ParseTreeWalker::DEFAULT.walk(&listener, tree);

	return 0;
}