// See https://aka.ms/new-console-template for more information
using sim;

var device = new Device();
device.EnableMachine(false);
Console.WriteLine(device.GetState());