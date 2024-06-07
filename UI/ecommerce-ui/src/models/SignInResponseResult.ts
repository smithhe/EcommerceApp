export enum SignInResponseResult
{
    Success,
    InvalidCredentials,
    AccountLocked,
    AccountNotAllowed,
    EmailNotConfirmed,
    TwoFactorRequired,
    UnexpectedError
}