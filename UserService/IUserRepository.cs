namespace UserService;

public interface IUserRepository
{
    Task SaveUserAsync(UserModel model);
}
