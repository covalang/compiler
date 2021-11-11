parser grammar CovaParser;

options {
	tokenVocab = CovaLexer;
}

file: (namespaceBody | Newline)* EOF;


dentedBodyBegin: dentedIgnorable* Indent;
dentedBodyContinue: dentedIgnorable* (Dent | SemiColon Whitespace+);
dentedBodyEnd: dentedIgnorable* Dedent;

dentedIgnorable: Newline | Tab;


linearBodyBegin: Whitespace+ Arrow Whitespace+;
linearBodyContinue: Whitespace+ Comma;
linearBodyEnd: anyWhitespace? LinearBodyEnd;


bracedBodyBegin: anyWhitespace? LeftBrace anyWhitespace?;
bracedBodyContinue: anyWhitespace? SemiColon anyWhitespace?;
bracedBodyEnd: anyWhitespace? RightBrace;

//bracedIgnorable: Newline | Tab | Whitespace | Indent | Dent | Dedent;


anyWhitespace: (Newline | Tab | Whitespace)+;


namespaceMemberDefinition
	: namespaceDefinition
	| typeDefinition
	| functionDefinition
	| statement
	;

namespaceDefinition
	: Namespace Whitespace+ qualifiedIdentifier (Whitespace+ visibility)? namespaceBody?
	;

namespaceBody
	: dentedBodyBegin (namespaceMemberDefinition | dentedBodyContinue)* dentedBodyEnd
	| bracedBodyBegin (namespaceMemberDefinition | bracedBodyContinue)* bracedBodyEnd
	| linearBodyBegin (namespaceMemberDefinition | linearBodyContinue)* linearBodyEnd
	;

typeDefinition
	: Type Whitespace+ (typeKind Whitespace+)? identifier typeParameters? (Whitespace+ visibility)? typeBody?
	;

typeBody
	: dentedBodyBegin (typeMemberDefinition | dentedBodyContinue)* dentedBodyEnd
	| bracedBodyBegin (typeMemberDefinition | bracedBodyContinue)* bracedBodyEnd
	| linearBodyBegin (typeMemberDefinition | linearBodyContinue)* linearBodyEnd
	;

typeMemberDefinition
	: typeDefinition
	| fieldDefinition
	| propertyDefinition
	| functionDefinition
	;

fieldDefinition
	: Field Whitespace+ identifier (Whitespace+ storageType)? Whitespace+ type (Whitespace+ visibility)?
	;

propertyDefinition
	: Prop Whitespace+ identifier (Whitespace+ storageType)? Whitespace+ type
	;

functionDefinition
	: Func Whitespace+ identifier typeParameters? parameters? (Whitespace+ type)? (Whitespace+ visibility)? body?
	;

typeParameters: LeftChevron typeParameter (Comma Whitespace+ typeParameter)* RightChevron;
typeParameter: type (Whitespace+ Colon Whitespace+ type)?;

typeArguments: LeftChevron typeArgument (Comma Whitespace+ typeArgument)* RightChevron;
typeArgument: type;

parameters: LeftParenthesis parameter (Comma Whitespace+ parameter)* RightParenthesis;
parameter: identifier (Whitespace+ type)?;

type: qualifiedIdentifier typeArguments?;

body
	: dentedBodyBegin (statement | dentedBodyContinue)* dentedBodyEnd
	| bracedBodyBegin (statement | bracedBodyContinue)* bracedBodyEnd
	| linearBodyBegin (statement | linearBodyContinue)* linearBodyEnd
	;
	
scope: Scope anyWhitespace? body;

qualifiedIdentifier
	: identifier (Dot identifier)*
	;

identifier
	: Identifier
	;


visibility
	: Minus #privateVisibility
	| Octothorp #protectedVisibility
	| Tilde #internalVisibility
	| Plus #publicVisibility
	;

//noVisibility: Underscore;
//privateVisibility: Minus;
//protectedVisibility: Octothorp;
//internalVisibility: Tilde;
//publicVisibility: Plus;


storageType : staticStorageType;// | instanceStorageType;
	staticStorageType : StaticStorageType;
	//instanceStorageType : InstanceStorageType;


typeKind : enumTypeKind | structTypeKind | interfaceTypeKind | traitTypeKind | delegateTypeKind;

enumTypeKind: Enum;
structTypeKind: Struct;
interfaceTypeKind: Interface;
traitTypeKind: Trait;
delegateTypeKind: Func;

statement
	: scope
	| assignment
	| invocation
	//| localDefinition
	| localDeclaration
	| expression
	;

//localDefinition: Local Whitespace+ identifier (Whitespace+ qualifiedIdentifier);
localDeclaration
	: Local Whitespace+ identifier (Whitespace+ type)
	| Local Whitespace+ identifier (Whitespace+ type)? (Whitespace+ assignmentOperator anyWhitespace+ expression)
	;

assignment : qualifiedIdentifier Whitespace+ assignmentOperator Whitespace* expression;

assignmentOperator: Equals;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;


// Expressions

// identityExpression
// 	: unaryExpression identityOperator unaryExpression
// 	;

memberAccessOperator: Whitespace Dot;

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
rootOperator: Slash Slash;//Percent Percent;//Backslash Slash;

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
joinOperator: Join;

nameOfOperator: NameOf;
funcOfOperator: FuncOf;
fieldOfOperator: FieldOf;
propOfOperator: PropOf;

unaryOperator
	: notOperator
	| bitwiseNotOperator
	| subtractionOperator
	| rootOperator
	| nameOfOperator Whitespace
	| funcOfOperator Whitespace
	| fieldOfOperator Whitespace
	| propOfOperator Whitespace
	;

binaryOperator
	: memberAccessOperator
	| identityOperator
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
	| joinOperator
	;

expression
	: LeftParenthesis Whitespace* arguments Whitespace* RightParenthesis                                   #parenthesisExpression
	| LeftBracket anyWhitespace? arguments anyWhitespace? RightBracket                                     #bracketExpression
	| expression LeftParenthesis arguments? RightParenthesis                                               #invocationExpression
	| (parameter | parameters) body                                                                        #closureExpression
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
	
arguments: expression (Comma anyWhitespace? expression)*;

arrayExpression
	: LeftBracket expression (Comma expression)* RightBracket
	;

bodyExpression
	: dentedBodyBegin expression dentedBodyEnd
	| bracedBodyBegin expression bracedBodyEnd
	| linearBodyBegin expression linearBodyEnd
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