grammar RaceLang;

options { language = CSharp; }

// ------------------ LEXER (słowa kluczowe przed IDENTIFIER!) ------------------
STRUCT: 'struct';
CAR: 'car';
ENGINE: 'engine';
TRACK: 'track';
RACE: 'race';

FN: 'fn';
LET: 'let';
MUT: 'mut';
RETURN: 'return';
IF: 'if';
ELSE: 'else';
FOR: 'for';
IN: 'in';
PARALLEL: 'parallel';
QUERY: 'query';
IMPORT: 'import';
MODULE: 'module';
SHADER_AT: '@shader';

BOOL: 'true' | 'false' ;

STRING: '"' (~["\r\n])* '"' ;

// number może mieć jednostkę (np. 1200kg)
NUMBER: DIGITS ('.' DIGITS)? UNIT? ;
UNIT: 'kg' | 'm' | 's' | 'Nm' | 'hp' ;
fragment DIGITS: [0-9]+ ;

IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]* ;

// whitespace / komentarze
WS: [ \t\r\n]+ -> skip ;
COMMENT: '//' ~[\r\n]* -> skip ;

// ------------------ PARSER ------------------

// *name_token* akceptuje zarówno zwykłe IDENTIFIER-y, jak i tokeny-klucze,
// dzięki czemu pole nazwane "engine" nie będzie problemem.
name_token
    : IDENTIFIER
    | ENGINE
    | CAR
    | TRACK
    | RACE
    | STRUCT
    ;

// program
program: module* EOF ;

// modules / imports
module: MODULE IDENTIFIER import_stmt* top_level+ ;
import_stmt: IMPORT IDENTIFIER ';' ;

// top-level
top_level: top_level_item+ ;
top_level_item
    : struct_decl
    | system_decl
    | car_instance
    | track_instance
    | race_instance
    | engine_instance
    | function_decl
    ;

// structs
struct_decl: STRUCT IDENTIFIER '{' field_decl* '}' ;
field_decl: name_token ':' type ';' ;

// types
type
    : 'i32'
    | 'f32'
    | BOOL
    | STRING
    | 'vec2' | 'vec3' | 'vec4'
    | 'quat'
    | IDENTIFIER // custom type
    | 'void'
    ;

// ECSystem
system_decl
    : 'system' IDENTIFIER '(' param_list? ')' QUERY '{' '}' (PARALLEL)? '{' statement* '}'
    ;
param_list: param (',' param)* ;
param: IDENTIFIER ':' type ;
component_list: IDENTIFIER (',' IDENTIFIER)* ;

// instances
car_instance: CAR IDENTIFIER '{' field_value* '}' ;
engine_instance: ENGINE IDENTIFIER '{' field_value* '}'  ;
track_instance: TRACK IDENTIFIER '{' field_value* '}' ;
race_instance: RACE IDENTIFIER '{' field_value* '}' ;

field_value: name_token ':' primary_expr ';' ;

// functions
function_decl: FN IDENTIFIER '(' param_list? ')' ('->' type)? block ;
block: '{' statement* '}' ;

// statements
statement
    : var_decl_stmt
    | assignment
    | if_stmt
    | for_stmt
    | expression_stmt
    | return_stmt
    ;

var_decl_stmt: LET (MUT)? IDENTIFIER ':' type ('=' primary_expr)? ';' ;

// lvalue: dopuszcza kropkowanie i nazwę mogącą być keywordiem
lvalue: name_token ('.' name_token)* ;
assignment: lvalue ( '=' | '+=' | '-=' | '*=' | '/=' ) expression ';' ;

if_stmt: IF '(' expression ')' block (ELSE block)? ;
for_stmt: FOR IDENTIFIER IN expression block ;
expression_stmt: expression ';' ;
return_stmt: RETURN expression? ';' ;

// expressions (precedence)
expression: expr1 | postfix_expr | primary_expr;

expr1
    : expr1 op=('*'|'/') expr2   # MulDivExpr
    | expr2                      # PassUp1
    ;

expr2
    : expr2 op=('+'|'-') expr3   # AddSubExpr
    | expr3                      # PassUp2
    ;

expr3
    : expr3 op=('=='|'!='|'<'|'>'|'<='|'>=') expr4  # CompareExpr
    | expr4                                         # PassUp3
    ;

expr4
    : '-' expr4                # UnaryMinusExpr
    | '!' expr4                # UnaryNotExpr
                 
    ;

// postfix i call / member
postfix_expr: IDENTIFIER (postfix_op)* ;
// member access or fn call
postfix_op: '.' IDENTIFIER | '(' arg_list? ')' ;
arg_list: expression (',' expression)* ;

// primary
primary_expr
    : literal                # LiteralExpr
    | IDENTIFIER             # IdExpr
    | array_expr             # ArrayExpr
    | struct_init            # StructInit
    | '(' expression ')'     # ParenExpr
    ;

literal: NUMBER | STRING | BOOL ;
array_expr: '[' (expression (',' expression)*)? ']' ;
struct_init: IDENTIFIER '{' (init_field (',' init_field)*)? '}' ;
init_field: IDENTIFIER ':' expression ;

// shaders (jeśli używasz)
shader_decl: SHADER_AT '(' shader_type ')' function_decl ;
shader_type: 'vertex' | 'fragment' | 'compute' ;
