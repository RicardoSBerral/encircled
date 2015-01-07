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
			CCApplication application = new CCApplication(false, new CCSize(720f/2f, 1280f/2f));
            application.ApplicationDelegate = new EncircledApplicationDelegate();
			application.StartGame();
		}
	}


}

