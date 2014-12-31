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
	public abstract class Orb : CCNode
	{
		readonly static List<CCColor4B> colors = new List<CCColor4B> {
			CCColor4B.Red,
			CCColor4B.Blue,
			CCColor4B.Yellow,
			CCColor4B.Gray,
			CCColor4B.Green
		};

		readonly CCDrawNode node;
		readonly CCParticleSun sun;
		private CCPoint direction;

		protected internal abstract b2Fixture Fixture { get; }
		public float Radius { get; set; }
		public abstract b2Body PhysicsBody { get; }

		public CCPoint Direction {
			get { return direction; }
			set { direction = CCPoint.Normalize (value); }
		}
		public override CCPoint Position {
			get {
				return base.Position;
			}
			set {
				// TODO

				float x = value.X;
				float y = value.Y;

				if (IgnoreAnchorPointForPosition) 
				{
					x += AnchorPointInPoints.X;
					y += AnchorPointInPoints.Y;
				}

				// Make matrix
				float radians = PhysicsBody.Angle;
				var c = (float)Math.Cos (radians);
				var s = (float)Math.Sin (radians);

				if (!AnchorPointInPoints.Equals (CCPoint.Zero)) 
				{
					x += c * -AnchorPointInPoints.X + -s * -AnchorPointInPoints.Y;
					y += s * -AnchorPointInPoints.X + c * -AnchorPointInPoints.Y;
				}

				base.Position = new CCPoint(x, y);
				PhysicsBody.SetTransform (new b2Vec2 (x / GameLayer.PTM_RATIO, y / GameLayer.PTM_RATIO), 0f);
				// TODO base.Position = value;
			}
		}

		public Orb (float radius)
		{
			// Copiar parámetros
			this.Radius = radius;

			// Dibujar círculo
			node = new CCDrawNode ();
			CCColor4B color = colors [CCRandom.Next (colors.Count)];
			node.DrawSolidCircle (CCPoint.Zero, radius, color);
			this.AddChild (node);

			// Dibujar brillo
			if (color == CCColor4B.Gray) {
				sun = new CCParticleSun (CCPoint.Zero);
				sun.StartColor = new CCColor4F (CCColor3B.Red);
				sun.EndColor = new CCColor4F (CCColor3B.Black);
				this.AddChild (sun);
			} else {
				sun = null;
			}
		}

		public static CCFiniteTimeAction Grow (float growing_time = 0.2f)
		{
			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[3];
			actions [0] = new CCScaleTo (0f, 0f);
			actions [1] = new CCCallFuncN(
				node => { ((Orb) node).Visible = true; });
			actions [2] = new CCScaleTo (growing_time, 1f);
			return new CCSequence (actions);
		}

		public CCParticleExplosion Explode ()
		{
			this.RemoveFromParent ();
			var explosion = new CCParticleExplosion (this.Position);
			explosion.TotalParticles = CCRandom.Next (8, 12);
			explosion.AutoRemoveOnFinish = true;
			PhysicsBody.Dump ();
			return explosion;
		}

		public void UpdateOrb ()
		{
			this.Position = new CCPoint (PhysicsBody.Position.x * GameLayer.PTM_RATIO, PhysicsBody.Position.y * GameLayer.PTM_RATIO);
		}

	}
}

