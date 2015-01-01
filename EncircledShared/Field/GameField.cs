using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

using Encircled.Orbs;
using Encircled.Orbs.Factories;

namespace Encircled
{
	public class GameField : CCNode
	{
		const float ARROW_PROPORTION = 0.8f;

		// Constructores
		private readonly MovingOrbFactory factory;

		// Partes
		readonly Frame frame;
		readonly Arrow arrow;
		readonly OrbBlock block;
		readonly b2World world;

		public Frame Frame { get { return frame; } }
		public Arrow Arrow { get { return arrow; } }
		public OrbBlock Block { get { return block; } }
		public b2World World { get { return world; } }

		readonly List<MovingOrb> nextOrb;
		readonly List<MovingOrb> shot;
		readonly CCPoint nextOrbPosition;

		readonly float width_to_orb_proportion;
		readonly float growing_time;

		public CCSize PlaySize { get { return frame.PlaySize; } }
		public CCSize StartSize { get { return frame.StartSize; } }
		public float OrbRadius { get { return frame.PlaySize.Width * width_to_orb_proportion; } }

		public CCPoint Head {
			get {
				return this.WorldToParentspace (this.ConvertToWorldspace (arrow.Head));
			}
		}

		public CCVector2 Direction {
			get {
				return arrow.Direction;
			}
			set {
				var origin = this.WorldToParentspace (this.ConvertToWorldspace (arrow.Position));
				var valuePoint = new CCPoint (value.X, value.Y);
				var sub = origin.Sub (ref valuePoint);
				arrow.Angle = CCMacros.CCRadiansToDegrees (sub.Angle);
			}
		}

		public GameField (float height, CCColor4F color, b2World world, 
			float growing_time,
			float width_proportion = 720f / 1280f, 
			float width_to_orb_proportion = 0.05f
		)
		{
			// Copiar parámetros
			this.width_to_orb_proportion = width_to_orb_proportion;
			this.world = world;
			this.growing_time = growing_time;

			// MARCO
			frame = new Frame (height, color, world, width_proportion);
			this.AddChild (frame);
			this.ContentSize = new CCSize (frame.PlaySize.Width, frame.PlaySize.Height + frame.StartSize.Height);

			// FLECHA
			float angle = 90 - CCMacros.CCRadiansToDegrees( (float) Math.Atan (frame.StartSize.Height / (frame.PlaySize.Width / 2)) ); 
			float arrow_length = frame.StartSize.Height * ARROW_PROPORTION;
			var arrow_origin = new CCPoint (frame.PlaySize.Width / 2, 0f);
			arrow = new Arrow (arrow_length, color, angle) {
				Position = arrow_origin,
				AnchorPoint = CCPoint.Zero
			};
			this.AddChild (arrow);

			// ORBES MÓVILES
			shot = new List<MovingOrb> ();
			nextOrb = new List<MovingOrb> ();
			nextOrbPosition = new CCPoint (frame.PlaySize.Width * 4 / 5, frame.StartSize.Height * 1 / 2);

			// ORBES PARADOS
			block = new OrbBlock (world, PlaySize, StartSize, OrbRadius);
			block.Position = CCPoint.Zero;
			block.AnchorPoint = CCPoint.Zero;
			this.AddChild (block);

			// Constructores
			factory = new MovingOrbFactory (OrbRadius, 1, world);

			// CRECER EL PRIMERO
			Grow ();
		}

		public void Grow () {
			var newOrb = factory.CreateOrb();
			newOrb.Visible = false;
			newOrb.PhysicsBody.SetActive (false);
			newOrb.Position = nextOrbPosition;
			newOrb.RunAction (Orb.Grow (growing_time));

			nextOrb.Add (newOrb);
			this.AddChild (newOrb);
		}

		public void Shoot ()
		{
			if (!nextOrb.Any ()) {
				// Primera bola
				return;
			}

			MovingOrb orb = nextOrb [0];
			nextOrb.Remove (orb);
			orb.Direction = arrow.Direction;

			CCFiniteTimeAction[] actions = new CCFiniteTimeAction [2];
			actions [0] = MovingOrb.FromGrowingToShooting (arrow.Head);
			actions [1] = new CCCallFuncN ( (node) => shot.Add (((MovingOrb) node)));
			orb.RunAction (new CCSequence (actions));
			this.AddChild (orb);
		}

		public void PushLine() {
			block.PushLine ();
		}

		public void CheckOrbsCollision ()
		{
			// TODO
			foreach (var orb in shot) {
				var collision = frame.CheckCollisionOrb (orb);
				switch (collision) {
				default:
					break;
				}
			}
		}

		public void UpdateOrbs() {
			foreach (var orb in this.shot) {
				orb.UpdateOrb ();
			}
		}
	}
}