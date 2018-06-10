using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timeline.Droid.Services
{
    class GoogleAuthService : Timeline.Services.IGoogleAuthService
    {
        public void Test()
        {
            //GooglePlayServicesUtil.isGooglePlayServicesAvailable(getApplicationContext());
            //AccountManager am = AccountManager.get(this);
            //Account[] accounts = am.getAccountsByType(GoogleAuthUtil.GOOGLE_ACCOUNT_TYPE);
            //String token = GoogleAuthUtil.getToken(getApplicationContext(), accounts[0].name,
            //        "audience:server:client_id:YOUR_GOOGLE_CLIENT_ID");
            //Map<String, String> logins = new HashMap<String, String>();
            //logins.put("accounts.google.com", token);
            //credentialsProvider.setLogins(logins);
        }

        public void Test2()
        {

        }



    }
}