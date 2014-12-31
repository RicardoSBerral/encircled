using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;
using Box2D.Dynamics.Joints;

namespace Encircled
{
	public class OrbBlock : CCNode
	{
		readonly b2World world;
		readonly List<StaticOrb> orbs;
		readonly List<b2Joint> joints;
		readonly CCSize playSize;
		readonly CCSize startSize;
		readonly float orb_radius;
		private bool x_offset;

		public OrbBlock (b2World world, CCSize playSize, CCSize startSize, float orb_radius)
		{
			this.world = world;
			this.playSize = playSize;
			this.startSize = startSize;
			this.orb_radius = orb_radius;
			orbs = new List<StaticOrb> ();
			joints = new List<b2Joint> ();
			x_offset = true;

			// ¿Se quita después? Primera línea

		}

		private List<StaticOrb> CreateLine()
		{
			float y = startSize.Height + playSize.Height + orb_radius;
			List<StaticOrb> line = new List<StaticOrb> ();

			float line_length;
			if (x_offset) {
				line_length = orb_radius;
			} else {
				line_length = 0;
			}
			x_offset = !x_offset;
			for ( ; line_length + orb_radius * 2 < playSize.Width; line_length += orb_radius * 2) {
				var orb = new StaticOrb (orb_radius, world);
				this.AddChild (orb);
				orb.Position = new CCPoint(line_length + orb_radius, y);
				orb.Visible = false;
				orb.PhysicsBody.SetType (b2BodyType.b2_staticBody);
				line.Add(orb);
			}
			return line;
		}

		public void PushLine()
		{
			List<StaticOrb> newLine = CreateLine();

			newLine.ForEach (
				orb => {
					orbs.Add(orb);
					orb.Visible = true;
				}
			);
			orbs.ForEach (
				orb => {
					orb.Position = new CCPoint (orb.Position.X, orb.Position.Y - orb_radius * 2);
				}
			);
		}

		public void ReceiveOrb (MovingOrb orb)
		{
			StaticOrb newOrb = StaticOrb.MovingToStaticOrb(orb);
			orb.RemoveFromParent (true);

			// TODO Moverla al espacio correspondiente

			// TODO ¿Es necesario?
			// orb.PhysicsBody.LinearVelocity = 0;
			this.AddChild (newOrb);
			orbs.Add (newOrb);
		}

		public void StickOrbs (StaticOrb a, MovingOrb b)
		{
			b2JointDef jointDef = new b2PrismaticJointDef ();
			jointDef.BodyA = a.PhysicsBody;
			jointDef.BodyB = b.PhysicsBody;
			jointDef.CollideConnected = false;

			b2Joint joint = world.CreateJoint (jointDef);

			joints.Add (joint);
		}

		public void TrembleStart ()
		{
		}

		public void TrembleStop ()
		{
		}
	}
}

