using System;
using System.Text;
using Generated;

class CodeGen
{
    string structDeclName;
    public bool include;

    public string GenIncludeIO()
    {
        return "#include <stdio.h>\n";
    }

    public string GenStructDecl(StructDeclNode structDecl)
    {
        var sb = new StringBuilder();
        sb.AppendLine("typedef struct{");

        foreach (var f in structDecl.Fields)
        {
            sb.AppendLine(GenFieldDecl(f));
        }
        structDeclName = structDecl.Name;
        sb.AppendLine($"}} {structDeclName};");
        return sb.ToString();
    }

    string MapType(string raceType) =>
        raceType switch
        {
            "i32" => "int",
            "f32" => "float",
            "string" => "char *",
            _ => "unknown type",
        };

    public string GenFieldDecl(FieldDeclNode fieldDecl)
    {
        return $"{MapType(fieldDecl.Type)} {fieldDecl.Name};";
    }

    /* Engine myEngine  = {
    .horsepower = 450,
    .torque = 550.5f
}; */
    public string GenEngineInstance(EngineInstanceNode engineInstance)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{structDeclName} {engineInstance?.Name} = {{");
        if (engineInstance.Fields != null)
        {
            int index = 0;
            foreach (var f in engineInstance.Fields)
            {
                if (f != null)
                {
                    // Console.WriteLine($"[DEBUG] Field: {f.Name},");

                    int last = engineInstance.Fields.Count - 1;
                    // with comma
                    if (index < last)
                    {
                        sb.AppendLine(GenFieldValue(f, true));
                    }
                    else
                    {
                        sb.AppendLine(GenFieldValue(f, false));
                    }
                    index++;
                }
            }
        }
        sb.AppendLine("};");
        return sb.ToString();
    }

    string NameValue(string name, string value, bool isComma)
    {
        if (isComma)
        {
            return $".{name} = {value},";
        }
        else
        {
            return $".{name} = {value}";
        }
    }

    public string GenFieldValue(FieldValueNode fieldValue, bool isComma)
    {
        if (fieldValue.ValueNode is NumberLiteralExpr num)
        {
            return NameValue(fieldValue.Name, num.Value, isComma);
        }
        else if (fieldValue.ValueNode is StringLiteralExpr str)
        {
            return NameValue(fieldValue.Name, str.Value, isComma);
        }
        else if (fieldValue.ValueNode is BoolLiteralExpr boolLit)
        {
            return NameValue(fieldValue.Name, boolLit.Value.ToString(), isComma);
        }

        return "";
    }

    /* void main(){
    }
    */
    public string GenFnDecl(FnDeclNode fnDecl)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{fnDecl.Type} {fnDecl.Name}(){{");
        foreach (var stmt in fnDecl.Statements)
        {
            if (stmt is VarDeclNode varDecl)
            {
                sb.AppendLine(GenVarDecl(varDecl));
            }
            else if (stmt is ExprStmtNode exprStmt)
            {
                sb.AppendLine(GenExpr(exprStmt.Expression));
            }
        }
        sb.AppendLine("}");

        return sb.ToString();
    }

    public string GenExpr(ExprNode exprNode)
    {
        switch (exprNode)
        {
            case FunctionCallNode fnCall:
                return GenFnCall(fnCall);
            case MemberAccessNode memberAccess:

                return GenMemberAccess(memberAccess);
            case IdentifierExprNode idExpr:
            {
                return idExpr.Name;
            }
        }
        return "";
    }

    /* char* result = "test"; */
    public string GenVarDecl(VarDeclNode varDecl)
    {
        var sb = new StringBuilder();
        var value = (varDecl.Initializer as NumberLiteralExpr).Value;
        if (value.EndsWith("Nm"))
        {
            value = value.Remove(value.IndexOf("Nm"));

            sb.AppendLine($"{MapType(varDecl.Type)} {varDecl.Name} = {value};");
        }

        return sb.ToString();
    }

    public string GenFnCall(FunctionCallNode fnCall)
    {
        Console.WriteLine("generating fn call");

        var sb = new StringBuilder();
        if (fnCall.Name == "println")
        {
            Console.WriteLine("detected println");

            string str = "";
            include = true;

            // as reference: printf("torque: %d\n", torque);

            for (int i = 0; i < fnCall.Arguments.Count; i++)
            {
                Console.WriteLine("arg type " + fnCall.Arguments[i]);
            }

            var firstArg = fnCall.Arguments[0];

            ExprNode secondArg = null;

            if (fnCall.Arguments.Count > 1)
            {
                secondArg = fnCall?.Arguments[1];
            }

            // torque: in format specifier
            if (firstArg != null && firstArg is StringLiteralExpr strLit)
            {
                string firstArgValue = strLit.Value.Trim('"');

                Console.WriteLine("1st arg: " + firstArgValue);

                str += $"printf(\"{firstArgValue}";

                str += "%d";

                str += "\\n\"";
            }

            Console.WriteLine("fn name, args count: " + fnCall.Name + " " + fnCall.Arguments.Count);

            if (secondArg == null)
            {
                Console.WriteLine("second arg null");
            }
            Console.WriteLine("second arg: " + secondArg);

            // torque

            str += $", {GenExpr(secondArg)}";

            str += ");";

            return str;
        }
        // string str = $"{fnCall.Name}({fnCall.Arguments});";

        /*         foreach(var arg in fnCall.Arguments){
        
                str += arg;
                }
                str += ")";
         */
        return $"{fnCall.Name}({fnCall.Arguments.ToString()})";
    }

    public string GenMemberAccess(MemberAccessNode memberAccess)
    {
        string targetValue = (memberAccess.Target as IdentifierExprNode).Name;
        return $"{targetValue}.{memberAccess.Member}";
    }
}
