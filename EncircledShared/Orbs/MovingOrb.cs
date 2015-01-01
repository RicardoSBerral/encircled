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
	public class MovingOrb : Orb
	{
		readonly float impulse;

		public MovingOrb (float radius, b2Fixture fixture, b2Body physicsBody, float impulse) : base(radius, fixture, physicsBody)
		{
			// Copiar parámetros
			this.impulse = impulse;
		}

		public static CCFiniteTimeAction FromGrowingToShooting (CCPoint position)
		{
			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[2];
			actions [0] = new CCCallFuncN (
				(node) => {
					var orb = (MovingOrb)node;
					orb.Position = position;
				}
			);
			actions [1] = new CCCallFuncN (
				node => {
					var orb = (MovingOrb)node;
					orb.PhysicsBody.SetActive (true);
					orb.PhysicsBody.ApplyLinearImpulse (
						new b2Vec2 (orb.Direction.X, orb.Direction.Y) * orb.impulse / GameLayer.PTM_RATIO,
						orb.PhysicsBody.WorldCenter);
				}
			);
			return new CCSequence (actions);
		}
	}
}

