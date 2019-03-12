<h1>QuickAD</h1>
<p>A WPF application to speed up high-level management of Active Directory</p>
<h2 id="Top">Index</h2>
<ol>
	<li><a href="#Usage">Usage</a></li>
		<ul>
			<li><a href="#Computers">Computers</a></li>
				<ul>
					<li><a href="#Search">Search</a></li>
					<li><a href="#Import">Import</a></li>
				</ul>
			<li><a href="#Users">Users</a></li>
		</ul>
	<li><a href="#Setup">Setup</a></li>
		<ul>
			<li><a href="#Configuration File">Configuration File</a></li>
				<ul>
					<li><a href="#Site Prefix">Site Prefix</a></li>
					<li><a href="#Connections">Connections</a></li>
				</ul>
			<li><a href="#PrefixData">PrefixData</a></li>
		</ul>
	<li><a href="#todo">TODO</a></li>
</ol>
<h2 id="Usage">Usage</h2>
<div>
	<p>QuickAD uses your windows authentication to modify Active Directory entries. Any organizational units and entries a user would normally have access to are editable. Any directories the user does not have permissions for should be excluded in the LDAP connection to reduce search times and ensure only relevant results are presented.</p>
	<br>
	<h3 id="Computers">Computers</h3>
	<h4 id="Search">Search</h4>
	<div>
		<p>The search tab is used to view and edit single computers.<br>Users can search for a computer name, and if multiple results are found, easily navigate through the results and edit them accordingly. This saves clicks for searching several computers and viewing their details.<br><br>This program prioritizes the description property, which can be used to describe the model, location, and/or status of a computer. Some aspects of the description can be removed from the description text box to prevent editing. Commonly these are computer models which are not subject to change and are repeated across multiple entries. See the <a href="#PrefixData.json">setup section</a> for how to edit the list contained in "PrefixData.json".</p>
		<a href="#Top">Top</a>
	</div>
	<h4 id="Import">Import</h4>
	<div>
		<p>The Import tab is useful for editing large numbers of entries to have similar descriptions, either because new computers have been added to the tree or because computers have moved location or changed status.</p>
		<p>Data can be imported from either an Excel Spreadsheet or .csv file. The first row should contain the new descriptions (this should not contain any prefix which is listed in the PrefixData.json file). All computers you would like to apply this description to should be listed in the same column, starting at row 2. Any number of columns or rows can be added, but be aware that importing can not currently be canceled and will continue until all cells have been tried. Computer, network, and server performance will all affect completion time.</p>
		<p>Other tasks can be completed while executing an import if necessary.</p>
		<a href="#Top">Top</a>
	</div>
	<h3 id="Users">Users</h3>
	<p>From the Users page, users can be searched for either by the user's first and/or last name or the SAM Account name. Both Staff and NonStaff users will be returned if both connections are present in the configuration file. Once found, results are displayed and the selected user's password can be changed. All rules applicable to the group policy that user belongs to are obeyed when setting a new password.<br>If settings a temporary password, selecting the "Auto Expire Password" will cause Windows to prompt the user to change their password on next log in. This will only work if passwords are set to expire after a set amount of time.</p>
	<a href="#Top">Top</a>
</div>
<h2 id="Setup">Setup</h2>
<div>
	<h3 id="Configuration File">Configuration File</h3>
	<p>On first run QuickAD will load the default configuration file (default.config). You will want to copy/edit/rename this file to include the correct LDAP connection for your specific usage. </p>
	<a href="#Top">Top</a>
	<h3 id="Site Prefix">Site Prefix</h3>
	<p>If your organization or group uses a prefix in their computer names to help identify computers from different sites, you can set that in the config file as well. This can be ignored otherwise.<br>If used, this can speed up searches as users don't need to add wildcards or type the prefix out each time.<br>Currently, only one site prefix is supported.</p>
	<a href="#Top">Top</a>
	<h3 id="Connections">Connections</h3>
	<h4>Default Connection</h4>
	<p>The default connection is the fall back in case the other LDAP connections aren't available. Usually this is the root of you Active Directory tree.</p>
	<h4>Computer Connection</h4>
	<p>The computer connection points to the OU containing computers. This should be the highest level the user has privledges to modify.</p>
	<h4>Staff Connection</h4>
	<p>The Staff Users connection points to the OU containing Staff Users. This should be the highest level the user has privledges to modify.</p>
	<h4>NonStaff Connection</h4>
	<p>The NonStaff Users connection points to the OU containing NonStaff Users. These are usually guests, customers, or similar. This should be the highest level the user has privledges to modify.This field can be left blank if it doesn't apply to your organization.</p>
	<br>
	<p>Once your config file is created, select Settings from within the application and browse for the file. The selected config file will be copied to the application directory and loaded into memory. The next time you run QuickAD this config file will be used as default.</p>
	<a href="#Top">Top</a>
	<h3 id="PrefixData.json">PrefixData</h3>
	<p>This file contains a list of strings which should be omitted from the description text box when searching for a computer. Additionally, when importing, a description containing one of the items in this list will have the new description taken from the Excel or csv file appended to the item found in the list.<br>These should be treated as non-modifiable beginnings of descriptions. If your organization does not utilize this pattern, the list can be kept empty.</p>
	<a href="#Top">Top</a>
</div>
<h2 id="todo">TODO:</h2>
<ul>
	<li>Better error and status reporting during search/import/save tasks.</li>
	<li>Option to cancel an import task.</li>
	<li>Ability to switch between multiple saved configurations without leaving the app interface/opening a file dialog.</li>
	<li>Option to load multiple configurations at once. (Not necessarily needed if previous is implemented.)</li>
	<li>Handle multiple Site Prefix strings at once.</li>
	<li>Change import interface to closer match the search tab in the computer page.</li>
	<li>View tree structure for each of the configured connections.</li>
	<li>Allow moving entries between OUs within configured connection scope.</li>
	<li>Make more information available about users, such as email address and/or current OU placement.</li>
	<li>Allow searching on user's email.</li>
</ul>
<a href="#Top">Top</a>
