using System;
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

namespace Encircled
{
	public class ContactListener : b2ContactListener
	{
		public ContactListener ()
		{
		}

		public override void BeginContact(b2Contact contact)
		{
		}
		public override void EndContact(b2Contact contact)
		{
		}

		public override void PreSolve(b2Contact contact, b2Manifold manifold)
		{
			Orb s;
			Orb m;

			if (!(contact.FixtureA.Body.UserData is Orb)) {
				return;
			} else if (!(contact.FixtureB.Body.UserData is Orb)) {
				return;
			}

			Orb a = (Orb) contact.FixtureA.Body.UserData;
			Orb b = (Orb) contact.FixtureB.Body.UserData;

			if (a.State == StateOrb.Stuck) {
				s = a;
				if (b.State == StateOrb.Shot) {
					m = b;
				} else {
					return;
				}
			} else if (a.State == StateOrb.Shot) {
				m = a;
				if (b.State == StateOrb.Stuck) {
					s = b;
				} else {
					return;
				}
			} else {
				return;
			}

			m.SlowDown ();
			m.UpdateOrb ();
			Encircled.GameLayer.Instance.Field.Block.ScheduleReceiveOrb (s, m);
		}

		public override void PostSolve(b2Contact contact, ref b2ContactImpulse contactImpulse)
		{
		}
	}
}

