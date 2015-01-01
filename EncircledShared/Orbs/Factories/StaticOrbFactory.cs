using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled.Orbs.Factories
{
	public class StaticOrbFactory : OrbFactory<StaticOrb>
	{
		private readonly b2FixtureDef hexagonDef;
		private readonly b2BodyDef bodyDef;

		protected override b2FixtureDef FixtureDef { get { return hexagonDef; } }
		protected override b2BodyDef BodyDef { get { return bodyDef; } }

		public StaticOrbFactory (float radius, float mass, b2World world) : base(radius, mass, world)
		{
			// Creación del hexágono
			radius /= GameLayer.PTM_RATIO;
			var angle = Math.PI / 6;
			b2Vec2[] vertices = new b2Vec2[6];
			float apothem = radius / (float)Math.Cos (angle);
			float halfSide = radius * (float)Math.Tan (angle);
			vertices [0].Set (0, -apothem);
			vertices [1].Set (radius, -halfSide);
			vertices [2].Set (radius, halfSide);
			vertices [3].Set (0, apothem);
			vertices [4].Set (-radius, halfSide);
			vertices [5].Set (-radius, -halfSide);

			var polygon = new b2PolygonShape ();
			polygon.Set (vertices, 6);

			hexagonDef = new b2FixtureDef ();
			hexagonDef.shape = polygon;
			//			fixtureDef.density = Mass;
			//			fixtureDef.friction = 0.1f;
			//			fixtureDef.restitution = 0.5f;

			bodyDef = new b2BodyDef ();
			bodyDef.bullet = true;
			bodyDef.fixedRotation = true;
			bodyDef.type = b2BodyType.b2_dynamicBody;
		}

		public StaticOrb MovingToStaticOrb (MovingOrb orb)
		{
			// Creación del hexágono
			orb.PhysicsBody.DestroyFixture (orb.Fixture);
			var hexagon = orb.PhysicsBody.CreateFixture (FixtureDef);

			// Creación de la nueva instancia
			StaticOrb newOrb = new StaticOrb (Radius, hexagon, orb.PhysicsBody);
			newOrb.Position = orb.Position;

			return newOrb;
		}

		protected override StaticOrb Instantiate (float radius, b2Fixture fixture, b2Body physicsBody)
		{
			return new StaticOrb (radius, fixture, physicsBody);
		}
	}
}

