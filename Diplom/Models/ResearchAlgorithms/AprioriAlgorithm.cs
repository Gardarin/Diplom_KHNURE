using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diplom.Models.ResearchAlgorithms
{
    public class AprioriAlgorithm:Diplom.Models.Research.Algorithm
    {
        public override Research.Result Perform(Research.InputData inputData)
        {
            
            List<List<string>> col = (List<List<string>>)inputData.GetInputData();
            
           
            List<string> items = new List<string>();
            Transaction<int> t;
            List<Transaction<int>> transactions2 = new List<Transaction<int>>();
            List<int[]> transactions = new List<int[]>();
            foreach (var trans in col)
            {
                t=new Transaction<int>();
                transactions2.Add(t);
                t.Elements = new List<int>();
                foreach (var item in trans)
                {
                    if (items.FirstOrDefault(x => x == item) == null)
                    {
                        t.Elements.Add(items.Count);
                        items.Add(item);
                    }
                    else
                    {
                        t.Elements.Add(items.FindIndex(x=>x==item));
                    }
                }
                transactions.Add(t.Elements.ToArray());
            }
            
          
            var res = Apriori.AprioriMethod(transactions2, 3);
            List<int[]> freqItemSets2 = new List<int[]>();
            int z = 0;
            for (int i = 1; i < res.Count - 1; i++)
            {
                for (int j = 0; j < res[i].Count; j++)
                {
                    freqItemSets2.Add(new int[res[i][j].El.Count]);
                    for (int c = 0; c < res[i][j].El.Count; c++)
                    {
                        freqItemSets2[z][c] = res[i][j].El[c];
                    }
                    z++;
                }
            }

            double minConPct = 0.10;

            List<Rule> goodRules = GetHighConfRules(freqItemSets2, transactions, minConPct);

            AllResults.AprioriResult aprioriResult = new AllResults.AprioriResult();
            aprioriResult.Name = "AprioriResult";
            MemoryStream ms = new MemoryStream();

            List<string> data = new List<string>();
            for (int i = 0; i < goodRules.Count; ++i)
            {
                goodRules[i].items = items;
                data.Add(goodRules[i].ToString());
            }
            
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, data);
            
            aprioriResult.Data = ms.GetBuffer();

            return aprioriResult;
        }

        public override Research.InputData CreateInputData()
        {
            return new InputDataTypes.AprioriInputData();
        }

         static void ShowList(List<int[]> trans)
        {
            for (int i = 0; i < trans.Count; ++i)
            {
                Console.Write(i.ToString().PadLeft(2) + ": ( ");
                for (int j = 0; j < trans[i].Length; ++j)
                    Console.Write(trans[i][j] + " ");
                Console.WriteLine(")");
            }
        }

        // ===============================================================================================================

        static List<Rule> GetHighConfRules(List<int[]> freqItemSets, List<int[]> trans, double minConfidencePct)
        {
            // generate candidate rules from freqItemSets, save rules that meet min confidence against transactions
            List<Rule> result = new List<Rule>();

            Dictionary<int[], int> itemSetCountDict = new Dictionary<int[], int>(); // count of item sets

            for (int i = 0; i < freqItemSets.Count; ++i) // each freq item-set generates multiple candidate rules
            {
                int[] currItemSet = freqItemSets[i]; // for clarity only
                int ctItemSet = CountInTrans(currItemSet, trans, itemSetCountDict); // needed for each candidate rule

                for (int len = 1; len <= currItemSet.Length - 1; ++len) // antecedent len = 1, 2, 3, . .
                {
                    int[] c = NewCombination(len); // a mathematical combination

                    while (c != null) // each combination makes a candidate rule
                    {
                        int[] ante = MakeAntecedent(currItemSet, c);
                        int[] cons = MakeConsequent(currItemSet, c); // could defer this until known if needed

                        int ctAntecendent = CountInTrans(ante, trans, itemSetCountDict); // use lookup if possible 
                        double confidence = (ctItemSet * 1.0) / ctAntecendent;

                        if (confidence >= minConfidencePct) // we have a winner!
                        {
                            Rule r = new Rule(ante, cons, confidence);
                            result.Add(r); // if freq item-sets are distinct, no dup rules ever created
                        }
                        c = NextCombination(c, currItemSet.Length);
                    } // while each combination
                } // len each possible antecedent for curr item-set
            } // i each freq item-set

            return result;
        } // GetHighConfRules

        static int[] NewCombination(int k)
        {
            // if k = 3, return is (0 1 2). n is external somewhere
            int[] result = new int[k];
            for (int i = 0; i < result.Length; ++i)
                result[i] = i;
            return result;
        }

        static int[] NextCombination(int[] comb, int n)
        {
            // if n = 5, combination = (0 3 4) next is (1 2 3)
            // if n = 5, combination = (3 4 5) next is null
            int[] result = new int[comb.Length];
            int k = comb.Length;

            if (comb[0] == n - k) return null;
            Array.Copy(comb, result, comb.Length);
            int i = k - 1;
            while (i > 0 && result[i] == n - k + i)
                --i;
            ++result[i];
            for (int j = i; j < k - 1; ++j)
                result[j + 1] = result[j] + 1;
            return result;
        }

        static int[] MakeAntecedent(int[] itemSet, int[] comb)
        {
            // if item-set = (1 3 4 6 8) and combination = (0 2) 
            // then antecedent = (1 4)
            int[] result = new int[comb.Length];
            for (int i = 0; i < comb.Length; ++i)
            {
                int idx = comb[i];
                result[i] = itemSet[idx];
            }
            return result;
        }

        static int[] MakeConsequent(int[] itemSet, int[] comb)
        {
            // if item-set = (1 3 4 6 8) and combination = (0 2) 
            // then consequent = (3 6 8)
            int[] result = new int[itemSet.Length - comb.Length];
            int j = 0; // ptr into combination
            int p = 0; // ptr into result
            for (int i = 0; i < itemSet.Length; ++i)
            {
                if (j < comb.Length && i == comb[j]) // we are at an antecedent
                    ++j; // so continue
                else
                    result[p++] = itemSet[i]; // at a consequent so add it
            }
            return result;
        }

        static int CountInTrans(int[] itemSet, List<int[]> trans, Dictionary<int[], int> countDict)
        {
            // number of times itemSet occurs in transactions, using a lookup dict
            if (countDict.ContainsKey(itemSet) == true)
                return countDict[itemSet]; // use already computed count

            int ct = 0;
            for (int i = 0; i < trans.Count; ++i)
                if (IsSubsetOf(itemSet, trans[i]) == true)
                    ++ct;
            countDict.Add(itemSet, ct);
            return ct;
        }

        static bool IsSubsetOf(int[] itemSet, int[] trans)
        {
            // 'trans' is an ordered transaction like [0 1 4 5 8]
            int foundIdx = -1;
            for (int j = 0; j < itemSet.Length; ++j)
            {
                foundIdx = IndexOf(trans, itemSet[j], foundIdx + 1);
                if (foundIdx == -1) return false;
            }
            return true;
        }

        static int IndexOf(int[] array, int item, int startIdx)
        {
            for (int i = startIdx; i < array.Length; ++i)
            {
                if (i > item) return -1; // i is past where the target could possibly be
                if (array[i] == item) return i;
            }
            return -1;
        }

    } // program class

    public class Rule
    {
        public int[] antecedent;
        public int[] consequent;
        public double confidence;
        public List<string> items;

        public Rule(int[] antecedent, int[] consequent, double confidence)
        {
            this.antecedent = new int[antecedent.Length];
            Array.Copy(antecedent, this.antecedent, antecedent.Length);
            this.consequent = new int[consequent.Length];
            Array.Copy(consequent, this.consequent, consequent.Length);
            this.confidence = confidence;
        }

        public override string ToString() // hacked for demo data
        {
            string s = "IF ( ";
            for (int i = 0; i < antecedent.Length; ++i)
                s += items[antecedent[i]] + " ";
            s += ")";
            s = s.PadRight(13);

            string t = " THEN ( ";
            for (int i = 0; i < consequent.Length; ++i)
                t += items[consequent[i]] + " ";
            t += ") ";
            t = t.PadRight(17);

            return s + t + "conf = " + confidence.ToString("F2");
        }
    
    }

    class Apriori
    {
        public static List<List<Elements<T>>> AprioriMethod<T>(List<Transaction<T>> transactions, double minSupport)
            where T : IComparable<T>
        {
            var elements = new List<List<Elements<T>>>();

            var candid = new List<Elements<T>>();
            foreach (var trs in transactions)
            {
                foreach (var tr in trs.Elements)
                {
                    var el = candid.FirstOrDefault(x => x.El.Contains(tr));
                    if (el == null)
                    {
                        candid.Add(new Elements<T>() { El = new List<T>() { tr }, Support = 1 });
                    }
                    else
                    {
                        el.Support++;
                    }
                }
            }
            elements.Add(new List<Elements<T>>());
            elements[0].AddRange(candid.Where(x => x.Support >= minSupport).OrderBy(x => x.El[0]));
            for (int k = 1; elements[k - 1].Count != 0; k++)
            {
                elements.Add(new List<Elements<T>>());
                for (int i = 0; i < elements[k - 1].Count - 1; i++)
                {
                    for (int j = i + 1; j < elements[k - 1].Count; j++)
                    {
                        if (elements[k - 1][j].El[k - 1].CompareTo(elements[k - 1][i].El[k - 1]) <= 0)
                            continue;
                        bool cont = false;
                        for (int c = 0; c < k - 1; c++)
                        {
                            if (elements[k - 1][j].El[c].CompareTo(elements[k - 1][i].El[c]) != 0)
                                cont = true;
                        }
                        if (cont)
                            continue;
                        var candidate = new Elements<T>();
                        candidate.El = new List<T>();
                        candidate.El.AddRange(elements[k - 1][i].El);
                        candidate.El.Add(elements[k - 1][j].El[k - 1]);
                        foreach (var trs in transactions)
                        {
                            bool ok = true;
                            foreach (var q in candidate.El)
                            {
                                if (!trs.Elements.Contains(q))
                                {
                                    ok = false;
                                    break;
                                }
                            }
                            if (ok)
                            {
                                candidate.Support++;
                            }
                        }
                        if (candidate.Support >= minSupport)
                        {
                            elements[k].Add(candidate);
                        }
                    }
                }
            }
            return elements;
        }
    }

    class Elements<T>
    {
        public List<T> El { get; set; }

        public double Support { get; set; }
    }

    class Transaction<T>
    {
        public List<T> Elements { get; set; }
    }
}