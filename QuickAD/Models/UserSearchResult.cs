namespace QuickAD.Models
{
    class UserSearchResult
    {
        #region Fields


        #endregion // Fields

        public UserSearchResult(string userName, string userRole, string distinguishedName, string userPrincipalName)
        {
            UserName = userName;
			UserRole = userRole;
			DistinguishedName = distinguishedName;
			UserPrincipalName = userPrincipalName;
        }

        #region Properties

        public string UserName { get; set; }

        public string UserRole { get; set; }

		public string DistinguishedName { get; set; }

		public string UserPrincipalName { get; set; }

        #endregion // Properties
    }
}
