using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Generated;

class Program
{
    static void PrintTree(IParseTree tree, Parser parser, string indent = "", bool last = true)
    {
        // Wypisz bieżący węzeł
        string nodeText = Trees.GetNodeText(tree, parser);
        Console.Write(indent);
        if (last)
        {
            Console.Write("└──");
            indent += "   ";
        }
        else
        {
            Console.Write("├──");
            indent += "│  ";
        }
        Console.WriteLine(nodeText);

        // Dzieci
        for (int i = 0; i < tree.ChildCount; i++)
        {
            PrintTree(tree.GetChild(i), parser, indent, i == tree.ChildCount - 1);
        }
    }

    static void Main(string[] args)
    {
        var sb = new StringBuilder();
        var inputFile = args[0];
        var input = new AntlrFileStream(inputFile);

        var lexer = new RaceLangLexer(input);

        var tokens = new CommonTokenStream(lexer);

        var parser = new RaceLangParser(tokens);

        var tree = parser.program();

        var visitor = new CodeGen();

        // Console.WriteLine(tree.ToStringTree());

        // PrintTree(tree, parser);

        /*
        tokens.Fill();
        foreach (var t in tokens.GetTokens())
        {
        
            Console.WriteLine($"{t.Text} : {t.Type}");
        }
        */

        // var structDecl = (StructDeclNode) visitor.Visit(tree);

        // var addSub = (BinaryExprNode) visitor.Visit(expression);
        //  var left = (NumberLiteralExpr) addSub.Left;
        //  var op = addSub.Op;
        //  var right = (NumberLiteralExpr) addSub.Right;

        //    int MapOp(string op) => op switch{
        //         "+" => int.Parse(left.Value) + int.Parse(right.Value)
        //     };

        //  Console.WriteLine(  MapOp(op)  );

        string MapType(string raceType) =>
            raceType switch
            {
                "i32" => "int",
                "f32" => "float",
            };
        // string fieldsCode = string.Join("\n", structDecl.Fields.Select(f =>
        //             $"{MapType(f.Type)} {f.Name};"
        // ));

        // Console.WriteLine(fieldsCode);

        // hardcoded temporarily for reference
        // to convert to dynamic code
        string code =
            @"
#include <stdio.h>

// ----------- struct declaration ------------
typedef struct {
    int horsepower;
    float torque;
} Engine;

typedef struct {
    int maxSpeed;
    int weight;
    Engine engine;
    char* color;
} Car;

// ----------- instances ---------------------
Engine myEngine = {
    .horsepower = 450,
    .torque = 550.5f
};

Car MySuperCar = {
    .maxSpeed = 320,
    .weight   = 1200,  // w C nie ma ""kg"", więc same liczby
    .engine   = { .horsepower = 450, .torque = 550.5f },
    .color    = ""red""
};

// ----------- functions ---------------------
int main() {
    float result = myEngine.horsepower + myEngine.torque;
    printf(""result = %.2f\n"", result);

    printf(""horsepower = %d\n"", myEngine.horsepower);
    printf(""torque = %.2f\n"", myEngine.torque);

    printf(""Car: %s, maxSpeed=%d, weight=%d\n"",
           MySuperCar.color, MySuperCar.maxSpeed, MySuperCar.weight);

    return 0;
}
";
        // Console.WriteLine(code);

        var cFile = "test.c";
        var outputFile = args[2];
        File.WriteAllText(cFile, code);

        // Sprawdzenie, czy plik C istnieje
        if (!File.Exists(cFile))
        {
            Console.WriteLine($"File {cFile} not found!");
            return;
        }




        // Wywołanie GCC
        var gcc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "gcc",
                Arguments = $"{cFile} -o {outputFile}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        gcc.Start();

        // Asynchronicznie odczytujemy stdout/stderr
        string stdout = gcc.StandardOutput.ReadToEnd();
        string stderr = gcc.StandardError.ReadToEnd();

        // Czekamy aż GCC zakończy
        gcc.WaitForExit();

        if (gcc.ExitCode != 0)
        {
            Console.WriteLine("Compilation failed:");
            Console.WriteLine(stderr);
            return;
        }

        if (!string.IsNullOrEmpty(stdout))
            Console.WriteLine(stdout);

        if (!string.IsNullOrEmpty(stderr))
            Console.WriteLine(stderr);

        Console.WriteLine($"Binary created: {outputFile}");


    }
}
