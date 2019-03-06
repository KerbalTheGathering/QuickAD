using QuickAD.Helper_Classes;

namespace QuickAD.Models
{
	class Computer
	{
		public Computer()
		{
		}

		public Computer(string name)
		{
			Name = name;
		}

		public Computer(string name, string description)
		{
			Name = name;
			DescPrefix = Helper_Classes.DescriptionPrefix.GetDescriptionPrefix(description);
			Description = Helper_Classes.DescriptionPrefix.GetTrimmedDescription(description, DescPrefix);
		}

		public Computer(string name, string description, string distinguishedName)
		{
			Name = name;
			DistinguishedName = distinguishedName;
			DescPrefix = DescriptionPrefix.GetDescriptionPrefix(description);
			Description = DescriptionPrefix.GetTrimmedDescription(description, DescPrefix);
		}

		public Computer(string name,string description, string distinguishedName, string descPrefix)
		{
			Name = name;
			Description = description;
			DistinguishedName = distinguishedName;
			DescPrefix = descPrefix;
		}

		#region Properties

		public string Name { get; set; }
		public string Description { get; set; }
		public string DistinguishedName { get; set; }
		public string DescPrefix { get; set; }

		#endregion // Properties
	}
}
