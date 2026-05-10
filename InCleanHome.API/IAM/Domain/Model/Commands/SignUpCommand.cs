namespace InCleanHome.API.IAM.Domain.Model.Commands;

public record SignUpCommand(string Email, string Password, string Role);
