using System;
using System.Text;
using Generated;

class CodeGen
{
    public string GenStructDecl(RaceLangParser.Struct_declContext ctx)
    {
        var sb = new StringBuilder();
        sb.AppendLine("typedef struct{");

        var visitor = new AstVisitor();
        var structDecl = visitor.Visit(ctx) as StructDeclNode;
        foreach (var f in structDecl.Fields)
        {
            sb.AppendLine(GenFieldDecl(f));
        }

        sb.AppendLine($"}} {structDecl.Name};");
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

    /* Engine myEngine = {
    .horsepower = 450,
    .torque = 550.5f
}; */
    public string GenEngineInstance(RaceLangParser.Engine_instanceContext ctx)
    {
        var sb = new StringBuilder();
        var visitor = new AstVisitor();
        var engineInstance = visitor.Visit(ctx) as EngineInstanceNode;
        sb.AppendLine($"Engine {engineInstance?.Name} = {{");
        if (engineInstance.Fields != null)
        {
            foreach (var f in engineInstance.Fields)
            {
                if (f != null)
                {
                Console.WriteLine($"[DEBUG] Field: {f.Name}, Value: {f.Value}");
                    if (f.Name != null && f.Value != null)
                    {


                        sb.AppendLine(GenFieldValue(f));
                    }
                }
            }
        }
        sb.AppendLine("};");
        return sb.ToString();
    }
    public string GenFieldValue(FieldValueNode fieldValue){
return $".{fieldValue.Name} = {fieldValue.Value},";
    }
}
