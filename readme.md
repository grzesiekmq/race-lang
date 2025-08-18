


---

Race Language (race) — v0.2 Draft Specification (Racing Game Engine Focus)

> A compiled, data-oriented language designed specifically for racing game engines — real-time physics, high-performance rendering, and deterministic simulation.




---

1) Design Goals & Philosophy

Racing Simulation First. Tailored for physics engines, rendering loops, AI, and networking in competitive racing games.

Deterministic Physics. Fixed-step loops, precise floating-point and fixed-point math, reproducibility across machines.

High-Performance Rendering. First-class GPU API bindings (Vulkan, DirectX, Metal) with minimal boilerplate.

Data-Oriented Design. Built-in ECS and SoA, optimized for cache and SIMD.

Predictable Memory. No GC, explicit allocation, arenas for frame-based workloads.

Engine Interop. Easy C/C++ ABI, direct embedding into custom engines.



---

2) Toolchain Overview

Compiler: racec with Cranelift (fast iteration) and LLVM (optimized release) backends.

Package Manager: racem, oriented around engine modules (physics, rendering, input, audio).

Build Scripts: build.race — designed for asset pipelines (shaders, meshes, car data).

Profiler & Debugger: built-in frame timeline inspector.



---

3) Language Surface — Racing Engine Focus

3.1 Game Loop

fn main() {
    let mut engine = Engine::new()
    engine.run(|dt| {
        physics_step(dt)
        render_frame(dt)
        input_poll()
    })
}

3.2 Physics Primitives

struct RigidBody { mass: f32, vel: vec3, pos: vec3 }
struct Wheel { radius: f32, grip: f32 }
struct Engine { torque_curve: fn(f32)->f32, rpm: f32 }

system Physics(dt: f32) query { RigidBody, Wheel, Engine } parallel {
    let tq = Engine.torque_curve(Engine.rpm)
    RigidBody.vel.x += tq * dt / RigidBody.mass
    RigidBody.pos += RigidBody.vel * dt
}

3.3 Rendering Integration

@shader(vertex)
fn vs_main(input: VertexInput) -> VertexOutput { /* GLSL-like */ }

@shader(fragment)
fn fs_main(input: VertexOutput) -> vec4 { return vec4(1,0,0,1) }

fn render_frame(dt: f32) {
    let pass = gfx.begin_pass(clear_color=vec4(0,0,0,1))
    pass.draw(mesh=car_mesh, material=paint)
    gfx.end_pass(pass)
}


---

4) Core Types (Racing Focus)

f32, f64 (fast math); q24.8 fixed-point for deterministic physics.

vec2, vec3, vec4, quat, mat4 — GPU-friendly.

transform type (pos: vec3, rot: quat, scale: vec3).

color type (rgba8, linear_f32).

mesh, texture, shader, pipeline — engine-native resources.

entity / component / system — ECS primitives.



---

5) ECS Racing Extensions

Entities: cars, wheels, track segments.

Components: Engine, Suspension, Tire, TrackSurface.

Systems: physics update, rendering, input handling, AI racing lines.

Deterministic scheduler ensures consistent simulation between machines.



---

6) Concurrency Model

Frame Jobs: tasks scheduled in engine.run() tick.

Parallel Systems: auto-batched ECS queries.

GPU Async: explicit command buffers; CPU/GPU sync points visible.

Networking: lockstep mode with deterministic inputs for multiplayer racing.



---

7) Rendering & Shaders

Built-in shader DSL (@shader) compiled to SPIR-V / DXIL / MSL.

pipeline objects created in Race code, strongly typed.

Hot-reload of shaders and pipelines during development.



---

8) Memory & Frame Management

Frame Allocator: fast arena allocator reset every frame.

Persistent Pools: meshes, textures, car data.

No hidden heap. Explicit alloc / free or RAII via Drop trait.



---

9) Example: Racing Car Engine

struct Car {
    engine: Engine,
    wheels: [4]Wheel,
    body: RigidBody,
}

system UpdateCar(dt: f32) query { Car } parallel {
    let tq = Car.engine.torque_curve(Car.engine.rpm)
    for w in Car.wheels {
        apply_torque(w, tq, dt)
    }
    Car.body.pos += Car.body.vel * dt
}


---

10) Roadmap (Racing Engine Alpha)

M0 (Weeks 1–4): Lexer, parser, AST, ECS core, frame loop primitive.
M1 (Weeks 5–8): Physics primitives (RigidBody, Wheel, Engine), deterministic fixed-step scheduler.
M2 (Weeks 9–12): Rendering API with GPU resource types, shader DSL → SPIR-V backend.
M3 (Weeks 13–16): Networking (lockstep racing), input, asset pipeline (meshes, car data).
M4 (Weeks 17–20): Demo: minimal racing engine (physics + rendering + input).


---

11) Naming & Aesthetics

File extension: .race

Module layout mirrors engine subsystems: physics.race, render.race, ecs.race.

Formatter defaults to engine-style readability.



---

Status: This version focuses Race as a domain-specific systems language for racing game engines — not general purpose like Rust, but tuned for physics/rendering workloads.


---
