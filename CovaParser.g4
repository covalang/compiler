parser grammar CovaParser;

options {
	tokenVocab = CovaLexer;
}

file: (NEWLINE | moduleMemberDeclaration)* EOF;

//block: blockStart statement (blockContinue statement)* blockEnd;

blockStart: NEWLINE INDENT;
blockContinue: NEWLINE;
blockEnd: DEDENT;

moduleMemberDeclaration
	: moduleDeclaration
	| typeDeclaration
	// | interfaceDefinition
	// | enumDefinition
	// | delegateDefinition
	;

moduleDeclaration
	: Module Whitespace qualifiedIdentifier moduleBody?
	;

qualifiedIdentifier
	: Identifier (Dot Identifier)*
	;

moduleBody
	: blockStart (blockContinue | moduleMemberDeclaration)* blockEnd
	//{ Console.WriteLine("moduleBody"); }
	;

typeDeclaration
	: Type Whitespace Identifier typeBody?
	;

typeBody
	: blockStart (blockContinue | typeMemberDeclaration)* blockEnd
	//{ Console.WriteLine("typeBody"); }
	;

typeMemberDeclaration
	: typeDeclaration
	| functionDeclaration
	;

functionDeclaration
	: Func Whitespace Identifier functionBody?
	;

functionBody
	: blockStart (blockContinue | statement)* blockEnd
	//{ Console.WriteLine("functionBody"); }
	;

statement
	: Identifier Whitespace EqualsSign Whitespace booleanLiteral
	//{ Console.WriteLine("statement"); }
	;

booleanLiteral
	: True
	| False
	;