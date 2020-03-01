parser grammar CovaParser;

options {
	tokenVocab = CovaLexer;
}

file: (NewLine | namespaceMemberDefinition)* EOF;

//block: blockStart statement (blockContinue statement)* blockEnd;

blockStart: NewLine Indent;
blockContinue: NewLine;
blockEnd: Dedent;

namespaceMemberDefinition
	: namespaceDefinition
	| typeDefinition
	;

namespaceDefinition
	: Namespace Whitespace qualifiedIdentifier namespaceBody?
	;

qualifiedIdentifier
	: identifier (Dot identifier)*
	;

identifier
	: Identifier
	;

namespaceBody
	: blockStart (blockContinue | namespaceMemberDefinition)* blockEnd
	//{ Console.WriteLine("namespaceBody"); }
	;

typeDefinition
	: visibilityModifier? Type Whitespace identifier (Whitespace typeKind)? typeBody?
	;

visibilityModifier
	: Public
	| Private
	| Protected
	| Internal
	;

typeKind
	: Enum
	| Struct
	| Interface
	| Trait
	| Delegate
	;

typeBody
	: blockStart (blockContinue | typeMemberDefinition)* blockEnd
	//{ Console.WriteLine("typeBody"); }
	;

typeMemberDefinition
	: typeDefinition
	| functionDefinition
	;

functionDefinition
	: visibilityModifier? Func Whitespace identifier functionBody?
	;

functionBody
	: blockStart (blockContinue | statement)* blockEnd
	//{ Console.WriteLine("functionBody"); }
	;

statement
	: identifier Whitespace EqualsSign Whitespace booleanLiteral
	//{ Console.WriteLine("statement"); }
	;

booleanLiteral
	: True
	| False
	;