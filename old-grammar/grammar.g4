grammar RaceLang;

program: module* EOF ;

// --- MODULES ---
module: 'module' IDENT import_stmt* top_level_decl* ;
import_stmt: 'import' IDENT ';' ;

// --- TOP LEVEL DECLARATIONS ---
top_level_decl
    : carDecl
    | structDecl
    | engineDecl
    | systemDecl
    | functionDecl
    | trackDecl
    | raceDecl
    ;

// --- IDENTIFIERS ---
IDENT: [a-zA-Z_][a-zA-Z0-9_]* ;

// --- TYPES ---
type
    : 'int'
    | 'float'
    | 'bool'
    | 'string'
    | vecType
    | quatType
    | customType
    ;

vecType: 'vec2' | 'vec3' | 'vec4' ;
quatType: 'quat' ;
customType: IDENT ;

// --- STRUCTS ---
structDecl: 'struct' IDENT '{' fieldDecl* '}' ;
fieldDecl: IDENT ':' type ';' ;

// --- CARS & ENGINES ---
carDecl: 'car' IDENT '{' carField* '}' ;
carField: IDENT ':' expression ';' ;

engineDecl: 'engine' IDENT '{' engineField* '}' ;
engineField: IDENT ':' expression ';' ;

// --- TRACK & RACE ---
trackDecl: 'track' IDENT '{' trackField* '}' ;
trackField: IDENT ':' expression ';' ;

raceDecl: 'race' IDENT '{' raceField* '}' ;
raceField: IDENT ':' expression ';' ;

// --- FUNCTIONS ---
functionDecl: 'fn' IDENT '(' paramList? ')' ('->' type)? block ;
paramList: param (',' param)* ;
param: IDENT ':' type ;

// --- SYSTEMS ---
systemDecl: 'system' IDENT '(' IDENT ':' type ')' 'query' '{' componentList '}' ('parallel')? block ;
componentList: IDENT (',' IDENT)* ;

// --- BLOCKS & STATEMENTS ---
block: '{' statement* '}' ;
statement
    : varDecl
    | assignment
    | ifStmt
    | forStmt
    | exprStmt
    | returnStmt
    ;

varDecl: 'let' ('mut')? IDENT ':' type '=' expression ';' ;
assignment: IDENT ('=' | '+=' | '-=' | '*=' | '/=') expression ';' ;
ifStmt: 'if' '(' expression ')' block ('else' block)? ;
forStmt: 'for' IDENT 'in' expression block ;
returnStmt: 'return' expression? ';' ;
exprStmt: expression ';' ;

// --- EXPRESSIONS ---
expression
    : literal
    | IDENT
    | functionCall
    | binaryExpr
    | unaryExpr
    | arrayExpr
    | structInit
    | memberAccess
    ;

functionCall: IDENT '(' argList? ')' ;
argList: expression (',' expression)* ;

binaryExpr: expression binaryOp expression ;
binaryOp: '+' | '-' | '*' | '/' | '==' | '!=' | '<' | '<=' | '>' | '>=' | '&&' | '||' ;

unaryExpr: unaryOp expression ;
unaryOp: '+' | '-' | '!' ;

arrayExpr: '[' (expression (',' expression)*)? ']' ;
structInit: IDENT '{' (initField (',' initField)*)? '}' ;
initField: IDENT ':' expression ;

memberAccess: expression '.' IDENT ;

// --- LITERALS ---
literal: number | stringLit | boolLit ;

number: (INT | FLOAT) unit? ;
INT: [0-9]+ ;
FLOAT: [0-9]+ '.' [0-9]+ ;
unit: 'kg' | 'm' | 's' | 'Nm' | 'hp' ;

stringLit: '"' (~["\r\n])* '"' ;
boolLit: 'true' | 'false' ;

// --- SHADERS ---
shaderDecl: '@shader' '(' shaderType ')' functionDecl ;
shaderType: 'vertex' | 'fragment' | 'compute' ;

// --- WHITESPACE ---
WS: [ \t\r\n]+ -> skip ;