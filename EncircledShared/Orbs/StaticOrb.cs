﻿using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled.Orbs
{
	public class StaticOrb : Orb
	{
		public StaticOrb (Orb orb, b2Fixture newFixture) : base(orb, newFixture) {}
		public StaticOrb (float radius, b2Fixture hexagon, b2Body physicsBody) : base (radius, hexagon, physicsBody) {}

		public void Freeze ()
		{
			PhysicsBody.SetType (b2BodyType.b2_staticBody);
		}
	}
}

