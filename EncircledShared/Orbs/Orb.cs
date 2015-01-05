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
		Growing,
		Shot,
		Stuck,
		Falling,
		BeforeDestroying,
		Destroyed,
		Undefined
	};

	public class Orb : CCNode
	{
		// Constantes
		public readonly static Dictionary<CCColor4B, int> colors = new Dictionary<CCColor4B, int> {
			{ CCColor4B.Red, 35 },
			{ CCColor4B.Blue, 35 },
			{ CCColor4B.Yellow, 20 },
			{ CCColor4B.Green, 18 },
			{ CCColor4B.Gray, 50 /*2*/ },
		};
		const float BLINK_INTERVAL = 1f;

		// Contador
		private static int counter = 0;
		private readonly int id;

		// Almacenamiento
		private readonly CCNode node;
		private b2Body physicsBody;
		private Action<b2Body> updateBody;

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
			this.state = StateOrb.Undefined;
			this.updateBody = null;

			// Dibujar círculo
			this.color = colors.Roulette ();
			if (color != CCColor4B.Gray) {
				CCDrawNode n = new CCDrawNode ();
				n.DrawSolidCircle (CCPoint.Zero, radius, color);
				node = n;
			} else {
				node = Blink (radius);
			}
			this.AddChild (node);

			// Posición
			UpdateOrb ();

			// Número de identidad
			id = System.Threading.Interlocked.Increment (ref counter);

			#if DEBUG
			Debug ();
			#endif
		}

		private static CCNode Blink(float radius) {

			CCNode gray = new CCNode ();
			var g = new CCDrawNode ();
			g.DrawSolidCircle (CCPoint.Zero, radius, CCColor4B.Gray);
			gray.AddChild (g);
			CCNode black = new CCNode ();
			var b = new CCDrawNode ();
			b.DrawSolidCircle (CCPoint.Zero, radius, CCColor4B.Black);
			black.AddChild (b);
			CCNode node = new CCNode ();
			node.AddChild (gray);
			node.AddChild (black);

			CCNode small = gray;
			CCNode large = black;

			var actions = new CCFiniteTimeAction[3];
			actions [0] = new CCCallFunc( () => {
//				large.ZOrder = 0;
//				small.ZOrder = 1;
//				small.Scale = 0f;
//				small.RunAction(new CCScaleTo(BLINK_INTERVAL, 1f));
				black.Scale = 0f;
				small.RunAction(new CCScaleTo(BLINK_INTERVAL, 1f));
			});
			actions [1] = new CCDelayTime (BLINK_INTERVAL);
			actions [2] = new CCCallFunc (() => {
//				var temp = small;
//				small = large;
//				large = temp;

				black.Scale = 1f;
				small.RunAction(new CCScaleTo(BLINK_INTERVAL, 0f));
			});
			node.RepeatForever (actions);
			return node;
		}

		public void Grow (float growing_time = 0.2f)
		{
			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[3];
			actions [0] = new CCScaleTo (0f, 0f);
			actions [1] = new CCCallFunc (
				() => {
					state = StateOrb.Growing;
					Visible = true;
					updateBody = (body) => {
						body.SetActive (false);
						body.SetType(b2BodyType.b2_staticBody);
					};
				});
			actions [2] = new CCScaleTo (growing_time, 1f);
			RunActions (actions);
		}

		public CCFiniteTimeAction Shoot (CCPoint position)
		{
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
			};
		}

		public void Freeze (CCPoint position)
		{
			state = StateOrb.Stuck;
			updateBody = (body) => {
				// 'Slow down'
				body.LinearVelocity = b2Vec2.Zero;
				body.FixtureList.Friction = 0f;
				body.FixtureList.Restitution = 0f;
				body.LinearDamping = 5000f;

				body.SetType (b2BodyType.b2_staticBody);
				this.Position = position;
			};
		}

		public void Destroy (float time = 0.3f)
		{
			var actions = new CCFiniteTimeAction[2];
			actions [0] = new CCScaleTo (time, 0f);
			actions [1] = new CCCallFunc (() => {
				this.RemoveAllChildren (true);
				this.RemoveFromParent (true);
				state = StateOrb.BeforeDestroying;
				updateBody = (body) => {
					var world = body.World;
					world.DestroyBody (body);
					state = StateOrb.Destroyed;
				};
			});
			RunActions(actions);
		}

		public void Fall ()
		{
			var actions = new CCFiniteTimeAction[3];
			actions [0] = new CCCallFunc (() => {
				state = StateOrb.Falling;
				updateBody = (body) => {
					body.SetType (b2BodyType.b2_dynamicBody);
					body.GravityScale = 8f;
				};
			});
			actions [1] = new CCDelayTime (4f);
			actions [2] = new CCCallFunc (() =>Destroy ());
			RunActions (actions);
		}

		public void UpdateOrb ()
		{
			if (physicsBody != null) {
				if (updateBody != null) {
					updateBody (physicsBody);
					updateBody = null;
				}
			}
			if (state == StateOrb.Destroyed) {
				physicsBody = null;
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

