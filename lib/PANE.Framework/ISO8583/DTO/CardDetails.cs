using System;
using System.Collections.Generic;
using System.Text;

namespace CBC.Framework.ISO8583.DTO
{
    public class CardDetails
    {
        private string _PAN;
        public bool UsePIN { get; set; }
        public string PAN
        {
            get { return _PAN; }
            set { _PAN = value; }
        }
        private DateTime _expiryDate;

        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set { _expiryDate = value; }
        }
        private byte[] _PIN = new byte[] { 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04 };

        public byte[] PIN
        {
            get { return _PIN; }
            set { _PIN = value; }
        }
    }
}
