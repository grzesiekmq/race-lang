using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// --- AST Base Nodes ---
abstract class AstNode { }

abstract class ExprNode : AstNode { }

class ProgramNode : AstNode
{
    public List<AstNode> Items { get; set; }
}

class TopLevelNode : AstNode
{
    public List<AstNode> Items { get; set; }
}

// --- Literals ---
class LiteralExpr : ExprNode {

}

class NumberLiteralExpr : LiteralExpr
{
    public string Value { get; set; }
}

class StringLiteralExpr : LiteralExpr
{
    public string Value { get; set; }
}

class BoolLiteralExpr : LiteralExpr
{
    public bool Value { get; set; }
}

class ParenExpr : ExprNode{

}

// --- Identifiers ---
class IdentifierExprNode : ExprNode
{
    public string Name { get; set; }
    public string ID { get; set; }
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
    public string Kind { get; set; }
    public string Name { get; set; }
    public List<FieldValueNode> Fields { get;set; } = new();
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
    public List<FieldDeclNode> Fields { get;set; } = new();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"struct {Name}");
        foreach (var f in Fields)
        {
            sb.AppendLine($"  {f.Type} {f.Name};");
        }
        return sb.ToString();
    }
}

class FieldDeclNode : AstNode
{
    public string Name { get; set; }
    public string Type { get; set; }

    public override string ToString()
    {
        return $"{Name} {Type}";
    }
}

// --- Variable Declarations ---
class VarDeclNode : AstNode
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsMutable { get; set; }
    public AstNode? Initializer { get; set; }
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

class StatementNode : AstNode { }

class IfNode : StatementNode{

}
class ForNode : StatementNode{

}

class ExprStmtNode : StatementNode{

}
class ReturnNode : StatementNode{

}

class MemberAccessNode : ExprNode
{
    public ExprNode Target { get; set; }
    public string Member { get; set; }
}

class FunctionCallNode : ExprNode
{
    public ExprNode Function { get; set; }
    public List<ExprNode> Arguments { get; set; }
}
