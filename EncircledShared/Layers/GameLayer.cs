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
		public const float GAME_OVER_DELAY = 2f;

		// Físicas
		private b2World world;
		private ContactListener contactListener;
		#if DEBUG
		private CCBox2dDraw debugDraw;
		#endif

		private Field.GameField field;
		public Field.GameField Field { get { return field; } }

		// Singleton
		private static GameLayer instance = null;
		public static GameLayer Instance { 
			get { 
				if (instance != null) {
					return instance;
				} else {
					throw new AccessViolationException ("The game layer isn't initialized.");
				}
			}
		}

		private GameLayer ()
		{
			// Interacción con el usuario
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesBegan = ListenerTouch;
			touchListener.OnTouchesMoved = ListenerTouch;
			AddEventListener (touchListener, this);

			// Interacción con la simulación física
			contactListener = new ContactListener ();

			Color = new CCColor3B (CCColor4B.White);
			Opacity = 255;     

			StartScheduling ();

			// TODO
			//CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic ("Sounds/backgroundMusic", true);
		}

		void StartScheduling ()
		{
			Schedule (t => {
				field.Shoot();
				field.Grow ();
			}, ORB_INTERVAL);

			Schedule (t => {
				field.PushLine ();
			}, NEWLINE_INTERVAL);

			Schedule (t => {
				world.Step (t, 8, 1);
				field.UpdateOrbs();
				if (ShouldEndGame()) {
					EndGame ();
				}
			});
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.NoBorder;

			InitPhysics ();
			#if DEBUG
				field = new Field.GameField (
					this.VisibleBoundsWorldspace.Size.Height / 2f,
					new CCColor4F (CCColor4B.Black),
					world,
					ORB_INTERVAL);
				field.AnchorPoint = CCPoint.Zero;
				field.Position = new CCPoint(field.PlaySize.Width, 0f);
			#else
				field = new Field.GameField (
					this.VisibleBoundsWorldspace.Size.Height * GAMEFIELD_PROPORTION,
					new CCColor4F (CCColor4B.Black),
					world,
					ORB_INTERVAL);
				field.AnchorPoint = CCPoint.AnchorMiddle;
				field.Position = this.VisibleBoundsWorldspace.Center;
			#endif

			this.AddChild (field);
		}

		public override void OnEnter ()
		{
			base.OnEnter ();
		}

		void InitPhysics ()
		{
			CCSize s = Layer.VisibleBoundsWorldspace.Size;

			var gravity = new b2Vec2(0.0f, -8.0f);
			world = new b2World (gravity);

			world.SetAllowSleeping (true);
			world.SetContinuousPhysics (true);
			world.SetContactListener (contactListener);

			#if DEBUG
			debugDraw = new CCBox2dDraw ("fonts/MarkerFelt-22", PTM_RATIO);
			world.SetDebugDraw(debugDraw);
			debugDraw.AppendFlags(b2DrawFlags.e_shapeBit);
			debugDraw.AppendFlags(b2DrawFlags.e_aabbBit);
			debugDraw.AppendFlags(b2DrawFlags.e_centerOfMassBit);
			debugDraw.AppendFlags(b2DrawFlags.e_jointBit);
			debugDraw.AppendFlags(b2DrawFlags.e_pairBit);
			#endif
		}

		void ListenerTouch (List<CCTouch> touches, CCEvent touchEvent)
		{
			Aim (touches [0].Location);
		}

		void Aim (CCPoint location)
		{
			field.Direction = location;
		}

		bool ShouldEndGame ()
		{
			return field.GameShouldEnd;
		}

		void EndGame ()
		{
			// Stop scheduled events as we transition to game over scene
			UnscheduleAll ();

			var actions = new CCFiniteTimeAction [2];
			actions [0] = new CCDelayTime (GAME_OVER_DELAY);
			actions [1] = new CCCallFunc (() => {
				var gameOverScene = GameOverLayer.SceneWithScore (Window, 10);
				var transitionToGameOver = new CCTransitionMoveInR (0.3f, gameOverScene);

				Director.ReplaceScene (transitionToGameOver);
			});
			this.RunActions (actions);
		}

		public static CCScene GameScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);

			instance = new GameLayer ();
			scene.AddChild (Instance);

			return scene;
		}

		#if DEBUG
		protected override void Draw()
		{
			base.Draw();
			debugDraw.Begin();
			world.DrawDebugData();
			debugDraw.End();
		}
		#endif
	}
}