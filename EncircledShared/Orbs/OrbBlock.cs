using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;
using Box2D.Dynamics.Joints;

using Encircled.Orbs.Factories;

namespace Encircled.Orbs
{
	public class OrbBlock : CCNode
	{
		// Constructores
		private readonly StaticOrbFactory factory;

		// Parámetros
		private readonly CCSize playSize;
		private readonly CCSize startSize;
		private readonly float orb_radius;
		private bool x_offset;

		// Físicas
		private readonly b2World world;
		private readonly float stick_delay;

		// Almacenamiento
		private List<StaticOrb> nextLine;
		private readonly List<StaticOrb> orbs;
		private readonly Queue<Tuple<StaticOrb,MovingOrb>> receiving;
		private List<List<CCPoint>> cells;

		// Propiedades
		public float StickDelay { get { return stick_delay; } }

		// Debug
		#if DEBUG
		private readonly List<CCDrawNode> dots;
		#endif

		public OrbBlock (b2World world, CCSize playSize, CCSize startSize, float orb_radius, float stick_delay = 0.1f)
		{
			factory = new StaticOrbFactory (orb_radius, 1, world);
			this.world = world;
			this.playSize = playSize;
			this.startSize = startSize;
			this.orb_radius = orb_radius;
			this.stick_delay = stick_delay;
			orbs = new List<StaticOrb> ();
			x_offset = true;
			nextLine = CreateLine ();
			receiving = new Queue<Tuple<StaticOrb,MovingOrb>> ();
			cells = new List<List<CCPoint>> ();

			//Debug
			#if DEBUG
			dots = new List<CCDrawNode> ();
			#endif

			// Cálculo de las celdas
			bool _x_offset = false;
			for (float y = startSize.Height + playSize.Height - orb_radius; 
				y - orb_radius >= startSize.Height;
				y -= orb_radius * 2) {
				float x = orb_radius;
				if (_x_offset) {
					x += orb_radius;
				}
				_x_offset = !_x_offset;
				var yCells = new List<CCPoint> ();
				cells.Add (yCells);
				for (;
					x + orb_radius <= playSize.Width;
					x += orb_radius * 2) {
					var cell = new CCPoint (x, y);
					yCells.Add (cell);
					#if DEBUG
					var node = new CCDrawNode ();
					node.Position = cell;
					node.DrawDot (CCPoint.Zero, 5f, new CCColor4F (CCColor4B.Black));
					dots.Add (node);
					this.AddChild (node);
					#endif
				}
			}
		}

		private List<StaticOrb> CreateLine ()
		{
			float y = startSize.Height + playSize.Height + orb_radius;
			List<StaticOrb> line = new List<StaticOrb> ();

			float x = orb_radius;
			if (x_offset) {
				x += orb_radius;
			}
			x_offset = !x_offset;
			for (; x + orb_radius <= playSize.Width; x += orb_radius * 2) {
				var position = new CCPoint (x, y);
				var orb = factory.CreateOrb (position);
				// TODO orb.Visible = false;
				orb.PhysicsBody.SetType (b2BodyType.b2_staticBody);
				line.Add (orb);
				this.AddChild (orb);
			}
			return line;
		}

		public void PushLine ()
		{
			nextLine.ForEach (
				orb => {
					orbs.Add (orb);
					orb.Visible = true;
				}
			);
			orbs.ForEach (
				orb => {
					orb.Position = new CCPoint (orb.Position.X, orb.Position.Y - orb_radius * 2);
				}
			);
			cells = cells.Select<List<CCPoint>,List<CCPoint>> (
				yCells => {
					bool derecha = yCells.First().X == orb_radius;
					var newYCells = yCells.Select (
						cell => {
							if (derecha) {
								cell.X += orb_radius;
							} else {
								cell.X -= orb_radius;
							}
							return cell;
						}).ToList ();
					if (derecha) {
						newYCells.Remove(newYCells.Last());
					} else {
						newYCells.Add(new CCPoint(playSize.Width - orb_radius, newYCells.First().Y));
					}
					return newYCells;
				}).ToList ();
			#if DEBUG
			dots.ForEach(dot => this.RemoveChild(dot,true));
			dots.Clear();

			cells.ForEach(yCells => yCells.ForEach( cell => {
				var node = new CCDrawNode ();
				node.Position = cell;
				node.DrawDot (CCPoint.Zero, 5f, new CCColor4F (CCColor4B.Black));
				dots.Add (node);
				this.AddChild (node);
			}));
			#endif

			nextLine = CreateLine ();
		}

		public void ScheduleReceiveOrbs (StaticOrb hit, MovingOrb hitter)
		{
			receiving.Enqueue (new Tuple<StaticOrb,MovingOrb> (hit, hitter));
		}

		private void ReceiveOrb (StaticOrb hit, MovingOrb hitter)
		{
			StaticOrb newOrb = factory.MovingToStaticOrb (hitter);
			GameLayer.Instance.Field.Remove (hitter);
			this.AddChild (newOrb);
			orbs.Add (newOrb);

			StickOrbs (hit, newOrb);
		}

		private void StickOrbs (StaticOrb hit, StaticOrb hitter)
		{
			hitter.PhysicsBody.LinearVelocity = b2Vec2.Zero;
			hitter.PhysicsBody.FixtureList.Restitution = 0f;

			CCPoint cell = cells.SelectMany(i => i).Aggregate ((a, b) => 
				CCPoint.Distance (a, hitter.Position) < CCPoint.Distance (b, hitter.Position) ? a : b);

			b2DistanceJointDef jointDef = new b2DistanceJointDef();
			jointDef.BodyA = hit.PhysicsBody;
			jointDef.BodyB = hitter.PhysicsBody;
			jointDef.CollideConnected = true;
			var joint = (b2DistanceJoint) world.CreateJoint(jointDef);

			var actions = new CCFiniteTimeAction[2];
			actions [0] = new CCMoveTo (StickDelay, cell);
			actions [1] = new CCCallFuncN (node => {
				((StaticOrb)node).Freeze ();
				world.DestroyJoint(joint);
			});
			hitter.RunActions (actions);
		}

		public void TrembleStart ()
		{
		}

		public void TrembleStop ()
		{
		}

		public void UpdateOrbs ()
		{
			while (receiving.Any ()) {
				var pair = receiving.Dequeue ();
				ReceiveOrb (pair.Item1, pair.Item2);
			}
			nextLine.ForEach (orb => orb.UpdateOrb ());
			orbs.ForEach (orb => orb.UpdateOrb ());
		}
	}
}

