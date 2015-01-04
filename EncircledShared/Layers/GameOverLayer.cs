using System;
using System.Collections.Generic;
using CocosSharp;

namespace Encircled
{
    public class GameOverLayer : CCLayerColor
    {

        string scoreMessage = string.Empty;

        public GameOverLayer (int score)
        {
            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

            AddEventListener (touchListener, this);

            scoreMessage = String.Format ("Game Over. ¡Destruiste {0} orbes!", score);

            Color = new CCColor3B (CCColor4B.Black);

            Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

			var scoreLabel = new CCLabelTtf (scoreMessage, "MarkerFelt", 22) {
                Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X, VisibleBoundsWorldspace.Size.Center.Y + 50),
                Color = new CCColor3B (CCColor4B.Yellow),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            AddChild (scoreLabel);

            var playAgainLabel = new CCLabelTtf ("Tap to Play Again", "MarkerFelt", 22) {
                Position = VisibleBoundsWorldspace.Size.Center,
                Color = new CCColor3B (CCColor4B.Green),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
            };

            AddChild (playAgainLabel);
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