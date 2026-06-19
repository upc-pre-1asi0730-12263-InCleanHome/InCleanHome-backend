using InCleanHome.API.IAM.Application.Internal.OutboundServices;
using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Model.Commands;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;
using InCleanHome.API.IAM.Domain.Repositories;
using InCleanHome.API.IAM.Domain.Services;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.IAM.Application.Internal.CommandServices;

// ====================================================================================
// MODULE: Identity & Access Management (IAM)
// LAYER: Application (Internal / CommandServices)
//
// Concrete implementation of IUserCommandService. Acts as the primary application 
// orchestrator for mutations and use cases within the IAM context. It processes
// incoming commands (SignIn, SignUp, Verify, Upload Document) to drive state changes.
//
// - Implements the Command handler pattern to enforce a clear separation between 
//   write operations (Commands) and read operations (Queries).
// - Security Integration: Collaborates with IHashingService for credential validation
//   and ITokenService to securely manage JWT lifecycles.
// - Transaction Management: Utilizes the IUnitOfWork pattern to ensure all database 
//   operations (e.g., adding a user, persisting worker documents, updating verification 
//   statuses) are executed atomically within a single database transaction.
// ====================================================================================

public class UserCommandService(
    IUserRepository userRepository,
    IWorkerDocumentRepository workerDocumentRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork) : IUserCommandService
{
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByEmailAsync(command.Email)
            ?? throw new Exception("Invalid email or password");

        if (!hashingService.VerifyPassword(command.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        if (user.Role == UserRole.Worker && !user.DocumentsVerified)
            throw new Exception("Documents not verified yet. Please upload your documents.");

        var token = tokenService.GenerateToken(user);
        return (user, token);
    }

    public async Task<User> Handle(SignUpCommand command)
    {
        if (userRepository.ExistsByEmail(command.Email))
            throw new Exception($"Email {command.Email} is already taken");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Email, hashedPassword, command.Role);

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();
        return user;
    }

    public async Task Handle(VerifyUserCommand command)
    {
        var user = await userRepository.FindByIdAsync(command.UserId)
            ?? throw new Exception($"User {command.UserId} not found");
        user.Verify();
        userRepository.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task Handle(UploadWorkerDocumentCommand command)
    {
        if (!DocumentType.IsValid(command.DocumentType))
            throw new Exception("Invalid document type");

        var user = await userRepository.FindByIdAsync(command.UserId)
            ?? throw new Exception("User not found");

        if (user.Role != UserRole.Worker)
            throw new Exception("Only workers can upload documents");

        var doc = new WorkerDocument(command.UserId, command.DocumentType, command.FileName, command.FileBase64);
        await workerDocumentRepository.AddAsync(doc);

        // If both required documents are present after this insert, mark as verified
        var existing = (await workerDocumentRepository.FindByUserIdAsync(command.UserId)).ToList();
        var types = existing.Select(d => d.DocumentType).Append(command.DocumentType).Distinct().ToHashSet();
        if (types.Contains(DocumentType.BackgroundCheck) && types.Contains(DocumentType.Experience))
        {
            user.MarkDocumentsAsVerified();
            userRepository.Update(user);
        }

        await unitOfWork.CompleteAsync();
    }

    public async Task<User> Handle(UpdateUserEmailCommand command)
    {
        var user = await userRepository.FindByIdAsync(command.UserId)
            ?? throw new Exception("User not found");

        if (userRepository.ExistsByEmail(command.Email) && user.Email != command.Email)
            throw new Exception($"Email {command.Email} is already taken");

        user.UpdateEmail(command.Email);
        userRepository.Update(user);
        await unitOfWork.CompleteAsync();
        return user;
    }

    public string GenerateTokenFor(User user) => tokenService.GenerateToken(user);
}
