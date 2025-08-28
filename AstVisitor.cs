using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Generated;

class AstVisitor : RaceLangBaseVisitor<AstNode>
{
    // TODO: complete visit program
    public override AstNode VisitProgram(RaceLangParser.ProgramContext ctx)
    {
        foreach (var module in ctx.module())
        {
            Visit(module);
        }
        return null;
    }

    // ------------ TOP LEVEL ------------
    public override AstNode VisitTop_level(RaceLangParser.Top_levelContext ctx)
    {
        var topLevel = new TopLevelNode();
        foreach (var itemCtx in ctx.top_level_item())
        {
            var item = Visit(itemCtx);
            if (topLevel.Items != null)
            {
                topLevel.Items.Add(item);
            }
        }
        return topLevel;
    }

    public override AstNode VisitTop_level_item(RaceLangParser.Top_level_itemContext ctx)
    {
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
        var structDecl = new StructDeclNode { Name = ctx.IDENTIFIER()?.GetText() };

        Console.WriteLine("struct decl: " + structDecl);

        var fields = ctx.field_decl();
        if (fields != null)
        {
            foreach (var f in fields)
            {
                var fieldNode = Visit(f) as FieldDeclNode;

                structDecl.Fields.Add(fieldNode);

                // Console.WriteLine("added field " + fieldNode);
            }
            Console.WriteLine("fields count " + structDecl.Fields.Count);
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
        foreach (var stmt in ctx.statement())
            Visit(stmt);
        return null;
    }

    // ------------ INSTANCES ------------
    InstanceNode BuildInstance(
        string name,
        IEnumerable<RaceLangParser.Field_valueContext> fields,
        string kind
    )

    {
        InstanceNode instanceNode = null;

        if (kind == "car"){

         instanceNode = new CarInstanceNode { Name = name };
    }
    else if (kind == "engine"){
            instanceNode = new EngineInstanceNode { Name = name };
        }
        else if (kind == "track"){
            instanceNode = new TrackInstanceNode { Name = name };
        }
        else if (kind == "race"){
            instanceNode = new RaceInstanceNode { Name = name };
        }
        if(fields != null){

        foreach (var f in fields){

            instanceNode.Fields.Add(
                Visit(f) as FieldValueNode);

        }
        }
        Console.WriteLine("instance node fields count " + instanceNode.Fields.Count);
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
        return Visit(ctx.expression()) as FieldValueNode;

    }

    // ------------ STATEMENTS ------------
    public override AstNode VisitVar_decl_stmt(RaceLangParser.Var_decl_stmtContext ctx)
    {
        if (ctx.expression() != null){

          return  Visit(ctx.expression()) as VarDeclNode;
        }
        return null;

    }

    public override AstNode VisitAssignment(RaceLangParser.AssignmentContext ctx)
    {
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
        return null;
    }

    // ------------ EXPRESSIONS ------------
    public override AstNode VisitMulDivExpr(RaceLangParser.MulDivExprContext ctx)
    {
        Visit(ctx.expr1());
        Visit(ctx.expr2());
        return null;
    }

    public override AstNode VisitPassUp1(RaceLangParser.PassUp1Context ctx) => Visit(ctx.expr2());

    public override AstNode VisitAddSubExpr(RaceLangParser.AddSubExprContext ctx)
    {
        Visit(ctx.expr2());
        Visit(ctx.expr3());
        return null;
    }

    public override AstNode VisitPassUp2(RaceLangParser.PassUp2Context ctx) => Visit(ctx.expr3());

    public override AstNode VisitCompareExpr(RaceLangParser.CompareExprContext ctx)
    {
        Visit(ctx.expr3());
        Visit(ctx.expr4());
        return null;
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
        return Visit(ctx.IDENTIFIER()) as IdentifierExprNode;
    }

    public override AstNode VisitLiteralExpr(RaceLangParser.LiteralExprContext ctx)
    {
        return Visit(ctx.literal()) as LiteralExpr;
    }

    // (expression)
    public override AstNode VisitParenExpr(RaceLangParser.ParenExprContext ctx)
    {
        return Visit(ctx.expression()) as ParenExpr;
    }

    // ------------ LITERAL ------------
    public override AstNode VisitLiteral(RaceLangParser.LiteralContext ctx)
    {
        return null;
    }

    // ------------ POSTFIX ------------
    public override AstNode VisitPostfix_expr(RaceLangParser.Postfix_exprContext ctx)
    {
        var result = Visit(ctx.primary_expr());

        if (ctx.postfix_op() != null)
        {
            foreach (var opCtx in ctx.postfix_op())
            {
                if (opCtx.GetChild(0).GetText() == "(")
                {
                    if (opCtx.arg_list() != null)
                    {
                        foreach (var e in opCtx.arg_list().expression())
                            Visit(e);
                    }
                    result = null;
                }
                else if (opCtx.GetChild(0).GetText() == ".")
                {
                    result = null;
                }
            }
        }

        return null;
    }

    public override AstNode VisitArg_list(RaceLangParser.Arg_listContext ctx)
    {
        foreach (var e in ctx.expression())
            Visit(e);
        return null;
    }
}
