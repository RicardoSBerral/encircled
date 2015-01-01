using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;
using Box2D.Dynamics.Joints;

using Encircled.Orbs.Factories;

namespace Encircled.Orbs
{
	public class OrbBlock : CCNode
	{

		// Constructores
		private readonly StaticOrbFactory factory;

		// Parámetros
		private readonly CCSize playSize;
		private readonly CCSize startSize;
		private readonly float orb_radius;
		private bool x_offset;
		private readonly b2World world;

		// Almacenamiento
		private List<StaticOrb> nextLine;
		private readonly List<StaticOrb> orbs;
		private readonly List<b2Joint> joints;

		public OrbBlock (b2World world, CCSize playSize, CCSize startSize, float orb_radius)
		{
			factory = new StaticOrbFactory (orb_radius, 1, world);
			this.world = world;
			this.playSize = playSize;
			this.startSize = startSize;
			this.orb_radius = orb_radius;
			orbs = new List<StaticOrb> ();
			joints = new List<b2Joint> ();
			x_offset = true;
			nextLine = CreateLine ();

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
			for ( ; line_length + orb_radius * 2 <= playSize.Width; line_length += orb_radius * 2) {
				var orb = factory.CreateOrb ();
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

		public void ReceiveOrb (StaticOrb hit, MovingOrb hitter)
		{
			StaticOrb newOrb = factory.MovingToStaticOrb(hitter);
			hitter.RemoveFromParent (true);
			this.AddChild (newOrb);
			orbs.Add (newOrb);

			StickOrbs (hit, newOrb);

			// TODO Moverla al espacio correspondiente

			// TODO ¿Es necesario?
			// orb.PhysicsBody.LinearVelocity = 0;
		}

		private void StickOrbs (StaticOrb hit, StaticOrb hitter)
		{
			b2JointDef jointDef = new b2PrismaticJointDef ();
			jointDef.BodyA = hit.PhysicsBody;
			jointDef.BodyB = hitter.PhysicsBody;
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

