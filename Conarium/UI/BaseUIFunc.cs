using System.Collections.Generic;
using System.Linq;
using Conarium.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Conarium.UI
{
    public interface INode
    {
        string Name {get;set;}
        UIVector Position {get;set;}
        UIVector Size {get;set;}
        List<INode> Children {get;}
        INode Parent {get;}
        Vector2 AbsoluteSize {get;}
        Vector2 AbsolutePosition {get;}
        bool Visible {get;}
        bool Active {get;}
        bool ClipsDescendants {get; set;}
        Vector2 AnchorPoint {get;set;}

        INode FindFirstChildWithName(string name);
        List<INode> FindChildrenWithName(string name);
        TNode FindNode<TNode>(string name);
        void Update(GameTime gt);
        void Draw();
    }
    
    public class BaseUIFunc: INode
    {
        public BaseUIFunc(string name)
        {
            Name = name;
            Children = new List<INode>();
            Visible = true;
            Active = true;
            AnchorPoint = Vector2.Zero;
            ClipsDescendants = true;
            Size = UIVector.FromPixels(200, 100);
            Position = UIVector.FromPixels(0, 0);
        }
        public string Name {get;set;}
        public UIVector Position {get;set;}
        public UIVector Size {get;set;}
        public bool Visible {get;set;}
        public bool ClipsDescendants {get;set;}
        public Vector2 AnchorPoint {get;set;}
        public bool Active {get;set;}

        public virtual bool IsMouseInside(MouseState mouse)
        {
            return (mouse.X > AbsolutePosition.X && mouse.Y > AbsolutePosition.Y 
            && mouse.X < (AbsolutePosition.X + AbsoluteSize.X)
            && mouse.Y < (AbsolutePosition.Y + AbsoluteSize.Y));
        }
        public virtual bool IsMouseInside() => IsMouseInside(Mouse.GetState());
        
        public INode FindFirstChildWithName(string name) 
            => Children.First(t => t.Name == name);
        public List<INode> FindChildrenWithName(string name) 
            => Children.FindAll(t => t.Name == name);

		public TNode FindNode<TNode>(string name) 
            => (TNode)Children.Where(t => (t is TNode)).First(t => t.Name == name);
        public List<INode> Children {get;}
        private INode _parent;
		public INode Parent
		{
			get { return _parent; }
			set
			{
				if (_parent != null)
					_parent.Children.Remove(this);

				_parent = value;
				_parent.Children.Add(this);
			}
		}

       
        public virtual Vector2 AbsoluteSize => new Vector2(
			Size.Pixels.X + (Parent.AbsoluteSize.X * Size.Scale.X),
			Size.Pixels.Y + (Parent.AbsoluteSize.Y * Size.Scale.Y)
		);
        public virtual Vector2 AbsolutePosition => new Vector2(
			Parent.AbsolutePosition.X + Position.Pixels.X + (Parent.AbsoluteSize.X * Position.Scale.X) - (AnchorPoint.X * AbsoluteSize.X),
			Parent.AbsolutePosition.Y + Position.Pixels.Y + (Parent.AbsoluteSize.Y * Position.Scale.Y) - (AnchorPoint.Y * AbsoluteSize.Y)
		);

        public virtual void Update(GameTime gt)
        {
            if (!Active)
                return;
            
            foreach(var child in Children)
                child.Update(gt);
        }
        public virtual void Draw()
        {
            if (!Visible)
                return;

                
            foreach(var child in Children)
                child.Draw();
        }
    }
}