using Conarium;
using Conarium.Datatypes;

namespace Wasteland.Common.Skeletal;
// vscode hasnt yet caught up to C# 10
// so the first class signature is linted incorrectly
public class Segment
{
	public Segment NextSegment {get;set;}
	public bool ArticulationAngleIsAbsolute {get;set;}

	public Rotation ArticulationAngle {get;set;}
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
	public Segment UpperSegment;
	public Segment LowerSegment;
	public Rotation ChildSegmentDirection;
}