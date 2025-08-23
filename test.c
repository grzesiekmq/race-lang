
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
    .weight   = 1200,  // w C nie ma "kg", więc same liczby
    .engine   = { .horsepower = 450, .torque = 550.5f },
    .color    = "red"
};

// ----------- functions ---------------------
int main() {
    float result = myEngine.horsepower + myEngine.torque;
    printf("result = %.2f\n", result);

    printf("horsepower = %d\n", myEngine.horsepower);
    printf("torque = %.2f\n", myEngine.torque);

    printf("Car: %s, maxSpeed=%d, weight=%d\n",
           MySuperCar.color, MySuperCar.maxSpeed, MySuperCar.weight);

    return 0;
}
