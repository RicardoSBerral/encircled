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
	public abstract class OrbFactory <O> where O : Orb
	{
		private readonly float radius;
		private readonly float mass;
		private readonly b2World world;

		public float Radius { get { return radius; } }
		protected abstract b2FixtureDef FixtureDef { get; }
		protected abstract b2BodyDef BodyDef { get; }

		public OrbFactory (float radius, float mass, b2World world)
		{
			this.radius = radius;
			this.mass = mass;
			this.world = world;
		}

		public O CreateOrb(CCPoint direction)
		{
			BodyDef.position = new b2Vec2 (direction.X / GameLayer.PTM_RATIO, direction.Y / GameLayer.PTM_RATIO);
			var physicsBody = world.CreateBody (BodyDef);
			var fixture = physicsBody.CreateFixture (FixtureDef);

			physicsBody.Mass = mass;

			return Instantiate (radius, fixture, physicsBody);
		}

		public O ReplaceFixture<O2> (O2 orb) where O2 : Orb
		{
			var body = orb.PhysicsBody;
			for (var fixture = body.FixtureList; fixture != null; fixture = fixture.Next) {
				body.DestroyFixture (fixture);
			}
			var newFixture = body.CreateFixture (FixtureDef);
			body.LinearVelocity = b2Vec2.Zero;
			return Instantiate<O2> (orb, newFixture);
		}

		protected abstract O Instantiate (float radius, b2Fixture fixture, b2Body physicsBody);
		protected abstract O Instantiate<O2> (O2 orb, b2Fixture newFixture) where O2 : Orb;
	}
}

