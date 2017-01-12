using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBC.Framework.DTO;

namespace CBC.Framework.ISO8583.DTO
{
    public class TransAmount : DataObject
    {
        public string Amount { get; set; }
        public string Sign { get; set; }
        public string CurrencyCode { get; set; }
    }
}
