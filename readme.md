# Threading and Parallelism POC in C#

This project demonstrates key concepts of threading, parallel execution, and thread safety in C# through three main Proofs of Concept (POCs). The code compares sequential and parallel execution, explores thread safety when incrementing counters, and shows the differences between non-thread-safe and thread-safe collections.

## Table of Contents
- [Overview](#overview)
- [Project Structure](#project-structure)
- [How to Run](#how-to-run)
- [POCs](#pocs)
    - [1. Sequential vs. Parallel Execution](#1-sequential-vs-parallel-execution)
    - [2. Thread-Safe vs. Non-Thread-Safe Counter Operations](#2-thread-safe-vs-non-thread-safe-counter-operations)
    - [3. Non-Thread-Safe vs. Thread-Safe Collections](#3-non-thread-safe-vs-thread-safe-collections)
- [Disclaimer](#disclaimer)

## Overview

This project provides:
- A demonstration of **sequential vs. parallel execution** using the `foreach` loop.
- An exploration of **thread safety** when incrementing shared counters using sequential loops, non-thread-safe parallel loops, `Interlocked` thread-safe operations, and `lock` statements.
- A comparison of **thread-safe and non-thread-safe collections**, showcasing how using `List<int>` in parallel can lead to crashes, while `ConcurrentBag<int>` ensures thread-safe operations.

## Project Structure

The main file is:

- **Program.cs**: Contains all the code for the three POCs.

## How to Run
Run Release configuration without debugger
```bash
dotnet run --configuration Release
```

## POCs

### 1. Sequential vs. Parallel Execution

The first POC compares the execution of a loop that processes an array sequentially with a `foreach` loop and then executes it in parallel using `Parallel.ForEach`, limiting the degree of parallelism to 3 threads. The time taken for both methods is measured.

- **1a. Sequential foreach**: Processes items in sequence, simulating work using `Thread.Sleep`.
- **1b. Parallel foreach (with 3 threads)**: Runs the same loop in parallel, printing the thread ID handling each item.

### 2. Thread-Safe vs. Non-Thread-Safe Counter Operations

This POC demonstrates the risks of parallel operations on shared state (counters) without proper synchronization, and compares methods for ensuring thread safety.

- **2a. Sequential counter increment**: A simple for loop that increments a counter in sequence.
- **2b. Parallel counter increment (non-thread-safe)**: A parallel loop increments a shared counter without protection, leading to potential race conditions.
- **2c. Thread-safe counter increment (using Interlocked)**: Uses `Interlocked.Increment` for thread-safe increments in a parallel loop.
- **2d. Thread-safe counter increment (using lock)**: Ensures only one thread can increment the counter at a time using the `lock` statement.

### 3. Non-Thread-Safe vs. Thread-Safe Collections

This POC compares the behavior of adding items to a non-thread-safe collection (`List<int>`) vs. a thread-safe collection (`ConcurrentBag<int>`).

- **3a. Parallel list addition (non-thread-safe)**: Adds random integers to a `List<int>` using a parallel loop, which is prone to crashes.
- **3b. Parallel collection addition (thread-safe)**: Uses `ConcurrentBag<int>` to safely add random integers in parallel.

## Disclaimer

- **Important**: Run the code in **Release** mode. 
- **Section 3a** (non-thread-safe list) may crash due to race conditions. If you encounter crashes, try lowering the `countTarget` or re-run the program.
- **Thread-Safety**: The project illustrates potential dangers in multithreaded environments when using non-thread-safe resources like lists and counters.
