grammar RaceLang;

options {
    language = CSharp;
}

// ------------------ LEXER ------------------
STRUCT: 'struct';
CAR: 'car';
ENGINE: 'engine';
TRACK: 'track';
RACE: 'race';

IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]* ;
STRING: '"' (~["\r\n])* '"' ;
NUMBER: DIGITS ('.' DIGITS)? UNIT? ;
BOOL: 'true' | 'false' ;

UNIT: 'kg' | 'm' | 's' | 'Nm' | 'hp' ;
fragment DIGITS: [0-9]+ ; 


WS: [ \t\r\n]+ -> skip ;
COMMENT: '//' ~[\r\n]* -> skip ;

// ------------------ TYPES ------------------
type: i32_type
    | f32_type
    | BOOL
    | STRING
    | vec_type
    | quat_type
    | custom_type
    ;

i32_type: 'i32';
f32_type: 'f32';
vec_type: 'vec2' | 'vec3' | 'vec4';
quat_type: 'quat';
custom_type: IDENTIFIER ;

program: module* EOF;

// --- MODULES ---
module: 'module' IDENTIFIER import_stmt+ top_level+ ;
import_stmt: 'import' IDENTIFIER ';' ;


// ------------------ TOP LEVEL ------------------
top_level: top_level_item+ ;
top_level_item
    : struct_decl
    | system_decl
    | car_instance
    | engine_instance
    | track_instance
    | race_instance
    | function_decl
    ;

// ------------------ STRUCTS ------------------
struct_decl
    : STRUCT IDENTIFIER '{' field_decl* '}' 
    ;
field_decl
    : IDENTIFIER ':' type ';'
    ;

// ------------------ SYSTEM ------------------
system_decl
    : 'system' IDENTIFIER '(' param_list? ')' 'query' '{'  '}' ('parallel')? '{' statement* '}'
    ;
component_list: IDENTIFIER (',' IDENTIFIER)* ;

param_list: param (',' param)* ;
param: IDENTIFIER ':' type ;

// ------------------ INSTANCES ------------------
car_instance: CAR IDENTIFIER '{' field_value* '}' ;
engine_instance: ENGINE IDENTIFIER '{' field_value* '}' ';' ;
track_instance: TRACK IDENTIFIER '{' field_value* '}' ;
race_instance: RACE IDENTIFIER '{' field_value* '}' ;

field_value: IDENTIFIER ':' expression ';' ;


// --- FUNCTIONS ---
function_decl: 'fn' IDENTIFIER '(' param_list? ')' ('->' type)? block ;



block: '{' statement* '}' ;
// ------------------ STATEMENTS ------------------
statement
    : var_decl_stmt
    | assignment
    | if
    | for
    | expression_stmt
    | return
     
    ;

if: 'if' '(' expression ')' block ('else' block)? ;
for: 'for' IDENTIFIER 'in' expression block ;
return: 'return' expression? ';' ;

expression_stmt: expression ';' ;

var_decl_stmt: 'let' ('mut')? IDENTIFIER ':' type ('=' expression)? ';' ;
assignment: IDENTIFIER ( '=' | '+=' | '-=' | '*=' | '/=' ) expression ';' ;

// ------------------ EXPRESSIONS ------------------

primary_expr
    : literal                # LiteralExpr
    | IDENTIFIER             # IdExpr
    | function_call          # FnCall
    | array_expr             # ArrayExpr
    | struct_init            # StructInit
    | member_access          # MemberAccess
    | '(' expression ')'     # ParenExpr
    ;

expression: expr1 ;

expr1
    : expr1 op=('*'|'/') expr2   # MulDivExpr
    | expr2                       # PassUp1
    ;

expr2
    : expr2 op=('+'|'-') expr3   # AddSubExpr
    | expr3                       # PassUp2
    ;

expr3
    : expr3 op=('=='|'!='|'<'|'>'|'<='|'>=') expr4  # CompareExpr
    | expr4                                         # PassUp3
    ;
// Unary
expr4
    : '-' expr4                # UnaryMinusExpr
    | '!' expr4                # UnaryNotExpr

    ;



literal
    : NUMBER
    | STRING
    | BOOL
    ;

function_call: IDENTIFIER  '(' arg_list? ')' ;
arg_list: expression (',' expression)* ;

array_expr: '[' (expression (',' expression)*)? ']' ;
struct_init: IDENTIFIER '{' (init_field (',' init_field)*)? '}' ;
init_field: IDENTIFIER ':' expression ;

member_access: expression '.' IDENTIFIER ;


// ------------------ POSTFIX ------------------
postfix_expr: primary_expr (postfix_op)* ;
postfix_op
    : '.' IDENTIFIER
    | '(' arg_list? ')'
    ;

// --- SHADERS ---
shader_decl: '@shader' '(' shader_type ')' function_decl ;
shader_type: 'vertex' | 'fragment' | 'compute' ;
