using System;
using System.Collections.Generic;
using System.Linq;

// --- AST Base Nodes ---
abstract class AstNode {}

abstract class ExprNode : AstNode {}

class ProgramNode : AstNode{
    public List<AstNode> Items {get; set;}
}

// --- Literals ---
class NumberLiteralExpr : ExprNode
{
    public string Value { get; set; }
}

class StringLiteralExpr : ExprNode
{
    public string Value { get; set; }
}

class BoolLiteralExpr : ExprNode
{
    public bool Value { get; set; }
}

// --- Identifiers ---
class IdentifierExprNode : ExprNode
{
    public string Name { get; set; }
    public string ID {get; set;}
}

// --- Unary Expressions ---
class UnaryExprNode : ExprNode
{
    public string Op { get; set; }
    public ExprNode Operand { get; set; }
}

// --- Binary Expressions ---
class BinaryExprNode : ExprNode
{
    public ExprNode Left { get; set; }
    public string Op { get; set; }
    public ExprNode Right { get; set; }
}

// --- Postfix Expressions (Member Access / Function Call) ---
class PostfixExprNode : ExprNode
{
    public ExprNode Base { get; set; }
    public List<ExprNode> PostfixOps { get; set; } = new List<ExprNode>();
}

// --- Instances ---
abstract class InstanceNode : AstNode
{
    public string Kind {get;set;}
    public string Name { get; set; }
    public List<FieldValueNode> Fields { get; } = new();
}

class CarInstanceNode : InstanceNode { }
class EngineInstanceNode : InstanceNode { }
class TrackInstanceNode : InstanceNode { }
class RaceInstanceNode : InstanceNode { }

class FieldValueNode : AstNode
{
    public string Name { get; set; }
    public ExprNode Value { get; set; }
}

// --- Structs ---
class StructDeclNode : AstNode
{
    public string Name { get; set; }
    public List<FieldDeclNode> Fields { get; } = new();
}

class FieldDeclNode : AstNode
{
    public string Name { get; set; }
    public string Type { get; set; }
}

// --- Variable Declarations ---
class VarDeclNode : AstNode
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsMutable { get; set; }
    public AstNode? Initializer {get; set;}
}

// --- Assignments ---
class AssignmentNode : AstNode
{
    public ExprNode Left { get; set; }
    public string Op { get; set; }
    public ExprNode Right { get; set; }
}

// --- Systems ---
class SystemDeclNode : AstNode
{
    public string Name { get; set; }
    public List<VarDeclNode> Params { get; set; } = new();
    public List<AstNode> Statements { get; set; } = new();
    public bool IsParallel { get; set; }

}



class StatementNode : AstNode{

}

 class StringNode : AstNode
{
    public string Code;
    public StringNode(string code) { Code = code; }
}

class MemberAccessNode : ExprNode {
    public ExprNode Target {get; set;}
    public string Member {get; set;}
}


class FunctionCallNode : ExprNode {
    public ExprNode Function {get; set;}
    public List<ExprNode> Arguments {get; set;}
}