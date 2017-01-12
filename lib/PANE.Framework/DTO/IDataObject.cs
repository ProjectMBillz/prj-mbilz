using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.DTO
{
    public interface IDataObject
    {
        Int64 ID { get; set; }

        //System.Boolean IsDeleted { get; set; }

        string MFBCode { get; set; }

    }
}
