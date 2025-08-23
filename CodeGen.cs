using System;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Generated;
using System.Collections.Generic;

class CodeGen : RaceLangBaseVisitor<StringNode>
{
    // ------------ TOP LEVEL ------------
    public override StringNode VisitTop_level(RaceLangParser.Top_levelContext ctx)
    {
        foreach (var item in ctx.top_level_item())
            Visit(item);
        return new StringNode(""); // puste, do wypełnienia
    }

    public override StringNode VisitTop_level_item(RaceLangParser.Top_level_itemContext ctx)
    {
        return Visit(ctx.GetChild(0));
    }

    // ------------ STRUCTS ------------
    public override StringNode VisitStruct_decl(RaceLangParser.Struct_declContext ctx)
    {
        foreach (var f in ctx.field_decl())
            Visit(f);
        return new StringNode("");
    }

    public override StringNode VisitField_decl(RaceLangParser.Field_declContext ctx)
    {
        return new StringNode("");
    }

    // ------------ SYSTEMS ------------
    public override StringNode VisitSystem_decl(RaceLangParser.System_declContext ctx)
    {
        foreach (var stmt in ctx.statement())
            Visit(stmt);
        return new StringNode("");
    }

    // ------------ INSTANCES ------------
    InstanceNode BuildInstance(string name, IEnumerable<RaceLangParser.Field_valueContext> fields, string kind)
    {
        foreach (var f in fields)
            Visit(f);
        return null; // możesz użyć StringNode, zostawiam puste
    }

    public override StringNode VisitCar_instance(RaceLangParser.Car_instanceContext ctx)
        => VisitCar_instance_helper(ctx.IDENTIFIER().GetText(), ctx.field_value(), "car");
    public override StringNode VisitEngine_instance(RaceLangParser.Engine_instanceContext ctx)
        => VisitCar_instance_helper(ctx.IDENTIFIER().GetText(), ctx.field_value(), "engine");
    public override StringNode VisitTrack_instance(RaceLangParser.Track_instanceContext ctx)
        => VisitCar_instance_helper(ctx.IDENTIFIER().GetText(), ctx.field_value(), "track");
    public override StringNode VisitRace_instance(RaceLangParser.Race_instanceContext ctx)
        => VisitCar_instance_helper(ctx.IDENTIFIER().GetText(), ctx.field_value(), "race");

    private StringNode VisitCar_instance_helper(string name, IEnumerable<RaceLangParser.Field_valueContext> fields, string kind)
    {
        foreach (var f in fields)
            Visit(f);
        return new StringNode("");
    }

    public override StringNode VisitField_value(RaceLangParser.Field_valueContext ctx)
    {
        Visit(ctx.expression());
        return new StringNode("");
    }

    // ------------ STATEMENTS ------------
    public override StringNode VisitVar_decl_stmt(RaceLangParser.Var_decl_stmtContext ctx)
    {
        if (ctx.expression() != null)
            Visit(ctx.expression());
        return new StringNode("");
    }

    public override StringNode VisitAssignment(RaceLangParser.AssignmentContext ctx)
    {
        Visit(ctx.expression());
        return new StringNode("");
    }

    public override StringNode VisitStatement(RaceLangParser.StatementContext ctx)
    {
        if (ctx.var_decl_stmt() != null) return Visit(ctx.var_decl_stmt());
        if (ctx.assignment() != null) return Visit(ctx.assignment());
        if (ctx.if_stmt() != null) return Visit(ctx.if_stmt());
        if (ctx.for_stmt() != null) return Visit(ctx.for_stmt());
        if (ctx.expression_stmt() != null) return Visit(ctx.expression_stmt());
        if (ctx.return_stmt() != null) return Visit(ctx.return_stmt());
        return new StringNode("");
    }

    // ------------ EXPRESSIONS ------------
    public override StringNode VisitMulDivExpr(RaceLangParser.MulDivExprContext ctx)
    {
        Visit(ctx.expr1());
        Visit(ctx.expr2());
        return new StringNode("");
    }

    public override StringNode VisitPassUp1(RaceLangParser.PassUp1Context ctx) => Visit(ctx.expr2());
    public override StringNode VisitAddSubExpr(RaceLangParser.AddSubExprContext ctx)
    {
        Visit(ctx.expr2());
        Visit(ctx.expr3());
        return new StringNode("");
    }
    public override StringNode VisitPassUp2(RaceLangParser.PassUp2Context ctx) => Visit(ctx.expr3());
    public override StringNode VisitCompareExpr(RaceLangParser.CompareExprContext ctx)
    {
        Visit(ctx.expr3());
        Visit(ctx.expr4());
        return new StringNode("");
    }
    public override StringNode VisitPassUp3(RaceLangParser.PassUp3Context ctx) => Visit(ctx.expr4());

    public override StringNode VisitUnaryMinusExpr(RaceLangParser.UnaryMinusExprContext ctx)
    {
        Visit(ctx.expr4());
        return new StringNode("");
    }

    public override StringNode VisitUnaryNotExpr(RaceLangParser.UnaryNotExprContext ctx)
    {
        Visit(ctx.expr4());
        return new StringNode("");
    }

    public override StringNode VisitIdExpr(RaceLangParser.IdExprContext ctx)
    {
        return new StringNode(""); 
    }

    public override StringNode VisitLiteralExpr(RaceLangParser.LiteralExprContext ctx)
    {
        return Visit(ctx.literal());
    }

    public override StringNode VisitParenExpr(RaceLangParser.ParenExprContext ctx)
    {
        return Visit(ctx.expression());
    }

    // ------------ LITERAL ------------
    public override StringNode VisitLiteral(RaceLangParser.LiteralContext ctx)
    {
        return new StringNode("");
    }

    // ------------ POSTFIX ------------
    public override StringNode VisitPostfix_expr(RaceLangParser.Postfix_exprContext ctx)
    {
        string result = Visit(ctx.primary_expr()).Code;

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
                    result = "";
                }
                else if (opCtx.GetChild(0).GetText() == ".")
                {
                    result = "";
                }
            }
        }

        return new StringNode(result);
    }

    public override StringNode VisitArg_list(RaceLangParser.Arg_listContext ctx)
    {
        foreach (var e in ctx.expression())
            Visit(e);
        return new StringNode("");
    }
}
