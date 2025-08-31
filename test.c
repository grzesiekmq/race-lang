#include <stdio.h>
typedef struct{
int horsepower;
float torque;
} Engine;

Engine myEngine = {
.horsepower = 450,
.torque = 550.5
};

void main(){
int torque = 500;

printf("torque: %d\n", torque);
printf("HP: %d\n", myEngine.horsepower);
printf("engine torque: %d\n", myEngine.torque);
}

