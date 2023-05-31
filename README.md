# FosterScript.Examples
Example code for the [FosterScript library](https://github.com/Exouxas/FosterScript.Core/) 

## FiniteExample
Shows how to use [FiniteWorld](https://github.com/Exouxas/FosterScript.Core/blob/main/src/Core/Worlds/FiniteWorld.cs) in a console application. Finite worlds have a set amount of steps they run before stopping, and will attempt to complete this steps as quickly as possible.

## IndefiniteExample
Shows how to use [IndefiniteWorld](https://github.com/Exouxas/FosterScript.Core/blob/main/src/Core/Worlds/IndefiniteWorld.cs) in a console application. Indefinite worlds have a predetermined amount of time between each step. It will attempt to complete each step with their starts offset by the specified time. If a step takes too long, and begins to overlap another step, it will delay the other step.

## BasicBrainExample
Shows how to use [Brain](https://github.com/Exouxas/FosterScript.Core/blob/main/src/Core/NeuralNetwork/Brain.cs) as the base class for a neural network, and runs it in an [Indefinite World](https://github.com/Exouxas/FosterScript.Core/blob/main/src/Core/Worlds/IndefiniteWorld.cs).