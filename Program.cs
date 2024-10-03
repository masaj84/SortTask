using AltiumTask;
using System.Diagnostics;


//Stopwatch stopwatch = new Stopwatch();
//stopwatch.Start();

var fileTest = new CreateOutput();
fileTest.CreateTextFile(fileSizeInMB: 1000, filePath: "D://altium/", fileName: "test.txt");

var sortTest = new SortFile();
await sortTest.Sort();

//stopwatch.Stop();
//Console.WriteLine($"Took: {stopwatch.ElapsedMilliseconds} ms");