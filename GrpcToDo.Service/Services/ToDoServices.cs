using Grpc.Core;
using Grpc.Entities.Models;
using GrpcToDo.Service.Data;
using Microsoft.EntityFrameworkCore;

namespace GrpcToDo.Service.Services
{
    public class ToDoServices : ToDoIt.ToDoItBase
    {
        private readonly ILogger<ToDoServices> _logger;
        private readonly AppDbContext _context;

        public ToDoServices(AppDbContext context, ILogger<ToDoServices> logger)
        {
            _context=context;
            _logger=logger;
        }


        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
                throw new RpcException(
                    new Status(
                        StatusCode.InvalidArgument,
                        "You must supply a valid object"));

            var toDoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description,
            };

            await _context.ToDoItems.AddAsync(toDoItem);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = toDoItem.Id
            });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(
                        new Status(
                            StatusCode.InvalidArgument,
                            "resource index must be greater than 0"));

            var todo = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (todo is null)
                throw new RpcException(
                             new Status(
                                 StatusCode.NotFound,
                                 $"resource with index ({request.Id}) is not exist."));

            return await Task.FromResult(new ReadToDoResponse
            {
                Id= todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                ToDoStatus = todo.ToDoStatus
            });
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
                throw new RpcException(
                    new Status(
                        StatusCode.InvalidArgument,
                        "You must supply a valid object"));

            var todo = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
            if (todo is null)
                throw new RpcException(
                             new Status(
                                 StatusCode.NotFound,
                                 $"resource with index ({request.Id}) is not exist."));

            todo.Title = request.Title;
            todo.Description = request.Description;
            todo.ToDoStatus = request.ToDoStatus;
            await _context.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = todo.Id
            });
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItems = await _context.ToDoItems.ToListAsync();

            foreach (var todo in toDoItems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    ToDoStatus = todo.ToDoStatus
                });
            }

            return await Task.FromResult<GetAllResponse>(response);
        }


        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(
                        new Status(
                            StatusCode.InvalidArgument,
                            "resource index must be greater than 0"));

            var todo = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (todo is null)
                throw new RpcException(
                             new Status(
                                 StatusCode.NotFound,
                                 $"resource with index ({request.Id}) is not exist."));

            _context.ToDoItems.Remove(todo);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse { Id = todo.Id });
        }

    }
}
