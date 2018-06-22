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
using Timeline.Models;

namespace Timeline.Objects.Auth.Cognito
{
    class CognitoAuthenticator
    {
        private string USERPOOL_ID = "eu-central-1_92Db6PCAi";
        private string IDENTITYPOOL_ID = "eu-central-1:fd027885-da62-40c8-a16e-44c8a7cb8300";
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

        public CognitoAWSCredentials GetCachedCognitoIdentity()
        {
            Console.WriteLine("GetCachedCognitoIdentity");
            if (!string.IsNullOrEmpty(credentials.GetCachedIdentityId()) || credentials.CurrentLoginProviders.Length > 0)
            {
                return credentials;
            }
            return null;
        }

        public async Task<GetCredentialsForIdentityResponse> GetCognitoIdentityWithGoogleToken(string token)
        {
            Console.WriteLine("GetCognitoIdentityWithGoogleToken");

            credentials.AddLogin("accounts.google.com", token);
            
            AmazonCognitoIdentityClient cli = new AmazonCognitoIdentityClient(credentials, RegionEndpoint.EUCentral1);

            var req = new Amazon.CognitoIdentity.Model.GetIdRequest();
            req.Logins.Add("accounts.google.com", token);
            req.IdentityPoolId = "eu-central-1:fd027885-da62-40c8-a16e-44c8a7cb8300";
            
            GetIdResponse getIdResponse = await cli.GetIdAsync(req);

            var getCredentialReq = new Amazon.CognitoIdentity.Model.GetCredentialsForIdentityRequest();
            getCredentialReq.IdentityId = getIdResponse.IdentityId;
            getCredentialReq.Logins.Add("accounts.google.com", token);

            return await cli.GetCredentialsForIdentityAsync(getCredentialReq);
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

            return await provider.SignUpAsync(signUpRequest);
        }

        public async Task<ConfirmSignUpResponse> VerifyAccessCode(string username, string code)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
            ConfirmSignUpRequest confirmSignUpRequest = new ConfirmSignUpRequest();
            confirmSignUpRequest.Username = username;
            confirmSignUpRequest.ConfirmationCode = code;
            confirmSignUpRequest.ClientId = CLIENTAPP_ID;

            ConfirmSignUpResponse confirmSignUpResult = await provider.ConfirmSignUpAsync(confirmSignUpRequest);
            return confirmSignUpResult;
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

        public async Task<MUser> ValidateUser(string username, string password)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
            CognitoUserPool userPool = new CognitoUserPool(this.USERPOOL_ID, this.CLIENTAPP_ID, provider);
            CognitoUser user = new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest();
            authRequest.Password = password;

            AuthFlowResponse authFlowResponse = await user.StartWithSrpAuthAsync(authRequest); //.ConfigureAwait(false);

            //https://aws.amazon.com/blogs/developer/cognitoauthentication-extension-library-developer-preview/
            while (authFlowResponse.AuthenticationResult == null)
            {
                if (authFlowResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
                {
                    //if user must change password...
                }
                else if (authFlowResponse.ChallengeName == ChallengeNameType.SMS_MFA)
                {
                    //if SMS code is required...
                }
                else
                {
                    Console.WriteLine("Unrecognized authentication challenge.");
                    break;
                }
            }

            if (authFlowResponse.AuthenticationResult == null) throw new Exception("Cognito authentication error");

            GetUserResponse userDetails = await user.GetUserDetailsAsync();

            CognitoAWSCredentials credentials = user.GetCognitoAWSCredentials(this.IDENTITYPOOL_ID, RegionEndpoint.EUCentral1);
            
            MUser timelineUser = new MUser();
            timelineUser.AWSCredentials = credentials;
            timelineUser.UserId = userDetails.UserAttributes.Find(x => x.Name.ToLower()=="sub").Value;
            timelineUser.UserName = user.Username;
            timelineUser.Email = userDetails.UserAttributes.Find(x => x.Name.ToLower() == "email").Value;
            timelineUser.PhotoUrl = "userphoto";
            timelineUser.Type = MUser.MUserType.Cognito;

            return timelineUser;
        }

    }
}
