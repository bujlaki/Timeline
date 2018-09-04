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
        private string CUSTOM_DOMAIN = "timeline";
        private string REGION = RegionEndpoint.EUCentral1.DisplayName;

        public CognitoAuthenticator() { }

        //public CognitoAWSCredentials GetCachedCognitoIdentity()
        //{
        //    Console.WriteLine("GetCachedCognitoIdentity");
        //    if (!string.IsNullOrEmpty(credentials.GetCachedIdentityId()) || credentials.CurrentLoginProviders.Length > 0)
        //    {
        //        return credentials;
        //    }
        //    return null;
        //}

        public async Task GetAWSCredentialsWithGoogleToken(string token, Ref<LoginData> loginData)
        {
            try
            {
                CognitoAWSCredentials credentials = new CognitoAWSCredentials(this.IDENTITYPOOL_ID, RegionEndpoint.EUCentral1);
                credentials.Clear();
                credentials.AddLogin("accounts.google.com", token);

                AmazonCognitoIdentityClient cli = new AmazonCognitoIdentityClient(credentials, RegionEndpoint.EUCentral1);

                var req = new Amazon.CognitoIdentity.Model.GetIdRequest();
                req.Logins.Add("accounts.google.com", token);
                req.IdentityPoolId = this.IDENTITYPOOL_ID;

                GetIdResponse getIdResponse = await cli.GetIdAsync(req);

                var getCredentialReq = new Amazon.CognitoIdentity.Model.GetCredentialsForIdentityRequest();
                getCredentialReq.IdentityId = getIdResponse.IdentityId;
                getCredentialReq.Logins.Add("accounts.google.com", token);

                GetCredentialsForIdentityResponse getCredentialsResponse = await cli.GetCredentialsForIdentityAsync(getCredentialReq);
                loginData.Value.AWSCredentials = getCredentialsResponse.Credentials;
                loginData.Value.IdentityId = getCredentialsResponse.IdentityId;
            }
            catch(Exception ex)
            {
                Console.WriteLine("GetAWSCredentialsWithGoogleToken ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task SignupUser(string username, string password, string email, Ref<LoginData> loginData)
        {
            try
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

                SignUpResponse signUpResponse = await provider.SignUpAsync(signUpRequest);
                loginData.Value.SignupVerified = signUpResponse.UserConfirmed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignupUser ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task VerifyAccessCode(string username, string code, Ref<LoginData> loginData)
        {
            try
            {
                AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
                ConfirmSignUpRequest confirmSignUpRequest = new ConfirmSignUpRequest();
                confirmSignUpRequest.Username = username;
                confirmSignUpRequest.ConfirmationCode = code;
                confirmSignUpRequest.ClientId = CLIENTAPP_ID;

                await provider.ConfirmSignUpAsync(confirmSignUpRequest);
                loginData.Value.SignupVerified = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("VerifyAccessCode ERROR: " + ex.Message);
                throw ex;
            }
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

        public async Task ValidateUser(string username, string password, Ref<LoginData> loginData)
        {
            try
            {
                AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
                CognitoUserPool userPool = new CognitoUserPool(this.USERPOOL_ID, this.CLIENTAPP_ID, provider);
                CognitoUser user = new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);
                
                InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest();
                authRequest.Password = password;

                AuthFlowResponse authFlowResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                if (authFlowResponse.AuthenticationResult == null) throw new Exception("Cognito authentication error");

                loginData.Value.Account = new LoginData.LoginAccount {
                    access_token = authFlowResponse.AuthenticationResult.AccessToken,
                    expires_in = authFlowResponse.AuthenticationResult.ExpiresIn,
                    token_type = authFlowResponse.AuthenticationResult.TokenType,
                    id_token = authFlowResponse.AuthenticationResult.IdToken,
                    refresh_token = authFlowResponse.AuthenticationResult.RefreshToken
                };

                GetUserResponse userDetails = await user.GetUserDetailsAsync();
                loginData.Value.UserId = userDetails.UserAttributes.Find(x => x.Name.ToLower() == "sub").Value;
                loginData.Value.UserName = user.Username;
                loginData.Value.Email = userDetails.UserAttributes.Find(x => x.Name.ToLower() == "email").Value;
                loginData.Value.Picture = "userphoto";

                //string idtoken = authFlowResponse.AuthenticationResult.IdToken;

                CognitoAWSCredentials creds = new CognitoAWSCredentials(this.IDENTITYPOOL_ID, RegionEndpoint.EUCentral1);
                creds.Clear();
                creds.AddLogin(
                    "cognito-idp." + RegionEndpoint.EUCentral1.SystemName + ".amazonaws.com/" + this.USERPOOL_ID, 
                    authFlowResponse.AuthenticationResult.IdToken);

                loginData.Value.AWSCredentials = creds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ValidateUser ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteUser(string username)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.EUCentral1);
            CognitoUserPool userPool = new CognitoUserPool(this.USERPOOL_ID, this.CLIENTAPP_ID, provider);
            CognitoUser user = new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);

            await user.DeleteUserAsync();
        }
    }
}
