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

typeMemberDefinition
	: typeDefinition
	| fieldDefinition
	| propertyDefinition
	| functionDefinition
	;

fieldDefinition
	: Field Space visibility? storageType? identifier Space qualifiedIdentifier
	;

propertyDefinition
	: Prop Space visibility? storageType? identifier Space qualifiedIdentifier
	;

functionDefinition
	: visibility? Func Space identifier (LeftParenthesis parameters RightParenthesis)? body?
	;
	parameters: parameter (Comma parameter)*;
	parameter: identifier Space qualifiedIdentifier;

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

local: Local Space identifier (Space qualifiedIdentifier)? (Space EqualsSign Space expression)?;

assignment : qualifiedIdentifier Space assignmentOperator Space expression;
	assignmentOperator: EqualsSign;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;
	arguments: expression (Comma expression)*;

expression
	: expression Power expression                                                                            #powerExpression
	| Minus expression                                                                                       #unaryMinusExpression
	| Not expression                                                                                         #notExpression
	| expression Space? op=(Multiply | Divide | Modulo) Space? expression                                    #multiplicationExpression
	| expression Space? op=(Plus | Minus) Space? expression                                                  #additiveExpression
	| expression Space? op=(LessThanOrEqual | GreaterThanOrEqual | LessThan | GreaterThan) Space? expression #relationalExpression
	| expression Space? op=(Equal | NotEqual) Space? expression                                              #equalityExpression
	| expression Space? And Space? expression                                                                #andExpression
	| expression Space? Or Space? expression                                                                 #orExpression
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