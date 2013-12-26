------------
Setting Up
------------
1) Download Source Codes of
	1.1) Windows Phone Toolkit from Codeplex.com
	1.2) Coding4fun toolkit
2) Place these sources in a folder called Code Samples/ which is at the same level of Medicines Catalogue
	2.1) Path for Windows Phone Toolkit: \Code Samples\Windows Phone Toolkit (Nov 2011)
	2.2) Path for Coding4fun toolkit: \Code Samples\Coding for fun toolkit\
3) Build both the projects before building the Medicines Catalogue Project
4) Update the references in Medicines Catalogue if required
5) Run the app!



--------------------
Database Migrations
--------------------
Migrations are handled in App.xaml.cs
"currentDBVersion" variable holds the latest version of the DB. Whenever we make a change to DDL, we increment this number manually by 1
--> Increments should always be by 1 and decimal increments are not allowed as UpdateDB() will fail