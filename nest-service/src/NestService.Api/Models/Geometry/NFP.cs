using NestService.Api.Extensions;
using NfpLib.Data;
using NfpLib.Util;
using System;
using System.Collections.Generic;

namespace NestService.Api.Models.Geometry
{
    public enum PolygonRelation { Contains, Adjacent }
    public class NFP
    {
        public UniPath A { get; set; }
        public UniPath B { get; set; }
        public PolygonRelation Relation { get; set; }
        public double RotationA { get; set; }
        public double RotationB { get; set; }

        public NFP(UniPath a, UniPath b, PolygonRelation relation, double rotationA, double rotationB)
        {
            A = a;
            B = b;
            Relation = relation;
            RotationA = rotationA;
            RotationB = rotationB;
        }

        public List<UniPath> Generate(int tolerance, bool holesUsing)
        {
            NestPath nestA = A.ToNestPath();
            NestPath nestB = B.ToNestPath();
            bool inside = Relation == PolygonRelation.Contains;
            Config.CLIPPER_SCALE = Math.Pow(10, tolerance);
            var data = NfpUtil.nfpGenerator(new NfpPair(nestA, nestB, new NfpKey(nestA.getId(), nestB.getId(), inside, RotationA * 180 / Math.PI, RotationB * 180 / Math.PI)), holesUsing, false);
            return data.value.ToUniPaths();
        }
    }
}