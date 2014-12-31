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
	public class StaticOrb : Orb
	{
		private readonly b2Fixture hexagon;
		private readonly b2Body physicsBody;

		override protected internal b2Fixture Fixture {
			get {
				return hexagon;
			}
		}
		public override b2Body PhysicsBody {
			get {
				return physicsBody;
			}
		}

		private StaticOrb (float radius, b2Body physicsBody, b2Fixture hexagon) : base (radius)
		{
			this.hexagon = hexagon;
			this.physicsBody = physicsBody;
		}

		public StaticOrb (float radius, b2World world) : base (radius)
		{
			var fixtureDef = StaticOrb.HexagonDefinition (radius);
			var def = new b2BodyDef ();
			def.bullet = true;
			def.fixedRotation = true;
			def.type = b2BodyType.b2_dynamicBody;
			physicsBody = world.CreateBody (def);
			hexagon = physicsBody.CreateFixture (fixtureDef);
			physicsBody.UserData = this;
		}

		static private b2FixtureDef HexagonDefinition (float radius)
		{
			// Creación del hexágono
			radius /= GameLayer.PTM_RATIO;
			var angle = Math.PI / 6;
			b2Vec2[] vertices = new b2Vec2[6];
			float apothem = radius / (float)Math.Cos (angle);
			float halfSide = apothem * (float)Math.Tan (angle);
			vertices [0].Set (0, -apothem);
			vertices [1].Set (radius, -halfSide);
			vertices [2].Set (radius, halfSide);
			vertices [3].Set (0, apothem);
			vertices [4].Set (-radius, halfSide);
			vertices [5].Set (-radius, -halfSide);
			var polygon = new b2PolygonShape ();
			polygon.Set (vertices, 6);

			var fixtureDef = new b2FixtureDef ();
			fixtureDef.shape = polygon;
			//			fixtureDef.density = Mass;
			//			fixtureDef.friction = 0.1f;
			//			fixtureDef.restitution = 0.5f;

			return fixtureDef;
		}

		public static StaticOrb MovingToStaticOrb (MovingOrb orb)
		{
			// Creación del hexágono
			float radius = orb.Radius / GameLayer.PTM_RATIO;
			var fixtureDef = StaticOrb.HexagonDefinition (radius);
			orb.PhysicsBody.DestroyFixture (orb.Fixture);
			var hexagon = orb.PhysicsBody.CreateFixture (fixtureDef);
			orb.PhysicsBody.SetType (b2BodyType.b2_staticBody);

			// Creación de la nueva instancia
			StaticOrb newOrb = new StaticOrb (radius, orb.PhysicsBody, hexagon);
			newOrb.Position = orb.Position;

			return newOrb;
		}
	}
}

