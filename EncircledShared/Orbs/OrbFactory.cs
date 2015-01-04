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
	public class OrbFactory
	{
		private readonly float radius;
		private readonly float impulse;
		private readonly b2World world;
		private readonly b2FixtureDef circleDef;
		private readonly b2BodyDef bodyDef;

		public float Radius { get { return radius; } }
		public float Mass { get { return 0f; } }

		public OrbFactory (float radius, b2World world, float impulse = 500f)
		{
			this.radius = radius;
			this.impulse = impulse;
			this.world = world;

			// Creación del círculo
			var c = new b2CircleShape();
			c.Radius = radius / GameLayer.PTM_RATIO;

			circleDef = new b2FixtureDef ();
			circleDef.shape = c;
			circleDef.friction = 0f;
			circleDef.restitution = 0.8f;

			bodyDef = new b2BodyDef ();
			bodyDef.bullet = true;
			bodyDef.fixedRotation = true;
			bodyDef.type = b2BodyType.b2_dynamicBody;
		}

		public Orb CreateOrb(CCPoint direction)
		{
			bodyDef.position = new b2Vec2 (direction.X / GameLayer.PTM_RATIO, direction.Y / GameLayer.PTM_RATIO);
			var physicsBody = world.CreateBody (bodyDef);
			var fixture = physicsBody.CreateFixture (circleDef);

			physicsBody.Mass = Mass;

			return new Orb (radius, impulse, physicsBody);
		}
	}
}

