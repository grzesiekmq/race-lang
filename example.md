example Race program demonstrating a minimal racing simulation using the spec:
```rs
// File: demo_race.race
module DemoRace

import Physics
import Rendering
import Input

// lacks define Engine

 ----------------------
// Instantiate Engine
// ----------------------
engine V8 {
    max_rpm: 8000
    torque_curve: fn(rpm: f32) -> f32 {
        if rpm < 1000 { return 120 }
        if rpm < 3000 { return 250 }
        if rpm < 6000 { return 300 }
        return 280
    }
}

// ----------------------
// Define Wheels
// ----------------------
struct Wheel {
    radius: f32
    grip: f32
}

wheels: [4]Wheel = [
    Wheel(0.34, 1.2),
    Wheel(0.34, 1.2),
    Wheel(0.36, 1.0),
    Wheel(0.36, 1.0)
]

// ----------------------
// Define Car
// ----------------------
struct RigidBody {
    pos: vec3
    vel: vec3
    mass: f32
}

struct Car {
    engine: Engine
    wheels: [4]Wheel
    body: RigidBody
}

// Instantiate Car

car PlayerCar = Car {
    engine: V8,
    wheels: wheels,
    body: RigidBody(vec3(0,0,0), vec3(0,0,0), 1200.0)
}

// ----------------------
// Physics System
// ----------------------
system PhysicsStep(dt: f32) query { PlayerCar } parallel {
    let torque = PlayerCar.engine.torque_curve(PlayerCar.body.vel.x)
    for w in PlayerCar.wheels {
        // Simplified longitudinal force
        PlayerCar.body.vel.x += torque * dt / PlayerCar.body.mass
    }
    // Update position
    PlayerCar.body.pos += PlayerCar.body.vel * dt
}

// ----------------------
// Rendering
// ----------------------
@shader(vertex)
fn vs_main(input: VertexInput) -> VertexOutput { /* vertex shader */ }

@shader(fragment)
fn fs_main(input: VertexOutput) -> vec4 { return vec4(0.8,0,0,1) }

fn render_frame(dt: f32) {
    let pass = gfx.begin_pass(clear_color=vec4(0.1,0.1,0.1,1))
    pass.draw(mesh=PlayerCar.body, material=red_material)
    gfx.end_pass(pass)
}

// ----------------------
// Input Handling
// ----------------------
fn input_poll() {
    if key_pressed("W") { PlayerCar.body.vel.x += 10 }
    if key_pressed("S") { PlayerCar.body.vel.x -= 10 }
}

// ----------------------
// Main Game Loop
// ----------------------
fn main() {
    engine = Engine::new()
    engine.run(|dt| {
        PhysicsStep(dt)
        render_frame(dt)
        input_poll()
    })
}

```

✅ Features Demonstrated:

1. Engine with torque curve.


2. Car with wheels and rigid body.


3. Parallel physics system.


4. Rendering hooks (shaders + frame pass).


5. Input polling.


6. Main loop orchestrating everything.




---
