


# RaceLang

**RaceLang** is an experimental programming language designed for modeling racing-related objects such as cars, engines, tracks, and races. The project is currently in early development and focuses on creating an **Abstract Syntax Tree (AST)** and a **minimal code generation system** targeting C.  

RaceLang is intended as a learning and prototyping platform for language design, AST manipulation, and code generation.

---

## Features

### Abstract Syntax Tree (AST)
RaceLang uses a rich AST representation to model the language’s structure. Current nodes include:

- **Literals**: number, string, boolean  
- **Identifiers** and variables  
- **Unary and binary expressions**  
- **Function declarations and calls**  
- **Struct declarations** and **instances** (Car, Engine, Track, Race)  
- **System declarations** (experimental)  
- **Statements**: `if`, `for` loops (traditional and `for-in`), `return`  
- **Blocks** and **component lists**  

### AST Visitor
A visitor pattern is implemented for traversing the AST, which allows:

- Inspection of AST nodes  
- Transformation of nodes  
- Code generation  

### Minimal C Code Generation
RaceLang currently supports generating C code for:

- **Struct declarations**  
- **Struct instances** with initialization  
- **Basic type mapping** from RaceLang types to C types:
  - `i32` → `int`
  - `f32` → `float`
  - `string` → `char*`  
- **Function declarations**  
- **Variable declarations** and simple expressions  
- **Function calls**, including a built-in `println` mapped to `printf`  

#### Example: Struct Declaration in RaceLang
```rs
struct Engine {
    horsepower: i32;
    torque: f32;
};
```

Generated C code:

```c
typedef struct {
    int horsepower;
    float torque;
} Engine;
```

#### Example: Instance Initialization

```rs
Engine myEngine = {
    horsepower: 450,
    torque: 550.5
};
```

Generated C code:

```c
Engine myEngine = {
    .horsepower = 450,
    .torque = 550.5
};
```

#### Example: Function Call

```rs
println("Horsepower: ", myEngine.horsepower);
```

Generated C code:

```c
printf("Horsepower: %d\n", myEngine.horsepower);
```

---

## Current Project Status

* **AST**: Almost complete, supports most language constructs
* **AST Visitor**: Fully implemented with debugging support
* **Codegen**: Minimal but functional for structs, instances, functions, and simple expressions
* **Parser**: Based on ANTLR, supports the current syntax of RaceLang

---

## Roadmap

* Extend type mapping and support for more RaceLang types
* Implement full expression evaluation in codegen
* Support complex function calls and member access chains
* Add testing framework for AST and code generation
* Develop module system and imports support
* Expand language features (loops, conditionals, and systems)

---

## Requirements

* **.NET 8 SDK** (or later)
* **Antlr4.Runtime**
* Standard C# libraries: `System.Text`, `System.Collections.Generic`, etc.

---

## Getting Started

Clone the repository and build it using .NET CLI:

```bash
git clone https://github.com/grzesiekmq/race-lang.git
cd race-lang
dotnet build
dotnet run
```

You can then parse RaceLang source files and generate C code using the provided AST and CodeGen classes.



## 💻 Usage Example

After building `racec`, you can compile RaceLang source files to C code with:

```bash
# Compile a single RaceLang file
racec test.race -o test


# then run 
./test
```

Example  
Given a RaceLang source file engine.race:

```rs

struct Engine {
    horsepower: i32;
    torque: f32;
};

Engine myEngine = {
    horsepower: 450,
    torque: 550.5
};

println("Horsepower: ", myEngine.horsepower);
```

Running:

```bash

racec engine.race -o engine

./engine
```

Will output:

```makefile

Horsepower: 450
```

This demonstrates how racec transforms RaceLang code into valid C code, ready to be compiled and executed.

---

## Contributing

Contributions are welcome! Currently, the project is experimental, and any improvements to the AST, visitor, or code generation are highly appreciated.

---

## License

This project is open-source under Unlicense





