using System;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public interface INamedObject : IComparable
    {
        string Name { get; }
        string Description { get; set; }
        INamedObjectList List { get; }
    }
}
