using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickAD.Models
{
	class Configuration
	{
		private string _sitePrefix;
		private string _defaultConnection;
		private string _computerSearch;
		private string _staffUserSearch;
		private string _nonStaffUserSearch;


		public Configuration()
		{

		}

		#region Properties

		public string SitePrefix
		{
			get { return _sitePrefix; }
		}

		public string ComputerSearch
		{
			get { return _computerSearch; }
		}

		public string StaffUserSearch
		{
			get { return _staffUserSearch; }
		}

		public string NonStaffUserSearch
		{
			get { return _nonStaffUserSearch; }
		}

		public string DefaultConnection
		{
			get { return _defaultConnection; }
		}

		#endregion // Properties
	}
}
