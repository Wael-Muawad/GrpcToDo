// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using GrpcToDo.Client;

Console.WriteLine("Hello, World!");

using var channel = GrpcChannel.ForAddress("https://localhost:7088");


var greaterClient = new Greeter.GreeterClient(channel);
var reply = await greaterClient.SayHelloAsync(
                    new HelloRequest { Name = "GrpcService.Client" }
                  );

Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");



var toDoClient = new ToDoIt.ToDoItClient(channel);

Console.WriteLine("Enter Title, Description respectively to create todo.");
var createRequest = new CreateToDoRequest
{
    Title = Console.ReadLine(),
    Description = Console.ReadLine(),
};
var createResponse = await toDoClient.CreateToDoAsync(createRequest);
Console.WriteLine("new toDo wa created: Id:" + createResponse.Id);


Console.WriteLine("All ToDos:");
var getAllResponse = await toDoClient.ListToDoAsync(new GetAllRequest());
foreach (var todo in getAllResponse.ToDo)
{
    Console.WriteLine($"Id: {todo.Id}");
    Console.WriteLine($"Title: {todo.Title}");
    Console.WriteLine($"Description: {todo.Description}");
    Console.WriteLine($"ToDoStatus: {todo.ToDoStatus}\n\n");
}






Console.ReadKey();