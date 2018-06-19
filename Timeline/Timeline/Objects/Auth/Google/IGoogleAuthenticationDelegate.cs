using System;
using Xamarin.Auth;

namespace Timeline.Objects.Auth.Google
{
    public interface IGoogleAuthenticationDelegate
    {
        void OnGoogleAuthCompleted(Account account);
        void OnGoogleAuthFailed(string message, Exception exception);
        void OnGoogleAuthCanceled();
    }
}
