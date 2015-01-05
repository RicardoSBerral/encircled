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

            var title = new CCLabelTtf("Encircled", "Parchment", 150) {
                Position = VisibleBoundsWorldspace.Center,
                Color = CCColor3B.Red,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };
			title.Scale = VisibleBoundsWorldspace.Size.Width * 0.8f / title.ContentSize.Width;

			var position = new CCPoint (VisibleBoundsWorldspace.Center.X, VisibleBoundsWorldspace.Center.Y);
			position.Y = position.Y / 2f;
			var touch = new CCLabelTtf("\nToca para jugar\n", "StoryBook", 50) {
				Position = position,
				Color = CCColor3B.Black,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			touch.Scale = VisibleBoundsWorldspace.Size.Width * 0.4f / touch.ContentSize.Width;

			AddChild (title);
			AddChild (touch);
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