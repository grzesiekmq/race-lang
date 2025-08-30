using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// --- AST Base Nodes ---
abstract class AstNode { }

abstract class ExprNode : AstNode { }

class ProgramNode : AstNode
{
    public List<AstNode> Items { get; set; } = new();
}

class TopLevelNode : AstNode
{
    public List<AstNode> Items { get; set; } = new();
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
    public ExprNode ValueNode { get; set; }
}

// --- Structs ---
class StructDeclNode : TopLevelNode
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

class FnDeclNode : TopLevelNode{

    public string Name { get; set; }
    public List<AstNode>? Params { get; set; } = new();
    public List<StatementNode>? Statements { get; set; } = new();
    public string? Type { get; set; }
}





// --- Variable Declarations ---
class VarDeclNode : StatementNode
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsMutable { get; set; }
    public LiteralExpr? Initializer { get; set; }
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

class StatementNode : AstNode {

 }

class IfNode : StatementNode{
    public ExprNode Condition { get; set; }
    public BlockNode ThenBlock { get; set; }
    public BlockNode ElseBlock { get; set; }
}

class ForNode : StatementNode{
    public StatementNode Init { get; set; }
    public ExprNode Condition { get; set; }
    public StatementNode Update { get; set; }
    public BlockNode Body { get; set; }
}

class ForInNode: StatementNode{
    public string IteratorName { get; set; }        // nazwa zmiennej iterującej
    public ExprNode Collection { get; set; }        // wyrażenie po którym iterujemy
    public List<StatementNode> Body { get; set; } = new();
}

class ExprStmtNode : StatementNode{
    public ExprNode Expression { get; set; }
}

class ReturnNode : StatementNode{
    public ExprNode ValueNode { get; set; }
}

class ArrayExprNode :AstNode {
    public List<AstNode> Elements { get; set; } = new();
}

class BlockNode : AstNode {
    public List<AstNode> Statements { get; set; } = new();

}
class ComponentListNode :AstNode{
        public List<AstNode> Components { get; set; } = new();

}

class ImportNode :AstNode{
    public string ModuleName;
}

class ModuleNode :AstNode{
    public string Name;
        public List<AstNode> Items { get; set; } = new();

}

class ShaderDeclNode :AstNode{
    public string Name;
    public ShaderTypeNode Type;
}

class ShaderTypeNode :AstNode{
    public string TypeName;
}

class LValueNode :AstNode{
    public string Name;
}

class ParamNode :AstNode{
    public string Name;
    public TypeNode Type;
}

class TypeNode :AstNode{
    public string Name;

}

class ParamListNode :AstNode{
        public List<AstNode> Params { get; set; } = new();
    
}

class PostfixOpNode :AstNode{
    public string Text;
}

class StructInitNode :AstNode{
    public string TypeName;
            public List<AstNode> Fields { get; set; } = new();

}




class MemberAccessNode : ExprNode
{
    public ExprNode Target { get; set; }
    public string Member { get; set; }
}

class FunctionCallNode : StatementNode
{
    public string Name { get; set; }
    // public List<ExprNode> Arguments { get; set; } = new();

    public string Arguments { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
