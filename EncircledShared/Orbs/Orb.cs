using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

using Encircled.Extensions;

namespace Encircled.Orbs
{
	public enum StateOrb
	{
		Start,
		Growing,
		Shot,
		Stuck,
		Falling,
		BeforeDestroying,
		Destroyed
	};

	public class Orb : CCNode
	{
		// Constantes
		// Colores con su probabilidad sobre 100
		public readonly static Dictionary<CCColor4B, int> colors = new Dictionary<CCColor4B, int> {
			{ CCColor4B.Red, 30 },
			{ CCColor4B.Blue, 30 },
			{ CCColor4B.Yellow, 20 },
			{ CCColor4B.Green, 10 },
			{ CCColor4B.Orange, 10 },
		};

		// Contador
		private static int counter = 0;
		private readonly int id;

		// Almacenamiento
		private readonly CCDrawNode node;
		private b2Body physicsBody;
		private Func<b2Body,bool> updateBody;

		// Parámetros
		private readonly float radius;
		private readonly float impulse;
		private readonly CCColor4B color;
		private CCPoint direction;
		private StateOrb state;

		// Propiedades
		public int Id { get { return id; } }
		public float Radius { get { return radius; } }
		public StateOrb State { get { return state; } }
		public CCColor4B OrbColor { get { return color; } }
		public CCPoint Direction {
			get { return direction; }
			set { direction = CCPoint.Normalize (value); }
		}
		public override CCPoint Position {
			get {
				return base.Position;
			}
			set {
				physicsBody.SetTransform (new b2Vec2 (value.X / GameLayer.PTM_RATIO, value.Y / GameLayer.PTM_RATIO), 0f);
				base.Position = value;
			}
		}

		public Orb (float radius, float impulse, b2Body physicsBody)
		{
			// Copiar parámetros
			this.radius = radius;
			this.impulse = impulse;
			this.physicsBody = physicsBody;
			this.physicsBody.UserData = this;
			this.physicsBody.GravityScale = 0f;
			this.state = StateOrb.Start;
			this.updateBody = null;

			// Dibujar círculo
			this.color = colors.Roulette ();
			node = new CCDrawNode ();
			node.DrawSolidCircle (CCPoint.Zero, radius, color);
			this.AddChild (node);

			// Posición
			UpdateOrb ();

			// Número de identidad
			id = System.Threading.Interlocked.Increment (ref counter);

			// Empezar parado
			physicsBody.SetActive (false);
			physicsBody.SetType(b2BodyType.b2_staticBody);

			#if DEBUG
			Debug ();
			#endif
		}

		public void Grow (float growing_time = 0.2f)
		{
			if (State != StateOrb.Start) {
				throw new InvalidOperationException ("The orb of id " + id + " is not in a state of 'start', so it can't 'grow'.");
			}
			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[3];
			actions [0] = new CCScaleTo (0f, 0f);
			actions [1] = new CCCallFunc (
				() => {
					state = StateOrb.Growing;
					Visible = true;
					updateBody = (body) => {
						body.SetActive (false);
						body.SetType(b2BodyType.b2_staticBody);
						return body.BodyType == b2BodyType.b2_staticBody;
					};
				});
			actions [2] = new CCScaleTo (growing_time, 1f);
			RunActions (actions);
		}

		public CCFiniteTimeAction Shoot (CCPoint position)
		{
			if (State != StateOrb.Growing) {
				throw new InvalidOperationException ("The orb of id " + id + " is not in a state of 'growing', so it can't 'be shot'.");
			}
			return new CCCallFunc (
				() => {
					Position = position;
					state = StateOrb.Shot;
					updateBody = (body) => {
						body.SetActive (true);
						body.SetType(b2BodyType.b2_dynamicBody);
						body.ApplyLinearImpulse (
							new b2Vec2 (Direction.X, Direction.Y) * impulse / GameLayer.PTM_RATIO,
							body.WorldCenter);
						return body.BodyType == b2BodyType.b2_dynamicBody;
					};
				}
			);
		}

		public void SlowDown ()
		{
			updateBody = (body) => {
				body.LinearVelocity = b2Vec2.Zero;
				body.FixtureList.Friction = 0f;
				body.FixtureList.Restitution = 0f;
				body.LinearDamping = 5000f;
				return body.LinearVelocity == b2Vec2.Zero;
			};
		}

		public void Freeze (CCPoint position)
		{
			if (State != StateOrb.Start && State != StateOrb.Shot) {
				throw new InvalidOperationException ("The orb of id " + id + " is not in a state of 'being shot' or 'start', so it can't 'froze'.");
			}
			state = StateOrb.Stuck;
			updateBody = (body) => {
				// 'Slow down'
				body.LinearVelocity = b2Vec2.Zero;
				body.FixtureList.Friction = 0f;
				body.FixtureList.Restitution = 0f;
				body.LinearDamping = 5000f;

				body.SetType (b2BodyType.b2_staticBody);
				body.SetActive(true);
				this.Position = position;
				return body.BodyType == b2BodyType.b2_staticBody;
			};
		}

		public void Destroy (float time = 0.3f)
		{
			state = StateOrb.BeforeDestroying;
			var actions = new CCFiniteTimeAction[2];
			actions [0] = new CCScaleTo (time, 0f);
			actions [1] = new CCCallFunc (() => {
				this.RemoveAllChildren (true);
				this.RemoveFromParent (true);
				updateBody = (body) => {
					if (body == null) {
						return true;
					}
					var world = body.World;
					world.DestroyBody (body);
					state = StateOrb.Destroyed;
					return false;
				};
			});
			RunActions(actions);
		}

		public void UpdateOrb ()
		{
			if (physicsBody != null) {
				if (updateBody != null) {
					if (updateBody (physicsBody)) {
						updateBody = null;
					}
				}
			}
			if (state == StateOrb.Destroyed) {
				physicsBody = null;
			}
			if (physicsBody == null) {
				state = StateOrb.Destroyed;
			}
			if (physicsBody != null) {
				base.Position = new CCPoint (physicsBody.Position.x * GameLayer.PTM_RATIO, physicsBody.Position.y * GameLayer.PTM_RATIO);
			}
		}

		#if DEBUG
		public void Debug ()
		{
			var debugNode = new CCLabelTtf ("" + Id, "MarkerFelt", 22) {
				Position = CCPoint.Zero,
				Color = CCColor3B.Black,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			this.AddChild (debugNode);
		}
		#endif
	}

	public class OrbComparer : EqualityComparer<Orb>
	{
		public override bool Equals (Orb b1, Orb b2)
		{
			return b1.Id == b2.Id;
		}

		public override int GetHashCode (Orb bx)
		{
			return bx.Id.GetHashCode ();
		}
	}
}

