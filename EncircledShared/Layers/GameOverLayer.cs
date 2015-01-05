using System;
using System.Collections.Generic;
using CocosSharp;

namespace Encircled
{
    public class GameOverLayer : CCLayerColor
    {
		int score;
        string scoreMessage = string.Empty;

        public GameOverLayer (int score)
        {
            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

            AddEventListener (touchListener, this);

			this.score = score;

			Color = CCColor3B.White;
			Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

//            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

			var label = new CCLabelTtf("Has destruido " + score + " orbes.", "MarkerFelt", 22) {
				Position = VisibleBoundsWorldspace.Center,
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			AddChild (label);

//            var playAgainLabel = new CCLabelTtf ("Tap to Play Again", "MarkerFelt", 22) {
//                Position = VisibleBoundsWorldspace.Size.Center,
//                Color = new CCColor3B (CCColor4B.Green),
//                HorizontalAlignment = CCTextAlignment.Center,
//                VerticalAlignment = CCVerticalTextAlignment.Center,
//                AnchorPoint = CCPoint.AnchorMiddle,
//            };
//
//            AddChild (playAgainLabel);
        }

        public static CCScene SceneWithScore (CCWindow mainWindow, int score)
        {
            var scene = new CCScene (mainWindow);
            var layer = new GameOverLayer (score);

            scene.AddChild (layer);

            return scene;
        }
    }
}