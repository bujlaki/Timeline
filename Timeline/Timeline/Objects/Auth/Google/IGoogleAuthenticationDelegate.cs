using System;

namespace Timeline.Objects.Auth.Google
{
    public interface IGoogleAuthenticationDelegate
    {
        void OnGoogleAuthCompleted(GoogleOAuthToken token);
        void OnGoogleAuthFailed(string message, Exception exception);
        void OnGoogleAuthCanceled();
    }
}
