using System.Collections.Generic;
using System.Linq;
using Evolution.Model;

namespace Evolution
{
    public static class StorageControllerExtension
    {
        public static IList<Point> ToStorageData(this IList<IPoint> points)
        {
            return points.Select(p => p.ToPoint()).ToList();
        }

        public static IList<IPoint> ToModel(this IList<Point> points)
        {
            return points.Select(p => p.ToIPoint()).ToList();
        }

        private static Point ToPoint(this IPoint point)
        {
            if (point is Creature creature)
            {
                return new Point
                {
                    T = PointType.Creature,
                    X = creature.X,
                    Y = creature.Y,
                    V = creature.FoodValue,
                    D = DnaInterpreter.Encode(DnaInterpreter.Processors, creature.Dna),
                    P = creature.Parent,
                    R = creature.Rotation
                };
            }

            if (point is Food food)
            {
                return new Point
                {
                    T = PointType.Food,
                    V = food.Value
                };
            }

            if (point is Wall)
            {
                return new Point
                {
                    T = PointType.Wall
                };
            }

            return null;
        }

        private static IPoint ToIPoint(this Point point)
        {
            if (point == null)
            {
                return null;
            }

            switch (point.T)
            {
                case PointType.Wall:
                    return Wall.Default;
                case PointType.Food:
                    return new Food {Value = point.V.Value};
                case PointType.Creature:
                    return new Creature
                    {
                        Dna = DnaInterpreter.Decode(DnaInterpreter.Processors.Select(p => p.Item2).ToList(), point.D),
                        Parent = point.P.Value,
                        Rotation = point.R.Value,
                        X = point.X.Value,
                        Y = point.Y.Value,
                        FoodValue = point.V.Value
                    };
                default:
                    return null;
            }
        }
    }
}