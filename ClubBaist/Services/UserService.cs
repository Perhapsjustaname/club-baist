namespace ClubBaist.Services;

public class UserService
{
    public ClubBaist.Models.User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    
    public event Action? OnChange;

    public void Login(ClubBaist.Models.User user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}