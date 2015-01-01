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

		public O CreateOrb()
		{
			var physicsBody = world.CreateBody (BodyDef);
			var fixture = physicsBody.CreateFixture (FixtureDef);

			physicsBody.Mass = mass;

			return Instantiate (radius, fixture, physicsBody);
		}

		protected abstract O Instantiate (float radius, b2Fixture fixture, b2Body physicsBody);
	}
}

