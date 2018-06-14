using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;

namespace Timeline.Objects.Auth.Cognito
{
    class CognitoAuthenticator
    {
        private string POOL_ID = "eu-central-1_92Db6PCAi";
        private string CLIENTAPP_ID = "365ckri3ic37q8crv6orpk7q7s";
        //private string FED_POOL_ID = ConfigurationManager.AppSettings["FED_POOL_id"];
        private string CUSTOM_DOMAIN = "timeline";
        private string REGION = RegionEndpoint.EUCentral1.DisplayName;

        CognitoAWSCredentials credentials;

        public string CognitoId { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public CognitoAuthenticator()
        {
            credentials = new CognitoAWSCredentials(
                "eu-central-1:fd027885-da62-40c8-a16e-44c8a7cb8300", // Identity pool ID
                RegionEndpoint.EUCentral1 // Region
            );
            CognitoId = "";
            IsLoggedIn = false;
        }

        public void GetCachedCognitoIdentity()
        {
            credentials.Clear();

            Console.WriteLine("GetCachedCognitoIdentity");
            if (!string.IsNullOrEmpty(credentials.GetCachedIdentityId()) || credentials.CurrentLoginProviders.Length > 0)
            {
                if (!IsLoggedIn) CognitoId = credentials.GetIdentityId();
                IsLoggedIn = true;
            }
        }

        public void GetCognitoIdentityWithGoogleToken(string token)
        {
            Console.WriteLine("GetCognitoIdentityWithGoogleToken");

            credentials.AddLogin("accounts.google.com", token);
            AmazonCognitoIdentityClient cli = new AmazonCognitoIdentityClient(credentials, RegionEndpoint.EUCentral1);

            var req = new Amazon.CognitoIdentity.Model.GetIdRequest();
            req.Logins.Add("accounts.google.com", token);
            req.IdentityPoolId = "eu-central-1:fd027885-da62-40c8-a16e-44c8a7cb8300";

            Task<GetIdResponse> task = cli.GetIdAsync(req);
            task.Wait();

            if ((task.Status == TaskStatus.RanToCompletion) && (task.Result != null))
            {
                CognitoId = task.Result.IdentityId;
                IsLoggedIn = true;
            }
            else
            {
                CognitoId = "";
                throw task.Exception;
            }
        }

        public void GetCognitoIdentityWithUserPass(string username, string password)
        {
            bool success;
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
            //AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(credentials,RegionEndpoint.EUCentral1);

            CognitoUserPool userPool = new CognitoUserPool("eu-central-1_92Db6PCAi", "365ckri3ic37q8crv6orpk7q7s", provider);
            CognitoUser user = new CognitoUser("username", "365ckri3ic37q8crv6orpk7q7s", userPool, provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = "userPassword123"
            };

            try
            {
                Task<AuthFlowResponse> task = user.StartWithSrpAuthAsync(authRequest);
                task.Wait();

                AuthFlowResponse authResponse = task.Result;
                var accessToken = authResponse.AuthenticationResult.AccessToken;
                success = true;
            }
            catch
            {
                success = false;
            }



            if (!success)
            {
                //SIGNUP
                SignUpRequest sr = new SignUpRequest();

                sr.ClientId = "365ckri3ic37q8crv6orpk7q7s";
                sr.Username = "username";
                sr.Password = "userPassword123";
                sr.UserAttributes = new List<AttributeType> {
                new AttributeType
                {
                        Name = "email",
                        Value = "balazs.ujlaki@gmail.com",
                }
            };

                Task<SignUpResponse> task2 = provider.SignUpAsync(sr);
                task2.Wait();
                SignUpResponse resp = task2.Result;
                
            }
        }

        //internal string GetCustomHostedURL()
        //{
        //    return string.Format("https://{0}.auth.{1}.amazoncognito.com/login?response_type=code&client_id={2}&redirect_uri=https://sid343.reinvent-workshop.com/", CUSTOM_DOMAIN, REGION, CLIENTAPP_ID);
        //}

        //public async Task<bool> SignUpUser(string username, string password, string email, string phonenumber)
        public async Task<SignUpResponse> SignupUser(string username, string password, string email)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
            SignUpRequest signUpRequest = new SignUpRequest();

            signUpRequest.ClientId = CLIENTAPP_ID;
            signUpRequest.Username = username;
            signUpRequest.Password = password;

            AttributeType attributeType1 = new AttributeType();
            attributeType1.Name = "email";
            attributeType1.Value = email;
            signUpRequest.UserAttributes.Add(attributeType1);

            SignUpResponse result = await provider.SignUpAsync(signUpRequest);

            return result;
        }

        public async Task<CognitoUser> VerifyAccessCode(string username, string code)
        {
            try
            {
                AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
                ConfirmSignUpRequest confirmSignUpRequest = new ConfirmSignUpRequest();
                confirmSignUpRequest.Username = username;
                confirmSignUpRequest.ConfirmationCode = code;
                confirmSignUpRequest.ClientId = CLIENTAPP_ID;

                Task<ConfirmSignUpResponse> task = provider.ConfirmSignUpAsync(confirmSignUpRequest);
                task.Wait();
                ConfirmSignUpResponse confirmSignUpResult = task.Result; //provider.ConfirmSignUpAsync(confirmSignUpRequest);
                Console.WriteLine(confirmSignUpResult.ToString());
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            return null;
        }

        //internal async Task<CognitoUser> ResetPassword(string username)
        //{
        //    AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());

        //    CognitoUserPool userPool = new CognitoUserPool(this.POOL_ID, this.CLIENTAPP_ID, provider);

        //    CognitoUser user = new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);
        //    await user.ForgotPasswordAsync();
        //    return user;
        //}

        //internal async Task<CognitoUser> UpdatePassword(string username, string code, string newpassword)
        //{
        //    AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());

        //    CognitoUserPool userPool = new CognitoUserPool(this.POOL_ID, this.CLIENTAPP_ID, provider);

        //    CognitoUser user = new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);
        //    await user.ConfirmForgotPasswordAsync(code, newpassword);
        //    return user;
        //}

        public async Task<CognitoUser> ValidateUser(string username, string password)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);

            CognitoUserPool userPool = new CognitoUserPool(this.POOL_ID, this.CLIENTAPP_ID, provider);

            CognitoUser user = new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            
            if (authResponse.AuthenticationResult != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

    }
}
