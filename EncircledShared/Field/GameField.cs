using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

using Encircled.Orbs;

namespace Encircled.Field
{
	public class GameField : CCNode
	{
		const float ARROW_PROPORTION = 0.8f;

		// Constructores
		private readonly OrbFactory factory;
		private readonly OrbComparer comparer;

		// Partes
		private readonly Frame frame;
		private readonly Arrow arrow;
		private readonly OrbBlock block;
		private readonly b2World world;

		// Parámetros
		readonly float width_to_orb_proportion;
		readonly float growing_time;

		// Propiedades
		public CCSize PlaySize { get { return frame.PlaySize; } }
		public CCSize StartSize { get { return frame.StartSize; } }
		public OrbBlock Block { get { return block; } }
		private float OrbRadius { get { return frame.PlaySize.Width * width_to_orb_proportion; } }
		public bool GameShouldEnd { get { return block.GameShouldEnd; } }

		// Almacenamiento
		readonly Queue<Orb> nextOrb;
		readonly HashSet<Orb> shot;
		readonly CCPoint nextOrbPosition;

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

			// Constructores
			factory = new OrbFactory (OrbRadius, world);
			comparer = new OrbComparer ();

			// ORBES MÓVILES
			shot = new HashSet<Orb> (comparer);
			nextOrb = new Queue<Orb> ();
			nextOrbPosition = new CCPoint (frame.PlaySize.Width * 4 / 5, frame.StartSize.Height * 1 / 2);

			// ORBES PARADOS
			block = new OrbBlock (world, factory, comparer, PlaySize, StartSize);
			block.Position = CCPoint.Zero;
			block.AnchorPoint = CCPoint.Zero;
			this.AddChild (block);

			// CRECER EL PRIMERO
			Grow ();
		}

		public void Grow () {
			Orb newOrb = factory.CreateOrb(nextOrbPosition);
			newOrb.Visible = false;
			newOrb.Grow (growing_time);

			nextOrb.Enqueue (newOrb);
			this.AddChild (newOrb);
		}

		public void Shoot ()
		{
			if (!nextOrb.Any ()) {
				// Primera bola
				return;
			}

			Orb orb = nextOrb.Dequeue();
			orb.Direction = arrow.Direction;

			CCFiniteTimeAction[] actions = new CCFiniteTimeAction [2];
			actions [0] = orb.Shoot (arrow.Head);
			actions [1] = new CCCallFunc ( () => shot.Add (orb));
			orb.RunActions (actions);
			this.AddChild (orb);
		}

		public void PushLine() {
			block.PushLine ();
		}

		public void UpdateOrbs() {
			shot.ToList().ForEach (orb => orb.UpdateOrb ());
			block.UpdateOrbs ();
		}

		public void Remove(Orb orb) {
			this.RemoveChild (orb, false);
			shot.Remove (orb);
		}
	}
}