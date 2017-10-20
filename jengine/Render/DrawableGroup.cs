using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render
{
	public class DrawableGroup
	{
		public List<IDrawable> Drawables { get; private set; }
		public Vector3 Position { get; set; } = Vector3.Zero;
		public Quaternion Orientation { get; set; } = Quaternion.Identity;
		public Vector3 Scale { get; set; } = Vector3.One;

		public DrawableGroup(IDrawable[] Drawables)
		{
			this.Drawables = new List<IDrawable>();
			foreach (IDrawable d in Drawables)
			{
				d.Parent = this;
			}
			this.Drawables.AddRange(Drawables);
		}
	}
}