using UnityEngine;


public class GridRenderer {

	private static Material lineMaterial;
	private static void CreateLineMaterial () {
		if (!lineMaterial) {
			Shader shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;

			// Turn on alpha blending
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		}
	}

	public static void RenderGrid (bool[,] grid, Matrix4x4 localToWorld, Vector3 xvec, Vector3 yvec, Color color = default(Color)) {
		CreateLineMaterial ();
		lineMaterial.SetPass (0);

		GL.PushMatrix ();
		GL.MultMatrix (localToWorld);


		GL.Begin (GL.LINES);
		GL.Color (color);

		// Draw grid squares
		for (int x = 0; x < grid.GetLength (0); x++) {
			for (int y = 0; y < grid.GetLength (1); y++) {
				if (grid [x, y]) {
					GL.Vertex (x * xvec + y * yvec);
					GL.Vertex (x * xvec + (y+1) * yvec);

					GL.Vertex (x * xvec + y * yvec);
					GL.Vertex ((x+1) * xvec + y * yvec);


					GL.Vertex ((x+1) * xvec + y * yvec);
					GL.Vertex ((x+1) * xvec + (y+1) * yvec);

					GL.Vertex (x * xvec + (y+1) * yvec);
					GL.Vertex ((x+1) * xvec + (y+1) * yvec);
				}
			}
		}

		GL.End ();


		GL.PopMatrix ();
	}

	public static void RenderGrid (int xsize, int ysize, Matrix4x4 localToWorld, Vector3 xvec, Vector3 yvec, Color color = default(Color)) {
		CreateLineMaterial ();
		lineMaterial.SetPass (0);

		GL.PushMatrix ();
		GL.MultMatrix (localToWorld);


		GL.Begin (GL.LINES);
		GL.Color (color);

		// Draw grid lines
		for (int x = 0; x <= xsize; x++) {
			GL.Vertex (x * xvec);
			GL.Vertex (x * xvec + ysize * yvec);
		}
		for (int y = 0; y <= ysize; y++) {
			GL.Vertex (y * yvec);
			GL.Vertex (y * yvec + xsize * xvec);
		}

		GL.End ();


		GL.PopMatrix ();
	}
}
