using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickAD
{
    class ADService
    {
        public ADService()
        {

        }

        public string getDescription()
        {
            using (DirectoryEntry entry = new DirectoryEntry("LDAP://HAM-DC/cn=Haile%20-%20HAM,cn=Middle,cn=Schools,cn=District%20Computers,dc=manateeschools,dc=net"))
            {
                using (DirectorySearcher adSearcher = new DirectorySearcher(entry))
                {
                    string computerName = "HAM295501";
                    adSearcher.Filter = "(&(objectClass=computer)(cn=" + computerName + "))";
                    adSearcher.SearchScope = SearchScope.Subtree;
                    adSearcher.PropertiesToLoad.Add("description");
                    SearchResult searchResult = adSearcher.FindOne();

                    return searchResult.GetDirectoryEntry().Properties["description"].Value.ToString();
                }
            }
        }
    }
}

/*
    How to get a computer's description:

     using (DirectoryEntry entry = new DirectoryEntry("LDAP://<your-ad-server-name>/dc=<domain-name-part>,dc=<domain-name-part>",
         "Administrator", "Your Secure Password", AuthenticationTypes.Secure))
    {
      using (DirectorySearcher adSearcher = new DirectorySearcher(entry))
      {
        string computerName = "computer01";
        adSearcher.Filter = "(&(objectClass=computer)(cn=" + computerName + "))";
        adSearcher.SearchScope = SearchScope.Subtree;
        adSearcher.PropertiesToLoad.Add("description");
        SearchResult searchResult = adSearcher.FindOne();

        Console.Out.WriteLine(searchResult.GetDirectoryEntry().Properties["description"].Value);
      }
    }

    Search only part of the tree:

    LDAP://<your-ad-server-name>/cn=Computers,dc=<domain-name-part>,dc=<domain-name-part>

 *
 */
