using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryApi
{
    public static class ClassGroupManager
    {
        private static List<ClassGroup> all = new List<ClassGroup>();
        public static List<ClassGroup> All { get => all; }

        public static int Count(bool endpointsOnly)
        {

            int result = 0;
            foreach (var group in All)
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
            if (!endpointsOnly) result += All.Count;
            return result;

        }

        public static async Task<bool> Load()
        {
            return await Task.Run(() =>
            {
                all.Clear();
                return LoadGroups(Connector.StudentPath, all);
            });

        }

        private static bool LoadGroups(string path, List<ClassGroup> list)
        {
            DirectorySearcher search = Connector.GetSearcher(path);
            search.Filter = "(&(objectClass=organizationalUnit))";
            search.SizeLimit = 500;
            search.PropertiesToLoad.Clear();
            search.PropertiesToLoad.Add("ou");
            search.SearchScope = SearchScope.OneLevel;
            SearchResultCollection results;

            try
            {
                results = search.FindAll();
            }
            catch (DirectoryServicesCOMException e)
            {
                Connector.Log.Add(e.Message, true);
                return false;
            }

            foreach (SearchResult r in results)
            {
                DirectoryEntry entry = r.GetDirectoryEntry();
                list.Add(new ClassGroup());
                list.Last().DN = entry.Path;
                list.Last().Name = entry.Properties.Contains("ou") ? entry.Properties["ou"].Value.ToString() : "";
            }

            foreach (ClassGroup cg in list)
            {
                if (!LoadGroups(cg.DN, cg.Children))
                {
                    return false;
                }
            }

            return true;
        }

        public static ClassGroup Get(string nameOfGroup)
        {
            foreach (var group in All)
            {
                ClassGroup result = group.Get(nameOfGroup);
                if (result != null) return result;
            }
            return null;
        }
    }
}
