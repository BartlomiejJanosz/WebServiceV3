using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testt
{
    //Class created to deliver information which table in LangWarDataBase should be cleared
    public class DeleteInfo
    {
        public string TableName {get;}
        public DeleteInfo(string tableName)
        {
            TableName = tableName;
        }
    }
}
