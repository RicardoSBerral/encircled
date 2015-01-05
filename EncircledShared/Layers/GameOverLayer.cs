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
			touchListener.OnTouchesEnded = (touches, ccevent) => RestartGame();

            AddEventListener (touchListener, this);

			this.score = score;

			Color = CCColor3B.White;
			Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

			var score = new CCLabelTtf("Has destruido " + this.score + " orbes.", "StoryBook", 50) {
				Position = VisibleBoundsWorldspace.Center,
				Color = CCColor3B.Black,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			score.Scale = VisibleBoundsWorldspace.Size.Width * 0.8f / score.ContentSize.Width;

			var position = new CCPoint (VisibleBoundsWorldspace.MidX, VisibleBoundsWorldspace.MidY * 0.5f);
			var touch = new CCLabelTtf("\nToca para jugar\n", "StoryBook", 50) {
				Position = position,
				Color = CCColor3B.Black,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			touch.Scale = VisibleBoundsWorldspace.Size.Width * 0.6f / touch.ContentSize.Width;

			AddChild (score);
			AddChild (touch);
        }

		public void RestartGame()
		{
			var transition = new CCTransitionCrossFade (1f, GameLayer.GameScene (Window));
			Window.DefaultDirector.ReplaceScene (transition);
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