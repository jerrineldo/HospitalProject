using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Numerics;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class PublicKeyDto
    {
        public int PublicKeyBase { get; set; }

        //This property should have been refered to as the "modulus", not "modulo"
        public BigInteger PublicKeyModulo { get; set; } 

    }
}