#if ANDROID
using AndroidX.Biometric;
using AndroidX.Core.Content;
using Java.Lang;
using Microsoft.Maui.ApplicationModel;
#endif
#if IOS
using LocalAuthentication;
using Foundation;
#endif
using Microsoft.Maui.Authentication;

namespace JPYCOffline.Services;

public class AuthenticatorService : IAuthenticatorService
{
    public async Task<bool> AuthenticateAsync()
    {
#if ANDROID
        var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("No current activity");
        var executor = ContextCompat.GetMainExecutor(activity);
        var tcs = new TaskCompletionSource<bool>();
        var callback = new AuthenticationCallback(tcs);
        var promptInfo = new BiometricPrompt.PromptInfo.Builder()
            .SetTitle("Authenticate")
            .SetSubtitle("Verify your identity")
            .SetNegativeButtonText("Cancel")
            .Build();
        var prompt = new BiometricPrompt(activity, executor, callback);
        prompt.Authenticate(promptInfo);
        return await tcs.Task;
#elif IOS
        var context = new LAContext();
        var result = await context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, "Authenticate to continue");
        return result.Item1;
#else
        return await AuthenticateWithPasskeyAsync();
#endif
    }

    public async Task<bool> AuthenticateWithPasskeyAsync()
    {
        try
        {
            var result = await WebAuthenticator.AuthenticateAsync(new Uri("https://example.com/passkey"), new Uri("mauiapp://callback"));
            return result is not null;
        }
        catch
        {
            return false;
        }
    }

#if ANDROID
    class AuthenticationCallback : BiometricPrompt.AuthenticationCallback
    {
        readonly TaskCompletionSource<bool> _tcs;
        public AuthenticationCallback(TaskCompletionSource<bool> tcs) => _tcs = tcs;
        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);
            _tcs.TrySetResult(true);
        }
        public override void OnAuthenticationError(int errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            _tcs.TrySetResult(false);
        }
        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
        }
    }
#endif
}
