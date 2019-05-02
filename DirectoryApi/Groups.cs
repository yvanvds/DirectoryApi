using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryApi
{
    public static class Groups
    {
        private static Dictionary<string, DirectoryEntry> all = null;
        public static Dictionary<string, DirectoryEntry> All { get => all; }


        public static bool ReloadGroups()
        {
            all?.Clear();
            return loadGroups();
        }

        private static bool loadGroups()
        {
            DirectorySearcher search = Connector.GetSearcher(Connector.GroupPath);
            search.Filter = "(&(objectClass=group))";
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
            catch (System.Runtime.InteropServices.COMException e)
            {
                Connector.Log.Add(e.Message, true);
                return false;
            }

            if (all == null)
            {
                all = new Dictionary<string, DirectoryEntry>();
            }

            foreach (SearchResult result in results)
            {
                DirectoryEntry entry = result.GetDirectoryEntry();
                if (entry.Properties.Contains("Name"))
                {
                    all.Add(entry.Properties["Name"].Value.ToString(), entry);
                }
            }

            return true;
        }
    }
}
