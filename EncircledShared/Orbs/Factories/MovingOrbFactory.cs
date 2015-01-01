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
	public class MovingOrbFactory : OrbFactory<MovingOrb>
	{
		private readonly float impulse;
		private readonly float mass;

		private readonly b2FixtureDef circleDef;
		private readonly b2BodyDef bodyDef;

		protected override b2FixtureDef FixtureDef { get { return circleDef; } }
		protected override b2BodyDef BodyDef { get { return bodyDef; } }

		public MovingOrbFactory (float radius, float mass, b2World world, float impulse = 500f) : base (radius, mass, world)
		{
			this.impulse = impulse;
			this.mass = mass;

			// Creación del círculo
			var c = new b2CircleShape();
			c.Radius = radius / GameLayer.PTM_RATIO;

			circleDef = new b2FixtureDef ();
			circleDef.shape = c;
			circleDef.density = 1;
			circleDef.friction = 1;
			circleDef.restitution = 1;

			bodyDef = new b2BodyDef ();
			bodyDef.bullet = true;
			bodyDef.fixedRotation = true;
			bodyDef.type = b2BodyType.b2_dynamicBody;

		}

		protected override MovingOrb Instantiate (float radius, b2Fixture fixture, b2Body physicsBody)
		{
			return new MovingOrb (radius, fixture, physicsBody, impulse);
		}

		protected override MovingOrb Instantiate<O2> (O2 orb, b2Fixture newFixture)
		{
			return new MovingOrb(orb, newFixture, impulse);
		}
	}
}

