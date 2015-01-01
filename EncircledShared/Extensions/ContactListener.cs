﻿using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;
using Box2D.Collision;
using Box2D.Collision.Shapes;

using Encircled.Orbs;
using Encircled.Orbs.Factories;

namespace Encircled
{
	public class ContactListener : b2ContactListener
	{
		public ContactListener ()
		{
		}

		public override void PreSolve(b2Contact contact, b2Manifold manifold)
		{
			StaticOrb s;
			MovingOrb m;

			if (contact.FixtureA.Body.UserData is StaticOrb) {
				s = (StaticOrb)contact.FixtureA.Body.UserData;
				if (contact.FixtureB.Body.UserData is MovingOrb) {
					m = (MovingOrb)contact.FixtureB.Body.UserData;
				} else {
					return;
				}
			} else if (contact.FixtureA.Body.UserData is MovingOrb) {
				m = (MovingOrb)contact.FixtureA.Body.UserData;
				if (contact.FixtureB.Body.UserData is StaticOrb) {
					s = (StaticOrb)contact.FixtureB.Body.UserData;
				} else {
					return;
				}
			} else {
				return;
			}

			Encircled.GameLayer.Instance.Field.Block.ReceiveOrb (s, m);
		}

		public override void PostSolve(b2Contact contact, ref b2ContactImpulse contactImpulse)
		{
		}
	}
}
