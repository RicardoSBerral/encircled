using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

using Encircled.Orbs;

namespace Encircled.Field
{
	public class GameField : CCNode
	{
		public const float ARROW_PROPORTION = 0.8f;
		public const float LINE_WIDTH = 3f;

		// Constructores
		private readonly OrbFactory factory;
		private readonly OrbComparer comparer;

		// Partes
		private readonly Frame frame;
		private readonly Arrow arrow;

		// Físicas
		private readonly b2World world;

		// Parámetros
		private readonly float width_to_orb_proportion;
		private readonly float growing_time;
		private readonly CCPoint nextOrbPosition;

		// Banderas
		private bool ending;
		private bool gameShouldEnd;

		// Propiedades
		public CCSize PlaySize { get { return frame.PlaySize; } }
		public CCSize StartSize { get { return frame.StartSize; } }
		public OrbBlock Block { get { return block; } }
		private float OrbRadius { get { return frame.PlaySize.Width * width_to_orb_proportion / 2f; } }
		public bool GameShouldEnd { get { return gameShouldEnd; } }
		public int DestroyedOrbs { get { return block.DestroyedOrbs; } }

		// Almacenamiento
		private readonly Queue<Orb> nextOrb;
		private readonly HashSet<Orb> shot;
		private readonly OrbBlock block;
		private List<Orb> allOrbs; // Sólo se utiliza para eliminarlos todos al final

		public CCPoint Head {
			get {
				return this.WorldToParentspace (this.ConvertToWorldspace (arrow.Head));
			}
		}

		public CCVector2 Direction {
			get {
				return arrow.Direction;
			}
			set {
				var origin = this.WorldToParentspace (this.ConvertToWorldspace (arrow.Position));
				var valuePoint = new CCPoint (value.X, value.Y);
				var sub = origin.Sub (ref valuePoint);
				arrow.Angle = CCMacros.CCRadiansToDegrees (sub.Angle);
			}
		}

		public GameField (float height, CCColor4F color, b2World world, 
			float growing_time,
			float width_proportion = 720f / 1280f, 
			float width_to_orb_proportion = 1f / 8f
		)
		{
			// Copiar parámetros
			this.width_to_orb_proportion = width_to_orb_proportion;
			this.world = world;
			this.growing_time = growing_time;
			this.ending = false;
			this.gameShouldEnd = false;
			this.allOrbs = null;

			// MARCO
			frame = new Frame (height, color, world, LINE_WIDTH, width_proportion);
			this.AddChild (frame);
			this.ContentSize = new CCSize (frame.PlaySize.Width, frame.PlaySize.Height + frame.StartSize.Height);

			// FLECHA
			float angle = 80 - CCMacros.CCRadiansToDegrees( (float) Math.Atan (frame.StartSize.Height / (frame.PlaySize.Width / 2)) ); 
			float arrow_length = frame.StartSize.Height * ARROW_PROPORTION;
			var arrow_origin = new CCPoint (frame.PlaySize.Width / 2, 0f);
			arrow = new Arrow (arrow_length, color, LINE_WIDTH, angle) {
				Position = arrow_origin,
				AnchorPoint = CCPoint.Zero,
			};
			this.AddChild (arrow);

			// Constructores
			factory = new OrbFactory (OrbRadius, world);
			comparer = new OrbComparer ();

			// ORBES MÓVILES
			shot = new HashSet<Orb> (comparer);
			nextOrb = new Queue<Orb> ();
			nextOrbPosition = new CCPoint (frame.PlaySize.Width * 4 / 5, frame.StartSize.Height * 1 / 2);

			// ORBES PARADOS
			block = new OrbBlock (world, factory, comparer, PlaySize, StartSize);
			block.Position = CCPoint.Zero;
			block.AnchorPoint = CCPoint.Zero;
			this.AddChild (block);

			// CRECER EL PRIMERO
			Grow ();
		}

		public void Grow () {
			if (ending) {
				return;
			}
			Orb newOrb = factory.CreateOrb(nextOrbPosition);
			newOrb.Visible = false;
			newOrb.Grow (growing_time);

			nextOrb.Enqueue (newOrb);
			this.AddChild (newOrb);
		}

		public void Shoot ()
		{
			if (ending) {
				return;
			}
			if (!nextOrb.Any ()) {
				// Primera bola
				return;
			}

			Orb orb = nextOrb.Dequeue();
			orb.Direction = arrow.Direction;

			CCFiniteTimeAction[] actions = new CCFiniteTimeAction [2];
			actions [0] = orb.Shoot (arrow.Head);
			actions [1] = new CCCallFunc ( () => shot.Add (orb));
			orb.RunActions (actions);
			this.AddChild (orb);
		}

		public void PushLine() {
			if (!ending) {
				try {
					block.PushLine ();
				} catch (GameShouldEndException) {
					DestroyAll ();
				}
			}
		}

		public void Remove(Orb orb) {
			this.RemoveChild (orb, false);
			shot.Remove (orb);
		}

		public void UpdateOrbs() {
			if (!ending) {
				try {
					shot.ToList ().ForEach (orb => orb.UpdateOrb ());
					block.UpdateOrbs ();
				} catch (GameShouldEndException) {
					DestroyAll ();
				}
			} else {
				EndingGame ();
			}
		}
		
		private void EndingGame()
		{
			if (allOrbs == null || ending == false) {
				throw new AccessViolationException ("Game hasn't begun to end yet.");
			}

			// Quitar los ya destruidos
			allOrbs = allOrbs
				.Where(orb => {
					if (orb.State != StateOrb.BeforeDestroying) {
						orb.Destroy();
					}
					orb.UpdateOrb();
					return orb.State != StateOrb.Destroyed;
				})
				.ToList();

			if (!allOrbs.Any ()) {
				gameShouldEnd = true;
			}
		}

		private void DestroyAll()
		{
			if (allOrbs != null || ending) {
				throw new AccessViolationException ("The game had already begun to end.");
			} else if (gameShouldEnd) {
				throw new AccessViolationException ("The game has already ended.");
			}

			allOrbs = new List<Orb> ();
			block.DestroyAll (ref allOrbs);
			shot.Union (nextOrb).ToList ().ForEach (orb => {
				orb.Destroy();
				allOrbs.Add (orb);
			});
			shot.Clear ();
			nextOrb.Clear ();

			ending = true;
		}
	}
}