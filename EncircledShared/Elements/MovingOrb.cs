using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled
{
	public class MovingOrb : Orb
	{
		private readonly b2Fixture circle;
		private readonly b2Body physicsBody;

		override protected internal b2Fixture Fixture {
			get {
				return circle;
			}
		}
		public override b2Body PhysicsBody {
			get {
				return physicsBody;
			}
		}

		readonly float impulse;

		public MovingOrb (float radius, b2World world, float impulse = 500f) : base(radius)
		{
			// Copiar parámetros
			this.impulse = impulse;

			// Creación del círculo
			var c = new b2CircleShape();
			c.Radius = radius / GameLayer.PTM_RATIO;

			var fixtureDef = new b2FixtureDef ();
			fixtureDef.shape = c;
			fixtureDef.density = 1;
			fixtureDef.friction = 1;
			fixtureDef.restitution = 1;

			var def = new b2BodyDef ();
			def.bullet = true;
			def.fixedRotation = true;
			def.type = b2BodyType.b2_dynamicBody;
			physicsBody = world.CreateBody (def);
			circle = physicsBody.CreateFixture (fixtureDef);
			physicsBody.Mass = 0;
			physicsBody.UserData = this;
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

			//			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[3];
			//			// TODO CCEaseElastic
			//			actions [0] = new CCFadeOut (time / 2);
			//			actions [1] = new CCMoveTo (0f, position);
			//			actions [2] = new CCFadeIn (time / 2);
			//			return new CCSequence (actions);
		}
	}
}

