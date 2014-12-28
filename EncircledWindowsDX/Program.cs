using System;
using System.Diagnostics;
using CocosSharp;
using Encircled;

namespace EncircledWindowsDX
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			bool fullScreen = false;
			CCSize size = new CCSize (1280, 720);
			CCApplication application = new CCApplication(fullScreen, null);
            application.ApplicationDelegate = new EncircledApplicationDelegate();
			application.StartGame();
		}
	}


}

