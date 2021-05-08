using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Numerics;
using EasyEncryption; //Encryption library for hashing authenication key

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Authentication
    {

        private static readonly HttpClient client;


        static Authentication()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44349/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        }

        /// <summary>
        /// The getServerPublicKey method accesses the server's API endpoint to obtain the server's public key, used for generating a token when combined with the client's private key. The token is used to access the APIs' protected resources.
        /// </summary>
        /// <returns>A PublicKeyDto object containing the server's public key elements - the base and modulo for a modular exponentiation equation</returns>
        public static PublicKeyDto getServerPublicKey()
        {
            string url = "publickeydistribution";

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                PublicKeyDto publicKeyDto = response.Content.ReadAsAsync<PublicKeyDto>().Result;

                return publicKeyDto;

            }
            else
            {
                throw new Exception("Unable to obtain public key from server");
            }
        }


        /// <summary>
        /// The getAuthKey method combines the server's public key and the client's private key using modular exponentiation to generate the server's API access key, the generated value is then hashed using SHA1
        /// </summary>
        /// <param name="privateKey">Client's private key</param>
        /// <returns>SHA1 hashed value of a server's API access key token</returns>
        public static string getAuthKey(int privateKey)
        {


            try
            {
                PublicKeyDto publicKeyDto = getServerPublicKey();

                BigInteger authKey = BigInteger.Pow(publicKeyDto.PublicKeyBase, privateKey) % publicKeyDto.PublicKeyModulo;

                string hashedAuthKeyString = EasyEncryption.SHA.ComputeSHA1Hash(authKey.ToString());

                return hashedAuthKeyString;

            }
            catch
            {
                throw new Exception("Unable to obtain public key or generate authenication key");
            }

        }


    }
}