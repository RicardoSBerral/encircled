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

namespace Encircled.Orbs
{
	public class CellBlock : CCNode
	{
		// Parámetros
		private readonly float orb_radius;
		private readonly float orb_radius_proj;
		private readonly CCSize playSize;
		private readonly CCSize startSize;
		private bool gameShouldEnd;
		private int destroyedOrbs;

		// Almacenamiento
		private readonly List<List<Cell>> cells;
		private readonly Queue<Cell> toBeEmptied;
		private readonly Queue<Cell> toBeLetFall;

		// Constructores
		private readonly CellComparer comparer;

		// Propiedades
		public bool GameShouldEnd { get { return gameShouldEnd; } }
		public int DestroyedOrbs { get { return destroyedOrbs; } }

		// Debug
		#if DEBUG
		private readonly List<CCDrawNode> dots;
		private readonly List<CCDrawNode> largeDots;
		#endif

		public CellBlock (CCSize playSize, CCSize startSize, float orb_radius, float orb_radius_proj)
		{
			// Copia de parámetros
			this.orb_radius = orb_radius;
			this.orb_radius_proj = orb_radius_proj;
			this.playSize = playSize;
			this.startSize = startSize;
			this.comparer = new CellComparer ();
			this.gameShouldEnd = false;
			this.destroyedOrbs = 0;
			this.toBeEmptied = new Queue<Cell> ();
			this.toBeLetFall = new Queue<Cell> ();

			// Cálculo de las celdas
			bool _x_offset = true;
			cells = new List<List<Cell>> ();

			int indY = 0;
			for (float y = startSize.Height + playSize.Height - orb_radius_proj; 
				y - orb_radius_proj >= startSize.Height;
				y -= orb_radius_proj * 2, indY++) {

				// Instanciación de la fila
				var yCells = CreateLine (_x_offset, y, indY);
				cells.Add (yCells);
				_x_offset = !_x_offset;
			}

			#if DEBUG
			//Debug
			dots = new List<CCDrawNode> ();
			largeDots = new List<CCDrawNode> ();
			Debug ();
			#endif
		}

		public List<Cell> CreateLine (bool x_offset, float y, int indY)
		{
			// Cálculo del offset
			float x = orb_radius;
			if (!x_offset) {
				x += orb_radius;
			}

			// Instanciación de la fila
			var yCells = new List<Cell> ();

			int indX = 0;
			for (;
				x + orb_radius <= playSize.Width;
				x += orb_radius * 2, indX++) {

				// Instanciación de la celda
				var cell = new Cell (indY, indX, new CCPoint(x, y));
				yCells.Add (cell);
			}

			return yCells;
		}

		public Cell NearestCell (CCPoint position)
		{
			return cells.SelectMany(i => i).Aggregate ((a, b) => 
				CCPoint.Distance (a.Position, position) < CCPoint.Distance (b.Position, position) ? a : b);
		}

		public Cell ReceiveOrb (Orb orb)
		{
			var cell = NearestCell (orb.Position);
			if (!cell.Empty) {
				throw new InvalidOperationException ("The neareast cell already houses an orb, of id " + cell.CurrentOrb.Id + ".");
			}

			cell.CurrentOrb = orb;
			this.AddChild (orb, -1); // TODO Z-order?

			#if DEBUG
			Debug(NextCells(cell), cell.CurrentOrb.OrbColor);
			#endif

			return cell;
		}

		public void ShiftCells (List<Orb> newOrbs, bool x_offset)
		{
			// Actualizamos las posiciones
			foreach (var yCells in cells) {
				foreach (var cell in yCells) {
					cell.Position = new CCPoint (cell.Position.X, cell.Position.Y - orb_radius_proj * 2);
					cell.Y = cell.Y + 1;
				}
			}
			float y = startSize.Height + playSize.Height - orb_radius_proj;
			var nextLine = CreateLine (x_offset, y, 0);
			for (int x = 0; x < nextLine.Count; x++) {
				nextLine [x].CurrentOrb = newOrbs [x];
				newOrbs [x].Visible = true;
				this.AddChild (newOrbs [x], -1); // TODO Z-order?
			}
			cells.Insert (0, nextLine);

			if (cells.Last ().All (cell => cell.Empty)) {
				cells.RemoveAt (cells.Count - 1);
			} else {
				gameShouldEnd = true;
				foreach (var cell in cells.SelectMany(i => i)){
					toBeLetFall.Enqueue(cell);
				}
			}

			#if DEBUG
			Debug ();
			#endif
		}

		public void AboveOrBelow (Cell cell, out int? xA, out int? xB)
		{
			// Obtenemos los índices
			int y = cell.Y;
			int x = cell.X;

			// Vemos si está desplazado a la izquierda o a la derecha
			bool izquierda = cells[y].First ().Position.X == orb_radius;
			int offset = izquierda ? 0 : 1;

			if (!izquierda || x != 0) {
				xA = x - 1 + offset;
			} else {
				xA = null;
			}
			if (!izquierda || x != cells [y].Last ().X) {
				xB = x + offset;
			} else {
				xB = null;
			}
		}

		public HashSet<Cell> AboveCells (Cell cell)
		{
			var above = new HashSet<Cell> (comparer);

			// Obtenemos los índices
			int y = cell.Y;
			int x = cell.X;

			// En primera línea
			if (y == 0) {
				return above;
			}

			int? xA, xB;
			AboveOrBelow (cell, out xA, out xB);
				
			if (xA.HasValue) {
				above.Add (cells [y - 1] [xA.Value]);
			}
			if (xB.HasValue) {
				above.Add (cells [y - 1] [xB.Value]);
			}

			return above;
		}

		public HashSet<Cell> BelowCells (Cell cell)
		{
			var below = new HashSet<Cell> (comparer);

			// Obtenemos los índices
			int y = cell.Y;
			int x = cell.X;

			// En primera línea
			if (y == cells.Last().First().Y) {
				return below;
			}

			int? xA, xB;
			AboveOrBelow (cell, out xA, out xB);

			if (xA.HasValue) {
				below.Add (cells [y + 1] [xA.Value]);
			}
			if (xB.HasValue) {
				below.Add (cells [y + 1] [xB.Value]);
			}

			return below;
		}

		private HashSet<Cell> NextCells (Cell cell) {
			var list = new HashSet<Cell> (comparer);

			// Obtenemos los índices
			int y = cell.Y;
			int x = cell.X;

			if (x != 0) {
				list.Add (cells [y] [x - 1]);
			}
			if (x != cells [y].Last ().X) {
				list.Add (cells [y] [x + 1]);
			}

			var above = AboveCells (cell);
			foreach (var a in above) {
				list.Add (a);
			}

			var below = BelowCells (cell);
			foreach (var b in below) {
				list.Add (b);
			}

			return list;
		}

		public void DestroySameColor (Cell cell, int min_same_color)
		{
			var sameColor = new HashSet<Cell> (comparer);
			DestroySameColor (cell, ref sameColor);
			if (sameColor.Count >= min_same_color) {
				var falling = new HashSet<Cell> (comparer);

				// Destruir las del mismo color
				foreach (var c in sameColor) {
					toBeEmptied.Enqueue (c);
				}
			}
		}

		private void DestroySameColor (Cell cell, ref HashSet<Cell> sameColor)
		{
			if (cell.Empty || sameColor.Contains (cell)) {
				return;
			}

			sameColor.Add (cell);
			CCColor4B color = cell.CurrentOrb.OrbColor;
			var next = NextCells (cell);

			foreach (var n in next) {
				if (!sameColor.Contains (n) && !n.Empty && n.CurrentOrb.OrbColor == cell.CurrentOrb.OrbColor) {
					DestroySameColor (n, ref sameColor);
				}
			}
		}

		private void LetFallLoose ()
		{
			var falling = new HashSet<Cell> (comparer);
			foreach (var cell in cells.SelectMany(i => i)) {
				LetFallLoose (cell, ref falling);
			}

			// Tirar las que se van a caer
			foreach (var f in falling) {
				toBeLetFall.Enqueue (f);
			}
		}

		private void LetFallLoose (Cell cell, ref HashSet<Cell> falling)
		{
			// Está vacía, ya está recorrida, o está en la primera fila
			if (cell.Empty || falling.Contains(cell) || cell.Y == 0) {
				return;
			}
				
			// No podemos comprobar la condición con .All(), por el 'ref'
			// Sus padres ni se van a caer ni se van a destruir
			bool fallenAbove = true;
			var aboveAndBesides = NextCells (cell).Except(BelowCells(cell));
			foreach (var a in aboveAndBesides) {
				fallenAbove = fallenAbove && (a.Empty || falling.Contains (a));
			}
			if (fallenAbove) {
				falling.Add (cell);
			}
		}

		#if DEBUG
		public void Debug()
		{
			// Actualizar en pantalla la imagen de las celdas
			dots.ForEach(dot => this.RemoveChild(dot,true));
			dots.Clear();
			largeDots.ForEach(dot => this.RemoveChild(dot,true));
			largeDots.Clear();
			foreach (var cell in cells.SelectMany(i => i)) {
				var node = new CCDrawNode ();
				node.Position = cell.Position;
				node.DrawDot (CCPoint.Zero, 5f, new CCColor4F (CCColor4B.Black));
				dots.Add (node);
				this.AddChild (node);
			}
		}

		public void Debug(HashSet<Cell> cellsToDebug, CCColor4B color)
		{
			// Actualizar en pantalla la imagen de las celdas
			largeDots.ForEach(dot => this.RemoveChild(dot,true));
			largeDots.Clear();
			foreach (var cell in cellsToDebug) {
				var node = new CCDrawNode ();
				node.Position = cell.Position;
				node.DrawDot (CCPoint.Zero, 8f, new CCColor4F (color));
				largeDots.Add (node);
				this.AddChild (node,100);
			}
		}
		#endif

		public void UpdateOrbs (ref HashSet<Orb> toBeDestroyed)
		{
			foreach (var orb in cells.SelectMany(i => i)
				.Where (cell => !cell.Empty)
				.Select (cell => cell.CurrentOrb))
			{
				orb.UpdateOrb ();
			}
				
			while (toBeEmptied.Any ()) {
				var cell = toBeEmptied.Dequeue ();
				toBeDestroyed.Add (cell.DestroyOrb ());
				destroyedOrbs++;
			}


			// Averiguar cuáles se van a caer
			LetFallLoose ();

			while (toBeLetFall.Any ()) {
				var cell = toBeLetFall.Dequeue ();
				toBeDestroyed.Add (cell.LetFall ());
				destroyedOrbs++;
			}
		}
	}
}

