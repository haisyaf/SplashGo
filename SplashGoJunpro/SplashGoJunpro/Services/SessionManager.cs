namespace SplashGoJunpro.Services
{
    public static class SessionManager
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static string CurrentUserEmail { get; set; }
        public static string LoginToken { get; set; }
        public static int CurrentUserId { get; set; }
    }
}
