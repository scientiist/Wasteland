using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Conarium.Datatypes
{

    public struct Rotation : IEquatable<Rotation>
    {
		#region static reference members
        public readonly static Rotation Zero       = new(0);
		public readonly static Rotation PiOverTwo  = new(MathHelper.Pi/2);
        public readonly static Rotation Pi         = new(MathHelper.Pi);
        public readonly static Rotation TwoPi      = new(MathHelper.Pi * 2);
        public readonly static Rotation RightAngle = new(90);
		#endregion


        public float Radians { get;set; }
		public float NormalizedRadians => Radians%TwoPi.Radians;
		public float Degrees 
		{
			get => MathHelper.ToDegrees(Radians);
			set => Radians = MathHelper.ToRadians(value);
		}
		public float NormalizedDegrees => Degrees%TwoPi.Degrees;

		public Rotation(float radians)
		{
			Radians = radians;
		}

		public Rotation(Vector2 unitVector)
		{
			Radians = UnitVectorToRadians(unitVector);
		}


		// Static Math Formulae
		public static float UnitVectorToRadians(Vector2 unit)
		{
			var copyVec = new Vector2(unit.X, unit.Y);
			copyVec.Normalize();
            return (float)Math.Atan2(copyVec.Y, copyVec.X);
		}
		public static Vector2 RadiansToUnitVector(float rads)
		{
			return new Vector2(
				x: (float)Math.Cos(rads),
				y: (float)Math.Sin(rads)
			);
		}

        public Vector2 ToUnitVector() => RadiansToUnitVector(this.Radians);


		
        //public static Rotation FromDeg(float degree)
        //{
            //return new Rotation { Radians = MathHelper.ToRadians(degree) }; //Degrees = degree };
        //}
 
        //public Rotation RotateDeg(float degree) => FromDeg(this.Degrees + degree);

        //public Rotation RotateRad(float radians) => new Rotation(this.Radians + radians);

		public void Normalize()
		{
			Radians %= TwoPi.Radians;
		}

		#region Operators
		// having to write these operators both ways is a hassle

		// arithmetic operators
		public static Rotation operator +(Rotation a, Rotation b) => new (a.Radians + b.Radians);
		public static Rotation operator -(Rotation a, Rotation b)
			=> new Rotation(a.Radians - b.Radians);
		public static Rotation operator +(Rotation a, float b)
			=> new Rotation(a.Radians + b);
		public static Rotation operator -(Rotation a, float b)
			=> new Rotation(a.Radians - b);
		public static Rotation operator +(float a, Rotation b)
			=> new Rotation(a + b.Radians);

		public static Rotation operator -(float a, Rotation b)
			=> new Rotation(a + b.Radians);

		// comparison operators
		public static bool operator >(Rotation a, Rotation b) => a.Radians > b.Radians;
		public static bool operator <(Rotation a, Rotation b) => a.Radians < b.Radians;
		public static bool operator >(Rotation a, float b) => a.Radians > b;
		public static bool operator <(Rotation a, float b) => a.Radians < b;
		public static bool operator >(float a, Rotation b) => a > b.Radians;
		public static bool operator <(float a, Rotation b) => a < b.Radians;

		public static bool operator >=(Rotation a, Rotation b)
			=> a.Radians >= b.Radians;
		public static bool operator <=(Rotation a, Rotation b)
			=> a.Radians <= b.Radians;
		public static bool operator >=(Rotation a, float b)
			=> a.Radians >= b;
		public static bool operator <=(Rotation a, float b)
			=> a.Radians <= b;
		public static bool operator >=(float a, Rotation b)
			=> a >= b.Radians;
		public static bool operator <=(float a, Rotation b)
			=> a <= b.Radians;
		// modulus operators
		
		public static Rotation operator %(Rotation a, Rotation b) 
			=> new Rotation(a.Radians % b.Radians);
		public static Rotation operator %(Rotation a, float b) 
			=> new Rotation(a.Radians % b);
		public static Rotation operator %(float a, Rotation b) 
			=> new Rotation(a % b.Radians);

		public static bool operator ==(Rotation a, Rotation b) => a.EqualsExact(b);
		public static bool operator !=(Rotation a, Rotation b) => !a.EqualsExact(b);


        public bool CloseEnough(float rad, float toleranceDegrees = 1.0f)
        {
            return Math.Abs(Radians - rad) < (toleranceDegrees * (MathHelper.Pi / 180.0f));
        }

		public bool CloseEnough(Rotation rot, float toleranceDegrees = 1.0f)
			=> CloseEnough(rot.Radians, toleranceDegrees);

		public bool EqualsExact(float rad) => this.Radians == rad;
		public bool EqualsExact(Rotation rot) => EqualsExact(rot.Radians);
		// defined for IEquatable
		public bool Equals(Rotation compare) => EqualsExact(compare);


		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}


		#endregion
	}
}
