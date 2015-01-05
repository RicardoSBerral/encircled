using System;
using System.Collections.Generic;
using CocosSharp;

namespace Encircled
{
    public class GameStartLayer : CCLayerColor
    {
        public GameStartLayer () : base ()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = (touches, ccevent) => StartGame();

            AddEventListener (touchListener, this);

            Color = CCColor3B.White;
            Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            var label = new CCLabelTtf("Encircled", "Parchment", 40) {
                Position = VisibleBoundsWorldspace.Center,
                Color = CCColor3B.Red,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            AddChild (label);
        }

		private void StartGame()
		{
			var transition = new CCTransitionCrossFade (1f, GameLayer.GameScene(Window));
			Director.ReplaceScene (transition);
		}

        public static CCScene GameStartLayerScene (CCWindow mainWindow)
        {
            var scene = new CCScene (mainWindow);
            var layer = new GameStartLayer ();

            scene.AddChild (layer);

            return scene;
        }
    }
}