using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sweet.BRE
{
    public interface INamedObject : IComparable
    {
        string Name { get; }
        string Description { get; set; }
        INamedObjectList List { get; }
    }
}
