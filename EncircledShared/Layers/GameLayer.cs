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
		public const float GAMEFIELD_PROPORTION = 0.85f;
		public const float ORB_INTERVAL = 3f;
		public const float NEWLINE_INTERVAL = 10f;
		public const int PTM_RATIO = 32;

		// Mundos
		private b2World world;
		CCBox2dDraw debugDraw;

		private GameField field;

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
				field.Shoot();
				field.Grow (ORB_INTERVAL);
			}, ORB_INTERVAL);

			Schedule (t => {
				field.PushLine ();
			}, NEWLINE_INTERVAL);

			Schedule (t => {
				world.Step (t, 8, 1);
				field.CheckOrbsCollision ();
				field.UpdateOrbs();
			});
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.NoBorder;

			InitPhysics ();
			field = new GameField (
				this.VisibleBoundsWorldspace.Size.Height * GAMEFIELD_PROPORTION,
				new CCColor4F (CCColor4B.Black),
				world);
			field.AnchorPoint = CCPoint.Zero;//CCPoint.AnchorMiddle;
			field.Position = new CCPoint(field.PlaySize.Width, 0f);//this.VisibleBoundsWorldspace.Center;
			this.AddChild (field);
		}

		public override void OnEnter ()
		{
			base.OnEnter ();
		}

		void InitPhysics ()
		{
			CCSize s = Layer.VisibleBoundsWorldspace.Size;

			var gravity = b2Vec2.Zero;
			world = new b2World (gravity);

			world.SetAllowSleeping (true);
			world.SetContinuousPhysics (true);

			// TODO
			EnableDebugMode();
		}

		void EnableDebugMode()
		{
			debugDraw = new CCBox2dDraw ("fonts/MarkerFelt-22", PTM_RATIO);//Constants.PTM_RATIO);
			world.SetDebugDraw(debugDraw);
			debugDraw.AppendFlags(b2DrawFlags.e_shapeBit);

		}

		void Aim (List<CCTouch> touches, CCEvent touchEvent)
		{
			var location = touches [0].Location;
			field.Direction = location;
		}

		bool ShouldEndGame ()
		{
			// TODO return elapsedTime > GAME_DURATION;
			return false;
		}

		void EndGame ()
		{
			// Stop scheduled events as we transition to game over scene
			UnscheduleAll ();

			var gameOverScene = GameOverLayer.SceneWithScore (Window, 10);
			var transitionToGameOver = new CCTransitionMoveInR (0.3f, gameOverScene);

			Director.ReplaceScene (transitionToGameOver);
		}

		public static CCScene GameScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new GameLayer ();
			
			scene.AddChild (layer);

			return scene;
		}
		protected override void Draw()
		{
			base.Draw();
			debugDraw.Begin();
			world.DrawDebugData();
			debugDraw.End();
		}
	}
}