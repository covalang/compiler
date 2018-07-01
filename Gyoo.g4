grammar Gyoo;

tokens { INDENT, DEDENT }
@lexer::members {
    private readonly DenterHelper denter = new DenterHelper(NL, GyooParser.INDENT, GyooParser.DEDENT, () => BaseNextToken());
    public override IToken NextToken() => denter.nextToken();
    private IToken BaseNextToken() => base.NextToken();
}
NL: ('\r'? '\n' '\t'*); // note the ' '*

program   : 'begin' statement+ 'end';
          
statement : assign | add | print ;

assign    : INDENT 'let' ID 'be' (NUMBER | ID) DEDENT;
print     : INDENT 'print' (NUMBER | ID) DEDENT;
add       : INDENT 'add' (NUMBER | ID) 'to' ID DEDENT;

ID     : [a-z]+ ;
NUMBER : [0-9]+ ;
WS     : [ \r\n\t]+ -> skip;