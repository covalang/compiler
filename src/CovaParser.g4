parser grammar CovaParser;

options {
	tokenVocab = CovaLexer;
}

@header {
	#pragma warning disable 3021
}

file: (Newline | namespaceMemberDefinition)* EOF;

//block: blockStart statement (blockContinue statement)* blockEnd;

blockStart: Newline Tab* Indent;
blockContinue: Newline Tab* | Comma;
blockEnd: Dedent;

singleLineStart: anyWhitespace? Arrow;
singleLineEnd: anyWhitespace? SingleLineEnd;

braceBlockStart: anyWhitespace? LeftBrace;
braceBlockContinue: anyWhitespace? SemiColon;
braceBlockEnd: anyWhitespace? LeftBrace;

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

storageType : staticStorageType;// | instanceStorageType;
	staticStorageType : StaticStorageType;
	//instanceStorageType : InstanceStorageType;

typeKind : enumTypeKind | structTypeKind | interfaceTypeKind | traitTypeKind | delegateTypeKind;
	enumTypeKind : Enum;
	structTypeKind : Struct;
	interfaceTypeKind : Interface;
	traitTypeKind : Trait;
	delegateTypeKind : Delegate;

statement
	: assignment
	| invocation
	| localDefinition
	| localDeclaration
	| expression
	;

localDefinition: Local Whitespace+ identifier (Whitespace+ qualifiedIdentifier);
localDeclaration: Local Whitespace+ identifier (Whitespace+ qualifiedIdentifier)? (Whitespace+ EqualsSign Whitespace+ expression)?;

assignment : qualifiedIdentifier Whitespace+ assignmentOperator Whitespace+ expression;
	assignmentOperator: EqualsSign;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;
	arguments: expression (Comma expression)*;
	
binaryExpression
	: unaryExpression binaryOperator unaryExpression
	| binaryExpression binaryOperator unaryExpression
    ;



expression
	: Minus expression                                                                                                 #unaryMinusExpression
	| Not expression                                                                                                   #notExpression
	| expression Whitespace+ Power Whitespace+ expression                                                              #powerExpression
	| Root Whitespace+ expression                                                                                      #unaryRootExpression
	| expression Whitespace+ Root Whitespace+ expression                                                               #rootExpression
	| expression Whitespace+ op=(Multiply | Divide | Modulo) Whitespace+ expression                                    #multiplicationExpression
	| expression Whitespace+ op=(Plus | Minus) Whitespace+ expression                                                  #additiveExpression
	| expression Whitespace+ op=(LessThanOrEqual | GreaterThanOrEqual | LessThan | GreaterThan) Whitespace+ expression #relationalExpression
	| expression Whitespace+ op=(Equal | NotEqual) Whitespace+ expression                                              #equalityExpression
	| expression Whitespace+ And Whitespace+ expression                                                                #andExpression
	| expression Whitespace+ Or Whitespace+ expression                                                                 #orExpression
	| expression Whitespace+ op=(Within | Without) Whitespace+ expression                                              #rangeRelationalExpression
	| expression Whitespace+ op=(Is | Isnt) qualifiedIdentifier                                                        #typeCheckExpression
	| expression Whitespace* DoubleDot Whitespace* expression                                                          #rangeExpression
	| expression Whitespace* DoubleDot Whitespace* expression Whitespace* Colon Whitespace* expression                 #sequenceExpression
	| expression Whitespace* PlusMinus Whitespace* expression                                                          #intervalExpression
	| expression Whitespace* Subset Whitespace* expression                                                             #subsetExpression
	| expression Whitespace* Superset Whitespace* expression                                                           #supersetExpression
	| expression Whitespace* ProperSubset Whitespace* expression                                                       #properSubsetExpression
	| expression Whitespace* ProperSuperset Whitespace* expression                                                     #properSupersetExpression
	| atom                                                                                                             #atomExpression
	;

arrayExpression
	: LeftBracket expression (Comma expression)* RightBracket
	;

atom
	: booleanLiteral
	| integerLiteral
	| realLiteral
	| stringLiteral
	| qualifiedIdentifier
	;

booleanLiteral
	: True
	| False
	;

integerLiteral: IntegerLiteral;

realLiteral: RealLiteral;

stringLiteral: StringLiteral;