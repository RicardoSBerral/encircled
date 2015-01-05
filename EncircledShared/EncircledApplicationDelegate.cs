using CocosDenshion;
using CocosSharp;

namespace Encircled
{

    public class EncircledApplicationDelegate : CCApplicationDelegate
	{
		public readonly string bmPath;
		public readonly string efPath;

		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
        {
            application.PreferMultiSampling = false;
            application.ContentRootDirectory = "Content";

            CCSize winSize = mainWindow.WindowSizeInPixels;
            mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

			CCScene scene = GameStartLayer.GameStartLayerScene(mainWindow);
			mainWindow.RunWithScene (scene);
        }

        public override void ApplicationDidEnterBackground (CCApplication application)
        {
            // stop all of the animation actions that are running.
            application.Paused = true;
        }

        public override void ApplicationWillEnterForeground (CCApplication application)
        {
            application.Paused = false;
        }
    }
}