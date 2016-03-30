using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Hex
{
	public Hex(int q, int r, int s)
	{
		this.q = q;
		this.r = r;
		this.s = s;
	}
	public Hex(Vector3 cubeCoord)
	{
		this.q = (int)cubeCoord.x;
		this.r = (int)cubeCoord.y;
		this.s = (int)cubeCoord.z;
	}

	public readonly int q;
	public readonly int r;
	public readonly int s;

	//	Explicit Addition
	static public Hex Add(Hex a, Hex b)
	{
		return new Hex(a.q + b.q, a.r + b.r, a.s + b.s);
	}

	//	Explicit Subtration
	static public Hex Subtract(Hex a, Hex b)
	{
		return new Hex(a.q - b.q, a.r - b.r, a.s - b.s);
	}

	//	Explicit Scale
	static public Hex Scale(Hex a, int k)
	{
		return new Hex(a.q * k, a.r * k, a.s * k);
	}

	//	Directions, used for 
	static public List<Hex> directions = new List<Hex>
	{
		new Hex(1, 0, -1), 
		new Hex(1, -1, 0), 
		new Hex(0, -1, 1), 
		new Hex(-1, 0, 1), 
		new Hex(-1, 1, 0), 
		new Hex(0, 1, -1)
	};

	static public Hex Direction(int direction)
	{
		return Hex.directions[direction];
	}

	//	Neighbor hex
	static public Hex Neighbor(Hex hex, int direction)
	{
		return Hex.Add(hex, Hex.Direction(direction));
	}

	static public List<Hex> diagonals = new List<Hex>
	{
		new Hex(2, -1, -1),
		new Hex(1, -2, 1),
		new Hex(-1, -1, 2),
		new Hex(-2, 1, 1),
		new Hex(-1, 2, -1),
		new Hex(1, 1, -2)
	};

	//	Diagonal neighbor
	static public Hex DiagonalNeighbor(Hex hex, int direction)
	{
		return Hex.Add(hex, Hex.diagonals[direction]);
	}

	static public int Length(Hex hex)
	{
		return (int)((Mathf.Abs(hex.q) + Mathf.Abs(hex.r) + Mathf.Abs(hex.s)) / 2);
	}

	//	Distance between two hexes
	static public int Distance(Hex a, Hex b)
	{
		return Hex.Length(Hex.Subtract(a, b));
	}

	/*	
	# convert cube coordinates to odd-q offsetcoordinates 
	col = x
	row = z + (x - (x&1)) / 2
	*/
	static public Vector2 CubeToOffset(Hex cubeCoord)
	{
		Vector2 offset = new Vector2();
		offset.x = (int)cubeCoord.q;
		offset.y = (int)(cubeCoord.r + (cubeCoord.q - (Mathf.Abs(cubeCoord.q) % 2)) * 0.5f);
		return offset;
	}
	/*
	# convert odd-q offset coordinates to cube coordinates
	q = col
	r = row - (col - (col&1)) / 2
	s = -x - z
	 */
	public static Hex OffsetToCube(Vector2 offsetCoord)
	{
		int q = (int)offsetCoord.x;
		int r = (int)(offsetCoord.y - (offsetCoord.x - (Mathf.Abs(offsetCoord.x) % 2)) * 0.5f);
		int s = -q - r;
		return new Hex(q,r,s);
	}

	public static Hex OffsetToCube(float x, float y)
	{
		int q = (int)x;
		int r = (int)(y - (x - (Mathf.Abs(x) % 2)) * 0.5f);
		int s = -q - r;
		return new Hex(q, r, s);
	}

	//	Conversion from Hex to Vector3
	public static explicit operator Vector3(Hex x)
	{
		return new Vector3(x.q, x.r, x.s);
	}

	//	Conversion from Vector3 to Hex
	public static explicit operator Hex(Vector3 x)
	{
		return new Hex((int)x.x, (int)x.y, (int)x.z);
	}

	public override int GetHashCode()
	{
		int hash = 17;
		unchecked
		{
			hash = hash * 31 + q.GetHashCode();
			hash = hash * 31 + r.GetHashCode();
			hash = hash * 31 + s.GetHashCode();
		}
		return hash;
	}
}
