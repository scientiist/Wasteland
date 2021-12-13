using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SkeletalAnimation
{
    public class Skeleton
    {
        [ContentSerializer(FlattenContent = true, CollectionItemName = "Joint")]
        public Joint[] Joints { get; set; }
        
        [ContentSerializer(FlattenContent = true)]
        public int[] DrawOrder { get; set; }

        [ContentSerializerIgnore]
        public Texture2D[] Textures;
        [ContentSerializerIgnore]
        private Transformation[] LocalTransforms;
        [ContentSerializerIgnore]
        public Transformation[] AbsoluteTransforms { get; private set; }

        public Skeleton()
        {        
        }

        public Skeleton(Joint[] joints)
        {
            Joints = joints;
        }

        public void LoadContent(ContentManager contentManager)
        {
            Textures = new Texture2D[Joints.Length];
            for (int i = 0; i < Joints.Length; i++)
            {
                Textures[i] = contentManager.Load<Texture2D>(Joints[i].TextureName);                
            }
        }

        public void ComputeAbsoluteTransforms()
        {
            if (AbsoluteTransforms == null)
                AbsoluteTransforms = new Transformation[Joints.Length];
            AbsoluteTransforms[0] = Transformation.Identity;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joint joint = Joints[i];
                AbsoluteTransforms[i] = Transformation.Compose(AbsoluteTransforms[joint.ParentId], LocalTransforms[i].Translate(joint.Offset));
            }
        }

        public void SetLocalTransform(int jointIndex, Transformation transform)
        {
            if (LocalTransforms == null)
                LocalTransforms = new Transformation[Joints.Length];
            LocalTransforms[jointIndex] = transform;
        }
    }
}
