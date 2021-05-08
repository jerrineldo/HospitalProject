using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Diagnostics;
using System.Web;
using Red_Lake_Hospital_Redesign_Team6.Models;
using System.Numerics;


namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class PublicKeyDistributionController : ApiController
    {

        //This controller is part of an attempt to secure access to the API controllers using public-private keys and the Diffie-Hellman key exchange method, but only one-half of the normal Diffie-Hellman key exchange is needed (since a public-private key token is sent by the client to the server for authentication, but not the other way around). The server is assumed to have knowledge of the client's private key. This implementation is vulnerable against man-in-the-middle attacks which will require furhter hardening to guard against (i.e. cryptograpic signature verification of the client's key token)


        //Set the public key components to implement the Diffie-Hellman/modular exponentiation method
        private readonly int KEYBASE = 7;
        private readonly BigInteger KEYMODULO = BigInteger.Pow(2, 607) - 1; //A large prime number


        /// <summary>
        /// The PublicKeyDistribution method was created to provide the server's public key to any requesting client so that key exchanges can be implemented programatically. The intention is to allow clients to obtain the public key from the server though this open resource, then use it to generate the key token for accessing the API server's protected resources. This method could breakdown if the value of KEYMODULO becomes vary large because the client machine may not be able to parse the data (possible solution: convert values to hexadecimal before transmission?)
        /// </summary>
        /// <returns>A PublicKeyDto object containing the server's public key elements</returns>
        [HttpGet]
        [Route("api/publickeydistribution")]
        [ResponseType(typeof(PublicKeyDto))]
        public IHttpActionResult PublicKeyDistribution()
        {

            PublicKeyDto publicKeyDto = new PublicKeyDto
            {
                PublicKeyBase = KEYBASE,
                PublicKeyModulo = KEYMODULO
            };

            return Ok(publicKeyDto);
        }


    }
}
