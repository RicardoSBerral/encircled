using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled
{
	public class GameLayer : CCLayerColor
	{
		const float GAMEFIELD_PROPORTION = 0.85f;
		const float ORB_INTERVAL = 1f;

		GameField field;

		public GameLayer ()
		{
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesBegan = Aim;
			touchListener.OnTouchesMoved = Aim;
			AddEventListener (touchListener, this);

			Color = new CCColor3B (CCColor4B.White);
			Opacity = 255;     

			StartScheduling ();

			//CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic ("Sounds/backgroundMusic", true);
		}

		void StartScheduling ()
		{
			Schedule (t => {
				field.Shoot(ORB_INTERVAL);
			}, ORB_INTERVAL);

			Schedule (t => field.CheckOrbsCollision ());
//
//            Schedule (t => {
//                world.Step (t, 8, 1);

//                foreach (CCPhysicsSprite sprite in ballsBatch.Children) {
//                    if (sprite.Visible && sprite.PhysicsBody.Position.x < 0f || sprite.PhysicsBody.Position.x * PTM_RATIO > ContentSize.Width) { //or should it be Layer.VisibleBoundsWorldspace.Size.Width
//                        world.DestroyBody (sprite.PhysicsBody);
//                        sprite.Visible = false;
//                        sprite.RemoveFromParent ();
//                    } else {
//                        sprite.UpdateBallTransform();
//                    }
//                }
//            });
		}

		void EndGame ()
		{
			// Stop scheduled events as we transition to game over scene
			UnscheduleAll ();

			var gameOverScene = GameOverLayer.SceneWithScore (Window, 10);
			var transitionToGameOver = new CCTransitionMoveInR (0.3f, gameOverScene);

			Director.ReplaceScene (transitionToGameOver);
		}

		bool ShouldEndGame ()
		{
			//return elapsedTime > GAME_DURATION;
			return false;
		}

		void Aim (List<CCTouch> touches, CCEvent touchEvent)
		{
			var location = touches [0].Location;
			field.Direction = location;
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.NoBorder;

//            grass.Position = VisibleBoundsWorldspace.Center;
//            monkey.Position = VisibleBoundsWorldspace.Center;

//            var b = VisibleBoundsWorldspace;
			//            sun.Position = b.UpperRight.Offset (-100, -100); //BUG: doesn't appear in visible area on Nexus 7 device

			field = new GameField (
				this.VisibleBoundsWorldspace.Size.Height * GAMEFIELD_PROPORTION,
				new CCColor4F (CCColor4B.Black))
			{
				AnchorPoint = CCPoint.AnchorMiddle,
				Position = this.VisibleBoundsWorldspace.Center
			};
			this.AddChild (field);
		}

		public override void OnEnter ()
		{
			base.OnEnter ();

			// InitPhysics ();
		}

		public static CCScene GameScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new GameLayer ();
			
			scene.AddChild (layer);

			return scene;
		}
	}
}