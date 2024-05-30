using Microsoft.AspNetCore.Mvc;

namespace UserService;

[Route("/")]
[ApiController]
public class UserController
{
    private readonly IUserRepository repository;

    public UserController(IUserRepository repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    public Task SaveUserAsync([FromBody] UserModel user)
    {
        return repository.SaveUserAsync(user);
    }
}