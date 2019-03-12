using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using QuickAD.Helper_Classes;
using QuickAD.Models;

namespace QuickAD.Services
{
    class AdService
    {
		#region Fields


		#endregion // Fields

		public AdService()
		{
		}

		#region Types

		public enum ConnectionType
		{
			Staff,
			NonStaff,
			Computers,
			Default
		}

		#endregion // Types

		#region Properties


		#endregion // Properties

		#region Methods

		/// <summary>
		/// Returns a DirectoryEntry based on the type of search to do.
		/// </summary>
		/// <param name="connType"></param>
		/// <returns></returns>
		private static DirectoryEntry _createConnection(ConnectionType connType = ConnectionType.Default)
		{
			DirectoryEntry tempEntry = null;
			DirectoryEntry entry = null;
			try
			{
				switch (connType)
				{
					case ConnectionType.Staff:
						tempEntry = new DirectoryEntry(AdConfiguration.StaffUserSearch);
						break;
					case ConnectionType.NonStaff:
						tempEntry = new DirectoryEntry(AdConfiguration.NonStaffUserSearch);
						break;
					case ConnectionType.Computers:
						tempEntry = new DirectoryEntry(AdConfiguration.ComputerSearch);
						break;
					case ConnectionType.Default:
					default:
						tempEntry = new DirectoryEntry(AdConfiguration.DefaultConnection);
						break;
				}
				entry = tempEntry;
				tempEntry = null;
			}
			finally
			{
				tempEntry?.Close();
			}

			return entry;
		}

		/// <summary>
		/// Check if directory and connection exists
		/// </summary>
		/// <param name="connType"></param>
		/// <returns>True if directory and connection exists</returns>
		public async Task<bool> ConnectionStatus(ConnectionType connType)
		{
			string path;
			bool exists = false;
			switch (connType)
			{
				case ConnectionType.Staff:
					path = AdConfiguration.StaffUserSearch;
					break;
				case ConnectionType.NonStaff:
					path = AdConfiguration.NonStaffUserSearch;
					break;
				case ConnectionType.Computers:
					path = AdConfiguration.ComputerSearch;
					break;
				case ConnectionType.Default:
				default:
					path = AdConfiguration.DefaultConnection;
					break;
			}

			await Task.Run(() =>
			{
				try
				{
					exists = DirectoryEntry.Exists(path);
				}
				catch (COMException)
				{
					exists = false;
				}
			});
			
			return exists;
		}

		/// <summary>
		/// Reset the specified user's password
		/// </summary>
		/// <param name="userName">Distinguished Name of User</param>
		/// <param name="password">New Password</param>
		/// <param name="autoExpire">Should the user change password at next login</param>
		/// <param name="message">Result Message</param>
		public void ResetUserPassword(string userName, SecureString password, bool autoExpire, Action<string> message)
		{
			try
			{
				using (DirectorySearcher search = new DirectorySearcher(_createConnection(ConnectionType.Staff)
																, "(&(|(cn=*" + userName + "*)(samaccountname=" + userName + "))(objectcategory=person)(objectclass=person))"
																, new string[] { "displayname", "distinguishedname", "description", "userprincipalname" }
																, SearchScope.Subtree))
				{
					search.PageSize = 100;
					search.CacheResults = true;
					using (SearchResultCollection result = search.FindAll())
					{
						foreach(SearchResult sr in result)
						{
							var srEntry = sr.GetDirectoryEntry();
							srEntry.Invoke("SetPassword", new object[] { _convertSecureToString(password) });
							srEntry.Properties["LockOutTime"].Value = 0;
							if (autoExpire) srEntry.Properties["pwdLastSet"][0] = 0;
						}
					}
				}
				using (DirectorySearcher search = new DirectorySearcher(_createConnection(ConnectionType.NonStaff)
																	, "(&(|(cn=*" + userName + "*)(samaccountname=" + userName + "))(objectcategory=person)(objectclass=person))"
																	, new[] { "displayname", "distinguishedname", "description", "userprincipalname" }
																	, SearchScope.Subtree))
				{
					search.PageSize = 100;
					search.CacheResults = true;
					using (SearchResultCollection result = search.FindAll())
					{
						foreach (SearchResult sr in result)
						{
							var srEntry = sr.GetDirectoryEntry();
							srEntry.Invoke("SetPassword", new object[] { _convertSecureToString(password) });
							srEntry.Properties["LockOutTime"].Value = 0;
							if (autoExpire) srEntry.Properties["pwdLastSet"][0] = 0;
						}
					}
				}
			}
			catch (COMException)
			{
				message("Connection to server could not be made.");
				return;
			}
			catch (Exception e)
			{
				message(e.Message);
				return;
			}

			message("Password successfully changed!");
		}

		/// <summary>
		/// Returns an ObservableCollection of Users from the username string
		/// </summary>
		/// <param name="userName">User's Name or Username to search</param>
		/// <param name="message">Result Message</param>
		/// <returns></returns>
		public async Task<ObservableCollection<UserSearchResult>> SearchUserAsync(string userName, Action<string> message)
		{
			ObservableCollection<UserSearchResult> resultCollection = new ObservableCollection<UserSearchResult>();
			var tmpCollection = new ObservableCollection<UserSearchResult>();

			try
			{
				using (DirectorySearcher search = new DirectorySearcher(_createConnection(ConnectionType.Staff)
																, "(&(|(cn=*" + userName + "*)(samaccountname=" + userName + "))(objectcategory=person)(objectclass=person))"
																, new string[] { "displayname", "distinguishedname", "description", "userprincipalname" }
																, SearchScope.Subtree))
				{
					search.PageSize = 100;
					search.Asynchronous = true;
					search.CacheResults = true;
					await Task.Run(() =>
					{
						try
						{
							using (SearchResultCollection result = search.FindAll())
							{
								resultCollection = ConvertCollectionToView(result, true, message);
							}
						}
						catch (COMException)
						{
							message("Connection to server could not be made.");
						}
					});
				}
				using (DirectorySearcher search = new DirectorySearcher(_createConnection(ConnectionType.NonStaff)
																, "(&(|(cn=*" + userName + "*)(samaccountname=" + userName + "))(objectcategory=person)(objectclass=person))"
																, new string[] { "displayname", "distinguishedname", "description", "userprincipalname" }
																, SearchScope.Subtree))
				{
					search.PageSize = 100;
					search.Asynchronous = true;
					search.CacheResults = true;
					await Task.Run(() =>
					{
						try
						{
							using (SearchResultCollection result = search.FindAll())
							{
								tmpCollection = ConvertCollectionToView(result, false, message);
							}
						}
						catch (COMException)
						{
							message("Connection to server could not be made.");
						}
					});
				}

				for (int i = 0; i < tmpCollection.Count; i++)
				{
					var isUnique = true;
					for (int j = 0; j < resultCollection.Count; j++)
					{
						if (resultCollection[j].DistinguishedName == tmpCollection[i].DistinguishedName)
						{
							isUnique = false;
							break;
						}
					}
					if (isUnique)	resultCollection.Add(tmpCollection[i]);
				}
				resultCollection = new ObservableCollection<UserSearchResult>(resultCollection.OrderBy(u => u.UserName));
				
			}
			catch (COMException)
			{
				message("Connection to server could not be made.");
				return new ObservableCollection<UserSearchResult>();
			}
			catch (Exception e)
			{
				message(e.Message);
				return new ObservableCollection<UserSearchResult>();
			}

			return resultCollection;
		}

		/// <summary>
		/// Converts the User SearchResultCollection into an ObservableCollection to be consumed by the ViewModel
		/// </summary>
		/// <param name="resultCollection">Collection to convert</param>
		/// <param name="isStaff">Is this apart of the staff collection</param>
		/// <param name="message">Result Message</param>
		/// <returns></returns>
		private static ObservableCollection<UserSearchResult> ConvertCollectionToView(IEnumerable resultCollection, bool isStaff, Action<string> message)
		{
			var convertedCollection = new ObservableCollection<UserSearchResult>();
			if (resultCollection == null) return convertedCollection;
			if (isStaff)
			{
				foreach (SearchResult c in resultCollection)
				{
					convertedCollection.Add(new UserSearchResult(c.GetDirectoryEntry().Properties["displayname"].Value.ToString(),
														c.GetDirectoryEntry().Properties["description"].Value.ToString(),
														c.GetDirectoryEntry().Properties["distinguishedname"].Value.ToString(),
														c.GetDirectoryEntry().Properties["userprincipalname"].Value.ToString()));
				}
			}
			else
			{
				foreach (SearchResult c in resultCollection)
				{
					convertedCollection.Add(new UserSearchResult(c.GetDirectoryEntry().Properties["displayname"].Value.ToString(),
														"",
														c.GetDirectoryEntry().Properties["distinguishedname"].Value.ToString(),
														c.GetDirectoryEntry().Properties["userprincipalname"].Value.ToString()));
				}
			}
			
			convertedCollection = new ObservableCollection<UserSearchResult>(convertedCollection.OrderBy(u => u.UserName));

			message("Results serialized and sorted!");
			return convertedCollection;
		}

		/// <summary>
		/// Returns an ObservableCollection of Computers from the search string
		/// </summary>
		/// <param name="search">Computer name to search</param>
		/// <param name="message">Result Message</param>
		/// <returns></returns>
		public async Task<ObservableCollection<Computer>> GetComputersAsync(string search, Action<string> message)
		{
			ObservableCollection<Computer> computerResult = new ObservableCollection<Computer>();
			try
			{
				using (DirectorySearcher searcher = new DirectorySearcher(_createConnection(ConnectionType.Computers)
													, "(&(|(cn=" + search + ")(name=*" + search + "*))(objectcategory=computer)(objectclass=computer))"
													, new string[] { "displayname", "distinguishedname", "description" }
													, SearchScope.Subtree))
				{
					searcher.PageSize = 100;
					searcher.CacheResults = true;
					await Task.Run(() =>
					{
						try
						{
							using (SearchResultCollection result = searcher.FindAll())
							{
								computerResult = ConvertCompCollectionToView(result, message);
							}
						}
						catch (COMException)
						{
							message("Connection to server could not be made.");
						}
					});
				}
			}
			catch (COMException)
			{
				message("Connection to server could not be made.");
				return new ObservableCollection<Computer>();
			}
			catch (Exception e)
			{
				message(e.Message);
				return new ObservableCollection<Computer>();
			}

			return computerResult;
		}

		/// <summary>
		/// Converts the Computer SearchResultCollection into an ObservableCollection to be consumed by the ViewModel
		/// </summary>
		/// <param name="resultCollection">Collection to convert</param>
		/// <param name="message">Result Message</param>
		/// <returns></returns>
		private static ObservableCollection<Computer> ConvertCompCollectionToView(IEnumerable resultCollection, Action<string> message)
		{
			var convertedCollection = new ObservableCollection<Computer>();
			if (resultCollection == null) return convertedCollection;
			foreach (SearchResult c in resultCollection)
			{
				convertedCollection.Add(new Computer(c.GetDirectoryEntry().Properties["name"].Value.ToString(),
					c.GetDirectoryEntry().Properties["description"].Value.ToString(),
					c.GetDirectoryEntry().Properties["distinguishedname"].Value.ToString()));
			}
			convertedCollection = new ObservableCollection<Computer>(convertedCollection.OrderBy(u => u.Name));

			message("Success!");
			return convertedCollection;
		}
		
		/// <summary>
		/// Sets the Description property of the searched computer
		/// </summary>
		/// <param name="compDn">Name of computer's description to set</param>
		/// <param name="compName">Computer's display name</param>
		/// <param name="desc">Description with prefix to set</param>
		/// <param name="message">Result Message</param>
		/// <returns>Returns the modified computer object</returns>
		public async Task<Computer> SetDescriptionAsync(string compDn, string compName, string desc, Action<string> message)
		{
			var changedComp = new Computer();
			try
			{
				using (DirectorySearcher adSearcher = new DirectorySearcher(_createConnection(ConnectionType.Computers)
					, "(name=*" + compName + "*)"
					, new string[] { "name", "distinguishedname", "description" }
					, SearchScope.Subtree))
				{
					adSearcher.Asynchronous = true;
					await Task.Run(() =>
					{
						try
						{
							using (SearchResultCollection result = adSearcher.FindAll())
							{
								foreach (SearchResult sr in result)
								{
									if (sr.GetDirectoryEntry().Properties["name"].Value.ToString() != compName)
										continue;
									var entry = sr.GetDirectoryEntry();
									entry.Properties["description"].Value = desc;
									entry.CommitChanges();
									entry.Close();
									changedComp = new Computer(compName, desc, compDn);
									break;
								}
							}
						}
						catch(COMException)
						{
							message("Connection to server could not be made.");
							changedComp = null;
						}
					});
				}
			}
			catch (COMException)
			{
				message("Connection to server could not be made.");
				return null;
			}
			catch (NullReferenceException e)
			{
				message(e.Message);
				return null;
			}

			message("Success!");
			return changedComp;
		}

		/// <summary>
		/// Sets the Description property of the searched computer
		/// </summary>
		/// <param name="computer">Computer's description to set</param>
		/// <param name="desc">Description (including prefix) to set</param>
		/// <param name="message">Result Message</param>
		/// <returns></returns>
		public async Task<Computer> SetDescriptionAsync(Computer computer, string desc, Action<string> message)
		{
			var changedComp = new Computer();
			try
			{
				using (DirectorySearcher adSearcher = new DirectorySearcher(_createConnection(ConnectionType.Computers)
					, "(name=*" + computer.Name + "*)"
					, new string[] { "name", "distinguishedname", "description" }
					, SearchScope.Subtree))
				{
					adSearcher.Asynchronous = true;
					await Task.Run(() =>
					{
						using (SearchResultCollection result = adSearcher.FindAll())
						{
							foreach (SearchResult sr in result)
							{
								if (sr.GetDirectoryEntry().Properties["name"].Value.ToString() == computer.Name)
								{
									var entry = sr.GetDirectoryEntry();
									entry.Properties["description"].Value = desc;
									entry.CommitChanges();
									entry.Close();
									changedComp = new Computer(computer.Name, desc, computer.DistinguishedName);
									break;
								}
							}
						}
					});
				}
			}
			catch (COMException)
			{
				message("Connection to server could not be made.");
				return null;
			}
			catch (NullReferenceException e)
			{
				message(e.Message);
				return null;
			}

			message("Success!");
			return changedComp;
		}

		/// <summary>
		/// Converts a SecureString into it's plain text form.
		/// </summary>
		/// <param name="secureString">Encrypted Password</param>
		/// <returns></returns>
		private static string _convertSecureToString(SecureString secureString)
		{
			IntPtr valuePtr = IntPtr.Zero;
			try
			{
				valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
				return Marshal.PtrToStringUni(valuePtr);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
			}
		}

		#endregion // Methods
	}
}
