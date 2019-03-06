using System.DirectoryServices;
using System.Linq;

namespace QuickAD.Services
{
	class ComputerObjectCache
	{
		#region Fields

		private DirectoryEntries _objectCache;
		private AdService _adService;

		#endregion //Fields

		public ComputerObjectCache(DirectoryEntries objectCache)
		{
			_objectCache = objectCache;

			//var query = from DirectoryEntry entry in ObjectCache
			//	where entry.Properties["Name"].Value.ToString() == ""
			//	orderby entry.Properties["Name"].Value.ToString() descending 
			//	select entry;
		}

		public ComputerObjectCache(AdService adService)
		{
			_adService = adService;
		}

		#region Properties

		public DirectoryEntries ObjectCache
		{
			get { return _objectCache; }
			set { _objectCache = value; }
		}

		#endregion // Properties

		#region Methods

		public DirectoryEntry GetComputer(string computerName)
		{
			var query = from DirectoryEntry entry in ObjectCache
						where entry.Properties["name"].Value.ToString() == computerName
						select entry;
			return query.First();
		}

		public DirectoryEntry SetComputerDescription(string computerName, string description)
		{
			var comp = GetComputer(computerName);
			comp.Properties["description"].Value = description;
			comp.CommitChanges();
			return comp;
		}



		#endregion // Methods
	}
}
