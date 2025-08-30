using System;
using System.Text;
using Generated;

class CodeGen
{
    string structDeclName;

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

    public string GenFieldDecl(FieldDeclNode fieldDecl)
    {
        string MapType(string raceType) =>
            raceType switch
            {
                "i32" => "int",
                "f32" => "float",
                "string" => "char *",
                _ => "unknown type",
            };

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

        sb.AppendLine("}");

        return sb.ToString();
    }

    /* char* result = "test"; */
    public string GenVarDecl(VarDeclNode varDecl)
    {
        var sb = new StringBuilder();
        var value = (varDecl.Initializer as NumberLiteralExpr).Value;
        if(value.EndsWith("Nm")){
            value = value.Remove(value.IndexOf("Nm"));

        sb.AppendLine($"{varDecl.Type} {varDecl.Name} = {value};");
        }

        return sb.ToString();
    }
    public string GenFnCall(FunctionCallNode fnCall){

        string str = $"{fnCall.Name}{fnCall.Arguments};";

/*         foreach(var arg in fnCall.Arguments){

        str += arg;
        }
        str += ")";
 */        return str;
    }
}
