using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision;
using Box2D.Collision.Shapes;
using Box2D.Dynamics.Joints;

using Encircled.Orbs;

namespace Encircled.Orbs
{
	public class OrbBlock : CCNode
	{
		// Constantes
		public const int MIN_SAME_COLOR = 3;

		// Constructores
		private readonly OrbFactory factory;
		private readonly OrbComparer comparer;

		// Parámetros
		private readonly CCSize playSize;
		private readonly CCSize startSize;
		private readonly float orb_radius;
		private readonly float orb_radius_proj;
		private bool x_offset;

		// Físicas
		private readonly b2World world;
		private readonly float stick_delay;

		// Almacenamiento
		private readonly List<Orb> nextLine;
		private readonly Queue<Tuple<Orb,Orb>> receiving;
		private readonly Queue<Orb> received;
		private List<Orb> toBeDestroyed;
		private readonly CellBlock cellBlock;

		// Propiedades
		public float StickDelay { get { return stick_delay; } }
		public int DestroyedOrbs { get { return cellBlock.DestroyedOrbs; } }


		public OrbBlock (b2World world, OrbFactory factory, OrbComparer comparer, CCSize playSize, CCSize startSize, float stick_delay = 0.05f)
		{
			this.factory = factory;
			this.comparer = comparer;
			this.world = world;
			this.playSize = playSize;
			this.startSize = startSize;
			this.orb_radius = factory.Radius;
			this.orb_radius_proj = orb_radius * (float) Math.Sqrt (2f / 3f);
			this.stick_delay = stick_delay;
			this.x_offset = true;
			this.nextLine = CreateLine();
			this.receiving = new Queue<Tuple<Orb,Orb>> ();
			this.received = new Queue<Orb> ();
			this.toBeDestroyed = new List<Orb>();

			// Celdas
			cellBlock = new CellBlock (playSize, startSize, orb_radius, orb_radius_proj);
			cellBlock.Position = CCPoint.Zero;
			cellBlock.AnchorPoint = CCPoint.Zero;
			this.AddChild (cellBlock);
		}

		private List<Orb> CreateLine ()
		{
			float y = startSize.Height + playSize.Height + orb_radius_proj;
			List<Orb> line = new List<Orb> ();

			float x = orb_radius;
			if (x_offset) {
				x += orb_radius;
			}
			x_offset = !x_offset;
			// Le dejamos un poco más de espacio para él último orbe
			for (; x + orb_radius / 1.5f <= playSize.Width; x += orb_radius * 2f) {
				var position = new CCPoint (x, y);
				var orb = factory.CreateOrb (position);
				orb.Freeze (position);
				orb.Visible = false;
				this.AddChild (orb);
				line.Add (orb);
			}
			return line;
		}

		public void PushLine ()
		{
			// Actualizamos desde el bloque de celdas
			nextLine.ForEach (orb => this.RemoveChild (orb));
			lock (cellBlock) {
				cellBlock.ShiftCells (nextLine, x_offset);
			}
			nextLine.Clear ();
			nextLine.AddRange (CreateLine ());
		}

		public void ScheduleReceiveOrb (Orb hit, Orb hitter)
		{
			// Si hemos detectado otro golpe con la misma bola, pasamos
			if (receiving.Any (c => c.Item2.Id == hitter.Id)) {
				return;
			}
			receiving.Enqueue(new Tuple<Orb, Orb>(hit, hitter));
		}

		private void ReceiveOrb (Orb hit, Orb hitter)
		{
			GameLayer.Instance.Field.Remove (hitter);
			received.Enqueue (hitter);
			this.AddChild (hitter);

			var cell = StickOrbs (hit, hitter);
			DestroySameColor (cell);
		}

		private Cell StickOrbs (Orb hit, Orb hitter)
		{
			// Insertar el orbe en la celda
			Cell cell;
			lock (cellBlock) {
				cell = cellBlock.ReceiveOrb (hitter);
			}

			return cell;
		}

		private void DestroySameColor (Cell cell)
		{
			lock (cellBlock) {
				cellBlock.DestroySameColor (cell, MIN_SAME_COLOR);
			}
		}

		public void TrembleStart ()
		{
		}

		public void TrembleStop ()
		{
		}

		public void UpdateOrbs ()
		{
			// Operaciones
			while (receiving.Any ()) {
				var pair = receiving.Dequeue ();
				ReceiveOrb (pair.Item1, pair.Item2);
			}

			// Actualizaciones
			while (receiving.Any()) {
				received.Dequeue().UpdateOrb ();
			}
			foreach (var orb in nextLine) {
				orb.UpdateOrb ();
			}
			lock (cellBlock) {
				cellBlock.UpdateOrbs (ref toBeDestroyed);
			}

			// Quitar inservibles
			toBeDestroyed = toBeDestroyed
				.Where(orb => {
					orb.UpdateOrb();
					return orb.State != StateOrb.Destroyed;
				})
				.ToList();
		}

		public void DestroyAll(ref List<Orb> allOrbs) {

			lock (cellBlock) {
				cellBlock.DestroyAll (ref allOrbs);
			}

			foreach (var orb in nextLine.Union(received).Union(toBeDestroyed).ToList()) {
				allOrbs.Add(orb);
			}
			nextLine.Clear();
			receiving.Clear();
			toBeDestroyed.Clear();
		}
	}
}
