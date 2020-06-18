#include <iostream>

#include "antlr4-runtime.h"
#include "CovaLexer.h"
#include "CovaParser.h"
#include "CovaParserBaseListener.h"

using namespace antlr4;
using namespace std;

enum Visibility { Public, Private, Protected, Internal };

struct Base {
	std::string name;
};
struct Function : Base {
	Visibility visibility;
};
struct Type : Base {
	Visibility visibility;
	std::vector<Function> functions;
};
struct CompilationUnit : Base {
	std::vector<Type> types;
};

std::string join(char const * delimiter, std::vector<std::string> const & strings) {
	size_t size = 0;
	for (auto i = 0; i != strings.size(); ++i)
		size += strings[i].size() + 1;
	string joined;
	joined.reserve(size);
	for (auto i = 0; i != strings.size(); ++i) {
		joined += strings[i];
		if (i + 1 != strings.size())
			joined += delimiter;
	}
	return joined;
}

class Listener : public CovaParserBaseListener
{
	vector<string> qualifiers;

	string getFullyQualifiedName() {
		return join(".", qualifiers);
	}

	void pushNames(CovaParser::QualifiedIdentifierContext * ctx) {
		for (auto const & identifier : ctx->identifier())
			pushName(identifier);
	}
	void pushName(CovaParser::IdentifierContext * ctx) {
		qualifiers.push_back(ctx->getText());
		// for (auto const & q : qualifiers)
		// 	cout << q << endl;
	}

	void popNames(CovaParser::QualifiedIdentifierContext * ctx) {
		for (auto i = 0; i != ctx->identifier().size(); i++)
			qualifiers.pop_back();
	}
	void popName(CovaParser::IdentifierContext * ctx) {
		qualifiers.pop_back();
	}

	// Visibility getVisibilityFromString(CovaParser::VisibilityModifierContext * ctx) {
	// 	if (ctx == nullptr)
	// 		return Private;
	// 	if (ctx->)
	// }

public:
	vector<Type> types;

	virtual void enterIdentifier(CovaParser::IdentifierContext * ctx) override {
		// cout << ctx->getText() << endl;
		// pushName(ctx);
	}
	virtual void exitIdentifier(CovaParser::IdentifierContext * ctx) override {
		// popName(ctx);
	}

	virtual void enterNamespaceDefinition(CovaParser::NamespaceDefinitionContext * ctx) override {
		pushNames(ctx->qualifiedIdentifier());
	}
	virtual void exitNamespaceDefinition(CovaParser::NamespaceDefinitionContext * ctx) override {
		popNames(ctx->qualifiedIdentifier());
	}

	virtual void enterTypeDefinition(CovaParser::TypeDefinitionContext * ctx) override {
		Type type;
		pushName(ctx->identifier());
		type.name = getFullyQualifiedName();
		// type.visibility = getVisibilityFromString(ctx->visibilityModifier());
		types.push_back(std::move(type));
	}
	virtual void exitTypeDefinition(CovaParser::TypeDefinitionContext * ctx) override {
		popName(ctx->identifier());
	}

	virtual void enterFunctionDefinition(CovaParser::FunctionDefinitionContext * ctx) override {
		Function function;
		pushName(ctx->identifier());
		function.name = getFullyQualifiedName();
		types.back().functions.push_back(std::move(function));
	}
	virtual void exitFunctionDefinition(CovaParser::FunctionDefinitionContext * ctx) override {
		popName(ctx->identifier());
	}
};

int main(int argc, const char* argv[]) {
	// std::cout << argv[1] << std::endl;
	std::ifstream stream;
	stream.open(argv[1]);
	ANTLRInputStream input(stream);
	CovaLexer lexer(&input);

	// for (auto const & token : lexer.getAllTokens())
	// 	std::cout << token->getText() << ", ";
	// lexer.reset();
	// return 0;

	CommonTokenStream tokens(&lexer);
	CovaParser parser(&tokens);

	tree::ParseTree *tree = parser.file();
	Listener listener;
	tree::ParseTreeWalker::DEFAULT.walk(&listener, tree);

	for (auto const & type : listener.types) {
		cout << type.name << endl;
		for (auto const & function : type.functions)
			cout << function.name << endl;
	}

	return 0;
}