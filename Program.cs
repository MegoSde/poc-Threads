using System.Collections.Concurrent;
/* 
 * This code demonstrates three main Proofs of Concept (POCs) related to threading, parallelism,
 * and thread safety in C#. The following topics are explored:
 *
 * 1. Sequential vs. Parallel Execution:
 *    - **1a. Sequential foreach**: Iterates over an array sequentially, simulating work by sleeping for a random duration.
 *    - **1b. Parallel foreach with 3 threads**: The same task is executed using a parallel loop, limiting the execution to 3 threads.
 *      It demonstrates how work can be distributed among threads, with each thread printing its ID.
 *
 * 2. Thread-Safe vs. Non-Thread-Safe Counter Operations:
 *    - **2a. Sequential counter increment**: A simple for loop that increments a counter sequentially.
 *    - **2b. Non-thread-safe parallel increment**: A parallel for loop increments a shared counter without any thread safety mechanisms, which may result in incorrect results.
 *    - **2c. Thread-safe counter using Interlocked**: Demonstrates how `Interlocked.Increment` ensures thread-safe increments in a parallel loop.
 *    - **2d. Thread-safe counter using lock**: Uses the `lock` statement to ensure thread safety by allowing only one thread to increment the counter at a time.
 *
 * 3. Non-Thread-Safe vs. Thread-Safe Collections:
 *    - **3a. Non-thread-safe list**: A parallel loop that adds random integers to a `List<int>`, which is not thread-safe and may cause a crash.
 *    - **3b. Thread-safe collection using ConcurrentBag**: Demonstrates how `ConcurrentBag<int>`, a thread-safe collection, can handle concurrent additions of random integers safely.
 *
 * Disclaimers:
 * Run in release configuration without debugger to showcase the POC the right way.
 * Due to the lack of thread safety, section 3a (adding to a List<int>)
 * may crash. If this happens, try lowering the `countTarget` value or run it again.
 */

// Initialize variables to set up and use in the following POC.
const int countTarget = 1000000;
var watch = new System.Diagnostics.Stopwatch();
var random = new Random();
var array = Enumerable.Range(1, 10).Select(id => new { Id = id, RandomNumber = random.Next(100, 1000) }).ToArray();


// 1a. Run an array in a foreach loop (sequential)
Console.WriteLine("1. Sequential foreach:");
watch.Start();
foreach (var item in array)
{
    Console.WriteLine($"Starting ID: {item.Id} Sleeping for {item.RandomNumber} seconds");
    Thread.Sleep(item.RandomNumber);
    Console.WriteLine($"Finished ID: {item.Id}");
}
watch.Stop();
Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds} ms");


// 1b. Run the array in Parallel.ForEach with a limit of 3 threads
Console.WriteLine("\n1b. Parallel foreach (with 3 threads):");
watch.Restart();
var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 3 };
Parallel.ForEach(array, parallelOptions, item =>
{
    var threadId = Environment.CurrentManagedThreadId;
    Console.WriteLine($"Starting ID: {item.Id} Sleeping for {item.RandomNumber} seconds. on Thread: {threadId}");
    Thread.Sleep(item.RandomNumber);
    Console.WriteLine($"Finished ID: {item.Id}. on Thread: {threadId}");
});
watch.Stop();
Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds} ms");


// 2a. Sequential counter increment
Console.WriteLine("\n2a. Sequential for loop incrementing a counter:");
watch.Restart();
var counter1 = 0;
for (var i = 0; i < countTarget; i++)
{
    counter1++;
}
watch.Stop();
Console.WriteLine($"Counter: {counter1}, Elapsed time: {watch.ElapsedMilliseconds} ms");


// 2b. Parallel for loop incrementing a counter without thread safety
Console.WriteLine("\n2b. Parallel for loop with NOT thread-safe counter:");
watch.Restart();
var counter2 = 0;
Parallel.For(0, countTarget, _ =>
{
    counter2++; // Not thread-safe increment
});
watch.Stop();
Console.WriteLine($"Counter: {counter2}, Elapsed time: {watch.ElapsedMilliseconds} ms");


// 2c. Parallel for loop with thread-safe incrementing
Console.WriteLine("\n2c. Parallel for loop with thread-safe counter (using Interlocked):");
watch.Restart();
var counter3 = 0;
Parallel.For(0, countTarget, _ =>
{
    Interlocked.Increment(ref counter3); // Thread-safe increment
});
watch.Stop();
Console.WriteLine($"Counter: {counter3}, Elapsed time: {watch.ElapsedMilliseconds} ms");


// 2d. Parallel for loop with lock on counter incrementing
Console.WriteLine("\n2d. Parallel for loop with lock on counter incrementing:");
watch.Restart();
var lockObj = new object();
var counter4 = 0;
Parallel.For(0, countTarget, _ =>
{
    lock (lockObj)
    {
        counter4++;
    }
});
watch.Stop();
Console.WriteLine($"Counter: {counter4}, Elapsed time: {watch.ElapsedMilliseconds} ms");

// 3a. Parallel loop adding random integers to a List<int> (not thread-safe)
Console.WriteLine("\n3a. Parallel for loop adding random integers to a List (NOT thread-safe):");
watch.Restart();
var randomList = new List<int>();
Parallel.For(0, countTarget, i =>
{
    try
    {
        randomList.Add(random.Next(100, 1000)); // Not thread-safe
    }
    catch (Exception)
    {
        Console.WriteLine("Failed with exception");
    }
    
});
watch.Stop();
Console.WriteLine($"Random List count (not thread-safe): {randomList.Count}, Elapsed time: {watch.ElapsedMilliseconds} ms");


// 3b. Parallel loop adding random integers to a ConcurrentBag (thread-safe)
Console.WriteLine("\n3b. Parallel for loop adding random integers to a ConcurrentBag (thread-safe):");
watch.Restart();
var concurrentBag = new ConcurrentBag<int>();
Parallel.For(0, countTarget, _ =>
{
    concurrentBag.Add(random.Next(100, 1000)); // Thread-safe
});
watch.Stop();
Console.WriteLine($"ConcurrentBag count (thread-safe): {concurrentBag.Count}, Elapsed time: {watch.ElapsedMilliseconds} ms");