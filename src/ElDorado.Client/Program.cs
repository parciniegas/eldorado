// See https://aka.ms/new-console-template for more information

using ElDorado.Client;

List<string> ids = ["FirstName", "LastName", "FullName", "FirstNameAndAge"];
var constraintRemovedSubscriber = new ConstraintRemoveSubscriber(ids);
Console.WriteLine("Waiting for messages. Press any key to exit.");
Console.ReadKey();
