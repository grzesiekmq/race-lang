using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Generated;

class AstVisitor : RaceLangBaseVisitor<AstNode>
{
    // TODO: complete visit program
    public override AstNode VisitProgram(RaceLangParser.ProgramContext ctx)
    {
        Console.WriteLine("visiting program");

        foreach (var module in ctx.module())
        {
            Visit(module);
        }
        return base.VisitProgram(ctx);
    }

    // ------------ TOP LEVEL ------------
    public override AstNode VisitTop_level(RaceLangParser.Top_levelContext ctx)
    {
        Console.WriteLine("visiting top level");

        var topLevel = new TopLevelNode();
        foreach (var itemCtx in ctx.top_level_item())
        {
            var item = Visit(itemCtx);

            topLevel.Items.Add(item);
        }
        return topLevel;
    }

    public override AstNode VisitTop_level_item(RaceLangParser.Top_level_itemContext ctx)
    {
        Console.WriteLine("visiting top level item");

        return Visit(ctx.GetChild(0));
    }

    // ------------ STRUCTS ------------

    /* typedef struct {
    int horsepower;
    float torque;
} Engine;
*/
    public override AstNode VisitStruct_decl(RaceLangParser.Struct_declContext ctx)
    {
        Console.WriteLine("visiting struct decl");

        var structDecl = new StructDeclNode { Name = ctx.IDENTIFIER()?.GetText() };

        // Console.WriteLine("struct decl: " + structDecl);

        var fields = ctx.field_decl();
        if (fields != null)
        {
            foreach (var f in fields)
            {
                var fieldNode = Visit(f) as FieldDeclNode;

                structDecl.Fields.Add(fieldNode);

                // Console.WriteLine("added field " + fieldNode);
            }
            // Console.WriteLine("fields count " + structDecl.Fields.Count);
        }

        return structDecl;
    }

    public override AstNode VisitField_decl(RaceLangParser.Field_declContext ctx)
    {
        var fieldDecl = new FieldDeclNode();

        fieldDecl.Type = ctx.type()?.GetText();
        fieldDecl.Name = ctx.name_token()?.GetText();

        // Console.WriteLine($"[DEBUG] Field parsed: {fieldDecl.Type} {fieldDecl.Name}");

        return fieldDecl;
    }

    // ------------ SYSTEMS ------------
    public override AstNode VisitSystem_decl(RaceLangParser.System_declContext ctx)
    {
        Console.WriteLine("visiting system decl");

        foreach (var stmt in ctx.statement())
            Visit(stmt);
        return base.VisitSystem_decl(ctx);
    }

    // ------------ INSTANCES ------------
    InstanceNode BuildInstance(
        string name,
        IEnumerable<RaceLangParser.Field_valueContext> fields,
        string kind
    )
    {
        InstanceNode instanceNode = null;

        if (kind == "car")
        {
            instanceNode = new CarInstanceNode { Name = name };
        }
        else if (kind == "engine")
        {
            instanceNode = new EngineInstanceNode { Name = name };
        }
        else if (kind == "track")
        {
            instanceNode = new TrackInstanceNode { Name = name };
        }
        else if (kind == "race")
        {
            instanceNode = new RaceInstanceNode { Name = name };
        }
        if (fields != null)
        {
            foreach (var f in fields)
            {
                instanceNode.Fields.Add(Visit(f) as FieldValueNode);
            }
        }
        // Console.WriteLine("instance node fields count " + instanceNode.Fields.Count);
        return instanceNode;
    }

    public override AstNode VisitCar_instance(RaceLangParser.Car_instanceContext ctx) =>
        BuildInstance(ctx.IDENTIFIER().GetText(), ctx.field_value(), "car");

    public override AstNode VisitEngine_instance(RaceLangParser.Engine_instanceContext ctx) =>
        BuildInstance(ctx.IDENTIFIER().GetText(), ctx.field_value(), "engine");

    public override AstNode VisitTrack_instance(RaceLangParser.Track_instanceContext ctx) =>
        BuildInstance(ctx.IDENTIFIER().GetText(), ctx.field_value(), "track");

    public override AstNode VisitRace_instance(RaceLangParser.Race_instanceContext ctx) =>
        BuildInstance(ctx.IDENTIFIER().GetText(), ctx.field_value(), "race");

    public override AstNode VisitField_value(RaceLangParser.Field_valueContext ctx)
    {
        var fieldValue = new FieldValueNode
        {
            Name = ctx.name_token().GetText(),
            ValueNode = new NumberLiteralExpr { Value = ctx.primary_expr().GetText() },
        };
        return fieldValue;
    }

    public override AstNode VisitFunction_decl([NotNull] RaceLangParser.Function_declContext ctx)
    {
        Console.WriteLine("visiting fn decl");

        var fnDecl = new FnDeclNode
        {
            Name = ctx.IDENTIFIER().GetText(),
            Type = ctx.type().GetText(),
        };

        foreach (var stmt in ctx.block().statement())
        {
            var stmtNode = Visit(stmt) as StatementNode;
            fnDecl.Statements.Add(stmtNode);
        }

        return fnDecl;
    }

    // ------------ STATEMENTS ------------
    public override AstNode VisitVar_decl_stmt(RaceLangParser.Var_decl_stmtContext ctx)
    {
        Console.WriteLine("visiting var decl");

        if (ctx.primary_expr() != null)
        {
            var varDecl = new VarDeclNode
            {
                Name = ctx.IDENTIFIER().GetText(),
                Type = ctx.type().GetText(),
                IsMutable = ctx.GetText().Contains("mut"),
                Initializer = Visit(ctx.primary_expr()) as LiteralExpr,
            };
            return varDecl;
        }
        return base.VisitVar_decl_stmt(ctx);
    }

    public override AstNode VisitAssignment(RaceLangParser.AssignmentContext ctx)
    {
        Console.WriteLine("visiting assignment");

        return Visit(ctx.expression()) as AssignmentNode;
    }

    public override AstNode VisitStatement(RaceLangParser.StatementContext ctx)
    {
        if (ctx.var_decl_stmt() != null)
            return Visit(ctx.var_decl_stmt()) as VarDeclNode;
        if (ctx.assignment() != null)
            return Visit(ctx.assignment()) as AssignmentNode;
        if (ctx.if_stmt() != null)
            return Visit(ctx.if_stmt()) as IfNode;
        if (ctx.for_stmt() != null)
            return Visit(ctx.for_stmt()) as ForNode;
        if (ctx.expression_stmt() != null)
            return Visit(ctx.expression_stmt()) as ExprStmtNode;
        if (ctx.return_stmt() != null)
            return Visit(ctx.return_stmt()) as ReturnNode;

        return base.VisitStatement(ctx);
    }

    // VisitExpression_stmt
    public override AstNode VisitExpression_stmt(RaceLangParser.Expression_stmtContext ctx)
    {
        var exprNode = Visit(ctx.expression());

        if (exprNode is FunctionCallNode fnCall)
        {
            // opakuj w ExprStmtNode
            return new ExprStmtNode { Expression = fnCall };
        }

        return exprNode; // np. inne wyrażenia
    }

    public override AstNode VisitExpression([NotNull] RaceLangParser.ExpressionContext ctx)
    {
        if (ctx.expr1() != null)
            return Visit(ctx.expr1());
        if (ctx.postfix_expr() != null)
            return Visit(ctx.postfix_expr());
        if (ctx.primary_expr() != null)
            return Visit(ctx.primary_expr());

        return base.VisitExpression(ctx);
    }

    public override AstNode VisitArray_expr(RaceLangParser.Array_exprContext context)
    {
        var arrNode = new ArrayExprNode();
        foreach (var e in context.expression())
            arrNode.Elements.Add(Visit(e) as ExprNode);
        return arrNode;
    }

    public override AstNode VisitBlock(RaceLangParser.BlockContext context)
    {
        var blockNode = new BlockNode();
        foreach (var stmtCtx in context.statement())
            blockNode.Statements.Add(Visit(stmtCtx) as StatementNode);
        return blockNode;
    }

    public override AstNode VisitComponent_list(RaceLangParser.Component_listContext context)
    {
        var listNode = new ComponentListNode();
        listNode.Components.Add(new IdentifierExprNode { Name = context.IDENTIFIER(0).GetText() });

        for (int i = 1; i < context.IDENTIFIER().Length; i++)
        {
            listNode.Components.Add(
                new IdentifierExprNode { Name = context.IDENTIFIER(i).GetText() }
            ); // lub konkretny typ
        }
        return listNode;
    }

    //    traditional for( ; ; )

    /* public override AstNode VisitFor_stmt(RaceLangParser.For_stmtContext context)
    {
        var forNode = new ForNode
        {
            Init = Visit(context.init_stmt()) as StatementNode,
            Condition = Visit(context.condition_expr()) as ExprNode,
            Update = Visit(context.update_stmt()) as StatementNode,
            Body = Visit(context.block()) as BlockNode,
        };
        return forNode;
    } */

    // attention VisitFor is for "for in"
    public override AstNode VisitFor_stmt([NotNull] RaceLangParser.For_stmtContext context)
    {
        var forInNode = new ForInNode
        {
            IteratorName = context.IDENTIFIER().GetText(),
            Collection = Visit(context.expression()) as ExprNode,
        };

        foreach (var stmt in context.block().statement())
        {
            forInNode.Body.Add(Visit(stmt) as StatementNode);
        }

        return forInNode;
    }

    public override AstNode VisitIf_stmt(RaceLangParser.If_stmtContext context)
    {
        var ifNode = new IfNode
        {
            Condition = Visit(context.expression()) as ExprNode,
            ThenBlock = Visit(context.block(0)) as BlockNode,
            ElseBlock = Visit(context.block(1)) as BlockNode,
        };

        return ifNode;
    }

    // alias
    public override AstNode VisitArrayExpr(RaceLangParser.ArrayExprContext context)
    {
        return VisitArray_expr(context.array_expr());
    }

    public override AstNode VisitImport_stmt(RaceLangParser.Import_stmtContext context)
    {
        return new ImportNode { ModuleName = context.IDENTIFIER().GetText() };
    }

    public override AstNode VisitModule(RaceLangParser.ModuleContext context)
    {
        var moduleNode = new ModuleNode { Name = context.IDENTIFIER().GetText() };
        foreach (var item in context.top_level())
            moduleNode.Items.Add(Visit(item));
        return moduleNode;
    }

    public override AstNode VisitName_token(RaceLangParser.Name_tokenContext context)
    {
        return new IdentifierExprNode { Name = context.GetText() };
    }

    public override AstNode VisitReturn_stmt(RaceLangParser.Return_stmtContext context)
    {
        return new ReturnNode { ValueNode = Visit(context.expression()) as ExprNode };
    }

    public override AstNode VisitShader_decl(RaceLangParser.Shader_declContext context)
    {
        var shaderNode = new ShaderDeclNode
        {
            Name = context.SHADER_AT().GetText(),
            Type = Visit(context.shader_type()) as ShaderTypeNode,
        };
        return shaderNode;
    }

    public override AstNode VisitShader_type(RaceLangParser.Shader_typeContext context)
    {
        return new ShaderTypeNode { TypeName = context.GetText() };
    }

    public override AstNode VisitInit_field(RaceLangParser.Init_fieldContext context)
    {
        return new FieldValueNode
        {
            Name = context.IDENTIFIER().GetText(),
            ValueNode = Visit(context.expression()) as ExprNode,
        };
    }

    public override AstNode VisitLvalue(RaceLangParser.LvalueContext context)
    {
        return new LValueNode { Name = context.GetText() };
    }

    public override AstNode VisitParam(RaceLangParser.ParamContext context)
    {
        return new ParamNode
        {
            Name = context.IDENTIFIER().GetText(),
            Type = Visit(context.type()) as TypeNode,
        };
    }

    public override AstNode VisitParam_list(RaceLangParser.Param_listContext context)
    {
        var listNode = new ParamListNode();
        foreach (var p in context.param())
            listNode.Params.Add(Visit(p) as ParamNode);
        return listNode;
    }

    public override AstNode VisitPostfix_op(RaceLangParser.Postfix_opContext context)
    {
        // Zależnie od postfixa, możesz tworzyć np. MemberAccessNode lub FunctionCallNode
        return new PostfixOpNode { Text = context.GetText() };
    }

    public override AstNode VisitStruct_init(RaceLangParser.Struct_initContext context)
    {
        var structNode = new StructInitNode { TypeName = context.IDENTIFIER().GetText() };
        foreach (var f in context.init_field())
            structNode.Fields.Add(Visit(f) as FieldValueNode);
        return structNode;
    }

    // alias
    public override AstNode VisitStructInit(RaceLangParser.StructInitContext context)
    {
        return VisitStruct_init(context.struct_init());
    }

    public override AstNode VisitType(RaceLangParser.TypeContext context)
    {
        return new TypeNode { Name = context.GetText() };
    }

    // ------------ EXPRESSIONS ------------
    public override AstNode VisitMulDivExpr(RaceLangParser.MulDivExprContext ctx)
    {
        Visit(ctx.expr1());
        Visit(ctx.expr2());
        return base.VisitMulDivExpr(ctx);
    }

    public override AstNode VisitPassUp1(RaceLangParser.PassUp1Context ctx) => Visit(ctx.expr2());

    public override AstNode VisitAddSubExpr(RaceLangParser.AddSubExprContext ctx)
    {
        Visit(ctx.expr2());
        Visit(ctx.expr3());
        return base.VisitAddSubExpr(ctx);
    }

    public override AstNode VisitPassUp2(RaceLangParser.PassUp2Context ctx) => Visit(ctx.expr3());

    public override AstNode VisitCompareExpr(RaceLangParser.CompareExprContext ctx)
    {
        Visit(ctx.expr3());
        Visit(ctx.expr4());
        return base.VisitCompareExpr(ctx);
    }

    public override AstNode VisitPassUp3(RaceLangParser.PassUp3Context ctx) => Visit(ctx.expr4());

    public override AstNode VisitUnaryMinusExpr(RaceLangParser.UnaryMinusExprContext ctx)
    {
        return Visit(ctx.expr4()) as UnaryExprNode;
    }

    public override AstNode VisitUnaryNotExpr(RaceLangParser.UnaryNotExprContext ctx)
    {
        return Visit(ctx.expr4()) as UnaryExprNode;
    }

    public override AstNode VisitIdExpr(RaceLangParser.IdExprContext ctx)
    {
        Console.WriteLine("visiting ID expr");

        if (ctx.IDENTIFIER() != null)
        {
            var idExpr = new IdentifierExprNode { Name = ctx.IDENTIFIER().GetText() };
            return idExpr;
        }
        return base.VisitIdExpr(ctx);
    }

    public override AstNode VisitLiteralExpr(RaceLangParser.LiteralExprContext ctx)
    {
        Console.WriteLine("visiting literal expr");

        return VisitLiteral(ctx.literal());
    }

    // (expression)
    public override AstNode VisitParenExpr(RaceLangParser.ParenExprContext ctx)
    {
        return Visit(ctx.expression()) as ParenExpr;
    }

    // ------------ LITERAL ------------
    public override AstNode VisitLiteral(RaceLangParser.LiteralContext ctx)
    {
        if (ctx.NUMBER() != null)
        {
            Console.WriteLine("visiting number literal");

            return new NumberLiteralExpr { Value = ctx.NUMBER().GetText() };
        }
        else if (ctx.STRING() != null)
        {
            Console.WriteLine("visiting string literal");

            return new StringLiteralExpr { Value = ctx.STRING().GetText() };
        }
        else if (ctx.BOOL() != null)
        {
            Console.WriteLine("visiting bool literal");

            return new BoolLiteralExpr { Value = ctx.BOOL().GetText() };
        }

        return base.VisitLiteral(ctx);
    }

    // ------------ POSTFIX ------------
    public override AstNode VisitPostfix_expr(RaceLangParser.Postfix_exprContext ctx)
    {
        // baza: IDENTIFIER
        ExprNode result = new IdentifierExprNode { Name = ctx.IDENTIFIER().GetText() };

        foreach (var op in ctx.postfix_op())
        {
            // member access: '.' IDENTIFIER
            if (op.IDENTIFIER() != null)
            {
                result = new MemberAccessNode
                {
                    Target = result,
                    Member = op.IDENTIFIER().GetText(),
                };
            }
            else
            {
                // funkcja: '(' arg_list? ')'
                var call = new FunctionCallNode
                {
                    // jeśli potrzebujesz nazwy, spróbuj wziąć z identyfikatora;
                    // dla wywołań typu obj.method(...) i tak zwykle nie używasz Name,
                    // tylko generujesz z Callee/Target.
                    Name = (result as IdentifierExprNode)?.Name,
                    // jeżeli masz w typie FunctionCallNode pole na callee/target, ustaw tutaj:
                    // Callee = result
                };

                if (op.arg_list() != null)
                {
                    foreach (var e in op.arg_list().expression())
                    {
                        var arg = Visit(e) as ExprNode;
                        if (arg != null)
                            call.Arguments.Add(arg);
                    }
                }

                result = call; // kontynuujemy łańcuch (np. foo()(...) albo foo().bar)
            }
        }

        return result;
    }
}
