using System;
using System.Linq;
using UnityEngine;

namespace Alteracia.Logic.Animation
{
	public static class CurveExtensions
	{
		public static float GetDuration(this AnimationCurve curve)
		{
			return curve.keys[curve.keys.Length - 1].time - curve.keys[0].time;
		}

		public static float GetMaxValue(this AnimationCurve curve)
		{
			return curve.keys.Select(key => key.value).Concat(new[] {0.0f}).Max();
		}

		public static float GetMinValue(this AnimationCurve curve)
		{
			return curve.keys.Select(key => key.value).Concat(new[] {Mathf.Infinity}).Min();
		}

		public static bool CheckCurve(this AnimationCurve curve)
		{
			return curve.keys.Length >= 2;
		}
		
		public static bool CheckCurveTime01(this AnimationCurve curve)
		{
			return Math.Abs(curve.keys[0].time) < float.Epsilon && Math.Abs(curve.keys[curve.keys.Length - 1].time - 1f) < float.Epsilon;
		}
		
		public static bool CheckCurveValues01(this AnimationCurve curve)
		{
			return curve.keys.All(keyframe => keyframe.value > -float.Epsilon && keyframe.value <= 1f + float.Epsilon);
		}

		public static bool CheckCurve01(this AnimationCurve curve)
		{
			return curve.CheckCurve() && curve.CheckCurveTime01() && curve.CheckCurveValues01();
		}
		
		public static void ClampCurveTime01(this AnimationCurve curve)
		{
			curve.keys[0].time = 0;
			curve.keys[curve.keys.Length - 1].time = 1;
		}
		
		public static void ClampCurveValue01(this AnimationCurve curve)
		{
			curve.keys[0].value = Mathf.Clamp01(curve.keys[0].value);
			curve.keys[curve.keys.Length - 1].value = Mathf.Clamp01(curve.keys[curve.keys.Length - 1].value);
		}
		
		public static void ClampCurve01(this AnimationCurve curve)
		{
			curve.ClampCurveTime01();
			curve.ClampCurveValue01();
		}
		
		public static float EvaluateBackward(this AnimationCurve curve, float time)
		{
			float duration = curve.GetDuration();
			return (curve.Evaluate(duration - time));
		}

		public static System.Object Evaluate(this AnimationCurve[] curves, float time, Type type)
		{
			if (curves.Length < 1)
				return null;

			if (type == typeof(float))
			{
				return curves[0].Evaluate(time);
			}
			
			if (type == typeof(Vector2))
			{
				if (curves.Length < 2)
					return null;

				return new Vector2(curves[0].Evaluate(time), curves[1].Evaluate(time));
			}
			
			if (type == typeof(Vector3))
			{
				if (curves.Length < 3)
					return null;

				return new Vector3(
						curves[0].Evaluate(time),
						curves[1].Evaluate(time),
						curves[2].Evaluate(time));
			}
			
			if (type == typeof(Vector4)) 
			{
				if (curves.Length < 4)
					return null;

				return new Vector4(
						curves[0].Evaluate(time),
						curves[1].Evaluate(time),
						curves[2].Evaluate(time),
						curves[3].Evaluate(time));
			}
			
			return null;
		}

		public static T RemapFromArray<T>(this T original, AnimationCurve[] curves)
		{
			if (curves.Length < 1)
				return (T)Convert.ChangeType(null, typeof(T));

			if (typeof(T) == typeof(float))
			{
				return (T)Convert.ChangeType(curves[0].Evaluate((float) (object) original), typeof(T));
			}
			
			if (typeof(T) == typeof(Vector2))
			{
				Vector2 sourse = (Vector2)(object)original;
				if (curves.Length < 2)
					return (T)Convert.ChangeType(null, typeof(T));

				return (T) Convert.ChangeType(
					new Vector2(curves[0].Evaluate(sourse.x), curves[1].Evaluate(sourse.y)),
					typeof(T));
			}
			
			if (typeof(T) == typeof(Vector3))
			{
				Vector3 sourse = (Vector3)(object)original;
				if (curves.Length < 3)
					return (T)Convert.ChangeType(null, typeof(T));

				return (T) Convert.ChangeType(
					new Vector3(
						curves[0].Evaluate(sourse.x),
						curves[1].Evaluate(sourse.y),
						curves[2].Evaluate(sourse.z)),
					typeof(T));
			}
			
			if (typeof(T) == typeof(Vector4)) 
			{
				Vector4 sourse = (Vector4)(object)original;
				if (curves.Length < 4)
					return (T)Convert.ChangeType(null, typeof(T));

				return (T) Convert.ChangeType(
					new Vector4(
						curves[0].Evaluate(sourse.x),
						curves[1].Evaluate(sourse.y),
						curves[2].Evaluate(sourse.z),
						curves[3].Evaluate(sourse.w)),
					typeof(T));
			}
			
			return (T)Convert.ChangeType(null, typeof(T));
		}
		
	}
}