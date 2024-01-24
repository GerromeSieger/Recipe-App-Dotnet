namespace RecipeApp.ServicesInterface
{
    public interface IMailer
    {
        public bool SendMail(string Email, string body, string Subject);
    }
}
