using System;

public static class AstPrinter
{
    internal static void Print(AstNode node, string indent = "")
    {
        if (node == null)
            return;
        string indentation = indent;

        if (node is TopLevelNode topLevel)
        {
            Console.WriteLine($"{indentation}TopLevelNode");
            foreach (var item in topLevel.Items)
            {
                Print(item, indentation + "  ");
            }
        }
        else if (node is StructDeclNode structDecl)
        {
            Console.WriteLine($"{indentation}StructDeclNode: {structDecl.Name}");
            foreach (var field in structDecl.Fields)
            {
                Print(field, indentation + "  ");
            }
        }
        else if (node is FieldDeclNode field)
        {
            Console.WriteLine($"{indentation}FieldDeclNode: {field.Type} {field.Name}");
        }
        else if (node is EngineInstanceNode engine)
        {
            Console.WriteLine($"{indentation}EngineInstanceNode: {engine.Name}");
            foreach (var f in engine.Fields)
                Print(f, indentation + "  ");
        }
        else if (node is FieldValueNode fieldValue)
        {
            Console.WriteLine(
                $"{indentation}FieldValueNode: {fieldValue.Name} = {fieldValue.ValueNode}"
            );
        }
        else if (node is FnDeclNode fnDecl)
        {
            Console.WriteLine($"{indentation}FnDeclNode: {fnDecl.Name}");
            if (fnDecl.Params != null)
            {
                Console.WriteLine($"{indentation}  Params:");
                foreach (var param in fnDecl.Params)
                    Print(param, indentation + "    ");
            }
            if (fnDecl.Statements != null)
            {
                Console.WriteLine($"{indentation}  Statements:");
                foreach (var stmt in fnDecl.Statements)
                    Print(stmt, indentation + "    ");
            }
        }
        else if (node is VarDeclNode varDecl)
        {
            Console.WriteLine(
                $"{indentation}VarDeclNode: {varDecl.Type} {varDecl.Name} = {varDecl.Initializer}"
            );
        }
        else if (node is FunctionCallNode fnCall)
        {
            Console.WriteLine($"{indentation}FunctionCallNode: {fnCall.Name}({fnCall.Arguments})");
        }
        else if (node is IfNode ifNode)
        {
            Console.WriteLine($"{indentation}IfNode:");
            Console.WriteLine($"{indentation}  Condition:");
            Print(ifNode.Condition, indentation + "    ");
            Console.WriteLine($"{indentation}  ThenBlock:");
            foreach (var stmt in ifNode.ThenBlock.Statements)
                Print(stmt, indentation + "    ");
            if (ifNode.ElseBlock != null)
            {
                Console.WriteLine($"{indentation}  ElseBlock:");
                foreach (var stmt in ifNode.ElseBlock.Statements)
                    Print(stmt, indentation + "    ");
            }
        }
        else if (node is ForNode forNode)
        {
            Console.WriteLine($"{indentation}ForNode:");
            Console.WriteLine($"{indentation}  Init:");
            Print(forNode.Init, indentation + "    ");
            Console.WriteLine($"{indentation}  Condition:");
            Print(forNode.Condition, indentation + "    ");
            Console.WriteLine($"{indentation}  Update:");
            Print(forNode.Update, indentation + "    ");
            Console.WriteLine($"{indentation}  Body:");
            foreach (var stmt in forNode.Body.Statements)
                Print(stmt, indentation + "    ");
        }
        else if (node is ForInNode forInNode)
        {
            Console.WriteLine(
                $"{indentation}ForInNode: {forInNode.IteratorName} in {forInNode.Collection}"
            );
            foreach (var stmt in forInNode.Body)
                Print(stmt, indentation + "  ");
        }
        else if (node is ExprStmtNode exprStmt)
        {
            Console.WriteLine($"{indentation}ExprStmtNode:");
            Print(exprStmt.Expression, indentation + "  ");
        }
        else if (node is ReturnNode returnNode)
        {
            Console.WriteLine($"{indentation}ReturnNode:");
            Print(returnNode.ValueNode, indentation + "  ");
        }
        else if (node is ArrayExprNode array)
        {
            Console.WriteLine($"{indentation}ArrayExprNode:");
            foreach (var elem in array.Elements)
                Print(elem, indentation + "  ");
        }
        else if (node is BlockNode block)
        {
            Console.WriteLine($"{indentation}BlockNode:");
            foreach (var stmt in block.Statements)
                Print(stmt, indentation + "  ");
        }
        else if (node is ComponentListNode compList)
        {
            Console.WriteLine($"{indentation}ComponentListNode:");
            foreach (var comp in compList.Components)
                Print(comp, indentation + "  ");
        }
        else if (node is ModuleNode module)
        {
            Console.WriteLine($"{indentation}ModuleNode: {module.Name}");
            foreach (var item in module.Items)
                Print(item, indentation + "  ");
        }
        else if (node is ShaderDeclNode shader)
        {
            Console.WriteLine(
                $"{indentation}ShaderDeclNode: {shader.Name} Type: {shader.Type.TypeName}"
            );
        }
        else if (node is ParamNode param)
        {
            Console.WriteLine($"{indentation}ParamNode: {param.Type.Name} {param.Name}");
        }
        else if (node is TypeNode typeNode)
        {
            Console.WriteLine($"{indentation}TypeNode: {typeNode.Name}");
        }
        else if (node is LValueNode lval)
        {
            Console.WriteLine($"{indentation}LValueNode: {lval.Name}");
        }
        else if (node is IdentifierExprNode idExpr)
        {
            Console.WriteLine($"{indentation}IdentifierExprNode: {idExpr.Name}");
        }
        else if (node is NumberLiteralExpr numLit)
        {
            Console.WriteLine($"{indentation}NumberLiteralExpr: {numLit.Value}");
        }
        else if (node is StringLiteralExpr strLit)
        {
            Console.WriteLine($"{indentation}StringLiteralExpr: {strLit.Value}");
        }
        else if (node is BoolLiteralExpr boolLit)
        {
            Console.WriteLine($"{indentation}BoolLiteralExpr: {boolLit.Value}");
        }
        else
        {
            Console.WriteLine($"{indentation}Unknown node type: {node.GetType().Name}");
        }
    }
}
