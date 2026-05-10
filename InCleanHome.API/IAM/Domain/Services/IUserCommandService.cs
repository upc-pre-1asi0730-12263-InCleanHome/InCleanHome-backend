using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Model.Commands;

namespace InCleanHome.API.IAM.Domain.Services;

public interface IUserCommandService
{
    Task<(User user, string token)> Handle(SignInCommand command);
    Task<User> Handle(SignUpCommand command);
    Task Handle(VerifyUserCommand command);
    Task Handle(UploadWorkerDocumentCommand command);
    Task<User> Handle(UpdateUserEmailCommand command);
    string GenerateTokenFor(User user);
}
