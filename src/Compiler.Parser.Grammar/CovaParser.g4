parser grammar CovaParser;

options {
	tokenVocab = CovaLexer;
}

compilationUnit: compilationUnitBody EOF;

compilationUnitBody
	: dentedBodyBegin (compilationUnitMember | dentedBodyContinue)* dentedBodyEnd // lexer should be emitting dent tokens at start and end of file
	| compilationUnitMember (Newline+ compilationUnitMember)*
	;

compilationUnitMember
	: useNamespaceStatement
	| namespaceDefinition
	| typeDefinition
	| functionDefinition
	| statement
	;

dentedBodyBegin: dentedIgnorable* Indent;
dentedBodyContinue: dentedIgnorable* (Dent | SemiColon Ws+);
dentedBodyEnd: dentedIgnorable* Dedent;

dentedIgnorable: Newline | Tab;


linearBodyBegin: Ws+ Arrow Ws+;
linearBodyContinue: Ws+ Comma;
linearBodyEnd: anyWhitespace? LinearBodyEnd;


bracedBodyBegin: anyWhitespace? LeftBrace anyWhitespace?;
bracedBodyContinue: anyWhitespace? SemiColon anyWhitespace?;
bracedBodyEnd: anyWhitespace? RightBrace;

//bracedIgnorable: Newline | Tab | Whitespace | Indent | Dent | Dedent;


anyWhitespace: (Newline | Tab | Ws)+;




typeIdentifier: identifier typeParameters?;
typeParameters: LeftChevron typeParameter (Comma Ws+ typeParameter)* RightChevron;
typeParameter: identifier (Ws+ Colon Ws+ typeExpression)?;

qualifiedTypeIdentifierReference: (typeOrNamespaceIdentifierReference Dot)* typeOrNamespaceIdentifierReference;
typeOrNamespaceIdentifierReference: identifier typeArguments?;

typeArguments: LeftChevron typeArgument (Comma Ws+ typeArgument)* RightChevron;
typeArgument: typeExpression;




useNamespaceStatement
	: Use Ws+ Namespace Ws+ qualifiedNamespaceIdentifier
	;

qualifiedNamespaceIdentifier
	: namespaceIdentifier (Dot namespaceIdentifier)*
	;

namespaceIdentifier
	: Identifier
	;

namespaceDefinition
	: Namespace Ws+ qualifiedNamespaceIdentifier namespaceBody?
	;

namespaceBody
	: dentedBodyBegin (namespaceMemberDefinition | dentedBodyContinue)* dentedBodyEnd
	| bracedBodyBegin (namespaceMemberDefinition | bracedBodyContinue)* bracedBodyEnd
	| linearBodyBegin (namespaceMemberDefinition | linearBodyContinue)* linearBodyEnd
	;


namespaceMemberDefinition
	: namespaceDefinition
	| typeDefinition
	| functionDefinition
	| statement
	;

typeDefinition
	: Type Ws+ typeIdentifier (Ws+ typeKind)? (Ws+ typeModifier+)? (Ws+ visibility) typeBody?
	;

anonymousTypeDefinition
	: Type Ws+ (Ws+ typeKind)? (Ws+ typeModifier+)? (Ws+ visibility)? typeBody?
	;

typeModifier: storageType | referenceSemantics | nullability;
storageModifier: storageType | referenceSemantics | nullability;

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
	: Field Ws+ identifier (Ws+ storageType)? Ws+ qualifiedTypeIdentifierReference (Ws+ visibility)?
	;

propertyDefinition
	: Prop Ws+ identifier (Ws+ storageType)? Ws+ qualifiedTypeIdentifierReference
	;

functionDefinition
	: Func Ws+ identifier typeParameters? parameters? (Ws+ storageType)? (Ws+ nullability)? (Ws+ qualifiedTypeIdentifierReference)? (Ws+ visibility)? body?
	;

typeExpression
//	: logicalOperator Whitespace* typeExpression            #unaryTypeExpression
	: typeExpression Ws* logicalOperator Ws* typeExpression #binaryTypeExpression
	| qualifiedTypeIdentifierReference                      #unaryTypeExpression
	| anonymousTypeDefinition                               #anonymousTypeExpression
	| literal                                               #literalTypeExpression
	;

parameters: LeftParenthesis parameter (Comma Ws+ parameter)* RightParenthesis;
parameter: identifier (Ws+ qualifiedTypeIdentifierReference)?;


body
	: dentedBodyBegin (statement | dentedBodyContinue)* dentedBodyEnd
	| bracedBodyBegin (statement | bracedBodyContinue)* bracedBodyEnd
	| linearBodyBegin (statement | linearBodyContinue)* linearBodyEnd
	;
	
scope: Scope anyWhitespace? body;

qualifiedIdentifier: symbolIdentifier (Dot symbolIdentifier)*;
symbolIdentifier: identifier typeArguments?;
identifierDefinition: Identifier;
genericIdentifierDefinition: Identifier typeParameters;
identifier: Identifier;


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
	
referenceSemantics
    : At #mutable
    | Asterisk #mutableToMutable
    | Caret #mutableToImmutable
    | Ampersand #immutableToMutable
    | Percent #immutableToImmutable
    ;

nullability
    : Question #nullable
    //| Exclamation #nonNullable
    ;

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
	: Local Ws+ identifier (Ws+ qualifiedTypeIdentifierReference)
	| Local Ws+ identifier (Ws+ qualifiedTypeIdentifierReference)? (Ws+ assignmentOperator anyWhitespace+ expression)
	;

assignment : qualifiedIdentifier Ws+ assignmentOperator Ws* expression;

assignmentOperator: Equals;

invocation : qualifiedIdentifier LeftParenthesis arguments? RightParenthesis;


// Expressions

// identityExpression
// 	: unaryExpression identityOperator unaryExpression
// 	;

memberAccessOperator: Ws Dot;

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

conditionalOperator
    : thenOperator
    | elseOperator
    ;
	
thenOperator: Then;
elseOperator: Else;

bitwiseOperator
	: bitwiseAndOperator
	| bitwiseOrOperator
	| bitwiseXorOperator
	| bitwiseLeftShiftOperator
	| bitwiseRightShiftOperator
	| bitwiseLeftRotateOperator
	| bitwiseRightRotateOperator
	;

bitwiseAndOperator: Ampersand;
bitwiseOrOperator: VerticalBar;
bitwiseXorOperator: Caret;
bitwiseNotOperator: Tilde;
bitwiseLeftShiftOperator: LeftChevron LeftChevron;
bitwiseRightShiftOperator: RightChevron RightChevron;
bitwiseLeftRotateOperator: LeftChevron LeftChevron LeftChevron;
bitwiseRightRotateOperator: RightChevron RightChevron RightChevron;

arithmeticOperator
	: additionOperator
	| Ws subtractionOperator Ws
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
	| nameOfOperator Ws
	| funcOfOperator Ws
	| fieldOfOperator Ws
	| propOfOperator Ws
	;

binaryOperator
	: memberAccessOperator
	| identityOperator
	| logicalOperator
	| conditionalOperator
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
	: LeftParenthesis Ws* arguments Ws* RightParenthesis                                   #parenthesisExpression
	| LeftBracket anyWhitespace? arguments anyWhitespace? RightBracket                                     #bracketExpression
	| expression LeftParenthesis arguments? RightParenthesis                                               #invocationExpression
	| (parameter | parameters) body                                                                        #closureExpression
	| unaryOperator Ws* expression                                                                 #unaryExpression
	| expression Ws* binaryOperator Ws* expression                                         #binaryExpression
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
	: literal
	| qualifiedIdentifier
	;

literal
	: booleanLiteral
	| integerLiteral
	| realLiteral
	| stringLiteral
	;

booleanLiteral
	: True
	| False
	;

integerLiteral: IntegerLiteral;

realLiteral: RealLiteral;

stringLiteral: StringLiteral;