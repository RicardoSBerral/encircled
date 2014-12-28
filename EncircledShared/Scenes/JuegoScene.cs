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
	public class JuegoScene : CCScene
	{
		public JuegoScene (CCWindow mainWindow) : base(mainWindow)
		{
			JuegoLayer layer = new JuegoLayer ();

			this.AddChild (layer);
		}
	}
}

