using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.DAO;

namespace CBC.Framework.ISO8583.DAO
{
    public class IsoMessageDAO : CoreDAO<IsoMessage, long>
    {
        public static IsoMessage RetrieveBySTAN(string mfbCode, long stan, MessageEntity source)
        {
            IsoMessage result = null;
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(IsoMessage)).Add(Expression.Eq("STAN", stan)).Add(Expression.Eq("Type", 200)).Add(Expression.Eq("Sender", source)).UniqueResult<IsoMessage>();
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static IsoMessage RetrieveBySTAN(string mfbCode, string originalDataElements, MessageEntity source)
        {
            IsoMessage result = null;
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(IsoMessage)).Add(Expression.Eq("OriginalDataElements", originalDataElements)).Add(Expression.Eq("Sender", source)).AddOrder(Order.Desc("RequestDate")).UniqueResult<IsoMessage>();
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static IList<IsoMessage> Search(string mfbCode, DateTime dateFrom, DateTime dateTo, string messageType, string stan, int sender, int receiver)
        {
            IList<IsoMessage> result = new List<IsoMessage>();
            ISession session = BuildSession(mfbCode);
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(IsoMessage)).AddOrder(Order.Desc("RequestDate"));
                if (!dateFrom.Equals(DateTime.MinValue) && !dateTo.Equals(DateTime.MinValue))
                {
                    criteria.Add(Expression.Between("RequestDate", dateFrom.Date, dateTo.Date.AddDays(1).AddSeconds(-1)));
                }
                if (!String.IsNullOrEmpty(messageType) && !String.IsNullOrEmpty(messageType.Trim()))
                {
                    criteria.Add(Expression.Like("Type", Convert.ToInt32(messageType)));
                }
                if (!String.IsNullOrEmpty(stan) && !String.IsNullOrEmpty(stan.Trim()))
                {
                    criteria.Add(Expression.Like("STAN", Convert.ToInt64(stan)));
                }                
                if (sender != 0)
                {
                    criteria.Add(Expression.Eq("Sender", (MessageEntity)sender));
                }
                if (receiver != 0)
                {
                    criteria.Add(Expression.Eq("Receiver", (MessageEntity)receiver));
                }
                result = criteria.List<IsoMessage>();
            }
            catch
            {
                throw;
            }

            return result;
        }


        public static IList<IsoMessage> Search(string mfbCode, DateTime dateFrom, DateTime dateTo, string messageType, string stan, int sender, int receiver, string cardPAN, int maximumRows, int startRowIndex)
        {
            IList<IsoMessage> result = new List<IsoMessage>();
            ISession session = BuildSession(mfbCode);
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(IsoMessage)).AddOrder(Order.Desc("RequestDate"));
                if (!dateFrom.Equals(DateTime.MinValue) && !dateTo.Equals(DateTime.MinValue))
                {
                    criteria.Add(Expression.Between("RequestDate", dateFrom.Date, dateTo.Date.AddDays(1).AddSeconds(-1)));
                }
                if (!String.IsNullOrEmpty(messageType) && !String.IsNullOrEmpty(messageType.Trim()))
                {
                    criteria.Add(Expression.Like("Type", Convert.ToInt32(messageType)));
                }
                if (!String.IsNullOrEmpty(stan) && !String.IsNullOrEmpty(stan.Trim()))
                {
                    criteria.Add(Expression.Like("STAN", Convert.ToInt64(stan)));
                }
                if (!String.IsNullOrEmpty(cardPAN) && !String.IsNullOrEmpty(cardPAN.Trim()))
                {
                    criteria.Add(Expression.Like("CardPAN", cardPAN));
                }
                if (sender != 0)
                {
                    criteria.Add(Expression.Eq("Sender", (MessageEntity)sender));
                }
                if (receiver != 0)
                {
                    criteria.Add(Expression.Eq("Receiver", (MessageEntity)receiver));
                }
                result = criteria.SetMaxResults(41).SetFirstResult(startRowIndex).List<IsoMessage>(); 
            }
            catch
            {
                throw;
            }

            return result;
        }
 
    }
}