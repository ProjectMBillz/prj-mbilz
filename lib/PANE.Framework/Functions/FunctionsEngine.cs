using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBC.Framework.Functions.DAO;
using CBC.Framework.Functions.DTO;

namespace CBC.Framework.Functions
{
    public class FunctionsEngine
    {
        public static List<UserRole> GetRoles(string mfbCode)
        {
            return UserRoleDAO.RetrieveAll(mfbCode);
        }
    }
}
