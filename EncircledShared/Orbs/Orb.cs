using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled.Orbs
{
	public abstract class Orb : CCNode
	{
		#if DEBUG
		public static int counter = 0;
		public int count = Orb.counter++;
		#endif

		readonly static List<CCColor4B> colors = new List<CCColor4B> {
			CCColor4B.Red,
			CCColor4B.Blue,
			CCColor4B.Yellow,
			CCColor4B.Gray,
			CCColor4B.Green
		};

		readonly CCDrawNode node;
		readonly CCParticleSun sun;

		private readonly float radius;
		private readonly CCColor4B color;
		private readonly b2Fixture fixture;
		private readonly b2Body physicsBody;
		private CCPoint direction;

		public float Radius { get { return radius; } }
		public b2Fixture Fixture { get { return fixture; } }
		public b2Body PhysicsBody { get { return physicsBody; } }

		public CCPoint Direction {
			get { return direction; }
			set { direction = CCPoint.Normalize (value); }
		}
		public override CCPoint Position {
			get {
				return base.Position;
			}
			set {
				PhysicsBody.SetTransform (new b2Vec2 (value.X / GameLayer.PTM_RATIO, value.Y / GameLayer.PTM_RATIO), 0f);
				base.Position = value;
			}
		}

		public Orb (Orb orb, b2Fixture newFixture)
			: this(orb.radius,orb.color,newFixture,orb.PhysicsBody)
		{}

		public Orb (float radius, b2Fixture fixture, b2Body physicsBody)
			: this(radius, colors [CCRandom.Next (colors.Count)], fixture, physicsBody) 
		{}

		private Orb (float radius, CCColor4B color, b2Fixture fixture, b2Body physicsBody)
		{
			// Copiar parámetros
			this.radius = radius;
			this.fixture = fixture;
			this.physicsBody = physicsBody;
			physicsBody.UserData = this;

			// Dibujar círculo
			node = new CCDrawNode ();
			this.color = color;
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

			// Posición
			UpdateOrb ();

			#if DEBUG
			// Número
			var label = new CCLabelTtf("" + count, "MarkerFelt", 22) {
				Position = CCPoint.Zero,
				Color = CCColor3B.Black,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			AddChild (label);

			#endif
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
			base.Position = new CCPoint (PhysicsBody.Position.x * GameLayer.PTM_RATIO, PhysicsBody.Position.y * GameLayer.PTM_RATIO);
		}
	}
}

