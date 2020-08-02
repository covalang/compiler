parser grammar CovaParser;

options {
	tokenVocab = CovaLexer;
}

@header {
	#pragma warning disable 3021
}

file: (Newline | namespaceMemberDefinition)* EOF;

//block: blockStart statement (blockContinue statement)* blockEnd;

blockStart: Newline Indent;
blockContinue: Newline;
blockEnd: Dedent;

singleLineStart: anyWhitespace? Arrow anyWhitespace?;
singleLineEnd:  anyWhitespace? SingleLineEnd;

braceBlockStart: anyWhitespace? LeftBrace anyWhitespace?;
braceBlockContinue: anyWhitespace? SemiColon anyWhitespace?;
braceBlockEnd: anyWhitespace? LeftBrace anyWhitespace?;

anyWhitespace: (Newline | Tab | Whitespace)+;

namespaceMemberDefinition
	: namespaceDefinition
	| typeDefinition
	;

namespaceDefinition
	: Namespace Whitespace+ qualifiedIdentifier namespaceBody?
	;

namespaceBody
	: blockStart (blockContinue | namespaceMemberDefinition)* blockEnd
	| braceBlockStart (braceBlockContinue | namespaceMemberDefinition)* braceBlockEnd
	| singleLineStart namespaceMemberDefinition singleLineEnd
	;

typeDefinition
	: visibility? Type Whitespace+ identifier (Whitespace+ typeKind)? typeBody?
	;

typeBody
	: blockStart (blockContinue | typeMemberDefinition)* blockEnd
	| braceBlockStart (braceBlockContinue | typeMemberDefinition)* braceBlockEnd
	| singleLineStart typeMemberDefinition singleLineEnd
	;

typeMemberDefinition
	: typeDefinition
	| fieldDefinition
	| propertyDefinition
	| functionDefinition
	;

fieldDefinition
	: Field Whitespace+ visibility? storageType? identifier Whitespace+ qualifiedIdentifier
	;

propertyDefinition
	: Prop Whitespace+ visibility? storageType? identifier Whitespace+ qualifiedIdentifier
	;

functionDefinition
	: visibility? Func Whitespace+ identifier (LeftParenthesis parameters RightParenthesis)? body?
	;
	parameters: parameter (Comma parameter)*;
	parameter: identifier Whitespace+ qualifiedIdentifier;

body
	: blockStart (blockContinue | statement)* blockEnd
	| braceBlockStart (braceBlockContinue | statement)* braceBlockEnd
	| singleLineStart statement singleLineEnd
	;

qualifiedIdentifier
	: identifier (Dot identifier)*
	;

identifier
	: Identifier
	;

visibility : publicVisibility | privateVisibility | protectedVisibility | internalVisibility;
	publicVisibility : Plus;
	privateVisibility : Minus;
	protectedVisibility : Octothorp;
	internalVisibility : Tilde;

storageType : staticStorageType | instanceStorageType;
	staticStorageType : StaticStorageType;
	instanceStorageType : InstanceStorageType;

typeKind : enumTypeKind | structTypeKind | interfaceTypeKind | traitTypeKind | delegateTypeKind;
	enumTypeKind : Enum;
	structTypeKind : Struct;
	interfaceTypeKind : Interface;
	traitTypeKind : Trait;
	delegateTypeKind : Delegate;

statement
	: assignment
	| invocation
	| local
	;

local: Local Whitespace+ identifier (Whitespace+ qualifiedIdentifier)? (Whitespace+ EqualsSign Whitespace+ expression)?;

assignment : qualifiedIdentifier Whitespace+ assignmentOperator Whitespace+ expression;
	assignmentOperator: EqualsSign;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;
	arguments: expression (Comma expression)*;

expression
	: expression Power expression                                                                            #powerExpression
	| Minus expression                                                                                       #unaryMinusExpression
	| Not expression                                                                                         #notExpression
	| expression Whitespace+ op=(Multiply | Divide | Modulo) Whitespace+ expression                                    #multiplicationExpression
	| expression Whitespace+ op=(Plus | Minus) Whitespace+ expression                                                  #additiveExpression
	| expression Whitespace+ op=(LessThanOrEqual | GreaterThanOrEqual | LessThan | GreaterThan) Whitespace+ expression #relationalExpression
	| expression Whitespace+ op=(Equal | NotEqual) Whitespace+ expression                                              #equalityExpression
	| expression Whitespace+ And Whitespace+ expression                                                                #andExpression
	| expression Whitespace+ Or Whitespace+ expression                                                                 #orExpression
	| atom                                                                                                   #atomExpression
	;

atom
	: booleanLiteral
	| integerLiteral
	| stringLiteral
	| qualifiedIdentifier
	;

booleanLiteral
	: True
	| False
	;

integerLiteral: IntegerLiteral;

stringLiteral: StringLiteral;