using System;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public delegate void ObjectEvent(INamedObjectList list, INamedObject obj);
    public delegate void ObjectNamedEvent(INamedObjectList list, INamedObject obj, string name);

    public interface INamedObjectList
    {
        event ObjectEvent OnRemove;
        event ObjectEvent OnDispose;
        event ObjectEvent OnValidate;
        event ObjectNamedEvent OnAdd;
        event ObjectNamedEvent OnValidateName;

        int Count { get; }
        string[] Names { get; }
        INamedObject[] Objects { get; }
        INamedObject this[string name] { get; set; }

        void Clear();
        bool Contains(string name);
        int IndexOf(INamedObject obj);

        INamedObject Get(string name);
        INamedObject Remove(string name);
        void Add(string name, INamedObject obj);
        void UpdateName(string name, string newName);
    }
}
