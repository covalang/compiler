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
	: Namespace Space qualifiedIdentifier namespaceBody?
	;

namespaceBody
	: blockStart (blockContinue | namespaceMemberDefinition)* blockEnd
	;

typeDefinition
	: visibility? Type Space identifier (Space typeKind)? typeBody?
	;

typeBody
	: blockStart (blockContinue | typeMemberDefinition)* blockEnd
	;

functionDefinition
	: visibility? Func Space identifier body?
	;

body
	: blockStart (blockContinue | statement)* blockEnd
	;

qualifiedIdentifier
	: identifier (Dot identifier)*
	;

identifier
	: Identifier
	;

visibility : publicVisibility | privateVisibility | protectedVisibility | internalVisibility;
	publicVisibility : PublicVisibility;
	privateVisibility : PrivateVisibility;
	protectedVisibility : ProtectedVisibility;
	internalVisibility : InternalVisibility;

storageType : staticStorageType | instanceStorageType;
	staticStorageType : StaticStorageType;
	instanceStorageType : InstanceStorageType;

typeKind : enumTypeKind | structTypeKind | interfaceTypeKind | traitTypeKind | delegateTypeKind;
	enumTypeKind : Enum;
	structTypeKind : Struct;
	interfaceTypeKind : Interface;
	traitTypeKind : Trait;
	delegateTypeKind : Delegate;

typeMemberDefinition
	: typeDefinition
	| functionDefinition
	;

statement
	: assignment
	| invocation
	;

assignment : identifier Space assignmentOperator Space expression;
	assignmentOperator: EqualsSign;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;
	arguments: expression (Comma expression)*;

expression
	: expression Power expression                                                              #powerExpression
	| Minus expression                                                                         #unaryMinusExpression
	| Not expression                                                                           #notExpression
	| expression op=(Multiply | Divide | Modulo) expression                            #multiplicationExpression
	| expression op=(Plus | Minus) expression                                                  #additiveExpression
	| expression op=(LessThanOrEqual | GreaterThanOrEqual | LessThan | GreaterThan) expression #relationalExpression
	| expression op=(Equal | NotEqual) expression                                              #equalityExpression
	| expression And expression                                                                #andExpression
	| expression Or expression                                                                 #orExpression
	| atom                                                                                     #atomExpression
	;

atom
	: booleanLiteral
	;

booleanLiteral
	: True
	| False
	;