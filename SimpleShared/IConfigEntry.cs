using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShared
{
    public interface IConfigEntry
    {
        int EntryID { get; set; }
        int ParentID { get; set; }
    }
}
