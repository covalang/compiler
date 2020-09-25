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
	: Namespace Whitespace+ visibilityModifier Whitespace+ qualifiedIdentifier namespaceBody?
	;

namespaceBody
	: blockStart (blockContinue | namespaceMemberDefinition)* blockEnd
	| braceBlockStart (braceBlockContinue | namespaceMemberDefinition)* braceBlockEnd
	| singleLineStart namespaceMemberDefinition singleLineEnd
	;

typeDefinition
	: Type (Whitespace+ typeKind)? Whitespace+ visibilityModifier identifier typeBody?
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
	: Field Whitespace+ visibilityModifier Whitespace+ storageType? Whitespace+ identifier Whitespace+ memberType
	;

propertyDefinition
	: Prop Whitespace+ storageType? Whitespace+ identifier Whitespace+ memberType
	;

functionDefinition
	: Func Whitespace+ visibilityModifier Whitespace+ identifier (LeftParenthesis parameters RightParenthesis)? memberType? body?
	;
	parameters: parameter (Comma parameter)*;
	parameter: identifier Whitespace+ qualifiedIdentifier;

memberType: qualifiedIdentifier;

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

visibilityModifier: readVisibility writeVisibility?;

readVisibility: visibility;
writeVisibility: visibility;

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
	delegateTypeKind : Func;

statement
	: assignment
	| invocation
	| localDefinition
	| localDeclaration
	| expression
	;

localDefinition: Local Whitespace+ identifier (Whitespace+ qualifiedIdentifier);
localDeclaration: Local Whitespace+ identifier (Whitespace+ qualifiedIdentifier)? (Whitespace+ assignmentOperator Whitespace+ expression);

assignment : qualifiedIdentifier Whitespace+ assignmentOperator Whitespace+ expression;

assignmentOperator: Equals;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;
	arguments: expression (Comma expression)*;


// Expressions

// identityExpression
// 	: unaryExpression identityOperator unaryExpression
// 	;

identityOperator: isOperator | isntOperator;
isOperator: Is;
isntOperator: Isnt;

// logicalExpression
// 	: unaryExpression logicalOperator unaryExpression
// 	| logicalExpression logicalOperator unaryExpression
// 	;

// relationalExpression
// 	: unaryExpression relationalOperator unaryExpression
// 	| relationalExpression relationalOperator unaryExpression
// 	;
	
// arithmeticExpression
// 	: unaryExpression arithmeticOperator unaryExpression
// 	| arithmeticExpression arithmeticOperator unaryExpression
// 	;

// bitwiseExpression
// 	: bitwiseNotOperator unaryExpression
// 	| unaryExpression 
// 	;

logicalOperator
	: andOperator
	| orOperator
	;

andOperator: And;
orOperator: Or;
notOperator: Not;

bitwiseOperator
	: bitwiseAndOperator
	| bitwiseOrOperator
	| bitwiseXorOperator
	;

bitwiseAndOperator: Ampersand;
bitwiseOrOperator: VerticalBar;
bitwiseXorOperator: Caret;
bitwiseNotOperator: Tilde;

arithmeticOperator
	: additionOperator
	| Whitespace subtractionOperator Whitespace
	| multiplicationOperator
	| divisionOperator
	| moduloOperator
	| powerOperator
	| rootOperator
	;

additionOperator: Plus;
subtractionOperator: Minus;
multiplicationOperator: Asterisk;
divisionOperator: Slash;
moduloOperator: Percent;
powerOperator: Asterisk Asterisk;
rootOperator: Backslash Slash;

relationalOperator
	: equalityOperator
	| inequalityOperator
	| lessThanOperator
	| greaterThanOperator
	| lessOrEqualThanOperator
	| greaterOrEqualThanOperator
	;

equalityOperator: Equals Equals;
inequalityOperator: Exclamation Equals;
lessThanOperator: LeftChevron;
greaterThanOperator: RightChevron;
lessOrEqualThanOperator: LeftChevron Equals;
greaterOrEqualThanOperator: RightChevron Equals;

rangeRelationalOperator
	: withinOperator
	| withoutOperator
	;

withinOperator: RightChevron LeftChevron;
withoutOperator: LeftChevron RightChevron;

// unaryExpression
// 	: Minus expression #unaryMinusExpression
// 	| Root expression  #unaryRootExpression
// 	| Not expression   #unaryNotExpression
// 	;

rangeOperator
	: halfOpenRangeOperator
	| closedRangeOperator
	;

halfOpenRangeOperator: Dot Dot;
closedRangeOperator: Dot Dot Dot;

sequenceOperator: Colon;

forOperator: For;
mapOperator: Map;
foldOperator: Fold;

unaryOperator
	: notOperator
	| bitwiseNotOperator
	| subtractionOperator
	| rootOperator
	;

binaryOperator
	: identityOperator
	| logicalOperator
	| bitwiseOperator
	| arithmeticOperator
	| relationalOperator
	| rangeRelationalOperator
	| rangeOperator
	| sequenceOperator
	| forOperator
	| mapOperator
	| foldOperator
	;

expression
	: LeftParenthesis Whitespace* expression Whitespace* RightParenthesis                                  #parenthesisExpression
	| LeftBracket Whitespace* expression Whitespace* RightBracket                                          #bracketExpression
	| unaryOperator expression                                                                             #unaryExpression
	| expression Whitespace* binaryOperator Whitespace* expression                                         #binaryExpression
	// | expression Whitespace+ arithmeticOperator Whitespace+ expression                                     #arithmeticExpression
	// | expression Whitespace+ rootOperator Whitespace+ expression                                           #rootExpression
	// | expression Whitespace+ relationalOperator Whitespace+ expression                                     #relationalExpression
	// | expression Whitespace+ And Whitespace+ expression                                                    #andExpression
	// | expression Whitespace+ Or Whitespace+ expression                                                     #orExpression
	// | expression Whitespace+ rangeRelationalOperator Whitespace+ expression                                #rangeRelationalExpression
	// | expression Whitespace+ identityOperator qualifiedIdentifier                                          #typeCheckExpression
	// | expression Whitespace* rangeOperator Whitespace* expression                                          #rangeExpression
	// | expression Whitespace* rangeOperator Whitespace* expression Whitespace* Colon Whitespace* expression #sequenceExpression
	// | expression Whitespace* PlusMinus Whitespace* expression                                              #intervalExpression
	// | expression Whitespace* Subset Whitespace* expression                                                 #subsetExpression
	// | expression Whitespace* Superset Whitespace* expression                                               #supersetExpression
	// | expression Whitespace* ProperSubset Whitespace* expression                                           #properSubsetExpression
	// | expression Whitespace* ProperSuperset Whitespace* expression                                         #properSupersetExpression
	| atom                                                                                                 #atomExpression
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