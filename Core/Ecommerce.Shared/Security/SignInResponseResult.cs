namespace Ecommerce.Shared.Security
{
    public enum SignInResponseResult
    {
        Success,
        InvalidCredentials,
        AccountLocked,
        AccountNotAllowed,
        EmailNotConfirmed,
        TwoFactorRequired,
        UnexpectedError
    }
}