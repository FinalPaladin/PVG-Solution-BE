using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Commons
{
    public class ColumType
    {
        public static string IntIdentity(int seed = 1, int increment = 1) => $"int IDENTITY({seed},{increment})";
        public static string TypeVarchar(string _length = "10") => $"varchar({_length})";
        public static string TypeDateTime() => "datetime";
    }
}
