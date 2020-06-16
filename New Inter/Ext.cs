using System;
using System.Collections;
using System.Collections.Generic;
using New_Inter.Classes;

namespace New_Inter
{
    static class Ext
    {
        public static bool ExEquals(this object left, object right)
        {
            //Compare the references
            if (right is null)
                return false;
            if (object.ReferenceEquals(left, right))
                return true;

            //Compare the types
            if (left.GetType() != right.GetType())
                return false;

            //Get all property infos of the right object
            var propertyInfos = right.GetType().GetProperties();

            //Compare the property values of the left and right object
            foreach (var propertyInfo in propertyInfos)
            {
                try
                {
                    var othersValue = propertyInfo.GetValue(right);
                    var currentsValue = propertyInfo.GetValue(left);
                    if (othersValue == null && currentsValue == null)
                        continue;

                    //Comparison if the property is a generic (IList type)
                    if ((currentsValue is IEnumerable && propertyInfo.PropertyType.IsGenericType) ||
                        (othersValue is IEnumerable && propertyInfo.PropertyType.IsGenericType))
                    {
                        //here we work with dynamics because don't need to care about the generic type
                        dynamic cur = currentsValue;
                        dynamic oth = othersValue;
                        if (cur != null && cur.Count > 0)
                        {
                            var result = false;
                            foreach (object cVal in cur)
                            {
                                foreach (object oVal in oth)
                                {
                                    //Recursively call the Equal method
                                    var areEqual = ExEquals(cVal, oVal);
                                    if (!areEqual) continue;

                                    result = true;
                                    break;
                                }

                            }
                            if (result == false)
                                return false;
                        }
                    }
                    else
                    {
                        //Comparison for properties of a non collection type
                        var curType = currentsValue.GetType();

                        //Comparison for primitive types
                        if (curType.IsValueType || currentsValue is string)
                        {
                            var areEquals = currentsValue.Equals(othersValue);
                            if (areEquals == false)
                                return false;      //This is the out point for this methods
                        }
                        else
                        {
                            //values are complex/classes
                            //that's why we have to recursively call the Equals methods
                            var areEqual = Equals(currentsValue, othersValue);
                            if (areEqual == false)
                                return false;
                        }
                    }
                }
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
                    
                }
            }

            return true;
        }

        public static List<string> SplitSkip(this string txt, string sepatator = "::")
        {
            var i = 0;
            var ii = 0;
            List<string> split = new List<string>();
            i = txt.IndexOf(sepatator, ii, StringComparison.Ordinal);

            if (i < 0)
            {
                return split;
            }

            do
            {
                var op = GetExpressionBr(txt);

                var overlap = i > op.i && i < op.e;
                if (overlap)
                {
                    ii = op.e;
                }
                else
                {
                    var splitT = SplitAtIndex(txt, i);
                    split.Add(splitT[0]);

                    txt = txt.Substring(i + sepatator.Length);

                    if (txt.IndexOf(sepatator, 0, StringComparison.Ordinal) < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(splitT[1].Trim().Replace(sepatator, "")))
                        {
                            split.Add(splitT[1].Substring(sepatator.Length));
                        }
                        break;
                    }

                    ii = 0;
                }

                i = txt.IndexOf(sepatator, ii, StringComparison.Ordinal);

                if (i < 0)
                {
                    if (!string.IsNullOrWhiteSpace(txt.Trim().Replace(sepatator, "")))
                    {
                        split.Add(txt);
                    }
                }

            } while (i > -1);

            return split;
        }

        public static (string txt, int i, int e) GetExpressionBr(this string txt)
        {
            var startBool = false;
            var start = 0;
            var countP = 0;

            for (int i = 0; i < txt.Length; i++)
            {
                var c = txt[i];
                if (!startBool)
                {
                    if (c == '{')
                    {
                        startBool = true;
                        start = i;
                        countP++;
                    }
                }
                else
                {
                    if (c == '{')
                    {
                        countP++;
                    }

                    if (c == '}')
                    {
                        countP--;

                        if (countP == 0)
                        {
                            var end = i;
                            var sub = txt.Substring(start + 1, end - start - 1);

                            return (sub, start, end);
                        }
                    }
                }
            }

            return (null, 0, 0);
        }

        public static string[] SplitAtIndex(this string txt, int index)
        {
            var halfs = new string[2];

            halfs[0] = txt.Substring(0, index);
            halfs[1] = txt.Substring(index, txt.Length - index);

            return halfs;
        }

        public static int GetInt(this object obj)
        {
            if (obj is IntClass intClass)
            {
                return intClass.Value;
            }

            if (obj is int i)
            {
                return i;
            }

            if (obj is BoolClass boolClass)
            {
                return boolClass.Value ? 1 : 0;
            }

            if (obj is string strValue)
            {
                if (int.TryParse(strValue, out var ii))
                {
                    return ii;
                }

                return strValue.Length;
            }

            if (obj is StrClass strClass)
            {
                var value = strClass.Value;
                if (int.TryParse(value, out var ii))
                {
                    return ii;
                }

                return strClass.Value.Length;
            }

            if (obj is Variable v)
            {
                var ob = v.GetValue();

                return GetInt(ob);
            }

            if (obj is null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public static bool GetInt(this object obj, out int i)
        {
            if (obj is IntClass intClass)
            {
                i = intClass.Value;
                return true;
            }

            if (obj is BoolClass boolClass)
            {
                i = boolClass.Value ? 1 : 0;
                return true;
            }

            if (obj is int ii)
            {
                i = ii;
                return true;
            }

            if (obj is StrClass strClass)
            {
                var value = strClass.Value;
                if (int.TryParse(value, out var iii))
                {
                    i = iii;
                    return true;
                }

                i = strClass.Value.Length;
            }

            if (obj is Variable v)
            {
                var ob = v.GetValue();

                i =  GetInt(ob);

                return true;
            }

            i = 0;
            return false;
        }

        public static string GetStr(this object obj)
        {
            if (obj is StrClass strClass)
            {
                return strClass.Value;
            }

            if (obj is BoolClass boolClass)
            {
                return (boolClass.Value ? "true" : "false");
            }

            if (GetInt(obj, out var i))
            {
                return i.ToString();
            }

            if (obj is string strValue)
            {
                return strValue;
            }

            if (obj is Variable v)
            {
                var ob = v.GetValue();

                return GetStr(ob);
            }

            return null;
        }
    }
}
