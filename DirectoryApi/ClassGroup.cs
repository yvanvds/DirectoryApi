using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryApi
{
    public class ClassGroup
    {
        public string DN;
        public string Path;
        public string Name;
        public List<ClassGroup> Children = new List<ClassGroup>();

        public bool IsContainer { get => Children.Count > 0; }

        public int Count(bool endpointsOnly)
        {
            int result = 0;
            foreach (var group in Children)
            {
                if (endpointsOnly)
                {
                    if (group.Children.Count == 0)
                    {
                        result++;
                    }
                    else
                    {
                        result += group.Count(endpointsOnly);
                    }
                }
                else
                {
                    result += group.Count(endpointsOnly);
                }

            }
            if (!endpointsOnly) result += Children.Count;
            return result;
        }

        public ClassGroup Get(string nameOfGroup)
        {
            if (Name.Equals(nameOfGroup, StringComparison.CurrentCultureIgnoreCase))
            {
                return this;
            }

            foreach (var group in Children)
            {
                ClassGroup result = group.Get(nameOfGroup);
                if (result != null) return result;
            }
            return null;
        }
    }
}
