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
	public class Orb : CCNode
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
		private b2Body physicsBody;
		readonly float orb_speed;

		public float Radius { get; set; }
		public CCPoint Direction { get; set; }
		public b2Body PhysicsBody { get { return physicsBody; } }
		public override CCPoint Position {
			get {
				return base.Position;
			}
			set {
				base.Position = value;
				PhysicsBody.SetTransform (new b2Vec2 (value.X, value.Y), 0f);
			}
		}

		public Orb (float radius, b2World world, float orb_speed = 50f)
		{
			// Copiar parámetros
			this.Radius = radius;
			this.orb_speed = orb_speed;

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

			// Creación del hexágono
			b2Vec2[] vertices = new b2Vec2[6];
			float side = radius * (float) Math.Tan (Math.PI / 6) * 2;
			float apothem = radius * (float) Math.Cos (Math.PI / 6);
			vertices[0].Set( - side / 2, - radius);
			vertices[1].Set(side / 2, - radius);
			vertices[2].Set(apothem, 0f);
			vertices[3].Set(side / 2, radius);
			vertices[4].Set(- side / 2, radius);
			vertices[5].Set(- apothem, 0);
			var polygon = new b2PolygonShape();
			polygon.Set(vertices, 6);

			var fixtureDef = new b2FixtureDef();
			fixtureDef.shape = polygon;
//			fixtureDef.density = Mass;
//			fixtureDef.friction = 0.1f;
//			fixtureDef.restitution = 0.5f;

			var def = new b2BodyDef ();
			def.bullet = true;
			def.fixedRotation = true;
			def.type = b2BodyType.b2_dynamicBody;
			physicsBody = world.CreateBody (def);
			physicsBody.CreateFixture (fixtureDef);
		}

		public static CCFiniteTimeAction Grow (float growing_time = 0.2f)
		{
			return new CCSequence (new CCScaleTo (0f, 0f), new CCScaleTo (growing_time, 1f));
		}

		public static CCFiniteTimeAction Shoot (float distance)
		{
			return new CCCallFuncN( (node) => {
				var orb = (Orb) node;
				var dt = distance / orb.orb_speed;
				orb.PhysicsBody.ApplyLinearImpulse(new b2Vec2(1f,1f), new b2Vec2(orb.Direction.X, orb.Direction.Y));
			});
		}

		public static CCFiniteTimeAction Teletransport (CCPoint position, float time = 0.2f) {

			return new CCCallFuncN(
						(node) => {
				var orb = (Orb) node;
						orb.Position = position;
					}
					);
//			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[3];
//			// TODO CCEaseElastic
//			actions [0] = new CCFadeOut (time / 2);
//			actions [1] = new CCMoveTo (0f, position);
//			actions [2] = new CCFadeIn (time / 2);
//			return new CCSequence (actions);
		}

		public void TrembleStart ()
		{
		}

		public void TrembleStop ()
		{
		}

		public CCParticleExplosion Explode ()
		{
			this.RemoveFromParent();
			var explosion = new CCParticleExplosion (this.Position);
			explosion.TotalParticles = CCRandom.Next(8, 12);
			explosion.AutoRemoveOnFinish = true;
			return explosion;
		}

		public void UpdateOrb()
		{
			this.Position = new CCPoint (PhysicsBody.Position.x, PhysicsBody.Position.y);
			// TODO
//			if (PhysicsBody != null)
//			{
//				b2Vec2 pos = PhysicsBody.Position;
//
//				float x = pos.x * ptmRatio;
//				float y = pos.y * ptmRatio;
//
//				if (IgnoreAnchorPointForPosition) 
//				{
//					x += AnchorPointInPoints.X;
//					y += AnchorPointInPoints.Y;
//				}
//
//				// Make matrix
//				float radians = PhysicsBody.Angle;
//				var c = (float)Math.Cos (radians);
//				var s = (float)Math.Sin (radians);
//
//				if (!AnchorPointInPoints.Equals (CCPoint.Zero)) 
//				{
//					x += c * -AnchorPointInPoints.X + -s * -AnchorPointInPoints.Y;
//					y += s * -AnchorPointInPoints.X + c * -AnchorPointInPoints.Y;
//				}
//
//				Position = new CCPoint(x, y);
//			}
		}
	}
}

