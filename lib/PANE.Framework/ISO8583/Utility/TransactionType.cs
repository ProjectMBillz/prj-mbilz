using System;
using System.Collections.Generic;
using System.Text;

namespace CBC.Framework.ISO8583.Utility
{
    //public class TransactionType
    //{
    //    public const string Purchase = "00";
    //    public const string CashWithdrawal = "01";
    //    public const string BalanceEnquiry = "31";
    //    public const string MiniStatementInquiry = "38";
    //    public const string LinkedAccountInquiry = "39"; 
    //    public const string AccountsTransfer = "40";
    //    public const string Payment = "50";
    //    public const string HotListCard = "90";
    //    public const string ChangePIN = "92";

    //}

    public enum TransactionType
    {
        Purchase = 0,
        CashWithdrawal = 1,
        BalanceEnquiry = 31,
        FullStatement = 36,
        MiniStatementInquiry = 38,
        LinkedAccountInquiry = 39,
        AccountsTransfer = 40,
        Payment = 50,
        HotListCard = 90,
        ChangePIN = 92

    }
}
