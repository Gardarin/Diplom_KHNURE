using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Diplom.Models.Research;

namespace Diplom.Models.ResearchAlgorithms
{
    public class ClassificationsAlgoritm:Algorithm
    {

        public override Result Perform(InputData inputData)
        {
            int n, k;
            NPoint[] x;
            using (var sr = new StreamReader("input.txt"))
            {
                string f = sr.ReadLine();
                n = int.Parse(f);
                f = sr.ReadLine();
                k = int.Parse(f);
                x = new NPoint[n];
                for (int i = 0; i < n; i++)
                    x[i] = new NPoint() { X = new double[k - 2] };
                for (int i = 0; i < n; i++)
                {
                    var str = sr.ReadLine().Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < k - 2; j++)
                    {
                        x[i].X[j] = double.Parse(str[j], CultureInfo.InvariantCulture);
                    }
                    x[i].Class = str[str.Length - 1];
                }
            }

            NPoint np1 = new NPoint() { X = new double[] { 5.7, 4.4 } }; // Iris-setosa
            NPoint np2 = new NPoint() { X = new double[] { 6.1, 2.8 } }; // Iris-versicolor
            NPoint np3 = new NPoint() { X = new double[] { 6.4, 2.8 } }; // Iris-virginica
            Methods.KNearestNeighbor(x.ToList(), np1, 30);
            Methods.KNearestNeighbor(x.ToList(), np2, 30);
            Methods.KNearestNeighbor(x.ToList(), np3, 30);
            Console.WriteLine(np1.Class);
            Console.WriteLine(np2.Class);
            Console.WriteLine(np3.Class);
            string s1 = Methods.BayesClassificator<string, NPoint>(x.ToList(), np1, c => c.Class);
            string s2 = Methods.BayesClassificator<string, NPoint>(x.ToList(), np2, c => c.Class);
            string s3 = Methods.BayesClassificator<string, NPoint>(x.ToList(), np3, c => c.Class);
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);

            string[][] table;
            using (var sr = new StreamReader("tree.txt", Encoding.Unicode))
            {
                string f = sr.ReadLine();
                n = int.Parse(f);
                f = sr.ReadLine();
                k = int.Parse(f);
                table = new string[n][];
                for (int i = 0; i < n; i++)
                    table[i] = new string[k + 1];
                for (int i = 0; i < n; i++)
                {
                    var str = sr.ReadLine().Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < k + 1; j++)
                    {
                        table[i][j] = str[j];
                    }
                }
            }
            Trees t = new Trees();
            Node root = t.Build(table, new bool[k]);
            string[] qwestion = { "Ниже", "ВГостях", "НаМесте ", "Нет", "" };
            Console.WriteLine(root.Find(qwestion));
            Console.ReadLine();

            throw new NotImplementedException();
        }

        class Methods
        {
            public static void KNearestNeighbor(List<NPoint> input, NPoint point, int k)
            {
                input.Sort((x, y) => (Distance(x, point) - Distance(y, point)).CompareTo(0));
                //Select(x=>new{Class = x.Class, count = 1}).Aggregate
                var cl = input.Take(k).GroupBy(x => x.Class).Select(x => new { Class = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).First().Class;
                point.Class = cl;
            }
            public static double Distance(NPoint a, NPoint b)
            {
                double sum = 0;
                for (int i = 0; i < a.X.Length; i++)
                {
                    sum += (a.X[i] - b.X[i]) * (a.X[i] - b.X[i]);
                }
                return Math.Sqrt(sum);
            }

            public static C BayesClassificator<C, E>(List<E> input, E point, Func<E, C> classSelector)
                where E : IEquatable<E>
            {
                var classes = new Dictionary<C, double>();
                var freq = new Dictionary<E, Dictionary<C, double>>();
                foreach (var el in input)
                {
                    if (!freq.ContainsKey(el))
                    {
                        freq.Add(el, new Dictionary<C, double>());
                        freq[el].Add(classSelector(el), 1);
                    }
                    else
                    {
                        if (!freq[el].ContainsKey(classSelector(el)))
                        {
                            freq[el].Add(classSelector(el), 1);
                        }
                        else
                        {
                            freq[el][classSelector(el)]++;
                        }
                    }
                    if (!classes.ContainsKey(classSelector(el)))
                    {
                        classes.Add(classSelector(el), 1);
                    }
                    else
                    {
                        classes[classSelector(el)]++;
                    }
                }
                foreach (var el in freq)
                {
                    foreach (var cl in classes)
                    {
                        if (el.Value.ContainsKey(cl.Key))
                            el.Value[cl.Key] /= cl.Value;
                    }
                }
                foreach (var cl in classes.ToArray())
                {
                    classes[cl.Key] /= input.Count;
                }
                double max = 0;
                C result = default(C);
                foreach (var el in freq)
                {
                    if (el.Key.Equals(point))
                        foreach (var cl in el.Value)
                        {
                            double p = cl.Value * classes[cl.Key];
                            if (p > max)
                            {
                                max = p;
                                result = cl.Key;
                            }
                        }
                }
                return result;
            }
        }

        class NPoint : IEquatable<NPoint>
        {
            public double[] X;
            public string Class { get; set; }

            public bool Equals(NPoint other)
            {
                for (int i = 0; i < X.Length; i++)
                {
                    if (Math.Abs(X[i] - other.X[i]) > 1e-6)
                        return false;
                }
                return true;
            }
        }

        class Node
        {
            public int Attribute { get; set; }
            public string[] Values { get; set; }
            public Node[] Attributes { get; set; }
            public string Result { get; set; }
            public string Find(string[] input)
            {
                if (Attribute == -1)
                    return Result;
                string res = input[Attribute];
                int idx = Array.IndexOf<string>(Values, res);
                if (idx < 0)
                {
                    return "unknown";
                }
                return Attributes[idx].Find(input);


            }
        }

        class Trees
        {
            //public bool[] MaskAttribute { get; set; }
            //public string[][] X { get; set; }
            private double calcEntropy(int positives, int negatives)
            {
                int total = positives + negatives;
                double ratioPositive = (double)positives / total;
                double ratioNegative = (double)negatives / total;

                if (ratioPositive != 0)
                    ratioPositive = -(ratioPositive) * System.Math.Log(ratioPositive, 2);
                if (ratioNegative != 0)
                    ratioNegative = -(ratioNegative) * System.Math.Log(ratioNegative, 2);

                double result = ratioPositive + ratioNegative;

                return result;
            }
            public double Gain(double entropy, string[][] table, int attr, string selector)
            {
                var groups = table.GroupBy(x => x[attr]);
                double sum = 0;
                foreach (var el in groups)
                {
                    int positive = el.Where(x => x[x.Length - 1] == selector).Count();
                    int count = el.Count();
                    int negative = count - positive;
                    sum += -((double)count / table.Length) * calcEntropy(positive, negative);
                }
                entropy += sum;
                return entropy;
            }
            public int getBestAttribute(string[][] table, bool[] maskAttribute, string selector, double entropy)
            {
                double max = 0;
                int res = -1;
                for (int i = 0; i < maskAttribute.Length; i++)
                {
                    if (!maskAttribute[i])
                    {
                        double gain = Gain(entropy, table, i, selector);
                        if (gain > max)
                        {
                            max = gain;
                            res = i;
                        }
                    }
                }
                return res;
            }
            public Node Build(string[][] table, bool[] maskAttribute)//x.Length = maskAttribute.Length+1
            {
                var distinct = table.Select(x => x.Last()).Distinct();
                if (distinct.Count() == 1)
                {
                    return new Node() { Attribute = -1, Result = distinct.First() };
                }
                int f = 0;
                bool free = false;
                for (; f < maskAttribute.Length; f++)
                {
                    if (!maskAttribute[f])
                    {
                        free = true;
                        break;
                    }
                }
                if (!free)
                {
                    var maxTable = table.Select(x => x.Last()).GroupBy(x => x);
                    int max = 0;
                    string res = "";
                    foreach (var row in maxTable)
                    {
                        int count = row.Count();
                        if (count > max)
                        {
                            max = count;
                            res = row.Key;
                        }
                    }
                    return new Node() { Attribute = -1, Result = res };
                }
                var maxTable2 = table.Select(x => x.Last()).GroupBy(x => x);
                double min = double.MaxValue;
                string selector = "";
                foreach (var row in maxTable2)
                {
                    int count = row.Count();
                    var entr = calcEntropy(count, table.Length - count);
                    if (entr < min)
                    {
                        min = entr;
                        selector = row.Key;
                    }
                }
                int best = getBestAttribute(table, maskAttribute, selector, min);
                var groups = table.Select(x => x[best]).GroupBy(x => x);
                Node treeNode = new Node() { Attribute = best, Attributes = new Node[groups.Count()], Values = new string[groups.Count()] };
                maskAttribute[best] = true;
                int i = 0;
                foreach (var gr in groups)
                {
                    var tableMini = table.ToList();

                    tableMini.RemoveAll(x => !(x[best] == gr.Key));

                    treeNode.Values[i] = gr.Key;
                    treeNode.Attributes[i] = Build(tableMini.ToArray(), maskAttribute);
                    i++;
                }
                return treeNode;
            }
        }
    }
}