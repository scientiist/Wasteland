using Conarium;
using Conarium.Datatypes;

namespace Wasteland.Common.Skeletal;
/*
protected static class Triangulation
{
	public static Vector2 Solve(float sideA, float sideB, float sideC)
	{
		var result = new Vector2(0,0);
		
		if (sideA > 0)
			result.X = (sideC*sideC - sideB*sideB + sideA*sideA) / (2*sideA);
		result.Y = Math.Sqrt(c*c - result.X*result.X);
		return result;
	}
	
}

// vscode hasnt yet caught up to C# 10
// so the first class signature is linted incorrectly
public class Bone
{
	public Joint RootJoint {get;set;}
	public Joint EndJoint {get;set;}
	public List<Joint> Joints {get;set;}
	public bool AngleIsAbsolute {get;set;}
	public Rotation Angle {get;set;}

	public Segment(Joint root)
	{

	}

	public Vector2 BasePosition => new Vector2(RootJoint.Position);


}


//https://math.stackexchange.com/questions/543961/determine-third-point-of-triangle-when-two-points-and-all-sides-are-known
public class Skeleton
{
	//[ContentS]
	public int DrawOrder {get;set;}
	public void Draw()
	{
		var Graphics = GraphicsService.Get();
		
		//for (int i = 0; i < DrawOrder.Length)

	}
}

public class Joint
{

	public Vector2 Position;
	public Segment UpperSegment;
	public Segment LowerSegment;
	public Rotation ChildSegmentDirection;
}
*/