using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Utils
{
    internal static class APIDataReader
    {
        public static DateTime? GetDateTimeNull(this SqlDataReader dataReader, string name)
        {
            if (dataReader.IsDBNull(dataReader.GetOrdinal(name)))
            {
                return null;
            }
            else
            {
                return dataReader.GetDateTime(dataReader.GetOrdinal(name));
            }
        }



        public static string GetStringValue(this DbDataReader reader, string name)
        {
            if (reader.IsDBNull(reader.GetOrdinal(name)))
            {
                return string.Empty;
            }
            else
            {
                return reader.GetString(reader.GetOrdinal(name));
            }
        }

        public static int GetInt32Value(this DbDataReader reader, string i)
        {
            if (reader.IsDBNull(reader.GetOrdinal(i)))
            {
                return 0;
            }
            else
            {
                return reader.GetInt32(reader.GetOrdinal(i));
            }
        }

        public static decimal GetDecimalValue(this DbDataReader reader, string i)
        {
            if (reader.IsDBNull(reader.GetOrdinal(i)))
            {
                return 0;
            }
            else
            {
                return reader.GetDecimal(reader.GetOrdinal(i));
            }
        }

        public static bool GetBooleanValue(this DbDataReader reader, string i)
        {
            if (reader.IsDBNull(reader.GetOrdinal(i)))
            {
                return false;
            }
            else
            {
                return reader.GetBoolean(reader.GetOrdinal(i));
            }
        }


    }
}
