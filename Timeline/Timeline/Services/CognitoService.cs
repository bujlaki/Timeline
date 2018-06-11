using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;

namespace Timeline.Services
{
    class CognitoService : ICognitoService
    {
        CognitoAWSCredentials credentials;

        public string CognitoId { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public CognitoService()
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
    }
}
