using System;
using System.Diagnostics;
using CocosSharp;
using Encircled;

namespace EncircledWindows
{

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			CCApplication application = new CCApplication(false, new CCSize(500f, 500f));
            application.ApplicationDelegate = new EncircledApplicationDelegate();
			application.StartGame();
		}
	}


}

