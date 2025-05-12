// See https://aka.ms/new-console-template for more information

using ElDorado.Client;

List<string> ids = ["FirstName", "LastName", "FullName", "FirstNameAndAge"];
var collector = new ConstraintRemoveSubscriber(ids);
Console.WriteLine("Waiting for messages.");
await collector.WaitForAllEventsAsync();
Console.WriteLine("ALL EVENTS RECEIVED.");
